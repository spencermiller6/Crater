using System.Collections.Generic;

namespace Crater.Models
{
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
}
