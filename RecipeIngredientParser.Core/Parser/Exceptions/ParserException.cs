using System;

namespace RecipeIngredientParser.Core.Parser.Exceptions
{
    /// <summary>
    /// Defines a base exception for all parser related exceptions.
    /// </summary>
    public abstract class ParserException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ParserException"/> class with the given message.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        protected ParserException(string message) 
            : base(message)
        {}
    }
}