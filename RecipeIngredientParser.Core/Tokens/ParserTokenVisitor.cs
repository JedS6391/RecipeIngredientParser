namespace RecipeIngredientParser.Core.Tokens
{
    public partial class ParserTokenVisitor
    {
        private readonly ParsedIngredient _parsedIngredient;

        public ParserTokenVisitor(ParsedIngredient parsedIngredient)
        {
            _parsedIngredient = parsedIngredient;
        }
    }
}