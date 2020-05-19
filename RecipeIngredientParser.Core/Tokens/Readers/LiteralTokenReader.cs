using System.Text;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    /// <summary>
    /// A token reader responsible for reading a literal value.
    /// </summary>
    /// <remarks>
    /// This class is meant for internal use only.
    /// </remarks>
    internal class LiteralTokenReader : ITokenReader
    {
        /// <inheritdoc/>
        public string TokenType { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="LiteralTokenReader"/> class.
        /// </summary>
        /// <param name="value">The literal value to be read.</param>
        public LiteralTokenReader(string value)
        {
            TokenType = value;
        }

        /// <inheritdoc/>
        public bool TryReadToken(ParserContext context, out IToken token)
        {
            var value = new StringBuilder();
            
            foreach (var t in TokenType)
            {
                if (context.Buffer.HasNext() && context.Buffer.Peek() == t)
                {
                    context.Buffer.Consume(t);

                    value.Append(t);
                }
                else
                {
                    token = null;
                    
                    return false;
                }
            }

            token = new LiteralToken()
            {
                Value = value.ToString()
            };

            return true;
        }
    }
}