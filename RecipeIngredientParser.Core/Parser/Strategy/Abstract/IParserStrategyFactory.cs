namespace RecipeIngredientParser.Core.Parser.Strategy.Abstract
{
    /// <summary>
    /// Defines a factory for <see cref="IParserStrategy"/> instances.
    /// </summary>
    public interface IParserStrategyFactory
    {
        /// <summary>
        /// Gets a <see cref="IParserStrategy"/> instance responsible for the given strategy option.
        /// </summary>
        /// <param name="strategyOption">The strategy option to get a <see cref="IParserStrategy"/> instance for.</param>
        /// <returns>A <see cref="IParserStrategy"/> instance.</returns>
        IParserStrategy GetStrategy(ParserStrategyOption strategyOption);
    }
}