using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens
{
    public class IngredientToken : IToken
    {
        public string Ingredient { get; set; }
        
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
    }
    
    #region Token visitors

    public partial class ParserTokenVisitor
    {
        public void Visit(IngredientToken token)
        {
            _parseResult.Ingredient.Ingredient = token.Ingredient;
        }
    }
    
    #endregion
}