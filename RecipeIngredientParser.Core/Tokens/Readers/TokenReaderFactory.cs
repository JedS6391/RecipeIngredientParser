using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    public class TokenReaderFactory : ITokenReaderFactory
    {
        private readonly IEnumerable<ITokenReader> _tokenReaders;

        public TokenReaderFactory(IEnumerable<ITokenReader> tokenReaders)
        {
            _tokenReaders = tokenReaders;
        }
        
        public ITokenReader GetTokenReader(string tokenType)
        {
            return _tokenReaders.FirstOrDefault(tr => tr.TokenType == tokenType);
        }
    }
}