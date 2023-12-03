namespace To_Do_List_App.List.Properties
{
    public class NumberProperty : Property
    {
        public override string Identifier => "Number";

        public override bool IsValidValue(string value) => double.TryParse(value, out _);
    }
}
