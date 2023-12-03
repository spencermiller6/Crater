using System;
using System.Collections.Generic;

namespace Crater.Models
{
    public class Item
    {
        public string Name;
        public bool IsComplete;
        public bool IsStarred;
        public DateTime Date;
        public Dictionary<string, Property> Properties;
        public Item? Parent;
        public List<Item> Children;

        public Item(string name, bool isComplete)
        {
            Name = name;
            IsComplete = isComplete;
            IsStarred = false;
            Properties = new Dictionary<string, Property>();
            Children = new List<Item> { };
        }
    }
}
