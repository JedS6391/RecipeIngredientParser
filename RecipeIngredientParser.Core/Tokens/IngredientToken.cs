using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens
{
    /// <summary>
    /// Represents an ingredient token.
    /// </summary>
    public sealed class IngredientToken : IToken
    {
        /// <summary>
        /// Gets or sets the ingredient.
        /// </summary>
        public string Ingredient { get; set; }
        
        /// <inheritdoc/>
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
    }
    
    #region Token visitor

    /// <summary>
    /// The implementation of <see cref="ParserTokenVisitor"/> for the ingredient token.
    /// </summary>
    public partial class ParserTokenVisitor
    {
        /// <summary>
        /// Visits a <see cref="IngredientToken"/>.
        /// </summary>
        /// <param name="token">A <see cref="IngredientToken"/> instance.</param>
        public void Visit(IngredientToken token)
        {
            _parseResult.Details.Ingredient = token.Ingredient;
        }
    }
    
    #endregion
}