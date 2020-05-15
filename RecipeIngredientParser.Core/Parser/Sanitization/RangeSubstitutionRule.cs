using System.Text.RegularExpressions;
using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;

namespace RecipeIngredientParser.Core.Parser.Sanitization
{
    /// <summary>
    /// Substitutes a pattern like 'x to y' with 'x-y'. 
    /// </summary>
    public class RangeSubstitutionRule : IInputSanitizationRule
    {
        /// <summary>
        /// Matches a string like 'x to y' where x/y are digits. The lookbehind/lookahead are used to ignore
        /// the x/y components so we just get the 'to'.
        /// </summary>
        private static readonly Regex ToRangeRegex = new Regex(@"((?<=[0-9])+ to (?=[0-9]+))", RegexOptions.Compiled);
        
        /// <inheritdoc/>
        public string Apply(string input)
        {
            return ToRangeRegex.Replace(input, "-");
        }
    }
}