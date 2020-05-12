using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens
{
    public enum AmountTokenType
    {
        Literal,
        Fraction,
        Range
    }

    public interface IAmountToken : IToken
    {
        AmountTokenType Type { get; }
    }
    
    public class LiteralAmountToken : IAmountToken
    {
        public int Amount { get; set; }

        public AmountTokenType Type => AmountTokenType.Literal;
        
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
    }

    public class FractionalAmountToken : IAmountToken
    {
        public int Numerator { get; set; }
        public int Denominator { get; set; }
        
        public AmountTokenType Type => AmountTokenType.Fraction;
        
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
    }

    public class RangeAmountToken : IAmountToken
    {
        public int LowerBound { get; set; }
        public int UpperBound { get; set; }
        
        public AmountTokenType Type => AmountTokenType.Range;
        
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
    }
    
    #region Token visitors

    public partial class ParserTokenVisitor
    {
        public void Visit(LiteralAmountToken token)
        {
            _parsedIngredient.Amount = token.Amount.ToString();
        }
    }
    
    public partial class ParserTokenVisitor
    {
        public void Visit(FractionalAmountToken token)
        {
            _parsedIngredient.Amount = $"{token.Denominator}/{token.Numerator}";
        }
    }
    
    public partial class ParserTokenVisitor
    {
        public void Visit(RangeAmountToken token)
        {
            _parsedIngredient.Amount = $"{token.LowerBound}-{token.UpperBound}";
        }
    }
    
    #endregion
}