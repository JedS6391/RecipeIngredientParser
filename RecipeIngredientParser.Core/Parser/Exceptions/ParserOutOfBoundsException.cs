namespace RecipeIngredientParser.Core.Parser.Exceptions
{
    /// <summary>
    /// An exception given when the <see cref="ParserContext"/> reached the end of its input.
    /// </summary>
    public class ParserOutOfBoundsException : ParserException
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ParserOutOfBoundsException"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public ParserOutOfBoundsException(string message) 
            : base(message)
        {}
    }
}