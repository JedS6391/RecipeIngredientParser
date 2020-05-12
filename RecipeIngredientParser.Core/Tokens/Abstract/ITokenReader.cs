using RecipeIngredientParser.Core.Parser;

namespace RecipeIngredientParser.Core.Tokens.Abstract
{
    public interface ITokenReader
    {
        string TokenType { get; }
        
        bool TryReadToken(ParserContext context, out IToken token);
    }
}