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

        public void ParseLine(string line)
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
                    ParseProperty(value, ordinalPosition);
                }
                else
                {
                    switch (content.Substring(2, 4))
                    {
                        case "[x] " or "[X] ":
                            value = content.Substring(6);
                            ParseItem(value, ordinalPosition, true);

                            break;
                        case "[ ] ":
                            value = content.Substring(6);
                            ParseItem(value, ordinalPosition, false);

                            break;
                        default:
                            value = content.Substring(2);
                            ParseProperty(value, ordinalPosition);
                            break;
                    }
                }
            }
            else if (content.Substring(0, 2) == "# ")
            {
                value = content.Substring(2);
                ParseSection(value);
            }
            else if (content.Length >= 3 && content.Substring(0, 3) == "## ")
            {
                value = content.Substring(3);
                ParseGroup(value);
            }
            else
            {
                Debug.WriteLine($"Unrecognized identifier in line: {content}");
                return;
            }
        }

        public void ParseItem(string line, int ordinalPosition, bool isComplete)
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

        public void ParseProperty(string line, int ordinalPosition)
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

        public void ParseSection(string line)
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

        public void ParseGroup(string line)
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
