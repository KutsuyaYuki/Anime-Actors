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

namespace AnimeActors.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage
    {
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();

            viewModel
                .WhenPropertyChanged(c => c.itemToScrollTo)
                .Where(c => c != null)
                .Subscribe(c => Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => ItemsListView?.ScrollTo(c.Value, animate: false, position: ScrollToPosition.Start)));
        }
    }
}