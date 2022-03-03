using AnimeActors.Helpers;
using Xamarin.Forms;

namespace AnimeActors.Models
{
    public class VoiceActorItem : IDefineJumpKey
    {
        public string Id { get; set; }
        public string AnimeName { get; set; }
        public string CharacterName { get; set; }
        public string VoiceActor { get; set; }
        public ImageSource Image { get; set; }
        public string JumpKey => AnimeName;
    }
}
