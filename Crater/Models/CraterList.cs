using Crater.Models.Properties;
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

            TextProperty notes = new TextProperty("Notes");
            TextProperty date = new TextProperty("Date"); // This will eventually be of type DateProperty

            Properties.Add(notes.Name, notes);
            Properties.Add(date.Name, date);
        }
    }
}
