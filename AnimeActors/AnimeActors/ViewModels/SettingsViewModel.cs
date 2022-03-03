using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace AnimeActors.ViewModels
{
    public class SettingsViewModel : ReactiveObject, IActivatableViewModel
    {
        [Reactive]
        public Boolean SortByAnime { get; set; }

        public ViewModelActivator Activator { get;  }

        public SettingsViewModel()
        {
            Activator = new ViewModelActivator();
            this.WhenActivated(async (disposable) =>
            {
                await HandleActivation();


                this.ObservableForProperty(x => x.SortByAnime)
                    .Subscribe(async newValue =>
                    {
                        await Xamarin.Essentials.SecureStorage.SetAsync("SortByAnime", newValue.GetValue().ToString());
                    })
                    .DisposeWith(disposable);

                Disposable
                    .Create(() => HandleDeactivation())
                    .DisposeWith(disposable);
            });
        }

        private async Task HandleActivation()
        {
            SortByAnime = bool.Parse((await Xamarin.Essentials.SecureStorage.GetAsync("SortByAnime")) ?? "false");
        }

        private void HandleDeactivation() { }
    }
}
