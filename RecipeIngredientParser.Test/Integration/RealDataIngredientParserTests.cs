using System;
using NUnit.Framework;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Test.Integration
{
    public class RealDataIngredientParserTests
    {
        [Test]
        [TestCaseSource(nameof(_veganNachosTestCases))]
        [TestCaseSource(nameof(_vegetarianBlackBeanEnchiladas))]
        public void IngredientParser_TryReadRealWorldDataIngredients_ShouldSuccessfullyParseIngredient(
            string rawIngredient,
            bool expectedResult,
            string expectedTemplateDefinition,
            IngredientDetails expectedDetails)
        {
            var parser = IngredientParserBuilder
                .New
                .WithDefaultConfiguration()
                .WithParserStrategy(
                    new BestFullMatchParserStrategy(
                        BestMatchHeuristics.WeightedTokenHeuristic(TokenWeightResolver)))
                .Build();

            var parseResult = parser.ParseIngredient(rawIngredient);
            
            Assert.AreEqual(expectedResult, parseResult.IsSuccess);

            if (expectedResult)
            {
                Assert.IsNotNull(parseResult);
                Assert.AreEqual(expectedDetails.Amount, parseResult.Details.Amount);
                Assert.AreEqual(expectedDetails.Unit, parseResult.Details.Unit);
                Assert.AreEqual(expectedDetails.Form, parseResult.Details.Form);
                Assert.AreEqual(expectedDetails.Ingredient, parseResult.Details.Ingredient);
                Assert.AreEqual(TemplateMatchResult.FullMatch, parseResult.Metadata.MatchResult);
                Assert.AreEqual(expectedTemplateDefinition, parseResult.Metadata.MatchedTemplate.Definition);
                Assert.IsNotEmpty(parseResult.Metadata.AttemptedTemplates);
            }
            else
            {
                Assert.IsNotNull(parseResult);
                Assert.IsNull(parseResult.Details);
                Assert.AreEqual(TemplateMatchResult.NoMatch, parseResult.Metadata.MatchResult);
                Assert.IsNull(parseResult.Metadata.MatchedTemplate);
                Assert.IsEmpty(parseResult.Metadata.MatchedTokens);
                Assert.IsNotEmpty(parseResult.Metadata.AttemptedTemplates);
            }
        }

        #region Helpers

        private static Func<IToken, decimal> TokenWeightResolver => token =>
        {
            switch (token)
            {
                case LiteralToken literalToken:
                    // Longer literals score more - the assumption being that
                    // a longer literal means a more specific value.
                    return 0.1m * literalToken.Value.Length;
                    
                case LiteralAmountToken _:
                case FractionalAmountToken _:
                case RangeAmountToken _:
                    return 1.0m;
                    
                case UnitToken unitToken:
                    return unitToken.Type == UnitType.Unknown ?
                        // Punish unknown unit types
                        -1.0m :
                        1.0m;
                    
                case FormToken _:
                    return 1.0m;
                    
                case IngredientToken _:
                    return 2.0m;
            }

            return 0.0m;
        };

        #endregion
        
        #region Test cases

        private static dynamic[][] _veganNachosTestCases =
        {
            new dynamic[]
            {
                "1 Full Recipe Vegan Taco Meat",
                // Successful?
                true,
                "{amount} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = null,
                    Form = null,
                    Ingredient = "full recipe vegan taco meat"
                }
            },
            new dynamic[]
            {
                "1 Tbsp Olive Oil",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = "tbsp",
                    Form = null,
                    Ingredient = "olive oil"
                }
            },
            new dynamic[]
            {
                "1/2 Onion (Chopped)",
                // Successful?
                true,
                "{amount} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1/2",
                    Unit = null,
                    Form = null,
                    Ingredient = "onion"
                }
            },            
            new dynamic[]
            {
                "1/4 tsp Cayenne Pepper",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1/4",
                    Unit = "tsp",
                    Form = null,
                    Ingredient = "cayenne pepper"
                }
            },
            new dynamic[]
            {
                "1/4 tsp Chili Flakes",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1/4",
                    Unit = "tsp",
                    Form = null,
                    Ingredient = "chili flakes"
                }
            },
            new dynamic[]
            {
                "1/2 tsp Cumin",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1/2",
                    Unit = "tsp",
                    Form = null,
                    Ingredient = "cumin"
                }
            },
            new dynamic[]
            {
                // TODO: Is it possible to get this test case to pass?
                "1 15oz (425g) Can Black Beans (Drained)",
                // Successful?
                false,
                null,
                null
            },
            new dynamic[]
            {
                "2 Tbsp Water",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "2",
                    Unit = "tbsp",
                    Form = null,
                    Ingredient = "water"
                }
            },
            new dynamic[]
            {
                "Sea Salt and Black Pepper (To Taste)",
                // Successful?
                true,
                "{ingredient}",
                new IngredientDetails()
                {
                    Amount = null,
                    Unit = null,
                    Form = null,
                    Ingredient = "sea salt and black pepper"
                }
            },  
            new dynamic[]
            {
                "1 Full Recipe Pico De Gallo",
                // Successful?
                true,
                "{amount} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = null,
                    Form = null,
                    Ingredient = "full recipe pico de gallo"
                }
            },  
            new dynamic[]
            {
                "1 Full Recipe Vegan Nacho Cheese",
                // Successful?
                true,
                "{amount} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = null,
                    Form = null,
                    Ingredient = "full recipe vegan nacho cheese"
                }
            },  
            new dynamic[]
            {
                "2 Avocados",
                // Successful?
                true,
                "{amount} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "2",
                    Unit = null,
                    Form = null,
                    Ingredient = "avocados"
                }
            },
            new dynamic[]
            {
                "1 Tbsp Fresh Lime Juice",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = "tbsp",
                    Form = null,
                    Ingredient = "fresh lime juice"
                }
            },
            new dynamic[]
            {
                "Sea Salt and Black Pepper (To Taste)",
                // Successful?
                true,
                "{ingredient}",
                new IngredientDetails()
                {
                    Amount = null,
                    Unit = null,
                    Form = null,
                    Ingredient = "sea salt and black pepper"
                }
            },
            new dynamic[]
            {
                "16oz (450g) Tortilla Chips",
                // Successful?
                true,
                "{amount}{unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "16",
                    Unit = "oz",
                    Form = null,
                    Ingredient = "tortilla chips"
                }
            },
            new dynamic[]
            {
                "Fresh Cilantro",
                // Successful?
                true,
                "{ingredient}",
                new IngredientDetails()
                {
                    Amount = null,
                    Unit = null,
                    Form = null,
                    Ingredient = "fresh cilantro"
                }
            }
        };

        private static dynamic[][] _vegetarianBlackBeanEnchiladas =
        {
            new dynamic[]
            {
                "2 cups homemade enchilada sauce",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "2",
                    Unit = "cups",
                    Form = null,
                    Ingredient = "homemade enchilada sauce"
                }
            },
            new dynamic[]
            {
                "2 tablespoons olive oil",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "2",
                    Unit = "tablespoons",
                    Form = null,
                    Ingredient = "olive oil"
                }
            },
            new dynamic[]
            {
                "1 cup chopped red onion (about 1 small red onion)",
                // Successful?
                true,
                "{amount} {unit} {form} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = "cup",
                    Form = "chopped",
                    Ingredient = "red onion"
                }
            },
            new dynamic[]
            {
                "1 red bell pepper, chopped",
                // Successful?
                true,
                "{amount} {ingredient}, {form}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = null,
                    Form = "chopped",
                    Ingredient = "red bell pepper"
                }
            },
            new dynamic[]
            {
                "1 bunch of broccoli or 1 small head of cauliflower (about 1 pound), florets removed and sliced into small, bite-sized pieces",
                // Successful?
                true,
                "{amount} {unit} of {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = "bunch",
                    Form = null,
                    Ingredient = "broccoli"
                }
            },
            new dynamic[]
            {
                "1 teaspoon Frontier Co-op Ground Cumin",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = "teaspoon",
                    Form = null,
                    Ingredient = "frontier co-op ground cumin"
                }
            },
            new dynamic[]
            {
                "¼ teaspoon Frontier Co-op Ground Cinnamon",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1/4",
                    Unit = "teaspoon",
                    Form = null,
                    Ingredient = "frontier co-op ground cinnamon"
                }
            },
            new dynamic[]
            {
                "5 to 6 ounces baby spinach (about 5 cups, packed)",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "5-6",
                    Unit = "ounces",
                    Form = null,
                    Ingredient = "baby spinach"
                }
            },
            new dynamic[]
            {
                "1 can (15 ounces) black beans, drained and rinsed, or 1 ½ cups cooked black beans",
                // Successful?
                true,
                "{amount} {unit} {ingredient}, {form}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = "can",
                    Form = "drained",
                    Ingredient = "black beans"
                }
            },
            new dynamic[]
            {
                "1 cup shredded Monterey Jack cheese, divided",
                // Successful?
                true,
                "{amount} {unit} {form} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1",
                    Unit = "cup",
                    Form = "shredded",
                    Ingredient = "monterey jack cheese"
                }
            },
            new dynamic[]
            {
                "½ teaspoon salt, to taste",
                // Successful?
                true,
                "{amount} {unit} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "1/2",
                    Unit = "teaspoon",
                    Form = null,
                    Ingredient = "salt"
                }
            },
            new dynamic[]
            {
                "Freshly ground black pepper, to taste",
                // Successful?
                true,
                "{ingredient}",
                new IngredientDetails()
                {
                    Amount = null,
                    Unit = null,
                    Form = null,
                    Ingredient = "freshly ground black pepper"
                }
            },
            new dynamic[]
            {
                "8 whole wheat tortillas (about 8” in diameter)",
                // Successful?
                true,
                "{amount} {ingredient}",
                new IngredientDetails()
                {
                    Amount = "8",
                    Unit = null,
                    Form = null,
                    Ingredient = "whole wheat tortillas"
                }
            },
            new dynamic[]
            {
                "Handful of chopped cilantro, for garnishing",
                // Successful?
                true,
                "{unit} of {form} {ingredient}",
                new IngredientDetails()
                {
                    Amount = null,
                    Unit = "handful",
                    Form = "chopped",
                    Ingredient = "cilantro"
                }
            }
        };

        #endregion
    }
}