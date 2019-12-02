using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

using AnimeActors.Models;
using AnimeActors.Services;
using AnimeActors.Views;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using ReactiveUI;

namespace AnimeActors.ViewModels
{
    public class ItemsViewModel : ReactiveObject, IActivatableViewModel
    {
        public const string emptyImage =
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==";
        public string Title => "Anime Actors";
        public bool IsBusy { get; set; }
        public string SearchText { get; set; }
        public ICommand SearchCommand { get; set; }
        private SourceCache<Edge, int> _cache;
        public IObservableCollection<Item> Items { get; set; }

        private readonly AnilistService _anilistService = new AnilistService();

        public ItemsViewModel()
        {
            Items = new ObservableCollectionExtended<Item>();
            _cache = new SourceCache<Edge, int>(actor => actor.id);
            _cache
                .Connect()
                .TransformMany(mediaEdges => mediaEdges.voiceActors
                    .SelectMany(voiceActor => voiceActor.characters.nodes
                        .SelectMany(character => character.media.edges
                            .Select(characterMedia => new Item
                            {
                                VoiceActor = voiceActor.name.full,
                                Text = characterMedia.node.title.userPreferred,
                                CharacterName = character.name.full,
                                Id = characterMedia.node.title.userPreferred + character.name.full,
                                Image = ImageSource.FromUri(new Uri(character.image?.large ?? emptyImage))
                            })
                        )
                    )

                    , c => c.CharacterName)
                .Bind(Items)
                .Subscribe();


            Activator = new ViewModelActivator();
            SearchCommand = ReactiveCommand.CreateFromTask(() => ExecuteLoadItemsCommand(SearchText));
        }

        async Task ExecuteLoadItemsCommand(string characterName)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var items = await _anilistService.GetVoiceActorByCharacter(characterName);
                _cache.Clear();
                _cache.AddOrUpdate(items);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ViewModelActivator Activator { get; }
    }
}