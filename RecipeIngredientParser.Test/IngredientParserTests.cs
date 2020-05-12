using System;
using System.Linq;
using NUnit.Framework;
using RecipeIngredientParser.Core;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Test
{
    public class IngredientParserTests
    {
        private IngredientParser _ingredientParser;
        private ITokenReader[] _tokenReaders = 
        {
            new AmountTokenReader(), 
            new UnitTokenReader(), 
            new IngredientTokenReader()
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
                new ParsedIngredient()
                {
                    RawIngredient = "1 bag vegan sausages",
                    Amount = "1",
                    Form = null,
                    Unit = "bag",
                    Ingredient = "vegan sausages"
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
                new ParsedIngredient()
                {
                    RawIngredient = "2 grams chocolate",
                    Amount = "2",
                    Form = null,
                    Unit = "grams",
                    Ingredient = "chocolate"
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
                new ParsedIngredient()
                {
                    RawIngredient = "cheese: 3 cups",
                    Amount = "3",
                    Form = null,
                    Unit = "cups",
                    Ingredient = "cheese"
                }
            }
        };

        [Test]
        [TestCaseSource(nameof(_testCases))]
        public void IngredientParser_TryReadIngredient_ShouldSuccessfullyParseIngredient(
            string templateDefinition,
            string rawIngredient,
            Type[] expectedTokens,
            ParsedIngredient expectedParsedIngredient)
        {
            var parser = IngredientParser.Builder
                .New
                .WithTemplateDefinitions(templateDefinition)
                .WithTokenReaderFactory(new TokenReaderFactory(_tokenReaders))
                .Build();

            var result = parser.TryParseIngredient(rawIngredient, out var parsedIngredient);
            
            Assert.IsTrue(result);
            Assert.AreEqual(TemplateMatchResult.FullMatch, parsedIngredient.Metadata.MatchResult);
            
            var tokenList = parsedIngredient.Metadata.Tokens.ToList();
            
            Assert.AreEqual(expectedTokens.Length, tokenList.Count);

            var tokenTypes = tokenList
                .Select(t => t.GetType())
                .ToList();
            
            CollectionAssert.AreEqual(expectedTokens, tokenTypes);
            
            Assert.AreEqual(expectedParsedIngredient.RawIngredient, parsedIngredient.RawIngredient);
            Assert.AreEqual(expectedParsedIngredient.Amount, parsedIngredient.Amount);
            Assert.AreEqual(expectedParsedIngredient.Unit, parsedIngredient.Unit);
            Assert.AreEqual(expectedParsedIngredient.Form, parsedIngredient.Form);
            Assert.AreEqual(expectedParsedIngredient.Ingredient, parsedIngredient.Ingredient);
        }
    }
}