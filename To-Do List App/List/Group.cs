using System.Collections.Generic;

namespace To_Do_List_App.List
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
