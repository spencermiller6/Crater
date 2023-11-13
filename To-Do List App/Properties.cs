using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace To_Do_List_App
{
    public class Properties : List<Property>
    {
        public void SetToDefault()
        {
            foreach (Property property in this)
            {
                property.SetToDefault();
            }
        }

        public bool IsAvailable(Property property)
        {
            if (property.Prerequisite == null) return true;

            if (Find(x => x.Name == property.Prerequisite).Value == "Disabled")
            {
                return false;
            }

            return true;
        }
    }

    public class Property
    {
        public string Name { get; }
        public List<string> Values { get; }
        public string? Prerequisite { get; }
        public string Value {
            get
            {
                return Value;
            }
            set
            {
                if (!Values.Contains(value))
                {
                    throw new ArgumentException($"Could not find a value for {Name} called {value}");
                }

                Value = value;
            }
        }

        public Property(string name, List<string> values, string? prerequisite = null)
        {
            if (!values.Any())
            {
                throw new ArgumentException("Cannot create a property without and values.");
            }

            Name = name;
            Values = values;
            Prerequisite = prerequisite;
            Value = Values[0];
        }

        public void SetToDefault()
        {
            Value = Values[0];
        }
    }
}
