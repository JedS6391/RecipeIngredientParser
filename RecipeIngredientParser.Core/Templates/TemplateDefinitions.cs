namespace RecipeIngredientParser.Core.Templates
{
    /// <summary>
    /// Defines a set of built-in template definitions.
    /// </summary>
    public static class TemplateDefinitions
    {
        public const string AmountUnitFormIngredient = "{amount} {unit} {form} {ingredient}";
        public const string AmountUnitIngredient = "{amount} {unit} {ingredient}";
        public const string AmountNoSpaceUnitIngredient = "{amount}{unit} {ingredient}";
        public const string IngredientAmountUnit = "{ingredient}: {amount} {unit}";
        public const string AmountIngredientForm = "{amount} {ingredient}, {form}";
        public const string AmountUnitIngredientForm = "{amount} {unit} {ingredient}, {form}";
        public const string AmountUnitOfFormIngredient = "{amount} {unit} of {form} {ingredient}";
        public const string UnitOfFormIngredient = "{unit} of {form} {ingredient}";
        public const string Ingredient = "{ingredient}";
        public const string AmountIngredient = "{amount} {ingredient}";

        public static readonly string[] DefaultTemplateDefinitions =
        {
            AmountUnitFormIngredient,
            AmountUnitIngredient,
            AmountNoSpaceUnitIngredient,
            IngredientAmountUnit,
            AmountIngredientForm,
            AmountUnitIngredientForm,
            AmountUnitOfFormIngredient,
            UnitOfFormIngredient,
            Ingredient,
            AmountIngredient
        };
    }
}