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
        /// Gets the actual value of the literal the token reader is responsible for.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="LiteralTokenReader"/> class.
        /// </summary>
        /// <param name="value">The literal value to be read.</param>
        public LiteralTokenReader(string value)
        {
            TokenType = $"Literal({value})";
            Value = value;
        }

        /// <inheritdoc/>
        public bool TryReadToken(ParserContext context, out IToken token)
        {
            var value = new StringBuilder();
            
            foreach (var c in Value)
            {
                if (context.Buffer.HasNext() && context.Buffer.Peek() == c)
                {
                    context.Buffer.Consume(c);

                    value.Append(c);
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