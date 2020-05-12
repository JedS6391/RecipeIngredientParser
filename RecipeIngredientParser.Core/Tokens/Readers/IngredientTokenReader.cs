using System.Text;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    /// <summary>
    /// A token reader responsible for the {ingredient} token type.
    /// </summary>
    public class IngredientTokenReader : ITokenReader
    {
        /// <inheritdoc/>
        public string TokenType => "{ingredient}";
        
        /// <inheritdoc/>
        public bool TryReadToken(ParserContext context, out IToken token)
        {
            var rawIngredient = new StringBuilder();
            
            while (context.HasNext() && 
                   (context.IsLetter() || context.IsWhitespace()))
            {
                var c = context.Next();
                
                rawIngredient.Append(c);
            }
            
            token = GenerateToken(rawIngredient.ToString());

            return token != null;
        }

        private IngredientToken GenerateToken(string rawIngredient)
        {
            if (string.IsNullOrEmpty(rawIngredient))
            {
                return null;
            }

            return new IngredientToken()
            {
                Ingredient = rawIngredient.Trim()
            };
        }
    }
}