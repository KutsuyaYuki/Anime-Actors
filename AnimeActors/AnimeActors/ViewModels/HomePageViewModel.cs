using System.Reactive;
using System.Windows.Input;
using AnimeActors.Views;
using ReactiveUI;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AnimeActors.ViewModels
{
    public class HomePageModel : ReactiveObject, IActivatableViewModel
    {
        public string Changelog { get; set; }
        public ICommand RouteCommand => ReactiveCommand.CreateFromTask<string>(async (parameter) => await Shell.Current.GoToAsync(parameter));
        public HomePageModel()
        {

            Routing.RegisterRoute("characters", typeof(ItemsPage));
            Routing.RegisterRoute("characters/details", typeof(ItemDetailPage));
            Routing.RegisterRoute("voiceactors", typeof(VoiceActorItemsPage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));

            Changelog = VersionTracking.CurrentVersion;
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
