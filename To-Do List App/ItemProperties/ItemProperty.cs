using System;
using System.Collections.Generic;
using System.Linq;

namespace To_Do_List_App.ItemProperties
{
    public abstract class ItemProperty
    {
        public List<string> Values { get; protected set; }

        public ItemProperty()
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

    public class TextProperty : ItemProperty
    {
        public override string Identifier => "Text";

        public override bool IsValidValue(string value) => true;
    }

    public class NumberProperty : ItemProperty
    {
        public override string Identifier => "Number";

        public override bool IsValidValue(string value) => double.TryParse(value, out _);
    }
}
