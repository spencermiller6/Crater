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
        string Name;
        Dictionary<string, string> Properties;
        List<Section> Sections;
    }

    public class Section
    {
        public string Name;
        public List<Item> Items;
    }

    public class Item
    {
        public string Name;
        public Dictionary<string, string> Properties;
        public List<Item> Children;
    }
}
