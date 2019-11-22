using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AnimeActors.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage
    {
        public HomePage()
        {
            InitializeComponent();
            Routing.RegisterRoute("characters", typeof(ItemsPage));
        }

        private async void OnCharacterButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("characters");
        }
    }
}