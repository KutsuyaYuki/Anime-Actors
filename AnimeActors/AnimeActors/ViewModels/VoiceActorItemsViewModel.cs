using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

using AnimeActors.Models;
using AnimeActors.Services;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Reactive.Disposables;

namespace AnimeActors.ViewModels
{
    public class VoiceActorItemsViewModel : ReactiveObject, IActivatableViewModel
    {
        public const string emptyImage =
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==";
        public string Title => "Voice Actors";
        [Reactive]
        public bool IsBusy { get; set; }
        public string SearchText { get; set; }
        [Reactive]
        public int ResultsAmount { get; set; }
        [Reactive]
        public CharacterItem itemToScrollTo { get; set; }
        public ICommand SearchCommand { get; set; }
        private SourceCache<VAOriginItem, int> _cache;
        public IObservableCollection<VoiceActorItem> Items { get; set; }

        private readonly AnilistService _anilistService = new AnilistService();

        public VoiceActorItemsViewModel()
        {
            Items = new ObservableCollectionExtended<VoiceActorItem>();

            Activator = new ViewModelActivator();

            this.WhenActivated(async disposed =>
            {
                var SortByAnime = bool.Parse((await Xamarin.Essentials.SecureStorage.GetAsync("SortByAnime")) ?? "false");

                _cache = new SourceCache<VAOriginItem, int>(actor => actor.Id);
                SortExpressionComparer<VoiceActorItem> CharacterNameComparer = SortExpressionComparer<VoiceActorItem>.Ascending(i => i.CharacterName);
                SortExpressionComparer<VoiceActorItem> AnimeComparer = SortExpressionComparer<VoiceActorItem>.Ascending(i => i.AnimeName);
                _cache
                  .Connect()
                  .Select(oi => new VoiceActorItem
                  {
                      VoiceActor = oi.VoiceActor.name.full,
                      AnimeName = oi.Anime.title.userPreferred,
                      CharacterName = oi.Character.name.full,
                      Id = oi.Id.ToString(),
                      Image = ImageSource.FromUri(new Uri(oi.Character.image?.large ?? emptyImage))
                  })
                  .Sort(SortByAnime ? AnimeComparer : CharacterNameComparer)
                  .Bind(Items)
                  .Subscribe((a) =>
                  {
                      ResultsAmount = Items.Count();
                  }).DisposeWith(disposed);
            });
            SearchCommand = ReactiveCommand.CreateFromTask(() => ExecuteLoadItemsCommand(SearchText));
        }

        async Task ExecuteLoadItemsCommand(string characterName)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                _cache.Clear();
                await foreach (var item in _anilistService.GetCharacterByName(characterName))
                {
                    var c = (await item).SelectMany(waitedItem =>
                    {
                        var staff = waitedItem.Item1;

                        return waitedItem.Item2.media.nodes.Select(node => new VAOriginItem(node.id, staff, node, waitedItem.Item2));
                    });
                    _cache.AddOrUpdate(c);
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public ViewModelActivator Activator { get; }
    }

    internal class VAOriginItem
    {
        public int Id { get; }
        public Models.Staff.Staff VoiceActor { get; }
        public Models.Staff.Node1 Anime { get; }
        public Models.Staff.Node Character { get; }

        public VAOriginItem(
            int id, 
            Models.Staff.Staff voiceActor, 
            Models.Staff.Node1 anime, 
            Models.Staff.Node character)
        {
            Id = id;
            VoiceActor = voiceActor;
            Anime = anime;
            Character = character;
        }
    }
}