using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens
{
    /// <summary>
    /// Defines a visitor to a <see cref="IToken"/> from a <see cref="IngredientParser"/>.
    /// </summary>
    public partial class ParserTokenVisitor
    {
        private readonly ParseResult _parseResult;

        /// <summary>
        /// Initialises a new instance of the <see cref="ParserTokenVisitor"/> class.
        /// </summary>
        /// <param name="parseResult">
        /// The parse result this visitor will allow to be altered by the visited tokens.
        /// </param>
        public ParserTokenVisitor(ParseResult parseResult)
        {
            _parseResult = parseResult;
        }
    }
}