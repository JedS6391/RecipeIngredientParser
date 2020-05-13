using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens.Abstract;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Core.Parser.Extensions
{
    /// <summary>
    /// Defines a set of extensions for building an <see cref="IngredientParser"/>.
    /// </summary>
    public static class IngredientParserBuilderExtensions
    {
        public static IngredientParser WithDefaultConfiguration(this IngredientParser.Builder builder)
        {
            return builder
                .WithTemplateDefinitions(
                    TemplateDefinitions.AmountUnitFormIngredient,
                    TemplateDefinitions.AmountUnitIngredient
                )
                .WithTokenReaderFactory(new TokenReaderFactory(new ITokenReader[]
                {
                    new AmountTokenReader(),
                    new UnitTokenReader(),
                    new FormTokenReader(),
                    new IngredientTokenReader()
                }))
                .WithParserStrategy(ParserStrategyOption.AcceptFirstFullMatch)
                .WithParserStrategyFactory(new ParserStrategyFactory(new IParserStrategy[]
                {
                    new FirstFullMatchParserStrategy()
                }))
                .Build();
        }
    }
}