using System;
using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Parser.Strategy
{
    /// <summary>
    /// Defines a delegate that can take a collection of parse metadata for template matches and extract
    /// the 'best' match.
    /// </summary>
    /// <param name="matches">A collection of parse metadata for template matches.</param>
    public delegate ParseResult.ParseMetadata BestMatchHeuristic(IEnumerable<ParseResult.ParseMetadata> matches);

    /// <summary>
    /// Defines a set of <see cref="BestMatchHeuristic"/>.
    /// </summary>
    public static class BestMatchHeuristics
    {
        /// <summary>
        /// A heuristic that will extract the match that has the greatest number of tokens.
        /// </summary>
        /// <returns>The match with the greatest number of tokens.</returns>
        public static BestMatchHeuristic GreatestNumberOfTokensHeuristic() => 
            matches => 
                matches
                    .OrderByDescending(m => m.Tokens.Count())
                    .First();

        /// <summary>
        /// A heuristic that will use a set of token weights to determine which match has the highest score.
        /// </summary>
        /// <param name="tokenWeightResolver">A function that can be used to get the weight for a given token.</param>
        /// <returns>The match with the greatest weight score.</returns>
        public static BestMatchHeuristic WeightedTokenHeuristic(Func<IToken, decimal> tokenWeightResolver) =>
            matches =>
            {
                var matchesWithSummedWeights = new List<(ParseResult.ParseMetadata Match, decimal SummedWeights)>();
                
                foreach (var match in matches)
                {
                    var summedWeights = match
                        .Tokens
                        .Sum(tokenWeightResolver);
                    
                    matchesWithSummedWeights.Add((match, summedWeights));
                }
                
                return matchesWithSummedWeights
                    .OrderByDescending(t => t.SummedWeights)
                    .First()
                    .Match;
            };
    } 
}