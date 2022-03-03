using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeActors.Models.Staff
{

    public class Rootobject
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public Page Page { get; set; }
    }

    public class Page
    {
        public Pageinfo pageInfo { get; set; }
        public Staff[] staff { get; set; }
    }

    public class Pageinfo
    {
        public int total { get; set; }
        public int currentPage { get; set; }
        public int lastPage { get; set; }
        public bool hasNextPage { get; set; }
        public int perPage { get; set; }
    }

    public class Staff
    {
        public Name name { get; set; }
        public Image image { get; set; }
        public Characters characters { get; set; }
    }

    public class Name
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

    public class Characters
    {
        public Pageinfo1 pageInfo { get; set; }
        public Node[] nodes { get; set; }
    }

    public class Pageinfo1
    {
        public int total { get; set; }
        public int currentPage { get; set; }
        public int lastPage { get; set; }
        public bool hasNextPage { get; set; }
        public int perPage { get; set; }
    }

    public class Node
    {
        public Name1 name { get; set; }
        public Image image { get; set; }
        public Media media { get; set; }
    }

    public class Name1
    {
        public string full { get; set; }
    }

    public class Media
    {
        public Node1[] nodes { get; set; }
    }

    public class Node1
    {
        public int id { get; set; }
        public Title title { get; set; }
    }

    public class Title
    {
        public string userPreferred { get; set; }
    }

}
