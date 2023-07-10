using System;
using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Parser.Strategy
{
    /// <summary>
    /// Defines a delegate that can take a collection of parse results and extract the 'best' match.
    /// </summary>
    /// <param name="results">A collection of parse results.</param>
    public delegate ParseResult BestMatchHeuristic(IEnumerable<ParseResult> results);

    /// <summary>
    /// Defines a set of <see cref="BestMatchHeuristic"/>.
    /// </summary>
    public static class BestMatchHeuristics
    {
        /// <summary>
        /// A heuristic that will extract the result that has the greatest number of tokens.
        /// </summary>
        /// <returns>The result with the greatest number of tokens.</returns>
        public static BestMatchHeuristic GreatestNumberOfTokensHeuristic() => 
            results =>
                results
                    .OrderByDescending(r => r.Metadata.MatchedTokens.Count())
                    .First();

        /// <summary>
        /// A heuristic that will use a set of token weights to determine which match has the highest score.
        /// </summary>
        /// <param name="tokenWeightResolver">A function that can be used to get the weight for a given token.</param>
        /// <returns>The match with the greatest weight score.</returns>
        public static BestMatchHeuristic WeightedTokenHeuristic(Func<IToken, decimal> tokenWeightResolver) =>
            results =>
            {
                var resultsWithSummedWeights = new List<(ParseResult Result, decimal SummedWeights)>();
                
                foreach (var result in results)
                {
                    var summedWeights = result
                        .Metadata
                        .MatchedTokens
                        .Sum(tokenWeightResolver);
                    
                    resultsWithSummedWeights.Add((result, summedWeights));
                }
                
                return resultsWithSummedWeights
                    .OrderByDescending(t => t.SummedWeights)
                    .First()
                    .Result;
            };
    } 
}