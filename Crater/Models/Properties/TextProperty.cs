using System.Collections.Generic;

namespace Crater.Models.Properties
{
    public class TextProperty : Property
    {
        public TextProperty(string name) : base(name)
        {
        }

        public override string Identifier => "Text";

        public override bool IsValidValue(string value) => true;

        public override Property Clone()
        {
            TextProperty property = (TextProperty)this.MemberwiseClone();
            property.Values = new List<string>();

            return property;
        }
    }
}
