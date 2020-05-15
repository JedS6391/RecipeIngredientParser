using System;

namespace RecipeIngredientParser.Core.Parser.Exceptions
{
    /// <summary>
    /// An exception given when a <see cref="IngredientParser"/> cannot parse the provided input.
    /// </summary>
    public class InvalidParserInputException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="InvalidParserInputException"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public InvalidParserInputException(string message) 
            : base(message) 
        {}
    }
}