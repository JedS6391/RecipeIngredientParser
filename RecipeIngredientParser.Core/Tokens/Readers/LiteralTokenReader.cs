using System.Text;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    public class LiteralTokenReader : ITokenReader
    {
        public string TokenType { get; }

        public LiteralTokenReader(string value)
        {
            TokenType = value;
        }

        public bool TryReadToken(ParserContext context, out IToken token)
        {
            var value = new StringBuilder();
            
            foreach (var t in TokenType)
            {
                if (context.HasNext() && context.Peek() == t)
                {
                    context.Consume(t);
                }
                else
                {
                    token = null;
                    
                    return false;
                }
            }

            token = new LiteralToken()
            {
                Value = value.ToString()
            };

            return true;
        }
    }
}