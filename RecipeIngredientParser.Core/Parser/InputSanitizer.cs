using System.Text.RegularExpressions;

namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Provides useful input sanitization methods.
    /// </summary>
    public static class InputSanitizer
    {
        /// <summary>
        /// Matches 2 or more space characters.
        /// </summary>
        private static readonly Regex SpacesRegex = new Regex(@"[ ]{2,}", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches a string like 'x to y' where x/y are digits. The lookbehind/lookahead are used to ignore
        /// the x/y components so we just get the 'to'.
        /// </summary>
        private static readonly Regex ToRangeRegex = new Regex(@"((?<=[0-9])+ to (?=[0-9]+))", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches any characters between brackets (e.g. '(text)').
        /// </summary>
        private static readonly Regex BracketedTextRegex = new Regex(@"(\(.+\))", RegexOptions.Compiled);
        
        /// <summary>
        /// Sanitizes the input by removing extraneous characters and substituting common patterns with known tokens.
        /// </summary>
        /// <param name="input">An input to sanitize.</param>
        /// <returns>The sanitized input.</returns>
        public static string Sanitize(string input)
        {
            // Ensure that any extraneous spaces are removed
            // 'test  string' -> 'test string'
            input = SpacesRegex.Replace(input, " ");

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