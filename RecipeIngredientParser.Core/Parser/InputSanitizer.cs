using System.Collections.Generic;
using System.Linq;
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
        /// Matches a string with any characters between brackets (e.g. '(text)').
        /// </summary>
        private static readonly Regex BracketedTextRegex = new Regex(@"(\(.*?\))", RegexOptions.Compiled);

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
            
            // Remove extraneous spaces a second time in case substitutions introduced any spaces.
            input = SpacesRegex.Replace(input, " ");

            // Replace any unicode fractions with non-unicode versions
            // '½' -> '1/2'
            input = ReplaceUnicodeFractions(input);

            // Always convert to lower case.
            return input.ToLower();
        }

        private static string ReplaceUnicodeFractions(string input)
        {
            return UnicodeFractionReplacementTable.Aggregate(
                input, 
                (current, replacement) => 
                    current.Replace(replacement.Key, replacement.Value));
        }
    }
}