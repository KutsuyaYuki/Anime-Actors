using System;

using AnimeActors.Models;

namespace AnimeActors.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public CharacterItem Item { get; set; }
        public ItemDetailViewModel(CharacterItem item = null)
        {
            Title = item?.Text;
            Item = item;
        }
    }
}
