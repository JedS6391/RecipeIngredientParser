namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Describes the ingredient details extracted by the parser.
    /// </summary>
    public class IngredientDetails
    {
        /// <summary>
        /// Gets or sets the amount of the ingredient.
        /// </summary>
        /// <remarks>
        /// e.g. 1, 1/2, 1-2
        /// </remarks>
        public string Amount { get; internal set; }
        
        /// <summary>
        /// Gets or sets the unit the ingredient is measured in.
        /// </summary>
        /// <remarks>
        /// e.g. cup, grams, tsp
        /// </remarks>
        public string Unit { get; internal set; }
        
        /// <summary>
        /// Gets or sets the form the ingredient is in.
        /// </summary>
        /// <remarks>
        /// e.g. shredded, chopped
        /// </remarks>
        public string Form { get; internal set; }

        /// <summary>
        /// Gets or sets the ingredient.
        /// </summary>
        /// <remarks>
        /// e.g. onion, cheese, vegan sausages
        /// </remarks>
        public string Ingredient { get; internal set; }
    }
}