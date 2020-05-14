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
        public bool Handles(ParserStrategyOption strategyOption) =>
            strategyOption == ParserStrategyOption.AcceptBestPartialMatch;

            /// <inheritdoc/>
        public bool TryParseIngredient(
            ParserContext context, 
            IEnumerable<Template> templates, 
            out ParseResult parseResult)
        {
            var partialMatches = new List<ParseResult.ParseMetadata>();
            
            foreach (var template in templates)
            {
                context.Buffer.Reset();
                
                var result = template.TryReadTokens(context, out var tokens);

                switch (result)
                {
                    case TemplateMatchResult.NoMatch:
                        // Always skip non-matches.
                        continue;
                    case TemplateMatchResult.PartialMatch:
                        // Take a note of partial matches, so that we can find
                        // the best match when no full matches are found
                        partialMatches.Add(new ParseResult.ParseMetadata()
                        {
                            Template = template,
                            MatchResult = TemplateMatchResult.PartialMatch,
                            Tokens = tokens.ToList()
                        });
                        
                        continue;
                    
                    case TemplateMatchResult.FullMatch:
                        // Stop on the first full match
                        parseResult = new ParseResult()
                        {
                            Details = new ParseResult.IngredientDetails(),
                            Metadata = new ParseResult.ParseMetadata()
                            {
                                Template = template,
                                MatchResult = TemplateMatchResult.FullMatch,
                                Tokens =  tokens.ToList()
                            }
                        };

                        VisitTokens(parseResult);
                        
                        return true;
                    
                    default:
                        throw new ArgumentOutOfRangeException(
                $"Encountered unknown template match result: {result}");
                }
            }
            
            if (partialMatches.Any())
            {
                var bestMatchMetadata = _bestMatchHeuristic.Invoke(partialMatches);

                parseResult = new ParseResult()
                {
                    Details = new ParseResult.IngredientDetails(),
                    Metadata = bestMatchMetadata
                };
                
                VisitTokens(parseResult);

                return true;
            }

            parseResult = null;

            return false;
        }

        private void VisitTokens(ParseResult parseResult)
        {
            foreach (var token in parseResult.Metadata.Tokens)
            {
                token.Accept(new ParserTokenVisitor(parseResult));
            }
        }
    }
}