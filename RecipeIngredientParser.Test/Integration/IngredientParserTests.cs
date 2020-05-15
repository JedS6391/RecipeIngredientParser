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

namespace RecipeIngredientParser.Test.Integration
{
    public class IngredientParserTests
    {
        private readonly ITokenReader[] _tokenReaders = 
        {
            new AmountTokenReader(), 
            new UnitTokenReader(), 
            new FormTokenReader(),
            new IngredientTokenReader()
        };

        private readonly IParserStrategy[] _parserStrategies =
        {
            new FirstFullMatchParserStrategy(),
            new BestFullMatchParserStrategy(), 
            new BestPartialMatchParserStrategy()
        };
        
        [Test]
        [TestCaseSource(nameof(_singleTemplateTestCases))]
        public void IngredientParser_TryReadIngredientWithSingleTemplate_ShouldSuccessfullyParseIngredient(
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
                .WithParserStrategy(ParserStrategyOption.AcceptFirstFullMatch)
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
            
            Assert.AreEqual(expectedParsedResult.Details.Amount, parseResult.Details.Amount);
            Assert.AreEqual(expectedParsedResult.Details.Unit, parseResult.Details.Unit);
            Assert.AreEqual(expectedParsedResult.Details.Form, parseResult.Details.Form);
            Assert.AreEqual(expectedParsedResult.Details.Ingredient, parseResult.Details.Ingredient);
        }
        
        [Test]
        [TestCaseSource(nameof(_multipleTemplateTestCases))]
        public void IngredientParser_TryReadIngredientWithMultipleTemplates_ShouldSuccessfullyParseIngredient(
            string[] templateDefinition,
            ParserStrategyOption strategyOption,
            string rawIngredient,
            Type[] expectedTokens,
            ParseResult expectedParsedResult)
        {
            var parser = IngredientParser
                .Builder
                .New
                .WithTemplateDefinitions(templateDefinition)
                .WithTokenReaderFactory(new TokenReaderFactory(_tokenReaders))
                .WithParserStrategy(strategyOption)
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
            
            Assert.AreEqual(expectedParsedResult.Details.Amount, parseResult.Details.Amount);
            Assert.AreEqual(expectedParsedResult.Details.Unit, parseResult.Details.Unit);
            Assert.AreEqual(expectedParsedResult.Details.Form, parseResult.Details.Form);
            Assert.AreEqual(expectedParsedResult.Details.Ingredient, parseResult.Details.Ingredient);
        }

        #region Test case definitions

        private static dynamic[][] _singleTemplateTestCases =
        {
            new dynamic[]
            {
                // Template definition
                TemplateDefinitions.AmountUnitIngredient,
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
                    Details = new ParseResult.IngredientDetails()
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
                TemplateDefinitions.AmountUnitIngredient,
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
                    Details = new ParseResult.IngredientDetails()
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
                TemplateDefinitions.IngredientAmountUnit,
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
                    Details = new ParseResult.IngredientDetails()
                    {
                        Amount = "3",
                        Form = null,
                        Unit = "cups",
                        Ingredient = "cheese"
                    },
                    Metadata = new ParseResult.ParseMetadata()
                }
            },
            new dynamic[]
            {
                // Template definition
                TemplateDefinitions.AmountUnitFormIngredient,
                // Raw ingredient
                "2 cups grated cheese",
                // Expected tokens
                new[]
                {
                    typeof(LiteralToken),
                    typeof(LiteralAmountToken),
                    typeof(LiteralToken),
                    typeof(UnitToken),
                    typeof(LiteralToken),
                    typeof(FormToken),
                    typeof(LiteralToken),
                    typeof(IngredientToken),
                    typeof(LiteralToken)
                },
                // Expected parsed ingredient
                new ParseResult()
                {
                    Details = new ParseResult.IngredientDetails()
                    {
                        Amount = "2",
                        Form = "grated",
                        Unit = "cups",
                        Ingredient = "cheese"
                    },
                    Metadata = new ParseResult.ParseMetadata()
                }
            }
        };
        
        private static dynamic[][] _multipleTemplateTestCases =
        {
            #region AcceptFirstFullMatch
            
            new dynamic[]
            {
                // Template definitions
                new
                {
                    TemplateDefinitions.AmountUnitFormIngredient,
                    TemplateDefinitions.AmountUnitIngredient
                },
                // Strategy option
                ParserStrategyOption.AcceptFirstFullMatch,
                // Raw ingredient
                "2 cups grated cheese",
                // Expected tokens
                new[]
                {
                    typeof(LiteralToken),
                    typeof(LiteralAmountToken),
                    typeof(LiteralToken),
                    typeof(UnitToken),
                    typeof(LiteralToken),
                    typeof(FormToken),
                    typeof(LiteralToken),
                    typeof(IngredientToken),
                    typeof(LiteralToken)
                },
                // Expected parsed ingredient
                new ParseResult()
                {
                    Details = new ParseResult.IngredientDetails()
                    {
                        Amount = "2",
                        Form = "grated",
                        Unit = "cups",
                        Ingredient = "cheese"
                    },
                    Metadata = new ParseResult.ParseMetadata()
                }
            },
            new dynamic[]
            {
                // Template definitions
                new
                {
                    TemplateDefinitions.AmountUnitIngredient,
                    TemplateDefinitions.AmountUnitFormIngredient
                },
                // Strategy option
                ParserStrategyOption.AcceptFirstFullMatch,
                // Raw ingredient
                "2 cups grated cheese",
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
                    Details = new ParseResult.IngredientDetails()
                    {
                        Amount = "2",
                        Form = null,
                        Unit = "cups",
                        Ingredient = "grated cheese"
                    },
                    Metadata = new ParseResult.ParseMetadata()
                }
            },
            
            #endregion
            
            #region AcceptBestFullMatch
            
            new dynamic[]
            {
                // Template definitions
                new
                {
                    TemplateDefinitions.AmountUnitFormIngredient,
                    TemplateDefinitions.AmountUnitIngredient
                },
                // Strategy option
                ParserStrategyOption.AcceptBestFullMatch,
                // Raw ingredient
                "2 cups grated cheese",
                // Expected tokens
                new[]
                {
                    typeof(LiteralToken),
                    typeof(LiteralAmountToken),
                    typeof(LiteralToken),
                    typeof(UnitToken),
                    typeof(LiteralToken),
                    typeof(FormToken),
                    typeof(LiteralToken),
                    typeof(IngredientToken),
                    typeof(LiteralToken)
                },
                // Expected parsed ingredient
                new ParseResult()
                {
                    Details = new ParseResult.IngredientDetails()
                    {
                        Amount = "2",
                        Form = "grated",
                        Unit = "cups",
                        Ingredient = "cheese"
                    },
                    Metadata = new ParseResult.ParseMetadata()
                }
            },
            new dynamic[]
            {
                // Template definitions
                new
                {
                    TemplateDefinitions.AmountUnitIngredient,
                    TemplateDefinitions.AmountUnitFormIngredient
                },
                // Strategy option
                ParserStrategyOption.AcceptBestFullMatch,
                // Raw ingredient
                "2 cups grated cheese",
                // Expected tokens
                new[]
                {
                    typeof(LiteralToken),
                    typeof(LiteralAmountToken),
                    typeof(LiteralToken),
                    typeof(UnitToken),
                    typeof(LiteralToken),
                    typeof(FormToken),
                    typeof(LiteralToken),
                    typeof(IngredientToken),
                    typeof(LiteralToken)
                },
                // Expected parsed ingredient
                new ParseResult()
                {
                    Details = new ParseResult.IngredientDetails()
                    {
                        Amount = "2",
                        Form = "grated",
                        Unit = "cups",
                        Ingredient = "cheese"
                    },
                    Metadata = new ParseResult.ParseMetadata()
                }
            }
            
            #endregion
        };
        
        #endregion
    }
}