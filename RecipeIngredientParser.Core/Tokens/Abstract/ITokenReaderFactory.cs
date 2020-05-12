namespace RecipeIngredientParser.Core.Tokens.Abstract
{
    /// <summary>
    /// Defines a factory for <see cref="ITokenReader"/> instances.
    /// </summary>
    public interface ITokenReaderFactory
    {
        /// <summary>
        /// Gets a <see cref="ITokenReader"/> instance responsible for the given token type.
        /// </summary>
        /// <param name="tokenType">The token type to get a <see cref="ITokenReader"/> instance for.</param>
        /// <returns>A <see cref="ITokenReader"/> instance.</returns>
        ITokenReader GetTokenReader(string tokenType);
    }
}