using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;

namespace RecipeIngredientParser.Core.Parser.Sanitization
{
    /// <summary>
    /// Replaces any unicode fractions with a non-unicode version.
    /// </summary>
    /// <remarks>e.g. '½' -> '1/2'</remarks>
    public class ReplaceUnicodeFractionsRule : IInputSanitizationRule
    {
        /// <summary>
        /// A set of fractions that have unicode representations which we want to replace with a non-unicode representation.
        /// </summary>
        private static readonly Dictionary<string, string> UnicodeFractionReplacementTable = new Dictionary<string, string>()
        {
            { "¼", "1/4" },
            { "½", "1/2" },
            { "¾", "3/4" },
            { "⅐", "1/7" },
            { "⅑", "1/9" },
            { "⅒", "1/10" },
            { "⅓", "1/3" },
            { "⅔", "2/3" },
            { "⅕", "1/5" },
            { "⅖", "2/5" },
            { "⅗", "3/5" },
            { "⅘", "4/5" },
            { "⅙", "1/6" },
            { "⅚", "5/6" },
            { "⅛", "1/8" },
            { "⅜", "3/8" },
            { "⅝", "5/8" },
            { "⅞", "7/8" },
            { "↉", "0/3" }
        };
        
        /// <inheritdoc/>
        public string Apply(string input)
        {
            return UnicodeFractionReplacementTable.Aggregate(
                input, 
                (current, replacement) => 
                    current.Replace(replacement.Key, replacement.Value));
        }
    }
}