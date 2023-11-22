using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using static To_Do_List_App.ToDoList;

namespace To_Do_List_App
{
    public class ToDoList
    {
        public string? Name;
        public Dictionary<string, string> ListProperties;
        public Dictionary<string, (ItemType, ItemCollection)> ItemProperties;
        public List<ListSection> Sections;

        public ToDoList()
        {
            ListProperties = new Dictionary<string, string>();
            ItemProperties = new Dictionary<string, (ItemType, ItemCollection)>();
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

        public enum ItemType
        {
            String,
            Int,
            Date,
            Bool
        }

        public enum ItemCollection
        {
            Single,
            UnorderedList,
            OrderedList
        }

        public static Dictionary<string, List<string>> MasterPropertyList = new Dictionary<string, List<string>>()
        {
            { "Type", new List<string>() { "Standard", "Template" } },
            { "Completion", new List<string>() { "Immediate", "Long-Term", "Disabled" } },
            { "Completed Items", new List<string>() { "Enabled", "Disabled" } },
            { "Children", new List<string>() { "Enabled", "Disabled" } },
            { "Notes", new List<string>() { "Enabled", "Disabled" } },
            { "Date", new List<string>() { "Enabled", "Disabled" } },
            { "Priority", new List<string>() { "Enabled", "Disabled" } }
        };
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
        public Dictionary<string, object> BuiltInProperties;
        public Dictionary<string, object> CustomProperties;
        public ListItem? Parent;
        public List<ListItem>? Children;

        public ListItem(string name, bool isComplete)
        {
            Name = name;
            IsComplete = isComplete;
            BuiltInProperties = new Dictionary<string, object>();
            CustomProperties = new Dictionary<string, object>();
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
        private string? _currentProperty;
        private bool _isCurrentlySettingItemProperties;

        public ListParser()
        {
            _list = new ToDoList();
            _currentListSection = "";
            _currentSection = null;
            _currentItem = null;
            _currentOrdinalPosition = 0;
            _currentProperty = null;
            _isCurrentlySettingItemProperties = false;
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
            try
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
                        if (line[2] != ' ') goto default;
                        return LineIdentifier.SectionHeader;
                    case "# ":
                        return LineIdentifier.ListHeader;
                    default:
                        return LineIdentifier.None;
                }
            }
            catch
            {
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

                    int index2 = line.IndexOf(':');
                    substrings.Add(line.Substring(index + 2, index2 - index - 2));


                    string substring = line.Substring(index2 + 1);
                    substrings.Add(substring.TrimStart());

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

            while (ordinalPosition <= _currentOrdinalPosition && _currentOrdinalPosition > 0)
            {
                _currentItem = _currentItem.Parent;
                _currentOrdinalPosition--;
            }

            // If the item's ordinal position is 0, add the new item directly to the current section
            if (ordinalPosition == 0)
            {
                //TODO: need to make sure there is a current section, move this logic to a method
                _currentSection.Items.Add(item);
            }
            // Otherwise, add the new item as a child of the current one
            else
            {
                item.Parent = _currentItem;
                _currentItem.Children.Add(item);
            }

            _currentItem = item;
            _currentOrdinalPosition = ordinalPosition;
        }

        private void ParseProperty(int ordinalPosition, string propertyName, string value)
        {
            // Add property to current item
            if (_currentItem is not null && ordinalPosition == _currentOrdinalPosition + 1)
            {
            }

            // Add property to current property
            else if (ordinalPosition == _currentOrdinalPosition + 1)
            {
                if (_isCurrentlySettingItemProperties && ordinalPosition == 1)
                {
                    if (_list.ItemProperties.ContainsKey(propertyName))
                    {
                        throw new Exception($"A property with the name {propertyName} is already defined.");
                    }

                    GetTypeParameters(value, out ItemType itemType, out ItemCollection itemCollection);
                    _list.ItemProperties.Add(propertyName, (itemType, itemCollection));
                    // TODO: find a place to reset the _isCurrentlySettingItemProperties flag once done setting item properties
                }
            }

            // Add property to list
            else if (_currentSection is null && _currentListSection is null && ordinalPosition == 0)
            {
                // Check if defining item properties
                if (propertyName == "Item")
                {
                    _isCurrentlySettingItemProperties = true;
                }

                // Check if property name and value are valid
                else if (MasterPropertyList.ContainsKey(propertyName) && MasterPropertyList[propertyName].Contains(value))
                {
                    _list.ListProperties.Add(propertyName, value);
                }
            }
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

        private void GetTypeParameters(string input, out ItemType itemType, out ItemCollection itemCollection)
        {
            string[] substrings = input.Split(' ');
            string potentialItemType;
            string potentialItemCollection;

            switch (substrings.Length)
            {
                case 1:
                    potentialItemType = substrings[0];
                    potentialItemCollection = "single";

                    break;
                case 2:
                    potentialItemType = substrings[1];
                    potentialItemCollection = "unordered " + substrings[0];

                    break;
                case 3:
                    potentialItemType = substrings[2];
                    potentialItemCollection = substrings[0] + " " + substrings[1];

                    break;
                default:
                    throw new Exception($"Unrecognized data type: {input}");
            }

            switch (potentialItemType)
            {
                case "string":
                    itemType = ItemType.String;
                    break;
                case "int":
                    itemType = ItemType.Int;
                    break;
                case "date":
                    itemType = ItemType.Date;
                    break;
                case "bool":
                    itemType = ItemType.Bool;
                    break;
                default:
                    throw new Exception($"Unrecognized data type: {input}");
            }

            switch (potentialItemCollection)
            {
                case "single":
                    itemCollection = ItemCollection.Single;
                    break;
                case "unordered list":
                    itemCollection = ItemCollection.UnorderedList;
                    break;
                case "ordered list":
                    itemCollection = ItemCollection.OrderedList;
                    break;
                default:
                    throw new Exception($"Unrecognized data type: {input}");
            }
        }
    }
}
