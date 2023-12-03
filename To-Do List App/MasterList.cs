using System.Collections.Generic;
using To_Do_List_App.ListProperties;

namespace To_Do_List_App
{
    public static class MasterList
    {
        public static List<ListProperty> ListProperties { get; private set; }

        static MasterList()
        {
            ListProperties = new List<ListProperty>
            {
                new CompletionProperty()
            };
        }
    }
}
