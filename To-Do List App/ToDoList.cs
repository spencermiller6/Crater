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
        private int _currentOrdinalPosition;

        public ListParser()
        {
            _list = new ToDoList();
            _currentListSection = "";
            _currentSection = null;
            _currentItem = null;
            _currentOrdinalPosition = 0;
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

                    ParseItem(value, true, ordinalPosition);
                    break;
                case LineIdentifier.IncompleteItem:
                    ordinalPosition = GetOrdinalPosition(substrings[0]);
                    value = substrings[1];

                    ParseItem(value, false, ordinalPosition);
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

        private LineIdentifier GetIdentifier(string line)
        {
            switch (line.Substring(0, 2))
            {
                case "- ":
                    switch (line.Substring(2, 4))
                    {
                        case "[x] ":
                            return LineIdentifier.CompleteItem;
                        case "[X] ":
                            return LineIdentifier.CompleteItem;
                        case "[ ] ":
                            return LineIdentifier.IncompleteItem;
                        default:
                            return LineIdentifier.Property;
                    }
                case "##":
                    if (line[2] != ' ') goto default; //TODO: might throw error
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

        private void ParseItem(string value, bool isCompleted, int ordinalPosition)
        {
            ListItem item = new ListItem(value, isCompleted);

            while (ordinalPosition <= _currentOrdinalPosition)
            {
                _currentItem = _currentItem.Parent;
                _currentOrdinalPosition--;
            }

            // If the current item's index is 0, add the new item directly to the current section
            if (_currentOrdinalPosition == 0)
            {
                //TODO: need to make sure there is a current section, move this logic to a method
                _currentSection.Items.Add(item);
            }
            // Otherwise, add the new item as a child of the current one
            else
            {
                _currentItem.Children.Add(item);
            }

            _currentItem = item;
            _currentOrdinalPosition = ordinalPosition;
        }

        private void ParseProperty(int ordinalPosition, string propertyName, string value)
        {

        }

        private void ParseListHeader(string value)
        {
            if (_list.Name is null)
            {
                _list.Name = value;
            }
            else if (value == "Active")
            {
                _currentListSection = "Active";
            }
            else if (value == "Completed")
            {
                _currentListSection = "Completed";
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
