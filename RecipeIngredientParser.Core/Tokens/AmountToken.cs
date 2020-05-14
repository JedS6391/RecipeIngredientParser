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
    public interface IAmountToken : IToken
    {
        /// <summary>
        /// Gets the type of amount the token represents.
        /// </summary>
        AmountTokenType Type { get; }
    }

    #endregion

    #region Tokens

    /// <summary>
    /// Represents a literal amount token.
    /// </summary>
    public class LiteralAmountToken : IAmountToken
    {
        /// <summary>
        /// Gets or sets the amount (e.g. 1, 42, etc).
        /// </summary>
        public int Amount { get; set; }

        /// <inheritdoc/>
        public AmountTokenType Type => AmountTokenType.Literal;
        
        /// <inheritdoc/>
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
    }

    /// <summary>
    /// Represents a fractional amount token.
    /// </summary>
    public class FractionalAmountToken : IAmountToken
    {
        /// <summary>
        /// Gets or sets the numerator of the fraction.
        /// </summary>
        public int Numerator { get; set; }
        
        /// <summary>
        /// Gets or sets the numerator of the fraction.
        /// </summary>
        public int Denominator { get; set; }
        
        /// <inheritdoc/>
        public AmountTokenType Type => AmountTokenType.Fraction;

        /// <summary>
        /// Attempts to create a <see cref="FractionalAmountToken"/> instance from a raw amount.
        /// </summary>
        /// <param name="rawAmount">The raw amount to create a <see cref="FractionalAmountToken"/> from.</param>
        /// <param name="fractionalAmountToken">
        /// When this method returns, contains a <see cref="FractionalAmountToken"/> that represents
        /// the provided raw amount when the parsing succeeded, or <see langword="null"/> if the parsing failed.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the token read succeeded; <see langword="false"/> otherwise.
        /// </returns>
        public static bool TryParse(string rawAmount, out FractionalAmountToken fractionalAmountToken)
        {
            if (rawAmount.Contains("/"))
            {
                var parts = rawAmount.Split('/');

                fractionalAmountToken = new FractionalAmountToken()
                {
                    Numerator = int.Parse(parts[0]),
                    Denominator = int.Parse(parts[1])
                };

                return true;
            }

            fractionalAmountToken = null;
            
            return false;
        }
        
        /// <inheritdoc/>
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
    }
    
    /// <summary>
    /// Represents a range amount token.
    /// </summary>
    public class RangeAmountToken : IAmountToken
    {
        /// <summary>
        /// Gets or sets the lower bound of the range.
        /// </summary>
        public int LowerBound { get; set; }
        
        /// <summary>
        /// Gets or sets the lower bound of the range.
        /// </summary>
        public int UpperBound { get; set; }
        
        /// <inheritdoc/>
        public AmountTokenType Type => AmountTokenType.Range;
        
        /// <summary>
        /// Attempts to create a <see cref="RangeAmountToken"/> instance from a raw amount.
        /// </summary>
        /// <param name="rawAmount">The raw amount to create a <see cref="RangeAmountToken"/> from.</param>
        /// <param name="rangeAmountToken">
        /// When this method returns, contains a <see cref="RangeAmountToken"/> that represents
        /// the provided raw amount when the parsing succeeded, or <see langword="null"/> if the parsing failed.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the token read succeeded; <see langword="false"/> otherwise.
        /// </returns>
        public static bool TryParse(string rawAmount, out RangeAmountToken rangeAmountToken)
        {
            if (rawAmount.Contains("-"))
            {
                var parts = rawAmount.Split('-');
                
                rangeAmountToken = new RangeAmountToken()
                {
                    LowerBound = int.Parse(parts[0]),
                    UpperBound = int.Parse(parts[1])
                };

                return true;
            }

            rangeAmountToken = null;
            
            return false;
        }        
        
        /// <inheritdoc/>
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
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
            _parseResult.Details.Amount = $"{token.Numerator}/{token.Denominator}";
        }
        
        /// <summary>
        /// Visits a <see cref="RangeAmountToken"/>.
        /// </summary>
        /// <param name="token">A <see cref="RangeAmountToken"/> instance.</param>
        public void Visit(RangeAmountToken token)
        {
            _parseResult.Details.Amount = $"{token.LowerBound}-{token.UpperBound}";
        }
    }

    #endregion
}