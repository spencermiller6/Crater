using System.Collections.Generic;

namespace Crater.Models.Properties
{
    public class NumberProperty : Property
    {
        public NumberProperty(string name) : base(name)
        {
        }

        public override string Identifier => "Number";

        public override bool IsValidValue(string value) => double.TryParse(value, out _);

        public override Property Clone()
        {
            NumberProperty property = (NumberProperty)this.MemberwiseClone();
            property.Values = new List<string>();

            return property;
        }
    }
}
