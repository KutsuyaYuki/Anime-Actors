using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeActors.Models
{
    public class PageInfo
    {
        public int total { get; set; }
        public int currentPage { get; set; }
        public int lastPage { get; set; }
        public bool hasNextPage { get; set; }
        public int perPage { get; set; }
    }

    public class CharacterName
    {
        public string first { get; set; }
        public string last { get; set; }
        public string full { get; set; }
        public string native { get; set; }
    }

    public class Image
    {
        public string large { get; set; }
    }

    public class Title
    {
        public string userPreferred { get; set; }
    }

    public class Node
    {
        public string type { get; set; }
        public Title title { get; set; }
    }

    public class Edge
    {
        public Node node { get; set; }
        public int id { get; set; }
        public string characterRole { get; set; }
        public List<VoiceActor> voiceActors { get; set; }
    }

    public class Media
    {
        public List<Edge> edges { get; set; }
    }

    public class Character
    {
        public CharacterName name { get; set; }
        public Image image { get; set; }
        public Media media { get; set; }
    }

    public class Data
    {
        public Page page { get; set; }
    }

    public class Page
    {
        public PageInfo pageInfo { get; set; }
        public List<Character> characters { get; set; }
    }

    public class AnimeName
    {
        public string full { get; set; }
    }


    public class AnimeNode
    {
        public string type { get; set; }
        public Title title { get; set; }
    }

    public class VoiceActorEdge
    {
        public AnimeNode node { get; set; }
    }

    public class VoiceActorMedia
    {
        public List<VoiceActorEdge> edges { get; set; }
    }

    public class VoiceActorNode
    {
        public AnimeName name { get; set; }
        public Image image { get; set; }
        public VoiceActorMedia media { get; set; }
    }

    public class Characters
    {
        public List<VoiceActorNode> nodes { get; set; }
    }

    public class VoiceActor
    {
        public AnimeName name { get; set; }
        public Image image { get; set; }
        public string language { get; set; }
        public Characters characters { get; set; }
    }
}

