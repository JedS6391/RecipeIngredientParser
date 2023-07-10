using System;
using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Templates;

namespace RecipeIngredientParser.Core.Parser.Strategy
{
    /// <summary>
    /// A <see cref="IParserStrategy"/> implementation that only accepts full template matches.
    /// </summary>
    /// <remarks>
    /// Note that only the best full match will be returned.
    /// </remarks>
    public class BestFullMatchParserStrategy : IParserStrategy
    {       
        private readonly BestMatchHeuristic _bestMatchHeuristic;

        /// <summary>
        /// Initialises a new instance of the <see cref="BestFullMatchParserStrategy"/>
        /// with the default <see cref="BestMatchHeuristic"/>.
        /// </summary>
        public BestFullMatchParserStrategy()
        {
            _bestMatchHeuristic = BestMatchHeuristics.GreatestNumberOfTokensHeuristic();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="BestFullMatchParserStrategy"/>
        /// with the specified <see cref="BestMatchHeuristic"/>.
        /// </summary>
        public BestFullMatchParserStrategy(BestMatchHeuristic bestMatchHeuristic)
        {
            _bestMatchHeuristic = bestMatchHeuristic;
        }
        
        /// <inheritdoc/>
        public ParseResult ParseIngredient(ParserContext context, IEnumerable<Template> templates)
        {
            var (attemptedTemplates, fullMatchResults) = FindFullMatches(context, templates);

            if (fullMatchResults.Any())
            {
                return _bestMatchHeuristic.Invoke(fullMatchResults);
            }

            return ParseResult.Fail(attemptedTemplates);
        }

        private (List<Template> attemptedTemplates, List<ParseResult> fullMatchResults) FindFullMatches(
            ParserContext context,
            IEnumerable<Template> templates)
        {
            var attemptedTemplates = new List<Template>();
            var fullMatchResults = new List<ParseResult>();
            
            foreach (var template in templates)
            {
                context.Buffer.Reset();
                
                var result = template.TryReadTokens(context, out var tokens);

                attemptedTemplates.Add(template);

                switch (result)
                {
                    case TemplateMatchResult.NoMatch:
                    case TemplateMatchResult.PartialMatch:
                        // Always skip non-matches or partial matches.
                        continue;
                    
                    case TemplateMatchResult.FullMatch:
                        // Keep track of full matches.
                        fullMatchResults.Add(ParseResult.Success(
                            template,
                            tokens.ToList(),
                            TemplateMatchResult.FullMatch,
                            attemptedTemplates));

                        continue;

                    default:
                        throw new ArgumentOutOfRangeException($"Encountered unknown template match result: {result}");
                }
            }

            return (attemptedTemplates, fullMatchResults);
        }
    }
}