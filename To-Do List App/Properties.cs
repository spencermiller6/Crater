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

        bool IsAvailable(Property property)
        {
            if (property.Prerequisites == null) return true;

            foreach ((Property prerequisiteProperty, List<String> prerequisiteValues) in property.Prerequisites)
            {
                if (!prerequisiteValues.Contains(Find(x => x.Name == prerequisiteProperty.Name).Value))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class Property
    {
        public string Name { get; }
        public List<string> Values { get; }
        public List<(Property, List<string>)>? Prerequisites { get; }
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

        public Property(string name, List<string> values, List<(Property, List<string>)>? prerequisites = null)
        {
            if (!values.Any())
            {
                throw new ArgumentException("Cannot create a property without and values.");
            }

            Name = name;
            Values = values;
            Prerequisites = prerequisites;
            Value = Values[0];
        }

        public void SetToDefault()
        {
            Value = Values[0];
        }
    }
}
