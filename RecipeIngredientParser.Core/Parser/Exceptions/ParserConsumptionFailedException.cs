namespace RecipeIngredientParser.Core.Parser.Exceptions
{
    /// <summary>
    /// An exception given when consumption of a character from the <see cref="ParserContext"/> failed.
    /// </summary>
    public class ParserConsumptionFailedException : ParserException
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ParserConsumptionFailedException"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public ParserConsumptionFailedException(string message) 
            : base(message)
        {}
    }
}