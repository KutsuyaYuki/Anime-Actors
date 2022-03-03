using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimeActors.ViewModels;
using ReactiveUI;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AnimeActors.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : IActivatableView
    {
        HomePageModel viewModel;

        public HomePage()
        {
            InitializeComponent();

            BindingContext = viewModel = new HomePageModel();

            this.WhenActivated(disposables =>
            {

            });
        }

        private async void OnCharacterButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("characters");
        }
    }
}