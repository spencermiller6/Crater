using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections;
using System.Collections.Generic;
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
            foreach (var block in markdownDocument)
            {
                ParseBlockElements(block);
            }
        }

        public void ParseBlockElements(Markdig.Syntax.Block block)
        {
            if (block is null)
            {
                return;
            }

            if (block is HeadingBlock headingBlock)
            {
                switch (headingBlock.Level)
                {
                    case 1:
                        {
                            if (String.IsNullOrEmpty(Name))
                            {
                                Name = headingBlock.Inline.FirstChild.ToString();
                            }

                            break;
                        }
                    case 2:
                        {
                            Section section = new Section();
                            section.Name = headingBlock.Inline.FirstChild.ToString();

                            section.ParseElements();

                            Sections.Add(section);

                            break;
                        }
                }
            }
            if (block is ListBlock listblock)
            {
                foreach (ListItemBlock listItemBlock in listblock)
                {
                    Item item = new Item();

                    if (listItemBlock.LastChild is ParagraphBlock paragraphBlock)
                    {
                        if (paragraphBlock.Inline.FirstChild is TaskList taskList)
                        {
                            //item.Completed = paragraphBlock.Inline.FirstChild.IsChecked;
                        }

                        if (paragraphBlock.Inline.LastChild is LiteralInline literalInline)
                        {
                            item.Name = literalInline.ToString();
                        }
                    }

                    //item.Name = listItemBlock.LastChild.Inline.
                }
            }
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
