using System.Linq;
using NUnit.Framework;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Test.Unit
{
    public class BestMatchHeuristicTests
    {
        private static readonly ParseResult[] Results =
        {
            ParseResult.Success(
                matchedTokens: new IToken[]
                {
                    new LiteralToken(),
                    AmountToken.Literal(1m),
                    new LiteralToken(),
                    new UnitToken(),
                    new LiteralToken(),
                    new IngredientToken(),
                    new LiteralToken()
                },
                // These properties don't matter.
                matchedTemplate: null,
                matchResult: TemplateMatchResult.FullMatch,
                attemptedTemplates: null),
            ParseResult.Success(
                matchedTokens: new IToken[]
                {
                    new LiteralToken(),
                    AmountToken.Literal(1m),
                    new LiteralToken(),
                    new UnitToken(),
                    new LiteralToken(),
                    new FormToken(),
                    new LiteralToken(),
                    new IngredientToken(),
                    new LiteralToken()
                },
                // These properties don't matter.
                matchedTemplate: null,
                matchResult: TemplateMatchResult.FullMatch,
                attemptedTemplates: null)
    };
        
        [Test]
        public void GreatestNumberOfTokensHeuristic_MultipleMatchesWithDifferentNumbersOfTokens_ShouldReturnTheMatchWithTheGreatestNumberOfTokens()
        {
            var heuristic = BestMatchHeuristics.GreatestNumberOfTokensHeuristic();
            
            var expectedBestResult = Results.Last();

            var bestMatch = heuristic.Invoke(Results);
            
            Assert.AreEqual(expectedBestResult, bestMatch);
        }

        private static dynamic[][] _weightedTokenHeuristicTestCases =
        {
            new dynamic[]
            {
                -2.0m,
                Results.First()
            },
            new dynamic[]
            {
                2.0m,
                Results.Last()
            }
        };
        
        [Test]
        [TestCaseSource(nameof(_weightedTokenHeuristicTestCases))]
        public void
            WeightedTokenHeuristic_MultipleMatchesWithDifferentNumbersOfTokens_ShouldReturnTheMatchWithTheGreatestWeightScore(
                decimal formTokenWeight,
                ParseResult expectedBestResult)
        {
            var heuristic = BestMatchHeuristics.WeightedTokenHeuristic(token =>
            {
                switch (token)
                {
                    case LiteralToken _:
                        return 0.0m;
                    
                    case LiteralAmountToken _:
                    case FractionalAmountToken _:
                    case RangeAmountToken _:
                        return 1.0m;
                    
                    case UnitToken _:
                        return 1.0m;
                    
                    case FormToken _:
                        return formTokenWeight;
                    
                    case IngredientToken _:
                        return 2.0m;
                }

                return 0.0m;
            });
            
            var bestMatch = heuristic.Invoke(Results);
            
            Assert.AreEqual(expectedBestResult, bestMatch);
        }
    }
}