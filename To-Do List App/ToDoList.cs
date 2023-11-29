using System.Collections.Generic;
using System;

namespace To_Do_List_App
{
    public class ToDoList
    {
        public string? Name;
        public Dictionary<string, string> ListProperties;
        public List<string> ItemProperties;
        public List<ListSection> Sections;

        public ToDoList()
        {
            ListProperties = new Dictionary<string, string>();
            ItemProperties = new List<string>();
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

        public Dictionary<string, List<string>> MasterPropertyList = new Dictionary<string, List<string>>()
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
        public bool IsStarred;
        public DateTime Date;
        public Dictionary<string, string> Properties;
        public ListItem? Parent;
        public List<ListItem>? Children;

        public ListItem(string name, bool isComplete)
        {
            Name = name;
            IsComplete = isComplete;
            IsStarred = false;
            Properties = new Dictionary<string, string>();
            Children = new List<ListItem> { };
        }
    }
}
