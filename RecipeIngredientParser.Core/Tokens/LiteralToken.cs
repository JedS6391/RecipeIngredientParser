using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens
{
    /// <summary>
    /// Represents a literal token.
    /// </summary>
    public sealed class LiteralToken : IToken
    {
        /// <summary>
        /// Gets or sets the literal value.
        /// </summary>
        public string Value { get; set; }
        
        /// <inheritdoc/>
        public void Accept(ParserTokenVisitor parserTokenVisitor)
        {
            // We do not accept visitors...
        }
    }
}