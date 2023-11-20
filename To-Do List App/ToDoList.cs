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
            Property
        }

        public static ToDoList CreateFromFilepath(string filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            string? line;

            ToDoList list = new ToDoList();
            ListSection? currentSection;
            ListItem? currentItem;
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
            LineIdentifier identifier;
            string property;
            string[] values;

            string[] substrings = line.Split(':', ';');

            property = substrings[0];
            values = new string[substrings.Length - 1];

            for (int i = 1; i < substrings.Length; i++)
            {
                values[i - 1] = substrings[i];
            }

            identifier = LineIdentifier.IncompleteItem;
            string value = null;
            int index = 0;

            switch (identifier)
            {
                case LineIdentifier.ListHeader:
                    if (Name is null)
                    {
                        Name = value;
                    }
                    else if (value == "Active")
                    {

                    }
                    else if (value == "Completed")
                    {

                    }

                    break;
                case LineIdentifier.SectionHeader:
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
                case LineIdentifier.IncompleteItem://TODO: combine this and the completeitem parsing logic, and move it to a method
                    ListItem incompleteItem = new ListItem(value, false);

                    while (index < currentIndex)
                    {
                        currentItem = currentItem.Parent;
                        currentIndex--;
                    }

                    // If the current item's index is 0, add the new item directly to the current section
                    if (currentIndex == 0)
                    {
                        //TODO: need to make sure there is a current section, move this logic to a method
                        currentSection.Items.Add(incompleteItem);
                    }
                    // Otherwise, add the new item as a child of the current one
                    else
                    {
                        currentItem.Children.Add(incompleteItem);
                    }

                    currentItem = incompleteItem;

                    break;
                case LineIdentifier.CompleteItem:
                    ListItem completeItem = new ListItem(value, true);
                    currentSection.Items.Add(completeItem);
                    currentItem = completeItem;

                    break;
                case LineIdentifier.Property:
                    break;
                default:
                    break;
            }




            if("" == "")
            {
                currentItem = currentItem?.Parent;
            }
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
