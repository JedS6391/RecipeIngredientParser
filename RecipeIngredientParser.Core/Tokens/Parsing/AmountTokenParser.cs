using System;
using System.Collections.Generic;
using System.Text;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Parsing
{
    /// <summary>
    /// Implements a recursive descent parser with backtracking for the grammar of an <see cref="AmountToken"/>.
    /// </summary>
    /// <remarks>
    /// This parser can handle the grammar:
    ///
    ///   amount:   range | fraction | literal;
    ///   range:    fraction-fraction | literal-literal;
    ///   fraction: literal-literal/literal literal literal/literal | literal/literal;
    ///   literal:  decimal;
    ///
    /// Some examples of supported amounts:
    ///     - 1-2
    ///     - 1/2
    ///     - 1.5
    ///     - 1/4-1/3
    ///     - 2 2/3  
    /// </remarks>
    internal class AmountTokenParser
    {
        /// <summary>
        /// Attempts to parse <paramref name="rawAmount"/> as an <see cref="AmountToken"/>.
        /// </summary>
        /// <param name="rawAmount">The raw amount to parse.</param>
        /// <param name="token">
        /// When this method returns, contains the token that was parsed from the buffer if
        /// the parsing succeeded, or <see langword="null"/> if the parsing failed.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the parsing succeeded; <see langword="false"/> otherwise.
        /// </returns>
        public bool TryParse(string rawAmount, out AmountToken token)
        {
            if (string.IsNullOrEmpty(rawAmount))
            {
                token = null;

                return false;
            }

            var buffer = new InputBuffer(rawAmount);

            return Amount(buffer, out token);
        }

        private bool Amount(InputBuffer buffer, out AmountToken amount) =>
            ExecuteRules(GetAmountRules, buffer, out amount);

        private bool Range(InputBuffer buffer, out RangeAmountToken range) =>
            ExecuteRules(GetRangeRules, buffer, out range);

        private bool Fraction(InputBuffer buffer, out FractionalAmountToken fraction) =>
            ExecuteRules(GetFractionRules, buffer, out fraction);

        private bool Literal(InputBuffer buffer, out LiteralAmountToken literal)
        {
            var seenDecimalPoint = false;
            var digits = new StringBuilder();

            // TODO: Would be good for this to use rules like the other parts
            // of the grammar, but for now let's keep it simple.
            while (buffer.HasNext() && (buffer.IsDigit() || buffer.Peek() == '.'))
            {
                var c = buffer.Next();

                if (c == '.' && seenDecimalPoint)
                {
                    // We only accept a single decimal point.
                    literal = null;

                    return false;
                }

                seenDecimalPoint = c == '.';

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

        private bool ExecuteRules<T>(Func<IEnumerable<ParserRule>> rulesProvider, InputBuffer buffer, out T token)
            where T : class, IToken
        {
            var rules = rulesProvider.Invoke();

            // We attempt to parse using each provided rule, backtracking to the 
            // checkpoint if the rule fails to match against the input.
            foreach (var rule in rules)
            {
                using (var checkpoint = buffer.Checkpoint())
                {
                    var result = rule.Execute(buffer);

                    if (result.Succeeded)
                    {
                        token = result.Token as T;

                        checkpoint.Commit();

                        return true;
                    }
                }
            }
           
            token = null;

            return false;
        }

        #region Rules

        // amount = range | fraction | literal 
        private ParserRule[] GetAmountRules() => new ParserRule[]
        {
                // Range e.g. 1-4
                ParserRuleBuilder
                    .New()
                    .Token<RangeAmountToken>(Range)
                    // Ensure all input is consumed
                    .Condition(buffer => !buffer.HasNext())
                    .Map(tokens =>
                    {
                        var range = tokens[0] as RangeAmountToken;

                        return ParserRuleResult.Success(range);
                    })
                    .Build(),

                // Fraction e.g. 1/2
                ParserRuleBuilder
                    .New()
                    .Token<FractionalAmountToken>(Fraction)
                    // Ensure all input is consumed
                    .Condition(buffer => !buffer.HasNext())
                    .Map(tokens =>
                    {
                        var fraction = tokens[0] as FractionalAmountToken;

                        return ParserRuleResult.Success(fraction);
                    })
                    .Build(),

                // Literal e.g. 2
                ParserRuleBuilder
                    .New()
                    .Token<LiteralAmountToken>(Literal)
                    // Ensure all input is consumed
                    .Condition(buffer => !buffer.HasNext())
                    .Map(tokens =>
                    {
                        var literal = tokens[0] as LiteralAmountToken;

                        return ParserRuleResult.Success(literal);
                    })
                    .Build()
        };

        // range = fraction-fraction | literal-literal
        private ParserRule[] GetRangeRules() => new ParserRule[]
        {
                // Fractional range e.g. 1/4-1/3
                ParserRuleBuilder
                    .New()
                    .Token<FractionalAmountToken>(Fraction)
                    .Condition(buffer => buffer.OptionallyConsume(' '))
                    .Condition(buffer => buffer.TryConsume('-'))
                    .Condition(buffer => buffer.OptionallyConsume(' '))
                    .Token<FractionalAmountToken>(Fraction)
                    .Map(tokens =>
                    {
                        var lowerBound = tokens[0] as FractionalAmountToken;
                        var upperBound = tokens[1] as FractionalAmountToken;

                        return ParserRuleResult.Success(AmountToken.Range(lowerBound, upperBound));
                    })
                    .Build(),

                // Literal range e.g. 1-2
                ParserRuleBuilder
                    .New()
                    .Token<LiteralAmountToken>(Literal)
                    .Condition(buffer => buffer.OptionallyConsume(' '))
                    .Condition(buffer => buffer.TryConsume('-'))
                    .Condition(buffer => buffer.OptionallyConsume(' '))
                    .Token<LiteralAmountToken>(Literal)
                    .Map(tokens =>
                    {
                        var lowerBound = tokens[0] as LiteralAmountToken;
                        var upperBound = tokens[1] as LiteralAmountToken;

                        return ParserRuleResult.Success(AmountToken.Range(lowerBound, upperBound));
                    })
                    .Build()
        };

        // fraction = literal-literal/literal | literal literal/literal | literal/literal
        private ParserRule[] GetFractionRules() => new ParserRule[]
        {
                // Mixed number fraction rule e.g. 1 1/2
                ParserRuleBuilder
                    .New()
                    .Token<LiteralAmountToken>(Literal)
                    .Condition(buffer => buffer.TryConsume(' ') || buffer.TryConsume('-'))
                    .Token<LiteralAmountToken>(Literal)
                    .Condition(buffer => buffer.TryConsume('/'))
                    .Token<LiteralAmountToken>(Literal)
                    .Map(tokens =>
                    {
                        var wholeNumber = tokens[0] as LiteralAmountToken;
                        var numerator = tokens[1] as LiteralAmountToken;
                        var denominator = tokens[2] as LiteralAmountToken;

                        return ParserRuleResult.Success(AmountToken.Fraction(wholeNumber, numerator, denominator));
                    })
                    .Build(),

                // Standard fraction rule e.g. 1/2
                ParserRuleBuilder
                    .New()
                    .Token<LiteralAmountToken>(Literal)
                    .Condition(buffer => buffer.TryConsume('/'))
                    .Token<LiteralAmountToken>(Literal)
                    .Map(tokens =>
                    {
                        var numerator = tokens[0] as LiteralAmountToken;
                        var denominator = tokens[1] as LiteralAmountToken;

                        return ParserRuleResult.Success(AmountToken.Fraction(wholeNumber: null, numerator, denominator));
                    })
                    .Build()
        };

        #endregion
    }
}
