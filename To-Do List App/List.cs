using Markdig.Extensions.TaskLists;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using static System.Collections.Specialized.BitVector32;

namespace To_Do_List_App
{
    public class List
    {
        string Name;
        Dictionary<string, string> Properties;
        List<Section> Sections;

        public List(string name)
        {
            Name = name;
            Properties = new Dictionary<string, string>();
            Sections = new List<Section>();
        }
    }

    public class Section
    {
        public string Name;
        public List<Item> Items;

        public Section(string name)
        {
            Name = name;
            Items = new List<Item>();
        }
    }

    public class Item
    {
        public string Name;
        public Dictionary<string, string> Properties;
        public List<Item> Children;

        public Item(string name)
        {
            Name = name;
            Properties = new Dictionary<string, string>();
            Children = new List<Item>();
        }
    }
}
