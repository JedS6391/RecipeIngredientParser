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
        public bool Handles(ParserStrategyOption strategyOption) =>
            strategyOption == ParserStrategyOption.AcceptBestFullMatch;

        /// <inheritdoc/>
        public bool TryParseIngredient(
            ParserContext context,
            IEnumerable<Template> templates,
            out ParseResult parseResult)
        {
            var fullMatches = FindFullMatches(context, templates);

            if (fullMatches.Any())
            {
                var bestMatchMetadata = _bestMatchHeuristic.Invoke(fullMatches);

                parseResult = new ParseResult()
                {
                    Details = new ParseResult.IngredientDetails(),
                    Metadata = bestMatchMetadata
                };
                
                foreach (var token in parseResult.Metadata.Tokens)
                {
                    token.Accept(new ParserTokenVisitor(parseResult));
                }

                return true;
            }
            
            parseResult = null;

            return false;
        }

        private List<ParseResult.ParseMetadata> FindFullMatches(
            ParserContext context, 
            IEnumerable<Template> templates)
        {
            var fullMatches = new List<ParseResult.ParseMetadata>();
            
            foreach (var template in templates)
            {
                context.Buffer.Reset();
                
                var result = template.TryReadTokens(context, out var tokens);

                switch (result)
                {
                    case TemplateMatchResult.NoMatch:
                    case TemplateMatchResult.PartialMatch:
                        // Always skip non-matches or partial matches.
                        continue;
                    
                    case TemplateMatchResult.FullMatch:
                        // Stop on the first full match
                        fullMatches.Add( new ParseResult.ParseMetadata()
                        {
                            Template = template,
                            MatchResult = TemplateMatchResult.FullMatch,
                            Tokens =  tokens.ToList()
                        });

                        continue;

                    default:
                        throw new ArgumentOutOfRangeException(
                $"Encountered unknown template match result: {result}");
                }
            }

            return fullMatches;
        }
    }
}