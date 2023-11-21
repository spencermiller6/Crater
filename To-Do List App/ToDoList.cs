using System.Collections.Generic;
using System.IO;
using System.Linq;
using static To_Do_List_App.ToDoList;

namespace To_Do_List_App
{
    public class ToDoList
    {
        public string? Name;
        public Dictionary<string, string> Properties;
        public List<ListSection> Sections;

        public ToDoList()
        {
            Properties = new Dictionary<string, string>();
            Sections = new List<ListSection>();
        }

        public enum LineIdentifier
        {
            ListHeader,
            SectionHeader,
            IncompleteItem,
            CompleteItem,
            Property,
            None
        }
    }

    public class ListSection
    {
        public string Name;
        public List<ListItem> Items;

        public ListSection(string name)
        {
            Name = name;
            Items = new List<ListItem>();
        }
    }

    public class ListItem
    {
        public string Name;
        public bool IsComplete;
        public Dictionary<string, string> BuiltInProperties;
        public Dictionary<string, string> CustomProperties;
        public ListItem? Parent;
        public List<ListItem>? Children;

        public ListItem(string name, bool isComplete)
        {
            Name = name;
            IsComplete = isComplete;
            BuiltInProperties = new Dictionary<string, string>();
            CustomProperties = new Dictionary<string, string>();
            Children = new List<ListItem> { };
        }
    }

    public class ListParser
    {
        private ToDoList _list;
        private string _currentListSection;
        private ListSection? _currentSection;
        private ListItem? _currentItem;
        private int _currentIndex;

        public ListParser()
        {
            _list = new ToDoList();
            _currentListSection = "";
            _currentSection = null;
            _currentItem = null;
            _currentIndex = 0;
        }

        public ToDoList CreateFromFilepath(string filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            string? line;

            try
            {
                line = reader.ReadLine();

                while (line != null)
                {
                    ParseLine(line);
                    line = reader.ReadLine();
                }
            }
            finally
            {
                reader.Close();
            }

            return _list;
        }

        public void ParseLine(string line)
        {
            LineIdentifier identifier = GetIdentifier(line.TrimStart());

            if (identifier == LineIdentifier.None) return;

            List<string> substrings = SplitLine(line, identifier);
            int ordinalPosition;
            string propertyName;
            string value;

            switch (identifier)
            {
                case LineIdentifier.CompleteItem:
                    ordinalPosition = GetOrdinalPosition(substrings[0]);
                    value = substrings[1];

                    ParseItem(ordinalPosition, true, value);
                    break;
                case LineIdentifier.IncompleteItem:
                    ordinalPosition = GetOrdinalPosition(substrings[0]);
                    value = substrings[1];

                    ParseItem(ordinalPosition, false, value);
                    break;
                case LineIdentifier.Property:
                    ordinalPosition = GetOrdinalPosition(substrings[0]);
                    propertyName = substrings[1];
                    value = substrings[2];

                    ParseProperty(ordinalPosition, propertyName, value);
                    break;
                case LineIdentifier.SectionHeader:
                    value = substrings[0];
                    ParseSectionHeader(value);

                    break;
                case LineIdentifier.ListHeader:
                    value = substrings[0];
                    ParseListHeader(value);

                    break;
            }
        }

        //        if (Sections.Any(section => section.Name == value))
        //{
        //    currentSection = Sections.Find(section => section.Name == value);
        //}
        //else
        //{
        //    ListSection section = new ListSection(value);
        //    Sections.Add(section);
        //    currentSection = section;
        //}

        private LineIdentifier GetIdentifier(string line)
        {
            switch (line.Substring(0, 2))
            {
                case "- ":
                    switch (line.Substring(0, 6))
                    {
                        case "- [x] ":
                            return LineIdentifier.CompleteItem;
                        case "- [X] ":
                            return LineIdentifier.CompleteItem;
                        case "- [ ] ":
                            return LineIdentifier.IncompleteItem;
                        default:
                            return LineIdentifier.Property;
                    }
                case "##":
                    if (line[2] != ' ') goto default;
                    return LineIdentifier.SectionHeader;
                case "# ":
                    return LineIdentifier.ListHeader;
                default:
                    return LineIdentifier.None;
            }
        }

        private List<string> SplitLine(string line, LineIdentifier identifier)
        {
            List<string> substrings = new List<string>();
            int index;

            switch (identifier)
            {
                case LineIdentifier.CompleteItem:
                    index = line.IndexOf('-');

                    substrings.Add(line.Substring(0, index));
                    substrings.Add(line.Substring(index + 6));

                    break;
                case LineIdentifier.IncompleteItem:
                    index = line.IndexOf('-');

                    substrings.Add(line.Substring(0, index));
                    substrings.Add(line.Substring(index + 6));

                    break;
                case LineIdentifier.Property:
                    index = line.IndexOf('-');
                    substrings.Add(line.Substring(0, index));

                    index = line.IndexOf(':');
                    substrings.Add(line.Substring(index));

                    break;
                case LineIdentifier.SectionHeader:
                    index = line.IndexOf('#');
                    substrings.Add(line.Substring(index + 3));

                    break;
                case LineIdentifier.ListHeader:
                    index = line.IndexOf('#');
                    substrings.Add(line.Substring(index + 2));

                    break;
            }

            return substrings;
        }

        private int GetOrdinalPosition(string line)
        {
            string tabsOnly = line.Replace("    ", "\t");
            int tabCount = 0;

            foreach (char c in tabsOnly)
            {
                if (c == '\t') tabCount++;
            }

            return tabCount;
        }

        private void ParseItem(int ordinalPosition, bool isCompleted, string value)
        {

        }

        private void ParseProperty(int ordinalPosition, string propertyName, string value)
        {

        }

        private void ParseListHeader(string value)
        {
            switch (value)
            {
                case null:
                    break;
                case "Active":
                    break;
                case "Completed":
                    break;
            }
        }

        private void ParseSectionHeader(string value)
        {
            if (_list.Sections.Any(section => section.Name == value))
            {
                _currentSection = _list.Sections.Find(section => section.Name == value);
            }
            else
            {
                ListSection section = new ListSection(value);
                _list.Sections.Add(section);
                _currentSection = section;
            }
        }
    }
}
