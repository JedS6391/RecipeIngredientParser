using System;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;

namespace RecipeIngredientParser.Core.Parser.Exceptions
{
    /// <summary>
    /// An exception given when no appropriate <see cref="IParserStrategy"/> can be found.
    /// </summary>
    public class ParserStrategyNotFoundException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ParserStrategyNotFoundException"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public ParserStrategyNotFoundException(string message)
            : base(message) 
        {}
    }
}