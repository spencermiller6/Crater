using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace To_Do_List_App.Properties.ListProperties
{
    public abstract class ListProperty
    {
        public List<string> AllowedValues { get; protected set; }

        public ListProperty()
        {
            AllowedValues = new List<string>();
            BuildValues();
        }

        protected abstract void BuildValues();
    }

    public class CompletionProperty : ListProperty
    {
        protected override void BuildValues()
        {
            AllowedValues.Add("Immediate");
            AllowedValues.Add("Long-Term");
            AllowedValues.Add("Disabled");
        }
    }
}
