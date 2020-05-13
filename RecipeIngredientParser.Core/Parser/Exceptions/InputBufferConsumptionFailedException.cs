namespace RecipeIngredientParser.Core.Parser.Exceptions
{
    /// <summary>
    /// An exception given when consumption of a character from the <see cref="InputBuffer"/> failed.
    /// </summary>
    public class InputBufferConsumptionFailedException : ParserException
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="InputBufferConsumptionFailedException"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public InputBufferConsumptionFailedException(string message) 
            : base(message)
        {}
    }
}