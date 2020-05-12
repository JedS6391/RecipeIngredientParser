using System.Collections.Generic;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Represents the result of parsing.
    /// </summary>
    public class ParseResult
    {
        /// <summary>
        /// Gets or sets the ingredient details extracted by the parser.
        /// </summary>
        public IngredientDetails Ingredient { get; set; }
        
        /// <summary>
        /// Gets or sets metadata relating to the parse operation.
        /// </summary>
        public ParseMetadata Metadata { get; set; }

        /// <summary>
        /// Describes the ingredient details extracted by the parser.
        /// </summary>
        public class IngredientDetails
        {
            /// <summary>
            /// Gets or sets the amount of the ingredient.
            /// </summary>
            /// <remarks>
            /// e.g. 1, 1/2, 1-2
            /// </remarks>
            public string Amount { get; set; }
            
            /// <summary>
            /// Gets or sets the unit the ingredient is measured in.
            /// </summary>
            /// <remarks>
            /// e.g. cup, grams, tsp
            /// </remarks>
            public string Unit { get; set; }
            
            /// <summary>
            /// Gets or sets the form the ingredient is in.
            /// </summary>
            /// <remarks>
            /// e.g. shredded, chopped
            /// </remarks>
            public string Form { get; set; }
            
            /// <summary>
            /// Gets or sets the ingredient.
            /// </summary>
            /// <remarks>
            /// e.g. onion, cheese, vegan sausages
            /// </remarks>
            public string Ingredient { get; set; }
        }
        
        /// <summary>
        /// Describes the metadata of the parse operation. 
        /// </summary>
        public class ParseMetadata
        {
            /// <summary>
            /// Gets or sets the template that was used to parse the <see cref="Ingredient"/>.
            /// </summary>
            public Template Template { get; set; }
            
            /// <summary>
            /// Gets or sets the type of match against the template.
            /// </summary>
            public TemplateMatchResult MatchResult { get; set; }
            
            /// <summary>
            /// Gets or sets the tokens that were extracted during the parsing.
            /// </summary>
            public IEnumerable<IToken> Tokens { get; set; } 
        }
    }
}