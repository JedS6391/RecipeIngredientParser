using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Parser.Exceptions;
using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Templates;

namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Provides the ability to parse an ingredient based on a set of templates.
    /// </summary>
    public class IngredientParser
    {
        private readonly IEnumerable<Template> _templates;
        private readonly IEnumerable<IInputSanitizationRule> _sanitizationRules;
        private readonly IParserStrategy _parserStrategy;

        /// <summary>
        /// Initialises a new instance of the <see cref="IngredientParser"/> class.
        /// </summary>
        /// <param name="templates">The set of templates the parser will attempt parsing with.</param>
        /// <param name="sanitizationRules">A set of rules the parser will use to sanitize the input.</param>
        /// <param name="parserStrategy">A strategy that will be used for parsing.</param>
        internal IngredientParser(
            IEnumerable<Template> templates,
            IEnumerable<IInputSanitizationRule> sanitizationRules,
            IParserStrategy parserStrategy)
        {
            _templates = templates;
            _sanitizationRules = sanitizationRules;
            _parserStrategy = parserStrategy;
        }
        
        /// <summary>
        /// Attempts to parse a raw ingredient according to the configured templates and parsing strategy.
        /// </summary>
        /// <param name="ingredient">An ingredient string to parse (e.g. 1 bag vegan sausages).</param>
        /// <param name="parseResult">
        /// When this method returns, contains the result of parsing if the parse operation
        /// succeeded, or <see langword="null"/> if the parse failed.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the parsing succeeded; <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="InvalidParserInputException">When the parser is provided an input that is not valid.</exception>
        public bool TryParseIngredient(string ingredient, out ParseResult parseResult)
        {
            var result = ParseIngredient(ingredient);

            parseResult = result.IsSuccess ?
                result :
                null;

            return result.IsSuccess;
        }

        /// <summary>
        /// Attempts to parse a raw ingredient according to the configured templates and parsing strategy.
        /// </summary>
        /// <param name="ingredient">An ingredient string to parse (e.g. 1 bag vegan sausages).</param>
        /// <returns>The result of the parse operation.</returns>
        /// <exception cref="InvalidParserInputException">When the parser is provided an input that is not valid.</exception>
        public ParseResult ParseIngredient(string ingredient)
        {
            if (string.IsNullOrEmpty(ingredient))
            {
                throw new InvalidParserInputException("Input is not able to be parsed.");
            }

            var sanitizedIngredient = Sanitize(ingredient);

            var context = new ParserContext(sanitizedIngredient);

            return _parserStrategy.ParseIngredient(context, _templates);
        }

        private string Sanitize(string input) => 
            _sanitizationRules.Aggregate(input, (current, rule) => rule.Apply(current));
    }
}