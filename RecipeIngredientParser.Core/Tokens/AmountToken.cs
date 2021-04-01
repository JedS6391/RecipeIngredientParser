using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens
{
    #region Base

    /// <summary>
    /// Defines the different amount token types supported.
    /// </summary>
    public enum AmountTokenType
    {
        /// <summary>
        /// A literal amount (e.g. 1, 42, etc).
        /// </summary>
        Literal,
        
        /// <summary>
        /// A fractional amount (e.g. 1/2, 3/4, etc).
        /// </summary>
        Fraction,
        
        /// <summary>
        /// A range amount (e.g. 1-2, 3-4, etc).
        /// </summary>
        Range
    }
    
    /// <summary>
    /// Defines the base structure of an amount token.
    /// </summary>
    public abstract class AmountToken : IToken
    {
        /// <summary>
        /// Gets the type of amount the token represents.
        /// </summary>
        public abstract AmountTokenType Type { get; }

        /// <inheritdoc/>
        public abstract void Accept(ParserTokenVisitor parserTokenVisitor);

        /// <summary>
        /// Creates a <see cref="LiteralAmountToken"/> instance.
        /// </summary>
        /// <param name="amount">The amount this literal token represents.</param>
        /// <returns>A <see cref="LiteralAmountToken"/> instance.</returns>
        public static LiteralAmountToken Literal(decimal amount) => new (amount);

        /// <summary>
        /// Creates a <see cref="FractionalAmountToken"/> instance.
        /// </summary>
        /// <param name="wholeNumber">
        /// A <see cref="LiteralAmountToken"/> representing the whole number component of the fraction.
        /// </param>
        /// <param name="numerator">
        /// A <see cref="LiteralAmountToken"/> representing the numerator component of the fraction.
        /// </param>
        /// <param name="denominator">
        /// A <see cref="LiteralAmountToken"/> representing the denominator component of the fraction.
        /// </param>
        /// <returns>A <see cref="FractionalAmountToken"/> instance.</returns>
        public static FractionalAmountToken Fraction(
            LiteralAmountToken wholeNumber,
            LiteralAmountToken numerator,
            LiteralAmountToken denominator) => new(wholeNumber, numerator, denominator);

        /// <summary>
        /// Creates a <see cref="RangeAmountToken"/> instance for a fractional range (e.g. 1/4-1/3)
        /// </summary>
        /// <param name="lowerBound">
        /// A <see cref="FractionalAmountToken"/> representing the lower bound of the range.
        /// </param>
        /// <param name="upperBound">
        /// A <see cref="FractionalAmountToken"/> representing the upper bound of the range.
        /// </param>
        /// <returns>A <see cref="RangeAmountToken"/> instance.</returns>
        public static RangeAmountToken Range(
            FractionalAmountToken lowerBound,
            FractionalAmountToken upperBound) => new(lowerBound, upperBound);

        /// <summary>
        /// Creates a <see cref="RangeAmountToken"/> instance for a literal range (e.g. 1-2)
        /// </summary>
        /// <param name="lowerBound">
        /// A <see cref="LiteralAmountToken"/> representing the lower bound of the range.
        /// </param>
        /// <param name="upperBound">
        /// A <see cref="LiteralAmountToken"/> representing the upper bound of the range.
        /// </param>
        /// <returns>A <see cref="RangeAmountToken"/> instance.</returns>
        public static RangeAmountToken Range(
            LiteralAmountToken lowerBound,
            LiteralAmountToken upperBound) => new(lowerBound, upperBound);
    }

    #endregion

    #region Tokens

    /// <summary>
    /// Represents a literal amount token.
    /// </summary>
    public sealed class LiteralAmountToken : AmountToken
    {
        /// <summary>
        /// Gets or sets the amount (e.g. 1, 42, 3.6, etc).
        /// </summary>
        public decimal Amount { get; private set; }

        /// <inheritdoc/>
        public override AmountTokenType Type => AmountTokenType.Literal;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralAmountToken"/> class.
        /// </summary>
        /// <param name="amount">The amount this literal represents.</param>
        internal LiteralAmountToken(decimal amount)
        {
            Amount = amount;
        }

        /// <inheritdoc/>
        public override void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(LiteralAmountToken)}({Amount})";
    }

    /// <summary>
    /// Represents a fractional amount token.
    /// </summary>
    public sealed class FractionalAmountToken : AmountToken
    {
        /// <summary>
        /// Gets or sets the whole number component of the fraction, if it is a mixed number.
        /// </summary>
        /// <remarks>Will be <see langword="null"/> when the value is not a mixed number.</remarks>
        public LiteralAmountToken WholeNumber { get; private set; }

        /// <summary>
        /// Gets or sets the numerator component of the fraction.
        /// </summary>
        public LiteralAmountToken Numerator { get; private set; }
        
        /// <summary>
        /// Gets or sets the denominator component of the fraction.
        /// </summary>
        public LiteralAmountToken  Denominator { get; private set; }
        
        /// <inheritdoc/>
        public override AmountTokenType Type => AmountTokenType.Fraction;

        /// <summary>
        /// Initializes a new instance of the <see cref="FractionalAmountToken"/> class.
        /// </summary>
        /// <param name="wholeNumber">The whole number component of the fraction, if any.</param>
        /// <param name="numerator">The numerator component of the fraction.</param>
        /// <param name="denominator">The denominator component of the fraction.</param>
        internal FractionalAmountToken(
            LiteralAmountToken wholeNumber,
            LiteralAmountToken numerator,
            LiteralAmountToken denominator)
        {
            WholeNumber = wholeNumber;
            Numerator = numerator;
            Denominator = denominator;
        }

        /// <inheritdoc/>
        public override void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }

        /// <inheritdoc/>
        public override string ToString() => WholeNumber != null ?
            $"{nameof(FractionalAmountToken)}({WholeNumber} {Numerator}/{Denominator})" :
            $"{nameof(FractionalAmountToken)}({Numerator}/{Denominator})"; 
    }

    /// <summary>
    /// Represents a range amount token.
    /// </summary>
    /// <remarks>
    /// A range amount token can either represent a fractional range (e.g. 1/4-1/3) or a literal range (e.g. 1-2).
    /// </remarks>
    public sealed class RangeAmountToken : AmountToken
    {
        /// <summary>
        /// Gets or sets the lower bound of the range.
        /// </summary>
        public AmountToken LowerBound { get; private set; }
        
        /// <summary>
        /// Gets or sets the lower bound of the range.
        /// </summary>
        public AmountToken UpperBound { get; private set; }
        
        /// <inheritdoc/>
        public override AmountTokenType Type => AmountTokenType.Range;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAmountToken"/> class.
        /// </summary>
        /// <param name="lowerBound">
        /// A <see cref="LiteralAmountToken"/> instance representing the lower bound of the range.
        /// </param>
        /// <param name="upperBound">
        /// A <see cref="LiteralAmountToken"/> instance representing the upper bound of the range.
        /// </param>
        internal RangeAmountToken(LiteralAmountToken lowerBound, LiteralAmountToken upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAmountToken"/> class.
        /// </summary>
        /// <param name="lowerBound">
        /// A <see cref="FractionalAmountToken"/> instance representing the lower bound of the range.
        /// </param>
        /// <param name="upperBound">
        /// A <see cref="FractionalAmountToken"/> instance representing the upper bound of the range.
        /// </param>
        internal RangeAmountToken(FractionalAmountToken lowerBound, FractionalAmountToken upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        /// <inheritdoc/>
        public override void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(RangeAmountToken)}({LowerBound}-{UpperBound})";
    }

    #endregion
    
    #region Token visitor

    /// <summary>
    /// The implementation of <see cref="ParserTokenVisitor"/> for the amount tokens.
    /// </summary>
    public partial class ParserTokenVisitor
    {
        /// <summary>
        /// Visits a <see cref="LiteralAmountToken"/>.
        /// </summary>
        /// <param name="token">A <see cref="LiteralAmountToken"/> instance.</param>
        public void Visit(LiteralAmountToken token)
        {
            _parseResult.Details.Amount = token.Amount.ToString();
        }
        
        /// <summary>
        /// Visits a <see cref="FractionalAmountToken"/>.
        /// </summary>
        /// <param name="token">A <see cref="FractionalAmountToken"/> instance.</param>
        public void Visit(FractionalAmountToken token)
        {
            _parseResult.Details.Amount = token.WholeNumber != null ?
                $"{token.WholeNumber.Amount} {token.Numerator.Amount}/{token.Denominator.Amount}" :
                $"{token.Numerator.Amount}/{token.Denominator.Amount}"; ;
        }
        
        /// <summary>
        /// Visits a <see cref="RangeAmountToken"/>.
        /// </summary>
        /// <param name="token">A <see cref="RangeAmountToken"/> instance.</param>
        public void Visit(RangeAmountToken token)
        {
            // A range token has two nested tokens, so we visit the first and store
            // the value before visiting the second. This allows us to combine the two values.
            token.LowerBound.Accept(this);

            var lowerBoundAmount = _parseResult.Details.Amount;

            token.UpperBound.Accept(this);

            _parseResult.Details.Amount = $"{lowerBoundAmount}-{_parseResult.Details.Amount}";
        }
    }

    #endregion
}