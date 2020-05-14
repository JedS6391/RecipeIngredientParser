using System.Text;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    /// <summary>
    /// A token reader responsible for the {amount} token type.
    /// </summary>
    public class AmountTokenReader : ITokenReader
    {
        /// <inheritdoc/>
        public string TokenType { get; } = "{amount}";

        /// <inheritdoc/>
        /// <remarks>
        /// There are multiple amount tokens supported:
        /// <list type="bullet">
        ///     <item>
        ///        <description><see cref="LiteralAmountToken"/> (e.g. 1, 42, etc)</description>
        ///     </item>
        ///     <item>
        ///        <description><see cref="FractionalAmountToken"/> (e.g. 1/2, 3/4, etc)</description>
        ///     </item>
        ///     <item>
        ///        <description><see cref="RangeAmountToken"/> (e.g. 1-2, 3-4, etc)</description>
        ///     </item>
        /// </list>
        /// </remarks>
        public bool TryReadToken(ParserContext context, out IToken token)
        {
            var rawAmount = new StringBuilder();
            
            while (context.Buffer.HasNext() && NextCharacterIsValid(context))
            {
                var c = context.Buffer.Next();
                
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

            if (FractionalAmountToken.TryParse(rawAmount, out var fractionalAmountToken))
            {
                return fractionalAmountToken;
            }

            if (RangeAmountToken.TryParse(rawAmount, out var rangeAmountToken))
            {
                return rangeAmountToken;
            }
            
            return new LiteralAmountToken()
            {
                Amount = int.Parse(rawAmount)
            };
        }

        private bool NextCharacterIsValid(ParserContext context) =>
            context.Buffer.IsDigit() ||
            context.Buffer.Matches(c => c == '-') ||
            context.Buffer.Matches(c => c == '/');
    }
}