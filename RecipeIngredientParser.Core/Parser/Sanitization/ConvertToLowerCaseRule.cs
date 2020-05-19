using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;

namespace RecipeIngredientParser.Core.Parser.Sanitization
{
    /// <summary>
    /// Converts the text to lower case to ease parsing.
    /// </summary>
    /// <remarks>e.g. 'X Y' -> 'x y'</remarks>
    public class ConvertToLowerCaseRule : IInputSanitizationRule
    {
        /// <inheritdoc/>
        public string Apply(string input)
        {
            return input.ToLower();
        }
    }
}