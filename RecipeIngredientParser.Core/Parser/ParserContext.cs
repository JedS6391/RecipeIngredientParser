using System;

namespace RecipeIngredientParser.Core.Parser
{
    public class ParserContext
    {
        private int _position;
        private readonly char[] _rawIngredientCharacters;

        public ParserContext(string rawIngredient)
        {
            _position = 0;
            _rawIngredientCharacters = rawIngredient.ToCharArray();
        }

        public bool HasNext()
        {
            return _position < _rawIngredientCharacters.Length;
        }

        public char Peek()
        {
            return _rawIngredientCharacters[_position];
        }

        public void Consume(char characterToConsume)
        {
            if (Matches(c => c == characterToConsume))
            {
                _position++;
            }
        }

        public char Next()
        {
            var c = Peek();
            
            Consume(c);

            return c;
        }

        public bool Matches(Func<char, bool> predicate)
        {
            return predicate(Peek());
        }
    }
}