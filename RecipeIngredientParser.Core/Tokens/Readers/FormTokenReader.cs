using System.Collections.Generic;
using System.Text;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    /// <summary>
    /// A token reader responsible for the {form} token type.
    /// </summary>
    public class FormTokenReader : ITokenReader
    {
        private static readonly HashSet<string> DefaultForms = new HashSet<string>
        {
            "grated",
            "chopped",
            "drained",
            "shredded"
        };

        private readonly HashSet<string> _forms;

        /// <summary>
        /// Initialises a new instance of the <see cref="FormTokenReader"/> class
        /// that will use the default forms.
        /// </summary>
        public FormTokenReader()
        {
            _forms = DefaultForms;
        }
        
        /// <summary>
        /// Initialises a new instance of the <see cref="FormTokenReader"/> class
        /// that will use the specified forms.
        /// </summary>
        public FormTokenReader(HashSet<string> forms)
        {
            _forms = forms;
        }
        
        /// <inheritdoc/>
        public string TokenType => "{form}";
        
        /// <inheritdoc/>
        public bool TryReadToken(ParserContext context, out IToken token)
        {
            var rawForm = new StringBuilder();
            
            while (context.Buffer.HasNext() && context.Buffer.IsLetter())
            {
                var c = context.Buffer.Next();
                
                rawForm.Append(c);

                // Continue reading until we find something that matches one of the known forms.
                if (!_forms.Contains(rawForm.ToString()))
                {
                    continue;
                }
                
                // Matched a known form.
                token = GenerateToken(rawForm.ToString());

                return true;
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