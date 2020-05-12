using System;

namespace RecipeIngredientParser.Core.Parser.Exceptions
{
    /// <summary>
    /// An exception given when a <see cref="IngredientParser"/> cannot be built by a <see cref="IngredientParser.Builder"/>
    /// </summary>
    public class ParserBuilderException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ParserBuilderException"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public ParserBuilderException(string message)
            : base(message)
        {}
    }
}