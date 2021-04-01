using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens
{
    /// <summary>
    /// Represents a form token.
    /// </summary>
    public sealed class FormToken : IToken
    {
        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        public string Form { get; set; }
        
        /// <inheritdoc/>
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            parserTokenVisitor.Visit(this);
        }
    }
    
    #region Token visitor

    /// <summary>
    /// The implementation of <see cref="ParserTokenVisitor"/> for the form token.
    /// </summary>
    public partial class ParserTokenVisitor
    {
        /// <summary>
        /// Visits a <see cref="FormToken"/>.
        /// </summary>
        /// <param name="token">A <see cref="FormToken"/> instance.</param>
        public void Visit(FormToken token)
        {
            _parseResult.Details.Form = token.Form;
        }
    }
    
    #endregion
}