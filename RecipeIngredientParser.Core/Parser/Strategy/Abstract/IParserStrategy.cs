using System.Collections.Generic;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Templates;

namespace RecipeIngredientParser.Core.Parser.Strategy.Abstract
{
    /// <summary>
    /// Defines a strategy that an <see cref="IngredientParser"/> can use.
    /// </summary>
    public interface IParserStrategy
    {
        /// <summary>
        /// Determines whether this strategy handles the supplied option.
        /// </summary>
        /// <param name="strategyOption">The strategy option.</param>
        /// <returns>
        /// <see langword="true"/> if this can handle the strategy option; <see langword="false"/> otherwise.
        /// </returns>
        bool Handles(ParserStrategyOption strategyOption);
        
        /// <summary>
        /// Attempts to parse an ingredient from the context based on a set of templates. 
        /// </summary>
        /// <param name="context">The context to parse an ingredient from.</param>
        /// <param name="templates">A set of templates to attempt parsing with.</param>
        /// <param name="parseResult">
        /// When this method returns, contains the result of parsing if the parse operation
        /// succeeded, or <see langword="null"/> if the parse failed.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the parsing succeeded; <see langword="false"/> otherwise.
        /// </returns>
        bool TryParseIngredient(
            ParserContext context, 
            IEnumerable<Template> templates, 
            out ParseResult parseResult);
    }
}