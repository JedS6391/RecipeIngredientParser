using System.Text.RegularExpressions;
using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;

namespace RecipeIngredientParser.Core.Parser.Sanitization
{
    /// <summary>
    /// Removes any bracketed text, with the assumption that it is non-essential.
    /// </summary>
    /// <remarks>e.g. 'x (y)' -> 'x'</remarks>
    public class RemoveBracketedTextRule : IInputSanitizationRule
    {
        /// <summary>
        /// Matches a string with any characters between brackets (e.g. '(text)').
        /// </summary>
        private static readonly Regex BracketedTextRegex = new Regex(@"(\(.*?\))", RegexOptions.Compiled);
        
        /// <inheritdoc/>
        public string Apply(string input)
        {
            return BracketedTextRegex.Replace(input, string.Empty);
        }
    }
}