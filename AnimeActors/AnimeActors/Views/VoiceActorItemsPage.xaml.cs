using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AnimeActors.Models;
using AnimeActors.Views;
using AnimeActors.ViewModels;
using DynamicData.Binding;
using System.Reactive.Linq;
using ReactiveUI;
using System.Reactive.Disposables;

namespace AnimeActors.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class VoiceActorItemsPage
    {
        VoiceActorItemsViewModel viewModel;

        public VoiceActorItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new VoiceActorItemsViewModel();
            this.WhenActivated(disposed => {
            });
            
        }
    }
}