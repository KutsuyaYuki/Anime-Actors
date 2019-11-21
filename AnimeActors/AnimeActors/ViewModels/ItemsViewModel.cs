using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
                .TransformMany(c =>
                    c.voiceActors
                        .SelectMany(h => h.characters.nodes)
                        .SelectMany(j =>
                            j.media.edges.Select(h => { 
                                var item = new Item
                                {
                                    Text = h.node.title.userPreferred,
                                    Description = j.name.full,
                                    Id = h.node.title.userPreferred+j.name.full,
                                    Image = ImageSource.FromUri(new Uri(j.image?.large ?? "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg=="))
                                };

                                return item;
                            })
                    ), c => c.Id)
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
                _cache.AddOrUpdate(items.Character.media.edges);
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