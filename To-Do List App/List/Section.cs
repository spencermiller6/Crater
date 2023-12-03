using System.Collections.Generic;

namespace To_Do_List_App.List
{
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
}
