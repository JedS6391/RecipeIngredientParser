using RecipeIngredientParser.Core.Parser;

namespace RecipeIngredientParser.Core.Tokens.Abstract
{
    public interface IToken
    {
        void Accept(ParserTokenVisitor parserTokenVisitor);
    }
}