using System;
using System.Linq;
using NUnit.Framework;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Test
{
    public class IngredientParserTests
    {
        private readonly ITokenReader[] _tokenReaders = 
        {
            new AmountTokenReader(), 
            new UnitTokenReader(), 
            new IngredientTokenReader()
        };

        private readonly IParserStrategy[] _parserStrategies =
        {
            new FullMatchParserStrategy()
        };

        private static dynamic[][] _testCases =
        {
            new dynamic[]
            {
                // Template definition
                "{amount} {unit} {ingredient}",
                // Raw ingredient
                "1 bag vegan sausages",
                // Expected tokens
                new[]
                {
                    typeof(LiteralToken),
                    typeof(LiteralAmountToken),
                    typeof(LiteralToken),
                    typeof(UnitToken),
                    typeof(LiteralToken),
                    typeof(IngredientToken),
                    typeof(LiteralToken)
                },
                // Expected parsed ingredient
                new ParseResult()
                {
                    Ingredient = new ParseResult.IngredientDetails()
                    {
                        Amount = "1",
                        Form = null,
                        Unit = "bag",
                        Ingredient = "vegan sausages"
                    },
                    Metadata = new ParseResult.ParseMetadata()
                }
            },
            new dynamic[]
            {
                // Template definition
                "{amount} {unit} {ingredient}",
                // Raw ingredient
                "2 grams chocolate",
                // Expected tokens
                new[]
                {
                    typeof(LiteralToken),
                    typeof(LiteralAmountToken),
                    typeof(LiteralToken),
                    typeof(UnitToken),
                    typeof(LiteralToken),
                    typeof(IngredientToken),
                    typeof(LiteralToken)
                },
                // Expected parsed ingredient
                new ParseResult()
                {
                    Ingredient = new ParseResult.IngredientDetails()
                    {
                        Amount = "2",
                        Form = null,
                        Unit = "grams",
                        Ingredient = "chocolate"
                    },
                    Metadata = new ParseResult.ParseMetadata()
                }
            },
            new dynamic[]
            {
                // Template definition
                "{ingredient}: {amount} {unit}",
                // Raw ingredient
                "cheese: 3 cups",
                // Expected tokens
                new[]
                {
                    typeof(LiteralToken),
                    typeof(IngredientToken),
                    typeof(LiteralToken),
                    typeof(LiteralAmountToken),
                    typeof(LiteralToken),
                    typeof(UnitToken),
                    typeof(LiteralToken)
                },
                // Expected parsed ingredient
                new ParseResult()
                {
                    Ingredient = new ParseResult.IngredientDetails()
                    {
                        Amount = "3",
                        Form = null,
                        Unit = "cups",
                        Ingredient = "cheese"
                    },
                    Metadata = new ParseResult.ParseMetadata()
                }
            }
        };

        [Test]
        [TestCaseSource(nameof(_testCases))]
        public void IngredientParser_TryReadIngredient_ShouldSuccessfullyParseIngredient(
            string templateDefinition,
            string rawIngredient,
            Type[] expectedTokens,
            ParseResult expectedParsedResult)
        {
            var parser = IngredientParser
                .Builder
                .New
                .WithTemplateDefinitions(templateDefinition)
                .WithTokenReaderFactory(new TokenReaderFactory(_tokenReaders))
                .WithParserStrategy(ParserStrategyOption.OnlyAcceptFullMatch)
                .WithParserStrategyFactory(new ParserStrategyFactory(_parserStrategies))
                .Build();

            var result = parser.TryParseIngredient(rawIngredient, out var parseResult);
            
            Assert.IsTrue(result);
            Assert.AreEqual(TemplateMatchResult.FullMatch, parseResult.Metadata.MatchResult);
            
            var tokenList = parseResult.Metadata.Tokens.ToList();
            
            Assert.AreEqual(expectedTokens.Length, tokenList.Count);

            var tokenTypes = tokenList
                .Select(t => t.GetType())
                .ToList();
            
            CollectionAssert.AreEqual(expectedTokens, tokenTypes);
            
            Assert.AreEqual(expectedParsedResult.Ingredient.Amount, parseResult.Ingredient.Amount);
            Assert.AreEqual(expectedParsedResult.Ingredient.Unit, parseResult.Ingredient.Unit);
            Assert.AreEqual(expectedParsedResult.Ingredient.Form, parseResult.Ingredient.Form);
            Assert.AreEqual(expectedParsedResult.Ingredient.Ingredient, parseResult.Ingredient.Ingredient);
        }
    }
}