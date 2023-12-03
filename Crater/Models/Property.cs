using System;
using System.Collections.Generic;
using System.Linq;

namespace Crater.Models
{
    public abstract class Property
    {
        public Property(string name)
        {
            Name = name;
            Values = new List<string>();
        }

        public abstract string Identifier { get; }
        public string Name { get; set; }
        public List<string> Values { get; protected set; }

        public abstract bool IsValidValue(string value);
        public abstract Property Clone();

        public void SetValue(string value, int index)
        {
            if (!IsValidValue(value))
            {
                throw new Exception($"Can't convert value \"{value}\" to a {Identifier}.");
            }

            if (Values.ElementAtOrDefault(index) is null)
            {
                throw new Exception($"Can't add value \"{value}\" to the list at index {index}.");
            }

            Values.Add(value);
        }

        public void AddValue(string value)
        {
            if (!IsValidValue(value))
            {
                throw new Exception($"Can't convert value \"{value}\" to a {Identifier}.");
            }

            Values.Add(value);
        }
    }
}
