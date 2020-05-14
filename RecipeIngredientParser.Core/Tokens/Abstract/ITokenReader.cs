using RecipeIngredientParser.Core.Parser.Context;

namespace RecipeIngredientParser.Core.Tokens.Abstract
{
    /// <summary>
    /// Defines a reader responsible for reading a token of a specified type from the <see cref="ParserContext"/>.
    /// </summary>
    public interface ITokenReader
    {
        /// <summary>
        /// Gets the type of token the reader is responsible for.
        /// </summary>
        string TokenType { get; }
        
        /// <summary>
        /// Attempts to read a token from the context.
        /// </summary>
        /// <param name="context">The context to read the token from.</param>
        /// <param name="token">
        /// When this method returns, contains the token that was extracted from the context if
        /// the read succeeded, or <see langword="null"/> if the read failed.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the token read succeeded; <see langword="false"/> otherwise.
        /// </returns>
        bool TryReadToken(ParserContext context, out IToken token);
    }
}