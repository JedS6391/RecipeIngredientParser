using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens
{
    public enum UnitType
    {
        Teaspoon,
        Tablespoon,
        Cup,
        Gram,
        Unknown
    }

    public class UnitToken : IToken
    {
        public string Unit { get; set; }
        public UnitType Type { get; set; }
       
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
    }
    
    #region Token visitors

    public partial class ParserTokenVisitor
    {
        public void Visit(UnitToken token)
        {
            _parsedIngredient.Unit = token.Unit;
        }
    }
    
    #endregion
}