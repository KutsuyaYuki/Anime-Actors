﻿using System;
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
    public class ItemsViewModel : ReactiveObject, IActivatableViewModel
    {
        public const string emptyImage =
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==";
        public string Title => "Anime Actors";
        [Reactive]
        public bool IsBusy { get; set; }
        public string SearchText { get; set; }
        [Reactive]
        public int ResultsAmount { get; set; }
        [Reactive]
        public CharacterItem itemToScrollTo { get; set; }

        [Reactive]
        public ObservableCollection<char> jumpLetterList { get; set;}
        [Reactive]
        public ReactiveCommand<char, Unit> jumpLetterTapCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        private SourceCache<OriginItem, int> _cache;
        public IObservableCollection<CharacterItem> Items { get; set; }

        private readonly AnilistService _anilistService = new AnilistService();

        public ItemsViewModel()
        {
            jumpLetterList = new ObservableCollection<char>();
            jumpLetterList.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());
            jumpLetterTapCommand = ReactiveCommand.Create<char>(jumpLetterTapTask);
            Items = new ObservableCollectionExtended<CharacterItem>();

            Activator = new ViewModelActivator();

            this.WhenActivated(async disposed =>
            {
                var SortByAnime = bool.Parse((await Xamarin.Essentials.SecureStorage.GetAsync("SortByAnime")) ?? "false");

                _cache = new SourceCache<OriginItem, int>(actor => actor.Id);
                SortExpressionComparer<CharacterItem> CharacterNameComparer = SortExpressionComparer<CharacterItem>.Ascending(i => i.CharacterName);
                SortExpressionComparer<CharacterItem> AnimeComparer = SortExpressionComparer<CharacterItem>.Ascending(i => i.Text);
                _cache
                  .Connect()
                  .Select(oi => new CharacterItem
                  {
                      VoiceActor = oi.VoiceActor.name.full,
                      Text = oi.Anime.title.userPreferred,
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

        private void jumpLetterTapTask(char arg)
        {
            var ltjt = Items.Select(c => c.CharacterName.FirstOrDefault()).Min(i => (Math.Abs(arg - i), i)).i;
            itemToScrollTo = Items.FirstOrDefault(item => item.CharacterName.StartsWith(ltjt));
        }

        async Task ExecuteLoadItemsCommand(string characterName)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                _cache.Clear();
                await foreach (var item in _anilistService.GetVoiceActorByCharacter(characterName))
                {
                    var c = (await item).SelectMany(a => a.voiceActors
                        .SelectMany(voiceActor => voiceActor.characters.nodes
                          .SelectMany(character => character.media.edges
                              .Select(characterMedia => new OriginItem(
                                character.id,
                                voiceActor,
                                characterMedia.node,
                                character
                              ))
                          )
                      ));
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

    internal class OriginItem
    {
        public int Id { get; }
        public Models.VoiceActors.VoiceActor VoiceActor { get; }
        public Models.VoiceActors.AnimeNode Anime { get; }
        public Models.VoiceActors.VoiceActorNode Character { get; }

        public OriginItem(
            int id, 
            Models.VoiceActors.VoiceActor voiceActor, 
            Models.VoiceActors.AnimeNode anime, 
            Models.VoiceActors.VoiceActorNode character)
        {
            Id = id;
            VoiceActor = voiceActor;
            Anime = anime;
            Character = character;
        }
    }
}