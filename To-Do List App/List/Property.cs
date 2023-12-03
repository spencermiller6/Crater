using System;
using System.Collections.Generic;
using System.Linq;

namespace To_Do_List_App.List
{
    public abstract class Property
    {
        public List<string> Values { get; protected set; }

        public Property()
        {
            Values = new List<string>();
        }

        public abstract string Identifier { get; }

        public abstract bool IsValidValue(string value);

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
