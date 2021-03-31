using System;
using System.Linq;
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

        /// <summary>
        /// Implements a recursive descent parser with backtracking for the grammar of an <see cref="AmountToken"/>.
        /// </summary>
        /// <remarks>
        /// This parser can handle the grammar:
        ///
        ///   amount:   range | fraction | literal;
        ///   range:    fraction-fraction | literal-literal;
        ///   fraction: literal/literal | literal literal/literal;
        ///   literal:  number;
        ///   number:   int | decimal;
        /// </remarks>
        private class AmountTokenParser
        {
            /// <summary>
            /// Attempts to parse <paramref name="rawAmount"/> as an <see cref="AmountToken"/>.
            /// </summary>
            /// <param name="rawAmount">The raw amount to parse.</param>
            /// <param name="token">
            /// When this method returns, contains the token that was extracted from the context if
            /// the read succeeded, or <see langword="null"/> if the read failed.
            /// </param>
            /// <returns>
            /// <see langword="true"/> when the token read succeeded; <see langword="false"/> otherwise.
            /// </returns>
            public bool TryParse(string rawAmount, out AmountToken token)
            {
                if (string.IsNullOrEmpty(rawAmount))
                {
                    token = null;

                    return false;
                }

                var buffer = new InputBuffer(rawAmount);

                // All input must have been consumed for a successful parse.
                return Amount(buffer, out token) && !buffer.HasNext();
            }

            private bool Amount(InputBuffer buffer, out AmountToken amount)
            {
                // TODO: These 3 checkpointed statements are very similar
                // Would be good to find a way to abstract them to shared logic.
                using (var checkpoint = buffer.Checkpoint())
                {
                    // First, try to parse a range amount
                    if (Range(buffer, out var range))
                    {
                        checkpoint.Commit();

                        amount = range;

                        return true;
                    }
                }

                using (var checkpoint = buffer.Checkpoint())
                {
                    // Next, try to parse a fractional amount
                    if (Fraction(buffer, out var fraction))
                    {
                        checkpoint.Commit();

                        amount = fraction;

                        return true;
                    }
                }

                using (var checkpoint = buffer.Checkpoint())
                {
                    // Last, try to parse a literal
                    if (Literal(buffer, out var literal))
                    {
                        checkpoint.Commit();

                        amount = literal;

                        return true;
                    }
                }

                // Failed to parse any kind of amount
                amount = null;

                return false;
            }

            private bool Range(InputBuffer buffer, out RangeAmountToken range)
            {
                // First, try to parse a fractional range
                using (var checkpoint = buffer.Checkpoint())
                {
                    if (Fraction(buffer, out var lowerBound) &&
                        buffer.TryConsume('-') &&
                        Fraction(buffer, out var upperBound))
                    {
                        range = AmountToken.Range(lowerBound, upperBound);

                        checkpoint.Commit();

                        return true;
                    }
                }

                // Otherwise, try to parse a literal range
                using (var checkpoint = buffer.Checkpoint())
                {
                    if (Literal(buffer, out var lowerBound) &&
                        buffer.TryConsume('-') &&
                        Literal(buffer, out var upperBound))
                    {
                        range = AmountToken.Range(lowerBound, upperBound);

                        checkpoint.Commit();

                        return true;
                    }
                }

                range = null;

                return false;
            }

            private bool Fraction(InputBuffer buffer, out FractionalAmountToken fraction)
            {
                if (!Literal(buffer, out var firstLiteral))
                {
                    fraction = null;

                    return false;
                }

                // TODO: Tidy this up

                // The first literal may either be the whole number or numerator 
                // component depending on what comes after it in the buffer.
                if (buffer.TryConsume(' ') || buffer.TryConsume('-'))
                {
                    // Dealing with a fraction that includes a whole number component                    
                    if (Literal(buffer, out var numerator) &&
                        buffer.TryConsume('/') &&
                        Literal(buffer, out var denominator))
                    {
                        fraction = AmountToken.Fraction(
                            wholeNumber: firstLiteral,
                            numerator: numerator,
                            denominator: denominator);

                        return true;
                    }
                }

                if (buffer.TryConsume('/'))
                {
                    // Dealing with a fraction without any whole number component                                        
                    if (Literal(buffer, out var denominator))
                    {
                        fraction = AmountToken.Fraction(
                            wholeNumber: null,
                            numerator: firstLiteral,
                            denominator: denominator);

                        return true;
                    }
                }

                fraction = null;

                return false;
            }

            private bool Literal(InputBuffer buffer, out LiteralAmountToken literal)
            {
                var seenDecimalPoint = false;
                var digits = new StringBuilder();

                while (buffer.HasNext() && (buffer.IsDigit() || buffer.Peek() == '.'))
                {
                    var c = buffer.Next();

                    if (c == '.')
                    {
                        // We only accept a single decimal point.
                        if (seenDecimalPoint)
                        {
                            literal = null;

                            return false;
                        }

                        seenDecimalPoint = true;
                    }

                    digits.Append(c);
                }

                if (!decimal.TryParse(digits.ToString(), out var amount))
                {
                    literal = null;

                    return false;                    
                }

                literal = AmountToken.Literal(amount);

                return true;
            }
        }
    }
}