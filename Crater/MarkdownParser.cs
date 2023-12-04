using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using Crater.Models;
using Crater.Models.Properties;

namespace Crater
{
    public class ListParser
    {
        private CraterList _list;
        private Section? _previousSection;
        private Group? _previousGroup;
        private Item? _previousItem;
        private Property? _previousProperty;
        private int _previousOrdinalPosition;

        private Section PreviousSection
        {
            get { return _previousSection; }
            set
            {
                _previousSection = value;
                PreviousGroup = null;
            }
        }

        private Group? PreviousGroup
        {
            get { return _previousGroup; }
            set
            {
                _previousGroup = value;
                PreviousItem = null;
            }
        }

        private Item? PreviousItem
        {
            get { return _previousItem; }
            set
            {
                _previousItem = value;
                _previousProperty = null;
            }
        }

        public ListParser()
        {
            _list = new CraterList();
            _previousSection = null;
            _previousGroup = null;
            _previousItem = null;
            _previousProperty = null;
            _previousOrdinalPosition = 0;
        }

        public CraterList CreateFromFilepath(string filepath)
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

            Item item = new Item(line, isComplete, _list.Properties);

            if (PreviousSection is null)
            {
                Section section = new Section("Incomplete");
                _list.Sections.Add(section.Name, section);
                PreviousSection = section;
            }

            if (PreviousGroup is null)
            {
                if (PreviousSection.Groups.ContainsKey("General"))
                {
                    PreviousGroup = PreviousSection.Groups["General"];
                }
                else
                {
                    Group group = new Group("General");
                    PreviousSection.Groups.Add(group.Name, group);
                    PreviousGroup = group;
                }
            }

            while (_previousOrdinalPosition >= ordinalPosition && _previousOrdinalPosition > 0)
            {
                PreviousItem = PreviousItem?.Parent;
                _previousOrdinalPosition--;
            }

            if (PreviousItem is not null && ordinalPosition > _previousOrdinalPosition)
            {
                item.Parent = PreviousItem;
                PreviousItem.Children.Add(item);
            }
            else
            {
                PreviousGroup.Items.Add(item);
            }

            PreviousItem = item;
            _previousOrdinalPosition = ordinalPosition;

            return;
        }

        public void ParseProperty(string line, int ordinalPosition)
        {
            Debug.WriteLine($"{ordinalPosition} Property: {line}");

            if (_previousSection is null)
            {
                ParseGlobalProperty(line, ordinalPosition);
            }
            else
            {
                ParseLocalProperty(line, ordinalPosition);
            }

            int separaterIndex = line.IndexOf(':');

            // If line represents merely a property value
            if (separaterIndex == -1)
            {
                if (_previousProperty is null || ordinalPosition <= _previousOrdinalPosition)
                {
                    Debug.WriteLine($"Can't set property value if property isn't specified.");
                    return;
                }

                _previousProperty.AddValue(line);
            }

            // If line represents a property declaration, with or without a value
            else
            {
                string name = line.Substring(0, separaterIndex);
                string value = line.Substring(separaterIndex + 1).TrimStart();

                if (!_list.Properties.ContainsKey(name))
                {
                    Debug.WriteLine($"{name} is not a defined property.");
                    return;
                }

                if (PreviousItem is null)
                {
                    Debug.WriteLine($"Can't add a property if no item has been defined.");
                    return;
                }

                _previousProperty = PreviousItem.Properties[name];

                if (!String.IsNullOrEmpty(value))
                {
                    _previousProperty.AddValue(value);
                }
            }

            return;
        }

        public void ParseGlobalProperty(string line, int ordinalPosition)
        {
            int separaterIndex = line.IndexOf(':');

            // If line represents merely a property value
            if (separaterIndex == -1)
            {
                Debug.WriteLine($"Multi-line global property definitions are not supported.");
                return;
            }
            
            string name = line.Substring(0, separaterIndex);
            string identifier = line.Substring(separaterIndex + 1).TrimStart();

            if (_list.Properties.ContainsKey(name))
            {
                Debug.WriteLine($"There alread exists a property named \"{name}\".");
                return;
            }

            if (string.IsNullOrEmpty(identifier))
            {
                Debug.WriteLine($"Can't create property \"{name}\" because it is missing an identifier.");
                return;
            }

            switch (identifier)
            {
                case "Text" or "text":
                    TextProperty property = new TextProperty(name);
                    _list.Properties.Add(property.Name, property);
                    break;
                default:
                    Debug.WriteLine($"\"{identifier}\" is not a recognized property identifier.");
                    break;
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
                PreviousSection = _list.Sections[line];
            }
            else if (ListTemplate.Sections.Contains(line))
            {
                PreviousSection = new Section(line);
                _list.Sections.Add(PreviousSection.Name, PreviousSection);
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

            if (PreviousSection is null)
            {
                PreviousSection = new Section("Incomplete");
                _list.Sections.Add(PreviousSection.Name, PreviousSection);
            }

            if (PreviousSection.Groups.ContainsKey(line))
            {
                PreviousGroup = PreviousSection.Groups[line];
            }
            else
            {
                PreviousGroup = new Group(line);
                PreviousSection.Groups.Add(PreviousGroup.Name, PreviousGroup);
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
        public static readonly Dictionary<string, Property> Properties;
        public static readonly List<string> Sections;

        static ListTemplate()
        {
            TextProperty textProperty = new TextProperty("");

            Properties = new Dictionary<string, Property>
            {
                { textProperty.Identifier, textProperty }
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
