using System.Collections.Generic;

namespace Crater.Models
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
