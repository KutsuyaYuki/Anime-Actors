using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using Xamarin.Forms;

namespace AnimeActors.ViewModels
{
    public class HomePageModel : ReactiveObject, IActivatableViewModel
    {
        public HomePageModel()
        {

        }

        public ViewModelActivator Activator { get; }
    }
}
