using System.Collections.Generic;

namespace Crater.Models
{
    public class CraterList
    {
        public string? Name;
        public Dictionary<string, Property> Properties;
        public Dictionary<string, Section> Sections;

        public CraterList()
        {
            Properties = new Dictionary<string, Property>();
            Sections = new Dictionary<string, Section>();
        }
    }
}
