using System;
using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens;

namespace RecipeIngredientParser.Core.Parser.Strategy
{
    /// <summary>
    /// A <see cref="IParserStrategy"/> implementation that accepts full matches
    /// or the best partial match if no full match is found.
    /// </summary>
    public class BestPartialMatchParserStrategy : IParserStrategy
    {
        private readonly BestMatchHeuristic _bestMatchHeuristic;

        /// <summary>
        /// Initialises a new instance of the <see cref="BestFullMatchParserStrategy"/>
        /// with the default <see cref="BestMatchHeuristic"/>.
        /// </summary>
        public BestPartialMatchParserStrategy()
        {
            _bestMatchHeuristic = BestMatchHeuristics.GreatestNumberOfTokensHeuristic();
        }
        
        /// <summary>
        /// Initialises a new instance of the <see cref="BestFullMatchParserStrategy"/>
        /// with the specified <see cref="BestMatchHeuristic"/>.
        /// </summary>
        public BestPartialMatchParserStrategy(BestMatchHeuristic bestMatchHeuristic)
        {
            _bestMatchHeuristic = bestMatchHeuristic;
        }
        
        /// <inheritdoc/>
        public ParseResult ParseIngredient(ParserContext context, IEnumerable<Template> templates)
        {
            var attemptedTemplates = new List<Template>();
            var partialMatchResults = new List<ParseResult>();
            
            foreach (var template in templates)
            {
                context.Buffer.Reset();
                
                var result = template.TryReadTokens(context, out var tokens);

                attemptedTemplates.Add(template);

                switch (result)
                {
                    case TemplateMatchResult.NoMatch:
                        // Always skip non-matches.
                        continue;

                    case TemplateMatchResult.PartialMatch:
                        // Take a note of partial matches, so that we can find the best match when no full matches are found
                        partialMatchResults.Add(ParseResult.Success(
                            template,
                            tokens.ToList(),
                            TemplateMatchResult.PartialMatch,
                            attemptedTemplates));
                        
                        continue;
                    
                    case TemplateMatchResult.FullMatch:
                        // Stop on the first full match.
                        return ParseResult.Success(
                            template,
                            tokens.ToList(),
                            TemplateMatchResult.FullMatch,
                            attemptedTemplates);
                    
                    default:
                        throw new ArgumentOutOfRangeException($"Encountered unknown template match result: {result}");
                }
            }
            
            if (partialMatchResults.Any())
            {
                var bestMatchResult = _bestMatchHeuristic.Invoke(partialMatchResults);

                return bestMatchResult;
            }

            return ParseResult.Fail(attemptedTemplates);
        }
    }
}