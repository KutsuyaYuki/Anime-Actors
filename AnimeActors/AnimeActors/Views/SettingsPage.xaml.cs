using AnimeActors.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AnimeActors.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ReactiveContentPage<SettingsViewModel>
    {
        public SettingsPage()
        {
            InitializeComponent();

            this.ViewModel = new SettingsViewModel();

            this.WhenActivated(disposables =>
            {
                this.Bind(this.ViewModel, vm => vm.SortByAnime, v => v.SortByAnime.IsToggled).DisposeWith(disposables);
            });
        }
    }
}