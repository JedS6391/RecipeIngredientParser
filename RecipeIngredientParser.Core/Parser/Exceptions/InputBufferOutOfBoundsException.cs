using RecipeIngredientParser.Core.Parser.Context;

namespace RecipeIngredientParser.Core.Parser.Exceptions
{
    /// <summary>
    /// An exception given when the <see cref="InputBuffer"/> reached the end of its input.
    /// </summary>
    public class InputBufferOutOfBoundsException : ParserException
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="InputBufferOutOfBoundsException"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public InputBufferOutOfBoundsException(string message) 
            : base(message)
        {}
    }
}