using System;
using System.Collections.Generic;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Represents the result of parsing.
    /// </summary>
    public class ParseResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult"/> class.
        /// </summary>
        private ParseResult()
        {
        }

        /// <summary>
        /// Gets a value indicating whether the result represents a successful parse operation.
        /// </summary>
        public bool IsSuccess => Details != null;

        /// <summary>
        /// Gets or sets the ingredient details extracted by the parser.
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null"/> if the parse operation failed.
        /// </remarks>
        public IngredientDetails Details { get; private set; }
        
        /// <summary>
        /// Gets or sets metadata relating to the parse operation.
        /// </summary>
        public ParseMetadata Metadata { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ParseResult"/> instance representing a successful parse operation.
        /// </summary>
        /// <param name="matchedTemplate">The template that was successfully matched.</param>
        /// <param name="matchedTokens">The collection of tokens parsed.</param>
        /// <param name="matchResult">The type of match result.</param>
        /// <param name="attemptedTemplates">The collection of templates that were attempted.</param>
        /// <returns>A <see cref="ParseResult"/> instance representing a successful parse operation.</returns>
        public static ParseResult Success(
            Template matchedTemplate,
            IReadOnlyCollection<IToken> matchedTokens,
            TemplateMatchResult matchResult,
            IReadOnlyCollection<Template> attemptedTemplates)
        {
            var result = new ParseResult()
            {
                Details = new IngredientDetails(),
                Metadata = new ParseMetadata()
                {
                    MatchedTemplate = matchedTemplate,
                    MatchedTokens = matchedTokens,
                    MatchResult = matchResult,
                    AttemptedTemplates = attemptedTemplates
                }
            };

            // Populate the ingredient details by visiting each matched token.
            var tokenVisitor = new ParserTokenVisitor(result);
                
            foreach (var token in result.Metadata.MatchedTokens)
            {
                token.Accept(tokenVisitor);
            }

            return result;
        }

        /// <summary>
        /// Creates a new <see cref="ParseResult"/> instance representing a failed parse operation.
        /// </summary>
        /// <param name="attemptedTemplates">The collection of templates that were attempted.</param>
        /// <returns>A <see cref="ParseResult"/> instance representing a failed parse operation.</returns>
        public static ParseResult Fail(IReadOnlyCollection<Template> attemptedTemplates) =>
            new ParseResult()
            {
                Details = null,
                Metadata = new ParseMetadata()
                {
                    MatchedTemplate = null,
                    MatchedTokens = Array.Empty<IToken>(),
                    MatchResult = TemplateMatchResult.NoMatch,
                    AttemptedTemplates = attemptedTemplates
                }
            };
    }
}