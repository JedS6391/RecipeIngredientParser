using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Parser.Exceptions;

namespace RecipeIngredientParser.Core.Parser.Strategy
{
    /// <summary>
    /// A default <see cref="IParserStrategyFactory"/> implementation that will attempt to find the first
    /// matching <see cref="IParserStrategy"/> instance that can handle a specified strategy option.
    /// </summary>
    public class ParserStrategyFactory : IParserStrategyFactory
    {
        private readonly IEnumerable<IParserStrategy> _parserStrategies;

        /// <summary>
        /// Initialises a new instance of the <see cref="ParserStrategyFactory"/> class.
        /// </summary>
        /// <param name="parserStrategies">A collection of parser strategies that are available.</param>
        public ParserStrategyFactory(IEnumerable<IParserStrategy> parserStrategies)
        {
            _parserStrategies = parserStrategies;
        }

        /// <inheritdoc/>
        public IParserStrategy GetStrategy(ParserStrategyOption strategyOption)
        {
            return _parserStrategies.FirstOrDefault(s => s.Handles(strategyOption))
                ?? throw new ParserStrategyNotFoundException($"No parser strategy found that can handle {strategyOption}.");
        }
    }
}