using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens
{
    /// <summary>
    /// Defines the different unit types supported.
    /// </summary>
    public enum UnitType
    {
        /// <summary>
        /// A teaspoon.
        /// </summary>
        Teaspoon,
        
        /// <summary>
        /// A tablespoon.
        /// </summary>
        Tablespoon,
        
        /// <summary>
        /// A cup.
        /// </summary>
        Cup,
        
        /// <summary>
        /// A gram.
        /// </summary>
        Gram,
        
        /// <summary>
        /// A handful.
        /// </summary>
        Handful,
        
        /// <summary>
        /// An ounce.
        /// </summary>
        Ounce,
        
        /// <summary>
        /// A can.
        /// </summary>
        Can,
        
        /// <summary>
        /// A catch-all for unknown unit types.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Represents a unit token.
    /// </summary>
    public class UnitToken : IToken
    {
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// Gets or sets the unit type.
        /// </summary>
        public UnitType Type { get; set; }
        
        /// <inheritdoc/>
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
    }
    
    #region Token visitor

    /// <summary>
    /// The implementation of <see cref="ParserTokenVisitor"/> for the unit token.
    /// </summary>
    public partial class ParserTokenVisitor
    {
        /// <summary>
        /// Visits a <see cref="UnitToken"/>.
        /// </summary>
        /// <param name="token">A <see cref="UnitToken"/> instance.</param>
        public void Visit(UnitToken token)
        {
            _parseResult.Ingredient.Unit = token.Unit;
        }
    }
    
    #endregion
}