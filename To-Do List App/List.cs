using Markdig.Extensions.TaskLists;
using Markdig.Parsers;
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
        public List<Field> Properties; //TODO
        public List<Section> Sections;
        //public List<Field> ItemFields;
        public Section CurrentSection;
        public Item CurrentItem;

        public List()
        {
            Name = "";
            //Properties = new List<Property>();
            Sections = new List<Section>();
            //ItemFields = new List<Field>();
        }

        public void ParseFromMarkdown(MarkdownDocument markdownDocument)
        {
            foreach (Markdig.Syntax.Block block in markdownDocument)
            {
                if (block == null)
                {
                    throw new Exception("Block is null.");
                }
                else if (block is HeadingBlock headingBlock)
                {
                    ParseHeadingBlock(headingBlock);
                }
                else if (block is ListBlock listBlock)
                {
                    ParseListBlock(listBlock);
                }
            }
        }

        public void ParseHeadingBlock(HeadingBlock headingBlock)
        {
            int level = headingBlock.Level;

            if (headingBlock.Inline?.FirstChild is not LiteralInline literalInline)
            {
                throw new Exception("Not LiteralInline.");
            }

            string name = literalInline.Content.ToString();

            if(string.IsNullOrEmpty(name) )
            {
                throw new Exception("Section name cannot be empty");
            }

            switch (level)
            {
                case 1:
                    if (!string.IsNullOrEmpty(Name))
                    {
                        //throw new Exception("List name already set.");
                    }

                    Name = name;
                    break;
                case 2:
                    Section? section = Sections.Find(x => x.Name == name);

                    if (section is null)
                    {
                        section = new Section(name);
                        Sections.Add(section);
                    }

                    CurrentSection = section;
                    break;
                default:
                    throw new Exception("Only headers of level 1 and 2 are supported.");
            }
        }

        public void ParseListBlock(Block listBlock)
        {
            foreach (ListItemBlock listItemBlock in listBlock)
            {
                if (listItemBlock.Count == 1)
                {
                    if(listItemBlock[0] is not ParagraphBlock paragraphBlock)
                    {
                        throw new Exception("Expected ParagraphBlock");
                    }

                    if (paragraphBlock.Inline?.FirstChild is TaskList taskList)
                    {
                        bool isChecked = taskList.Checked;

                        if (taskList.NextSibling is LiteralInline literalInline)
                        {
                            string name = literalInline.Content.ToString();
                            Item item = new Item(name, isChecked);

                            if (CurrentItem is not null)
                            {
                                item.Parent = CurrentItem;
                            }

                            CurrentSection.Items.Add(item);
                            CurrentItem = item;
                        }
                        else
                        {
                            throw new Exception("Found TaskList but not LiteralInline.");
                        }
                    }
                    else if (paragraphBlock.Inline?.FirstChild is LiteralInline literalInline)
                    {
                        string nameAndValue = literalInline.Content.ToString();
                        Field field = new Field(nameAndValue);

                        if (CurrentItem is null)
                        {
                            Properties.Add(field);
                        }
                        else
                        {
                            CurrentItem.Fields.Add(field);
                        }
                    }
                    else
                    {
                        throw new Exception("Value was not item or field.");
                    }
                }
                else
                {
                    ParseListBlock(listItemBlock);
                }
            }
        }
    }

    public class Section
    {
        public string Name;
        public List<Item> Items;

        public Section(string name)
        {
            Name = name;
            Items = new List<Item>();
        }

        public void ParseElements()
        {

        }
    }

    public class Item
    {
        public string Name;
        public bool IsChecked;
        //public Dictionary<string, object> Fields;
        public List<Field> Fields;
        public List<Item> SubItems;
        public Item? Parent;

        public Item(string name, bool isChecked)
        {
            Name = name;
            IsChecked = isChecked;
            Fields = new List<Field>();
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

        public Field(string nameAndValue)
        {
            Name = nameAndValue; // Not actually correct, just for debug purposes
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
