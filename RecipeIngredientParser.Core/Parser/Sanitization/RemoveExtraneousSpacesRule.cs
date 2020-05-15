using System.Text.RegularExpressions;
using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;

namespace RecipeIngredientParser.Core.Parser.Sanitization
{
    /// <summary>
    /// Removes extra spaces characters so that the string will only ever have a single consecutive space character.
    /// </summary>
    /// <remarks>e.g. 'x  y' -> 'x y'</remarks>
    public class RemoveExtraneousSpacesRule : IInputSanitizationRule
    {
        /// <summary>
        /// Matches 2 or more space characters.
        /// </summary>
        private static readonly Regex SpacesRegex = new Regex(@"[ ]{2,}", RegexOptions.Compiled);
        
        /// <inheritdoc/>
        public string Apply(string input)
        {
            return SpacesRegex.Replace(input, " ");
        }
    }
}