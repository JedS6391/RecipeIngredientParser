using System.Text.RegularExpressions;
using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;

namespace RecipeIngredientParser.Core.Parser.Sanitization
{
    /// <summary>
    /// Removes what appears to be an alternate ingredient, with the assumption that it is non-essential.
    /// </summary>
    /// <remarks>e.g. 'x or y' -> 'x'</remarks>
    public class RemoveAlternateIngredientsRule : IInputSanitizationRule
    {
        /// <summary>
        /// Matches a string like ' or ...'.
        /// </summary>
        private static readonly Regex AlternateIngredientRegex = new Regex(@"( or .*)", RegexOptions.Compiled);
        
        /// <inheritdoc/>
        public string Apply(string input)
        {
            return AlternateIngredientRegex.Replace(input, string.Empty);
        }
    }
}