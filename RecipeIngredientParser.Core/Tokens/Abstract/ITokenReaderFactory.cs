namespace RecipeIngredientParser.Core.Tokens.Abstract
{
    public interface ITokenReaderFactory
    {
        ITokenReader GetTokenReader(string tokenType);
    }
}