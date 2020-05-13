namespace RecipeIngredientParser.Core.Templates
{
    /// <summary>
    /// Defines a set of built-in template definitions.
    /// </summary>
    public static class TemplateDefinitions
    {
        public const string AmountUnitFormIngredient = "{amount} {unit} {form} {ingredient}";
        public const string AmountUnitIngredient = "{amount} {unit} {ingredient}";
        public const string IngredientAmountUnit = "{ingredient}: {amount} {unit}";
    }
}