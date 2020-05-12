using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens
{
    public class LiteralToken : IToken
    {
        public string Value { get; set; }
        
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            // We do not accept visitors...
        }
    }
}