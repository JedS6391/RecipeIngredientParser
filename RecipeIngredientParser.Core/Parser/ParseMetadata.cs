using System.Collections.Generic;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Describes the metadata of the parse operation.
    /// </summary>
    public class ParseMetadata
    {
        /// <summary>
        /// Gets or sets the template that was successfully matched and used to parse the <see cref="ParseResult.Details"/>.
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null"/> if the parse operation failed.
        /// </remarks>
        public Template MatchedTemplate { get; internal set; }

        /// <summary>
        /// Gets or sets the tokens that were extracted as a result of successful parsing.
        /// </summary>
        /// <remarks>
        /// Will be empty if the parse operation failed.
        /// </remarks>
        public IReadOnlyCollection<IToken> MatchedTokens { get; internal set; } 

        /// <summary>
        /// Gets or sets the type of match against the template.
        /// </summary>
        public TemplateMatchResult MatchResult { get; internal set; }

        /// <summary>
        /// Gets or sets the collection of templates that matching was attempted against.
        /// </summary>
        public IReadOnlyCollection<Template> AttemptedTemplates { get; internal set; }
    }
}