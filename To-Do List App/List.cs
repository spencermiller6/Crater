using System;
using System.Collections.Generic;

namespace To_Do_List_App
{
    public class List
    {
        public List<Section> Sections;
        public Properties Properties;
        public List<Field> ItemFields;

        public List()
        {
            Properties = new Properties
            {
                new Property("Type", new List<string> { "Standard", "Template" }),
                new Property("Completion Mode", new List<string> { "Immediate", "Long-Term", "Disabled" }),
                new Property("Completed Items", new List<string> { "Hidden", "Visible", "Disabled" }),
                new Property("Children", new List<string> { "Enabled", "Disabled" }),
                new Property("Notes", new List<string> { "Enabled", "Disabled" }),
                new Property("Date", new List<string> { "Enabled", "Disabled" })
            };

            Sections = new List<Section>();
            Properties = new Properties();
            ItemFields = new List<Field>();
        }
    }

    public class Section
    {
        public List<Item> Items;

        public Section()
        {
            Items = new List<Item>();
        }
    }

    public class Item
    {
        public string? Name;
        public string? Notes;
        public DateTime? Date;
        public List<Field>? Fields;
        public List<Item>? SubItems;

        public Item(string name)
        {
            Name = name;
        }
    }

    public class Field
    {
        public string Name;
        public ValueTypeEnum ValueType;
        public ListTypeEnum ListType;
        public object? Value;

        public enum ValueTypeEnum
        {
            String,
            Int,
            Date,
            DateTime
        }

        public enum ListTypeEnum
        {
            None,
            Unordered,
            Ordered
        }

        public Field(string name, ValueTypeEnum valueType, ListTypeEnum listType, object? value)
        {
            Name = name;
            ValueType = valueType;
            ListType = listType;
            Value = value;
        }
    }
}
