using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace To_Do_List_App
{
    public class List
    {
        public string? Name;
        public Dictionary<string, ItemProperty> Properties;
        public Dictionary<string, Section> Sections;

        public List()
        {
            Properties = new Dictionary<string, ItemProperty>();
            Sections = new Dictionary<string, Section>();
        }
    }

    public class Section
    {
        public string Name;
        public Dictionary<string, Group> Groups;

        public Section(string name)
        {
            Name = name;
            Groups = new Dictionary<string, Group>();
        }
    }

    public class Group
    {
        public string Name;
        public List<Item> Items;

        public Group(string name)
        {
            Name = name;
            Items = new List<Item>();
        }
    }

    public class Item
    {
        public string Name;
        public bool IsComplete;
        public bool IsStarred;
        public DateTime Date;
        public Dictionary<string, ItemProperty> Properties;
        public Item? Parent;
        public List<Item>? Children;

        public Item(string name, bool isComplete)
        {
            Name = name;
            IsComplete = isComplete;
            IsStarred = false;
            Properties = new Dictionary<string, ItemProperty>();
            Children = new List<Item> { };
        }

        public void AddChild(Item item)
        {
            if (Children is null)
            {
                Children = new List<Item>();
            }

            Children.Add(item);
        }
    }

    public delegate bool ValueCheck(string value);
}
