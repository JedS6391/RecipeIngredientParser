namespace RecipeIngredientParser.Core.Tokens.Abstract
{
    /// <summary>
    /// Defines a token that can be read during parsing.
    /// </summary>
    public interface IToken
    {
        /// <summary>
        /// Allows a <see cref="ParserTokenVisitor"/> to visit this token.
        /// </summary>
        /// <remarks>
        /// This method can allow the parse result to be modified by each token.
        /// </remarks>
        /// <param name="parserTokenVisitor">A <see cref="ParserTokenVisitor"/> instance.</param>
        void Accept(ParserTokenVisitor parserTokenVisitor);
    }
}