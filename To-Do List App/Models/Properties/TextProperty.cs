namespace Crater.Models.Properties
{
    public class TextProperty : Property
    {
        public override string Identifier => "Text";

        public override bool IsValidValue(string value) => true;
    }
}
