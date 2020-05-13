using System.Collections.Generic;
using System.Text;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    public class FormTokenReader : ITokenReader
    {
        private static readonly HashSet<string> DefaultForms = new HashSet<string>
        {
            "grated",
            "chopped",
            "drained",
            "shredded"
        };
        
        public string TokenType => "{form}";
        
        public bool TryReadToken(ParserContext context, out IToken token)
        {
            var rawForm = new StringBuilder();
            
            while (context.HasNext() && context.IsLetter())
            {
                var c = context.Next();
                
                rawForm.Append(c);

                // Stop once we find something that matches one of the known forms.
                if (DefaultForms.Contains(rawForm.ToString()))
                {
                    token = GenerateToken(rawForm.ToString());

                    return true;
                }
            }

            token = null;

            return false;
        }
        
        private FormToken GenerateToken(string rawForm)
        {
            if (string.IsNullOrEmpty(rawForm))
            {
                return null;
            }

            return new FormToken()
            {
                Form = rawForm
            };
        }
    }
}