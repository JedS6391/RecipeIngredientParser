namespace RecipeIngredientParser.Core.Parser.Context
{
    /// <summary>
    /// Represents the context of an <see cref="IngredientParser"/> during the parse operation.
    /// </summary>
    public class ParserContext
    {
        /// <summary>
        /// Gets the input buffer for this context.
        /// </summary>
        public InputBuffer Buffer { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ParserContext"/> class.
        /// </summary>
        /// <param name="rawIngredient">The raw ingredient string to be parsed.</param>
        public ParserContext(string rawIngredient)
        {
            Buffer = new InputBuffer(rawIngredient);
        }
    }
}