using RecipeIngredientParser.Core.Parser.Context;

namespace RecipeIngredientParser.Core.Parser.Extensions
{
    /// <summary>
    /// Defines a set of extension methods for <see cref="ParserContext"/>.
    /// </summary>
    public static class InputBufferExtensions
    {
        /// <summary>
        /// Determines whether the next character is a digit.
        /// </summary>
        /// <param name="buffer">The input buffer to check the next character of.</param>
        /// <returns>
        /// <see langword="true"/> when the next character is a digit; <see langword="false"/> otherwise.
        /// </returns>
        public static bool IsDigit(this InputBuffer buffer) => buffer.Matches(char.IsDigit);
        
        /// <summary>
        /// Determines whether the next character is a letter.
        /// </summary>
        /// <param name="buffer">The input buffer to check the next character of.</param>
        /// <returns>
        /// <see langword="true"/> when the next character is a letter; <see langword="false"/> otherwise.
        /// </returns>
        public static bool IsLetter(this InputBuffer buffer) => buffer.Matches(char.IsLetter);
        
        /// <summary>
        /// Determines whether the next character is whitespace.
        /// </summary>
        /// <param name="buffer">The input buffer to check the next character of.</param>
        /// <returns>
        /// <see langword="true"/> when the next character is whitespace; <see langword="false"/> otherwise.
        /// </returns>
        public static bool IsWhitespace(this InputBuffer buffer) => buffer.Matches(char.IsWhiteSpace);

        /// <summary>
        /// Optionally consumes the specified character.
        /// </summary>
        /// <param name="buffer">The input buffer to consume the character from.</param>
        /// <param name="characterToConsume">The character to consume.</param>
        /// <returns><see langword="true"/> regardless of whether the character was consumed or not.</returns>
        public static bool OptionallyConsume(this InputBuffer buffer, char characterToConsume) =>
            buffer.TryConsume(characterToConsume) || true;       
    }
}