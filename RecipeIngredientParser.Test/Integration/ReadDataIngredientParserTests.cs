using System;
using NUnit.Framework;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
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
            bool expectedResult)
        {
            var parserStrategies = new IParserStrategy[]
            {
                new BestFullMatchParserStrategy(BestMatchHeuristics.WeightedTokenHeuristic(TokenWeightResolver))
            };
            
            var parser = IngredientParser
                .Builder
                .New
                .WithDefaultConfiguration()
                .WithParserStrategy(ParserStrategyOption.AcceptBestFullMatch)
                .WithParserStrategyFactory(new ParserStrategyFactory(parserStrategies))
                .Build();

            var result = parser.TryParseIngredient(rawIngredient, out var parseResult);
            
            Assert.AreEqual(expectedResult, result);

            if (expectedResult)
            {
                Assert.AreEqual(TemplateMatchResult.FullMatch, parseResult.Metadata.MatchResult);
            }
            else
            {
                Assert.IsNull(parseResult);
            }
        }

        #region Helpers

        private static Func<IToken, decimal> TokenWeightResolver => token =>
        {
            switch (token)
            {
                case LiteralToken _:
                    return 0.0m;
                    
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
                true
            },
            new dynamic[]
            {
                "1 Tbsp Olive Oil",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1/2 Onion (Chopped)",
                // Successful?
                true
            },            
            new dynamic[]
            {
                "1/4 tsp Cayenne Pepper",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1/4 tsp Cayenne Pepper",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1/4 tsp Chili Flakes",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1/2 tsp Cumin",
                // Successful?
                true
            },      
            // TODO: Figure out a way to get this test case to parse successfully.
            new dynamic[]
            {
                "1 15oz (425g) Can Black Beans (Drained)",
                // Successful?
                false
            },
            new dynamic[]
            {
                "2 Tbsp Water",
                // Successful?
                true
            },
            new dynamic[]
            {
                "Sea Salt and Black Pepper (To Taste)",
                // Successful?
                true
            },  
            new dynamic[]
            {
                "1 Full Recipe Pico De Gallo",
                // Successful?
                true
            },  
            new dynamic[]
            {
                "1 Full Recipe Vegan Nacho Cheese",
                // Successful?
                true
            },  
            new dynamic[]
            {
                "2 Avocados",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1 Tbsp Fresh Lime Juice",
                // Successful?
                true
            },
            new dynamic[]
            {
                "Sea Salt and Black Pepper (To Taste)",
                // Successful?
                true
            },
            new dynamic[]
            {
                "16oz (450g) Tortilla Chips",
                // Successful?
                true
            },
            new dynamic[]
            {
                "Fresh Cilantro",
                // Successful?
                true
            }
        };

        private static dynamic[][] _vegetarianBlackBeanEnchiladas =
        {
            new dynamic[]
            {
                "2 cups homemade enchilada sauce",
                // Successful?
                true
            },
            new dynamic[]
            {
                "2 tablespoons olive oil",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1 cup chopped red onion (about 1 small red onion)",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1 red bell pepper, chopped",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1 bunch of broccoli or 1 small head of cauliflower (about 1 pound), florets removed and sliced into small, bite-sized pieces",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1 teaspoon Frontier Co-op Ground Cumin",
                // Successful?
                true
            },
            new dynamic[]
            {
                "¼ teaspoon Frontier Co-op Ground Cinnamon",
                // Successful?
                true
            },
            new dynamic[]
            {
                "5 to 6 ounces baby spinach (about 5 cups, packed)",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1 can (15 ounces) black beans, drained and rinsed, or 1 ½ cups cooked black beans",
                // Successful?
                true
            },
            new dynamic[]
            {
                "1 cup shredded Monterey Jack cheese, divided",
                // Successful?
                true
            },
            new dynamic[]
            {
                "½ teaspoon salt, to taste",
                // Successful?
                true
            },
            new dynamic[]
            {
                "Freshly ground black pepper, to taste",
                // Successful?
                true
            },
            new dynamic[]
            {
                "8 whole wheat tortillas (about 8” in diameter)",
                // Successful?
                true
            },
            new dynamic[]
            {
                "Handful of chopped cilantro, for garnishing",
                // Successful?
                true
            }
        };

        #endregion
    }
}