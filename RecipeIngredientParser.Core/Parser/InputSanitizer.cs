using System.Text.RegularExpressions;

namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Provides useful input sanitization methods.
    /// </summary>
    public static class InputSanitizer
    {
        private static readonly Regex WhitespaceRegex = new Regex(@"[ ]{2,}", RegexOptions.Compiled);
        private static readonly Regex ToRangeRegex = new Regex(@"((?<=[0-9])+ to (?=[0-9]+))", RegexOptions.Compiled);
        private static readonly Regex BracketedTextRegex = new Regex(@"(\(.+\))", RegexOptions.Compiled);
        
        /// <summary>
        /// Sanitizes the input by removing extraneous characters and substituting common patterns with known tokens.
        /// </summary>
        /// <param name="input">An input to sanitize.</param>
        /// <returns>The sanitized input.</returns>
        public static string Sanitize(string input)
        {
            // Ensure that any extraneous whitespace is removed
            // 'test  string' -> 'test string'
            input = WhitespaceRegex.Replace(input, " ");

            // Try to substitute some common expressions
            // 'x to y' -> 'x-y'
            input = ToRangeRegex.Replace(input, "-");

            // Remove bracketed text
            // 'x (y)' -> x 
            input = BracketedTextRegex.Replace(input, string.Empty);

            // Always convert to lower case.
            return input.ToLower();
        }
    }
}