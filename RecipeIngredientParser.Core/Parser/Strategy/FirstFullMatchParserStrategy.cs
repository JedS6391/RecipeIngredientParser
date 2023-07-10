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
    /// Note that only the first full match will be returned.
    /// </remarks>
    public class FirstFullMatchParserStrategy : IParserStrategy
    {
        /// <inheritdoc/>
        public ParseResult ParseIngredient(ParserContext context, IEnumerable<Template> templates)
        {
            var attemptedTemplates = new List<Template>();

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

            return ParseResult.Fail(attemptedTemplates);
        }
    }
}