using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;

namespace To_Do_List_App
{
    public class List
    {
        public string Name;
        //public List<Property> Properties;
        public List<Section> Sections;
        //public List<Field> ItemFields;

        public List()
        {
            Name = "";
            //Properties = new List<Property>();
            Sections = new List<Section>();
            //ItemFields = new List<Field>();
        }

        public void ParseFromMarkdown(MarkdownDocument markdownDocument)
        {
            ParseContainerBlock(markdownDocument);
        }

        public void ParseContainerBlock(ContainerBlock parentBlock)
        {
            foreach (Markdig.Syntax.Block block in parentBlock)
            {
                if (block == null)
                {
                    return;
                }
                if (block is ContainerBlock containerBlock)
                {
                    ParseContainerBlock(containerBlock);
                }
                if (block is LeafBlock leafBlock)
                {
                    ParseLeafBlock(leafBlock);
                }
            }
        }

        public void ParseLeafBlock(LeafBlock leafBlock)
        {
            if (leafBlock?.Inline is null)
            {
                return;
            }

            foreach (LiteralInline literalInline in leafBlock.Inline)
            {
                ParseLiteralInline(literalInline);
            }
        }

        public void ParseLiteralInline(LiteralInline literalInline)
        {
            Debug.WriteLine(literalInline.Content.ToString());
        }
    }

    public class Section
    {
        public string Name;
        public List<Item> Items;

        public Section()
        {
            Name = "";
            Items = new List<Item>();
        }

        public void ParseElements()
        {

        }
    }

    public class Item
    {
        public string Name;
        public Dictionary<string, object> Fields;
        public List<Item> SubItems;

        public Item()
        {
            Name = "";
            Fields = new Dictionary<string, object>();
            SubItems = new List<Item>();
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
