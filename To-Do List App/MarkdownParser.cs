﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static To_Do_List_App.ToDoList;

namespace To_Do_List_App
{
    public class ListParser
    {
        private ToDoList _list;
        private string _currentListSection;
        private ListSection? _currentSection;
        private ListItem? _currentItem;
        private int _currentOrdinalPosition;
        private string? _currentProperty;

        public ListParser()
        {
            _list = new ToDoList();
            _currentListSection = "";
            _currentSection = null;
            _currentItem = null;
            _currentOrdinalPosition = 0;
            _currentProperty = null;
        }

        public ToDoList CreateFromFilepath(string filepath)
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
            LineIdentifier identifier = GetIdentifier(line.TrimStart());

            if (identifier == LineIdentifier.None) return;

            List<string> substrings = SplitLine(line, identifier);
            int ordinalPosition;
            string propertyName;
            string value;

            switch (identifier)
            {
                case LineIdentifier.CompleteItem:
                    _currentProperty = null;
                    ordinalPosition = GetOrdinalPosition(substrings[0]);
                    value = substrings[1];

                    ParseItem(value, true, ordinalPosition);
                    break;
                case LineIdentifier.IncompleteItem:
                    _currentProperty = null;
                    ordinalPosition = GetOrdinalPosition(substrings[0]);
                    value = substrings[1];

                    ParseItem(value, false, ordinalPosition);
                    break;
                case LineIdentifier.Property:
                    ordinalPosition = GetOrdinalPosition(substrings[0]);
                    propertyName = substrings[1];
                    value = substrings[2];

                    ParseProperty(ordinalPosition, propertyName, value);
                    break;
                case LineIdentifier.SectionHeader:
                    _currentItem = null;
                    _currentProperty = null;
                    value = substrings[0];
                    ParseSectionHeader(value);

                    break;
                case LineIdentifier.ListHeader:
                    _currentSection = null;
                    _currentItem = null;
                    _currentProperty = null;
                    value = substrings[0];
                    ParseListHeader(value);

                    break;
            }
        }

        private LineIdentifier GetIdentifier(string line)
        {
            try
            {
                switch (line.Substring(0, 2))
                {
                    case "- ":
                        switch (line.Substring(2, 4))
                        {
                            case "[x] ":
                                return LineIdentifier.CompleteItem;
                            case "[X] ":
                                return LineIdentifier.CompleteItem;
                            case "[ ] ":
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
            catch
            {
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

                    int index2 = line.IndexOf(':');
                    substrings.Add(line.Substring(index + 2, index2 - index - 2));


                    string substring = line.Substring(index2 + 1);
                    substrings.Add(substring.TrimStart());

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

        private void ParseItem(string value, bool isCompleted, int ordinalPosition)
        {
            ListItem item = new ListItem(value, isCompleted);

            while (ordinalPosition <= _currentOrdinalPosition && _currentOrdinalPosition > 0)
            {
                _currentItem = _currentItem.Parent;
                _currentOrdinalPosition--;
            }

            // If the item's ordinal position is 0, add the new item directly to the current section
            if (ordinalPosition == 0)
            {
                // If there is no defined section, create a general one. The name general is quasi-reserved and can be user-defined,
                // however unsorted items will also be added to this section. If there are no other sections defined, its title will
                // not be shown as there is no need to descern between non-existant sections.
                if (_currentSection is null)
                {
                    ListSection section = new ListSection("General");

                    _list.Sections.Add(section);
                    _currentSection = section;
                }

                _currentSection.Items.Add(item);
            }
            // Otherwise, add the new item as a child of the current one
            else
            {
                if (_currentItem is null)
                {
                    throw new Exception("Can't add item to missing parent.");
                }

                item.Parent = _currentItem;
                _currentItem.Children.Add(item);
            }

            _currentItem = item;
            _currentOrdinalPosition = ordinalPosition;
        }

        private void ParseProperty(int ordinalPosition, string propertyName, string value)
        {
            // Add property to current item
            if (_currentItem is not null && ordinalPosition == _currentOrdinalPosition + 1)
            {
            }

            // Add property to current property
            else if (ordinalPosition == _currentOrdinalPosition + 1)
            {
                if (_isCurrentlySettingItemProperties && ordinalPosition == 1)
                {
                    if (_list.ItemProperties.ContainsKey(propertyName))
                    {
                        throw new Exception($"A property with the name {propertyName} is already defined.");
                    }

                    GetTypeParameters(value, out ListItem.ItemType itemType, out ListItem.ItemCollection itemCollection);
                    _list.ItemProperties.Add(propertyName, (itemType, itemCollection));
                    // TODO: find a place to reset the _isCurrentlySettingItemProperties flag once done setting item properties
                }
            }

            // Add property to list
            else if (_currentSection is null && _currentListSection is null && ordinalPosition == 0)
            {
                // Check if defining item properties
                if (propertyName == "Item")
                {
                    _currentProperty = "Item";
                }

                // Check if property name and value are valid
                else if (MasterPropertyList.ContainsKey(propertyName) && MasterPropertyList[propertyName].Contains(value))
                {
                    _list.ListProperties.Add(propertyName, value);
                }
            }
        }

        private void ParseListHeader(string value)
        {
            if (_list.Name is null)
            {
                _list.Name = value;
            }
            else if (value == "Active")
            {
                _currentListSection = "Active";
            }
            else if (value == "Completed")
            {
                _currentListSection = "Completed";
            }
        }

        private void ParseSectionHeader(string value)
        {
            if (_list.Sections.Any(section => section.Name == value))
            {
                _currentSection = _list.Sections.Find(section => section.Name == value);
            }
            else
            {
                ListSection section = new ListSection(value);
                _list.Sections.Add(section);
                _currentSection = section;
            }
        }

        private void GetTypeParameters(string input, out ListItem.ItemType itemType, out ListItem.ItemCollection itemCollection)
        {
            string[] substrings = input.Split(' ');
            string potentialItemType;
            string potentialItemCollection;

            switch (substrings.Length)
            {
                // input looks like "string"
                case 1:
                    potentialItemType = substrings[0];
                    potentialItemCollection = "single";

                    break;

                // input looks like "list string"
                case 2:
                    potentialItemType = substrings[1];
                    potentialItemCollection = "unordered " + substrings[0];

                    break;

                // input looks like "unordered list string" or "ordered list string"
                case 3:
                    potentialItemType = substrings[2];
                    potentialItemCollection = substrings[0] + " " + substrings[1];

                    break;
                default:
                    throw new Exception($"Unrecognized data type: {input}");
            }

            switch (potentialItemType)
            {
                case "string":
                    itemType = ListItem.ItemType.String;
                    break;
                case "int":
                    itemType = ListItem.ItemType.Int;
                    break;
                case "date":
                    itemType = ListItem.ItemType.Date;
                    break;
                case "bool":
                    itemType = ListItem.ItemType.Bool;
                    break;
                default:
                    throw new Exception($"Unrecognized data type: {input}");
            }

            switch (potentialItemCollection)
            {
                case "single":
                    itemCollection = ListItem.ItemCollection.Single;
                    break;
                case "unordered list":
                    itemCollection = ListItem.ItemCollection.UnorderedList;
                    break;
                case "ordered list":
                    itemCollection = ListItem.ItemCollection.OrderedList;
                    break;
                default:
                    throw new Exception($"Unrecognized data type: {input}");
            }
        }
    }
}