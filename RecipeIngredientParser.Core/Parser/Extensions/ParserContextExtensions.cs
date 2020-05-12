namespace RecipeIngredientParser.Core.Parser.Extensions
{
    /// <summary>
    /// Defines a set of extension methods for <see cref="ParserContext"/>.
    /// </summary>
    public static class ParserContextExtensions
    {
        /// <summary>
        /// Determines whether the next character is a digit.
        /// </summary>
        /// <param name="context">The parser context to check the next character of.</param>
        /// <returns>
        /// <see langword="true"/> when the next character is a digit; <see langword="false"/> otherwise.
        /// </returns>
        public static bool IsDigit(this ParserContext context) => context.Matches(char.IsDigit);
        
        /// <summary>
        /// Determines whether the next character is a letter.
        /// </summary>
        /// <param name="context">The parser context to check the next character of.</param>
        /// <returns>
        /// <see langword="true"/> when the next character is a letter; <see langword="false"/> otherwise.
        /// </returns>
        public static bool IsLetter(this ParserContext context) => context.Matches(char.IsLetter);
        
        /// <summary>
        /// Determines whether the next character is whitespace.
        /// </summary>
        /// <param name="context">The parser context to check the next character of.</param>
        /// <returns>
        /// <see langword="true"/> when the next character is whitespace; <see langword="false"/> otherwise.
        /// </returns>
        public static bool IsWhitespace(this ParserContext context) => context.Matches(char.IsWhiteSpace);
    }
}