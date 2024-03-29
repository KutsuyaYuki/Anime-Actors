﻿using AnimeActors.Helpers;
using System;
using Xamarin.Forms;

namespace AnimeActors.Models
{
    public class CharacterItem : IDefineJumpKey
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string CharacterName { get; set; }
        public string VoiceActor{ get; set; }
        public ImageSource Image { get; set; }
        public string JumpKey => Text;
    }
}