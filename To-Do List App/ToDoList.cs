using Markdig.Extensions.TaskLists;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Windows.Documents;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

namespace To_Do_List_App
{
    public class ToDoList
    {
        string? Name;
        Dictionary<string, string> Properties;
        List<ListSection> Sections;

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

        public static ToDoList CreateFromFilepath(string filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            string? line;

            ToDoList list = new ToDoList();
            ListSection? currentSection = null;
            ListItem? currentItem = null;
            int currentIndex = 0;

            try
            {
                line = reader.ReadLine();

                while (line != null)
                {
                    list.ParseLine(line, currentSection, currentItem, currentIndex);
                    line = reader.ReadLine();
                }
            }
            finally
            {
                reader.Close();
            }

            return list;
        }

        public void ParseLine(string line, ListSection? currentSection, ListItem? currentItem, int currentIndex)
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

                    if (Sections.Any(section => section.Name == value))
                    {
                        currentSection = Sections.Find(section => section.Name == value);
                    }
                    else
                    {
                        ListSection section = new ListSection(value);
                        Sections.Add(section);
                        currentSection = section;
                    }

                    break;
                case LineIdentifier.ListHeader:
                    value = substrings[0];

                    switch (value)
                    {
                        case null:
                            break;
                        case "Active":
                            break;
                        case "Completed":
                            break;
                    }

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
}
