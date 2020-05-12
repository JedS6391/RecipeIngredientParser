using System.Collections.Generic;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core
{
    public class ParsedIngredient
    {
        public string RawIngredient { get; set; }
        public string Amount { get; set; }
        public string Unit { get; set; }
        public string Form { get; set; }
        public string Ingredient { get; set; }
        public ParseMetadata Metadata { get; set; }

        public class ParseMetadata
        {
            public Template Template { get; set; }
            public TemplateMatchResult MatchResult { get; set; }
            public IEnumerable<IToken> Tokens { get; set; } 
        }
        
        
    }
}