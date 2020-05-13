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
            _parseResult.Ingredient.Amount = token.Amount.ToString();
        }
        
        /// <summary>
        /// Visits a <see cref="FractionalAmountToken"/>.
        /// </summary>
        /// <param name="token">A <see cref="FractionalAmountToken"/> instance.</param>
        public void Visit(FractionalAmountToken token)
        {
            _parseResult.Ingredient.Amount = $"{token.Numerator}/{token.Denominator}";
        }
        
        /// <summary>
        /// Visits a <see cref="RangeAmountToken"/>.
        /// </summary>
        /// <param name="token">A <see cref="RangeAmountToken"/> instance.</param>
        public void Visit(RangeAmountToken token)
        {
            _parseResult.Ingredient.Amount = $"{token.LowerBound}-{token.UpperBound}";
        }
    }

    #endregion
}