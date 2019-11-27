using System;
using System.Collections.Generic;
using System.Text;
using AnimeActors.Views;
using ReactiveUI;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AnimeActors.ViewModels
{
    public class HomePageModel : ReactiveObject, IActivatableViewModel
    {
        public string Changelog { get; set; }
        public HomePageModel()
        {

            Routing.RegisterRoute("characters", typeof(ItemsPage));
            Routing.RegisterRoute("characters/details", typeof(ItemDetailPage));

            Changelog = VersionTracking.CurrentVersion;
        }

        public ViewModelActivator Activator { get; }
    }
}
