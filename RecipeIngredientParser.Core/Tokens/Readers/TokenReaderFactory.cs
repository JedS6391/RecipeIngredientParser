using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    /// <summary>
    /// A default <see cref="ITokenReader"/> implementation that will attempt to find the first
    /// matching <see cref="ITokenReader"/> instance that can handle a specified token type.
    /// </summary>
    public class TokenReaderFactory : ITokenReaderFactory
    {
        private readonly IEnumerable<ITokenReader> _tokenReaders;

        /// <summary>
        /// Initialises a new instance of the <see cref="TokenReaderFactory"/> class.
        /// </summary>
        /// <param name="tokenReaders">A collection of token readers that are available.</param>
        public TokenReaderFactory(IEnumerable<ITokenReader> tokenReaders)
        {
            _tokenReaders = tokenReaders;
        }
        
        /// <inheritdoc/>
        public ITokenReader GetTokenReader(string tokenType)
        {
            return _tokenReaders.FirstOrDefault(tr => tr.TokenType == tokenType);
        }
    }
}