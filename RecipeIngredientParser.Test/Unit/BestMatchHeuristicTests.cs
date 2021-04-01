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
        private static readonly ParseResult.ParseMetadata[] Matches =
        {
            new ParseResult.ParseMetadata()
            {
                Tokens = new IToken[]
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
                MatchResult = TemplateMatchResult.FullMatch,
                Template = null
            },
            new ParseResult.ParseMetadata()
            {
                Tokens = new IToken[]
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
                MatchResult = TemplateMatchResult.FullMatch,
                Template = null
            }
        };
        
        [Test]
        public void
            GreatestNumberOfTokensHeuristic_MultipleMatchesWithDifferentNumbersOfTokens_ShouldReturnTheMatchWithTheGreatestNumberOfTokens()
        {
            var heuristic = BestMatchHeuristics.GreatestNumberOfTokensHeuristic();
            
            var expectedBestMatch = Matches.Last();

            var bestMatch = heuristic.Invoke(Matches);
            
            Assert.AreEqual(expectedBestMatch, bestMatch);
        }

        private static dynamic[][] _weightedTokenHeuristicTestCases =
        {
            new dynamic[]
            {
                -2.0m,
                Matches.First()
            },
            new dynamic[]
            {
                2.0m,
                Matches.Last()
            }
        };
        
        [Test]
        [TestCaseSource(nameof(_weightedTokenHeuristicTestCases))]
        public void
            WeightedTokenHeuristic_MultipleMatchesWithDifferentNumbersOfTokens_ShouldReturnTheMatchWithTheGreatestWeightScore(
                decimal formTokenWeight,
                ParseResult.ParseMetadata expectedBestMatch)
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
            
            var bestMatch = heuristic.Invoke(Matches);
            
            Assert.AreEqual(expectedBestMatch, bestMatch);
        }
    }
}