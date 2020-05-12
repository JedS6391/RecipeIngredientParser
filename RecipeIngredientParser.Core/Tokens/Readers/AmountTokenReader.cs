using System.Text;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    public class AmountTokenReader : ITokenReader
    {
        public string TokenType { get; } = "{amount}";

        public bool TryReadToken(ParserContext context, out IToken token)
        {
            var rawAmount = new StringBuilder();
            
            while (context.HasNext() && NextCharacterIsValid(context))
            {
                var c = context.Next();
                
                rawAmount.Append(c);
            }
            
            token = GenerateToken(rawAmount.ToString());

            return token != null;
        }

        private IAmountToken GenerateToken(string rawAmount)
        {
            if (string.IsNullOrEmpty(rawAmount))
            {
                return null;
            }

            var isFraction = rawAmount.Contains("/");
            var isRange = rawAmount.Contains("-");

            if (isFraction)
            {
                var parts = rawAmount.Split('/');

                return new FractionalAmountToken()
                {
                    Numerator = int.Parse(parts[0]),
                    Denominator = int.Parse(parts[1])
                };
            }
            
            if (isRange)
            {
                var parts = rawAmount.Split('-');

                return new RangeAmountToken()
                {
                    LowerBound = int.Parse(parts[0]),
                    UpperBound = int.Parse(parts[1])
                };
            }

            return new LiteralAmountToken()
            {
                Amount = int.Parse(rawAmount)
            };
        }

        private bool NextCharacterIsValid(ParserContext context) =>
            context.IsDigit() ||
            context.Matches(c => c == '-') ||
            context.Matches(c => c == '/');
    }
}