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
        /// Attempts to parse an ingredient from the context based on a set of templates.
        /// </summary>
        /// <param name="context">The context to parse an ingredient from.</param>
        /// <param name="templates">A set of templates to attempt parsing with.</param>
        /// <returns>The result of the parse operation.</returns>
        ParseResult ParseIngredient(ParserContext context, IEnumerable<Template> templates);
    }
}