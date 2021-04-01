using System;
using System.Linq;
using System.Text;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Tokens.Abstract;
using RecipeIngredientParser.Core.Tokens.Parsing;

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
            // First, read the raw amount from the context.
            // e.g. context = "1/4-1/3 cup milk" -> raw amount = "1/4-1/3"
            var rawAmount = ReadRawAmount(context);

            // Now we've got a raw amount, utilise a recursive descent parser with 
            // backtracking to extract the tokenized amount.
            var amountTokenParser = new AmountTokenParser();
           
            var parsingSucceeded = amountTokenParser.TryParse(rawAmount, out var amountToken);

            token = parsingSucceeded ?
                amountToken :
                null;

            return parsingSucceeded;
        }

        private string ReadRawAmount(ParserContext context)
        {
            var rawAmount = new StringBuilder();
            
            while (HasMoreAmountCharacters(context))
            {
                var c = context.Buffer.Next();

                rawAmount.Append(c);
            }

            return rawAmount.ToString();
        }

        private bool HasMoreAmountCharacters(ParserContext context)
        {
            if (!context.Buffer.HasNext())
            {
                return false;
            }

            if (NextCharacterIsValid(context))
            {
                return true;
            }

            // When whitespace is next there are two cases:
            //
            //   1. Whitespace is being used to separate in the amount e.g. 1 1/2 grams, 1/4 - 1/3 cup
            //   2. Whitespace is being used to separate the next token e.g. 1/2 cup milk, 2 grams flour
            //
            // We handle these by looking ahead past the space to see if there are further characters valid
            // for an amount token - if not this is a single that the amount token is complete.
            if (context.Buffer.IsWhitespace())
            {
                // Offset by 1 to skip the whitepsace at the current position.
                var nextCharacters = context.Buffer.Peek(offset: 1, count: 1);

                if (!nextCharacters.Any())
                {
                    // When there's nothing after the whitespace then we definitely are done reading.
                    return false;
                }

                var characterAfterWhitespace = nextCharacters.First();

                // There's something after the whitespace, check if it is a valid amount token character.
                return ValidNextCharacterPredicates.Any(p => p(characterAfterWhitespace));

            }

            return false;
        }     

        private bool NextCharacterIsValid(ParserContext context) =>
            ValidNextCharacterPredicates.Any(p => context.Buffer.Matches(p));

        private static readonly Func<char, bool>[] ValidNextCharacterPredicates = new Func<char, bool>[]
        {
            char.IsDigit,
            c => c == '-',
            c => c == '/',
            c => c == '.'
        };
    }
}