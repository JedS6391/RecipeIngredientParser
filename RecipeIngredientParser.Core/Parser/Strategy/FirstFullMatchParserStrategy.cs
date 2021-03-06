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
    /// Note that only the first full match will be returned.
    /// </remarks>
    public class FirstFullMatchParserStrategy : IParserStrategy
    {
        /// <inheritdoc/>
        public bool TryParseIngredient(
            ParserContext context, 
            IEnumerable<Template> templates, 
            out ParseResult parseResult)
        {
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

                        var tokenVisitor = new ParserTokenVisitor(parseResult);

                        foreach (var token in parseResult.Metadata.Tokens)
                        {
                            token.Accept(tokenVisitor);
                        }

                        return true;
                    
                    default:
                        throw new ArgumentOutOfRangeException(
                            $"Encountered unknown template match result: {result}");
                }
            }

            parseResult = null;

            return false;
        }
    }
}