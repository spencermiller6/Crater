using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using static To_Do_List_App.ToDoList;
using System.Windows.Documents;

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
}
