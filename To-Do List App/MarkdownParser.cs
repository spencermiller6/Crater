using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static To_Do_List_App.List;
using System.Reflection;
using System.Xml.Linq;

namespace To_Do_List_App
{
    public class ListParser
    {
        private List _list;
        private Section? _currentSection;
        private Group? _currentGroup;
        private Item? _currentItem;
        private Property? _currentProperty;
        private int? _currentOrdinalPosition;

        private Section CurrentSection
        {
            get { return _currentSection; }
            set
            {
                _currentSection = value;
                CurrentGroup = null;
            }
        }

        private Group? CurrentGroup
        {
            get { return _currentGroup; }
            set
            {
                _currentGroup = value;
                CurrentItem = null;
            }
        }

        private Item? CurrentItem
        {
            get { return _currentItem; }
            set
            {
                _currentItem = value;
                _currentProperty = null;
                _currentOrdinalPosition = null;
            }
        }

        public ListParser()
        {
            _list = new List();
            _currentSection = null;
            _currentGroup = null;
            _currentItem = null;
            _currentProperty = null;
            _currentOrdinalPosition = 0;
        }

        public List CreateFromFilepath(string filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            string? line;

            SetDefaultListProperties();

            try
            {
                line = reader.ReadLine();

                while (line != null)
                {
                    ParseLine2(line);
                    line = reader.ReadLine();
                }
            }
            finally
            {
                reader.Close();
            }

            return _list;
        }

        private void SetDefaultListProperties()
        {
            foreach (var propertyDefinition in ListTemplate.ListProperties)
            {
                Property property = new Property(propertyDefinition.Key, x => propertyDefinition.Value.Contains(x));
                _list.ListProperties.Add(property.Name, property);
            }

            Property itemProperties = new Property("Item");

            itemProperties.AddValue("Notes");
            itemProperties.AddValue("Date");

            _list.ListProperties.Add(itemProperties.Name, itemProperties);
        }

        public void ParseLine2(string line)
        {
            if (line.Length < 2)
            {
                Debug.WriteLine("No valid elements have a length less than two.");
                return;
            }

            int ordinalPosition = OrdinalPosition(line);
            string content = line.TrimStart();
            string value;

            if (content.Substring(0, 2) == "- ")
            {
                if (content.Length < 6)
                {
                    value = content.Substring(2);
                    ParseProperty2(value, ordinalPosition);
                }
                else
                {
                    switch (content.Substring(2, 4))
                    {
                        case "[x] " or "[X] ":
                            value = content.Substring(6);
                            ParseItem2(value, ordinalPosition, true);

                            break;
                        case "[ ] ":
                            value = content.Substring(6);
                            ParseItem2(value, ordinalPosition, false);

                            break;
                        default:
                            value = content.Substring(2);
                            ParseProperty2(value, ordinalPosition);
                            break;
                    }
                }
            }
            else if (content.Substring(0, 2) == "# ")
            {
                value = content.Substring(2);
                ParseSection2(value);
            }
            else if (content.Length >= 3 && content.Substring(0, 3) == "## ")
            {
                value = content.Substring(3);
                ParseGroup2(value);
            }
            else
            {
                Debug.WriteLine($"Unrecognized identifier in line: {content}");
                return;
            }
        }

        public void ParseItem2(string line, int ordinalPosition, bool isComplete)
        {
            Debug.WriteLine($"{ordinalPosition} Item ({isComplete}): {line}");

            Item item = new Item(line, isComplete);

            if (CurrentSection is null)
            {
                Section section = new Section("Incomplete");
                _list.Sections.Add(section.Name, section);
                CurrentSection = section;
            }

            if (CurrentGroup is null)
            {
                if (CurrentSection.Groups.ContainsKey("General"))
                {
                    CurrentGroup = CurrentSection.Groups["General"];
                }
                else
                {
                    Group group = new Group("General");
                    CurrentSection.Groups.Add(group.Name, group);
                    CurrentGroup = group;
                }
            }

            while (_currentOrdinalPosition > ordinalPosition)
            {
                CurrentItem = CurrentItem?.Parent;
                _currentOrdinalPosition--;
            }

            if (CurrentItem is not null && ordinalPosition > _currentOrdinalPosition)
            {
                item.Parent = CurrentItem;
                CurrentItem.AddChild(item);
            }
            else
            {
                CurrentGroup.Items.Add(item);
            }

            CurrentItem = item;
            _currentOrdinalPosition = ordinalPosition;

            return;
        }

        public void ParseProperty2(string line, int ordinalPosition)
        {
            Debug.WriteLine($"{ordinalPosition} Property: {line}");

            bool settingListProperties = _currentSection is null ? true : false;
            int separaterIndex = line.IndexOf(':');

            // If line represents merely a property value
            if (separaterIndex == -1)
            {
                if (_currentProperty is null || _currentOrdinalPosition >= ordinalPosition)
                {
                    Debug.WriteLine($"Can't set property value if property isn't specified.");
                    return;
                }

                _currentProperty.Values.Add(line);
            }

            // If line represents a property declaration, with or without a value
            else
            {
                string name = line.Substring(0, separaterIndex);
                string value = line.Substring(separaterIndex + 1).TrimStart();

                if (settingListProperties)
                {
                    if (!_list.ListProperties.ContainsKey(name))
                    {

                    }
                }

                if (!_list.ItemProperties.Exists(x => x.Name == name))
                {
                    Debug.WriteLine($"{name} is not a defined property.");
                    return;
                }

                if (CurrentItem is null)
                {
                    Debug.WriteLine($"Can't add a property if no item has been defined.");
                    return;
                }

                if (CurrentItem.Properties.ContainsKey(name))
                {
                    _currentProperty = CurrentItem.Properties[name];
                }
                else
                {
                    Property property = new Property(name);
                    _currentProperty = property;
                }

                if (value is not null)
                {
                    _currentProperty.AddValue(value);
                }
            }

            return;
        }

        public void ParseSection2(string line)
        {
            Debug.WriteLine($"Section: {line}");

            if (_list.Name is null)
            {
                _list.Name = line;
            }
            else if (_list.Sections.ContainsKey(line))
            {
                CurrentSection = _list.Sections[line];
            }
            else if (ListTemplate.Sections.Contains(line))
            {
                Section section = new Section(line);
                CurrentSection = section;
            }
            else
            {
                Debug.WriteLine($"{line} is not a recognized section name;");
                return;
            }

            return;
        }

        public void ParseGroup2(string line)
        {
            Debug.WriteLine($"Group: {line}");

            if (_currentSection.Groups.ContainsKey(line))
            {
                _currentGroup = _currentSection.Groups[line];
            }
            else
            {
                Group group = new Group(line);
                _currentGroup = group;
            }

            return;
        }

        private int OrdinalPosition(string line)
        {
            int whitespaceCharacters = line.Length - line.TrimStart().Length;

            string whitespace = line.Substring(0, whitespaceCharacters);
            string tabsOnly = whitespace.Replace("    ", "\t");

            int tabCount = 0;

            foreach (char c in tabsOnly)
            {
                if (c == '\t') tabCount++;
            }

            return tabCount;
        }

        //public void ParseLine(string line)
        //{
        //    LineIdentifier identifier = GetIdentifier(line.TrimStart());

        //    if (identifier == LineIdentifier.None) return;

        //    List<string> substrings = SplitLine(line, identifier);
        //    int ordinalPosition;
        //    string propertyName;
        //    string value;

        //    switch (identifier)
        //    {
        //        case LineIdentifier.CompleteItem:
        //            _currentProperty = null;
        //            ordinalPosition = OrdinalPosition(substrings[0]);
        //            value = substrings[1];

        //            ParseItem(value, true, ordinalPosition);
        //            break;
        //        case LineIdentifier.IncompleteItem:
        //            _currentProperty = null;
        //            ordinalPosition = OrdinalPosition(substrings[0]);
        //            value = substrings[1];

        //            ParseItem(value, false, ordinalPosition);
        //            break;
        //        case LineIdentifier.Property:
        //            ordinalPosition = OrdinalPosition(substrings[0]);
        //            propertyName = substrings[1];
        //            value = substrings[2];

        //            //ParseProperty(ordinalPosition, propertyName, value);
        //            break;
        //        case LineIdentifier.SectionHeader:
        //            _currentItem = null;
        //            _currentProperty = null;
        //            value = substrings[0];
        //            //ParseSectionHeader(value);

        //            break;
        //        case LineIdentifier.ListHeader:
        //            _currentGroup = null;
        //            _currentItem = null;
        //            _currentProperty = null;
        //            value = substrings[0];
        //            //ParseListHeader(value);

        //            break;
        //    }
        //}

        //private LineIdentifier GetIdentifier(string line)
        //{
        //    try
        //    {
        //        switch (line.Substring(0, 2))
        //        {
        //            case "- ":
        //                switch (line.Substring(2, 4))
        //                {
        //                    case "[x] ":
        //                        return LineIdentifier.CompleteItem;
        //                    case "[X] ":
        //                        return LineIdentifier.CompleteItem;
        //                    case "[ ] ":
        //                        return LineIdentifier.IncompleteItem;
        //                    default:
        //                        return LineIdentifier.Property;
        //                }
        //            case "##":
        //                if (line[2] != ' ') goto default;
        //                return LineIdentifier.SectionHeader;
        //            case "# ":
        //                return LineIdentifier.ListHeader;
        //            default:
        //                return LineIdentifier.None;
        //        }
        //    }
        //    catch
        //    {
        //        return LineIdentifier.None;
        //    }
        //}

        ///// <summary>
        ///// Splits a line of markdown into its relevant tokens
        ///// </summary>
        ///// <param name="line">A line of markdown to be split</param>.
        ///// <param name="identifier">The identifier representing the type of parsed element that's being split</param>
        ///// <returns>A list of strings containing the various tokens, with its composition depending on the identifier:</returns>
        //private List<string> SplitLine(string line, LineIdentifier identifier)
        //{
        //    List<string> substrings = new List<string>();
        //    int index;

        //    switch (identifier)
        //    {
        //        case LineIdentifier.CompleteItem:
        //            index = line.IndexOf('-');

        //            substrings.Add(line.Substring(0, index)); // Substing 0: Indentation
        //            substrings.Add(line.Substring(index + 6)); // Substring 1: Item name

        //            break;
        //        case LineIdentifier.IncompleteItem:
        //            index = line.IndexOf('-');

        //            substrings.Add(line.Substring(0, index)); // Substing 0: Indentation
        //            substrings.Add(line.Substring(index + 6)); // Substing 1: Item name

        //            break;
        //        case LineIdentifier.Property:
        //            index = line.IndexOf('-');
        //            substrings.Add(line.Substring(0, index)); // Substing 0: Indentation

        //            int index2 = line.IndexOf(':');

        //            // If line represents merely a property value
        //            if (index2 == -1)
        //            {
        //                substrings.Add(_currentProperty); //Substing 1: Property name
        //                substrings.Add(line.Substring(index + 2)); // Substing 2: Property value
        //            }

        //            // If line represents a property declaration, with or without a value
        //            else
        //            {
        //                substrings.Add(line.Substring(index + 2, index2 - index - 2)); // Substing 1: Property name

        //                string substring = line.Substring(index2 + 1);
        //                substrings.Add(substring.TrimStart()); // Substing 2: Property value
        //            }

        //            break;
        //        case LineIdentifier.SectionHeader:
        //            index = line.IndexOf('#');
        //            substrings.Add(line.Substring(index + 3)); // Substing 0: Section name

        //            break;
        //        case LineIdentifier.ListHeader:
        //            index = line.IndexOf('#');
        //            substrings.Add(line.Substring(index + 2)); // Substing 0: Indentation

        //            break;
        //    }

        //    return substrings;
        //}

        //private void ParseItem(string value, bool isCompleted, int ordinalPosition)
        //{
        //    Item item = new Item(value, isCompleted);

        //    while (ordinalPosition <= _currentOrdinalPosition && _currentOrdinalPosition > 0)
        //    {
        //        _currentItem = _currentItem.Parent;
        //        _currentOrdinalPosition--;
        //    }

        //    // If the item's ordinal position is 0, add the new item directly to the current section
        //    if (ordinalPosition == 0)
        //    {
        //        // If there is no defined section, create a general one. The name general is quasi-reserved and can be user-defined,
        //        // however unsorted items will also be added to this section. If there are no other sections defined, its title will
        //        // not be shown as there is no need to descern between non-existant sections.
        //        if (_currentGroup is null)
        //        {
        //            Group section = new Group("General");

        //            _list.Groups.Add(section);
        //            _currentGroup = section;
        //        }

        //        _currentGroup.Items.Add(item);
        //    }
        //    // Otherwise, add the new item as a child of the current one
        //    else
        //    {
        //        if (_currentItem is null)
        //        {
        //            throw new Exception("Can't add item to missing parent.");
        //        }

        //        item.Parent = _currentItem;
        //        _currentItem.Children.Add(item);
        //    }

        //    _currentItem = item;
        //    _currentOrdinalPosition = ordinalPosition;
        //}

        //private void ParseProperty(int ordinalPosition, string propertyName, string value)
        //{
        //    // If declaring list properties
        //    if (_currentSection is null && _currentListSection is null)
        //    {
        //        if (_currentProperty is null && propertyName == "Item")
        //        {
        //            _currentProperty = propertyName;

        //            if (value is not null)
        //            {
        //                throw new Exception("Can't assign property value inline with the declaration of item properties.");
        //            }
        //        }

        //        if (_currentProperty == "Item")
        //        {
        //            // set item property
        //            return;
        //        }

        //        if (ToDoList.MasterPropertyList.ContainsKey(propertyName))
        //        {
        //            if (ToDoList.MasterPropertyList[propertyName].Contains(value))
        //            {

        //            }

        //            return;
        //        }
        //        else
        //        {
        //            throw new Exception($"{propertyName} is not a recognized list property.");
        //        }
        //    }


        //    // If beginning the declaration of item properties
        //    if (propertyName == "Item" && _currentSection is null && _currentListSection is null)
        //    {
        //        _currentProperty = propertyName;

        //        if (value is not null)
        //        {
        //            throw new Exception("Can't assign property value inline with the declaration of item properties.");
        //        }
        //    }

        //    // If adding a property to the item template
        //    else if (_currentProperty == "Item" && )




        //    // Add property to current item
        //    if (_currentItem is not null && ordinalPosition == _currentOrdinalPosition + 1)
        //    {
        //    }

        //    // Add property to current property
        //    else if (ordinalPosition == _currentOrdinalPosition + 1)
        //    {
        //        if (_currentItem != null && ordinalPosition == 1)
        //        {
        //            if (_list.ItemProperties.ContainsKey(propertyName))
        //            {
        //                throw new Exception($"A property with the name {propertyName} is already defined.");
        //            }

        //            GetTypeParameters(value, out ListItem.ItemType itemType, out ListItem.ItemCollection itemCollection);
        //            _list.ItemProperties.Add(propertyName, (itemType, itemCollection));
        //            // TODO: find a place to reset the _isCurrentlySettingItemProperties flag once done setting item properties
        //        }
        //    }

        //    // Add property to list
        //    else if (_currentSection is null && _currentListSection is null && ordinalPosition == 0)
        //    {
        //        // Check if defining item properties
        //        if (propertyName == "Item")
        //        {
        //            _currentProperty = "Item";
        //        }

        //        // Check if property name and value are valid
        //        else if (MasterPropertyList.ContainsKey(propertyName) && MasterPropertyList[propertyName].Contains(value))
        //        {
        //            _list.ListProperties.Add(propertyName, value);
        //        }
        //    }
        //}

        //private void ParseListHeader(string value)
        //{
        //    if (_list.Name is null)
        //    {
        //        _list.Name = value;
        //    }
        //    else if (value == "Active")
        //    {
        //        _currentListSection = "Active";
        //    }
        //    else if (value == "Completed")
        //    {
        //        _currentListSection = "Completed";
        //    }
        //}

        //private void ParseSectionHeader(string value)
        //{
        //    if (_list.Sections.Any(section => section.Name == value))
        //    {
        //        _currentSection = _list.Sections.Find(section => section.Name == value);
        //    }
        //    else
        //    {
        //        ListSection section = new ListSection(value);
        //        _list.Sections.Add(section);
        //        _currentSection = section;
        //    }
        //}

        //private void GetTypeParameters(string input, out ListItem.ItemType itemType, out ListItem.ItemCollection itemCollection)
        //{
        //    string[] substrings = input.Split(' ');
        //    string potentialItemType;
        //    string potentialItemCollection;

        //    switch (substrings.Length)
        //    {
        //        // input looks like "string"
        //        case 1:
        //            potentialItemType = substrings[0];
        //            potentialItemCollection = "single";

        //            break;

        //        // input looks like "list string"
        //        case 2:
        //            potentialItemType = substrings[1];
        //            potentialItemCollection = "unordered " + substrings[0];

        //            break;

        //        // input looks like "unordered list string" or "ordered list string"
        //        case 3:
        //            potentialItemType = substrings[2];
        //            potentialItemCollection = substrings[0] + " " + substrings[1];

        //            break;
        //        default:
        //            throw new Exception($"Unrecognized data type: {input}");
        //    }

        //    switch (potentialItemType)
        //    {
        //        case "string":
        //            itemType = ListItem.ItemType.String;
        //            break;
        //        case "int":
        //            itemType = ListItem.ItemType.Int;
        //            break;
        //        case "date":
        //            itemType = ListItem.ItemType.Date;
        //            break;
        //        case "bool":
        //            itemType = ListItem.ItemType.Bool;
        //            break;
        //        default:
        //            throw new Exception($"Unrecognized data type: {input}");
        //    }

        //    switch (potentialItemCollection)
        //    {
        //        case "single":
        //            itemCollection = ListItem.ItemCollection.Single;
        //            break;
        //        case "unordered list":
        //            itemCollection = ListItem.ItemCollection.UnorderedList;
        //            break;
        //        case "ordered list":
        //            itemCollection = ListItem.ItemCollection.OrderedList;
        //            break;
        //        default:
        //            throw new Exception($"Unrecognized data type: {input}");
        //    }
        //}
    }

    public static class ListTemplate
    {
        public static readonly Dictionary<string, List<string>> ListProperties;
        public static readonly List<string> Sections;

        static ListTemplate()
        {
            ListProperties = new Dictionary<string, List<string>>()
            {
                { "Type", new List<string>() { "Standard", "Template" } },
                { "Completion", new List<string>() { "Immediate", "Long-Term", "Disabled" } },
                { "Completed Items", new List<string>() { "Enabled", "Disabled" } },
                { "Children", new List<string>() { "Enabled", "Disabled" } },
                { "Notes", new List<string>() { "Enabled", "Disabled" } },
                { "Date", new List<string>() { "Enabled", "Disabled" } },
                { "Priority", new List<string>() { "Enabled", "Disabled" } }
            };

            Sections = new List<string>()
            {
                "Incomplete",
                "Complete",
                "Recurring",
                "Template"
            };
        }
    }
}
