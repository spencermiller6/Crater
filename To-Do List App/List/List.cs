using System.Collections.Generic;
using System;

namespace To_Do_List_App.List
{
    public class List
    {
        public string? Name;
        public Dictionary<string, Property> Properties;
        public Dictionary<string, Section> Sections;

        public List()
        {
            Properties = new Dictionary<string, Property>();
            Sections = new Dictionary<string, Section>();
        }
    }
}
