using System;
using RecipeIngredientParser.Core.Parser.Exceptions;

namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Represents the context of an <see cref="IngredientParser"/> during the parse operation.
    /// </summary>
    public class ParserContext
    {
        private int _position;
        private readonly char[] _rawIngredientCharacters;

        /// <summary>
        /// Initialises a new instance of the <see cref="ParserContext"/> class.
        /// </summary>
        /// <param name="rawIngredient">The raw ingredient string being parsed.</param>
        public ParserContext(string rawIngredient)
        {
            _position = 0;
            _rawIngredientCharacters = rawIngredient.ToCharArray();
        }
        
        /// <summary>
        /// Determines whether there is another character available for parsing.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when there is another character; <see langword="false"/> otherwise.
        /// </returns>
        public bool HasNext() => _position < _rawIngredientCharacters.Length;

        /// <summary>
        /// Reads the next character without consuming it.
        /// </summary>
        /// <returns>The next character available for parsing.</returns>
        /// <exception cref="ParserOutOfBoundsException">
        /// When the next character cannot be read as all characters have been consumed.
        /// </exception>
        public char Peek()
        {
            if (_position < _rawIngredientCharacters.Length)
            {
                return _rawIngredientCharacters[_position];
            }

            throw new ParserOutOfBoundsException(
        "Unable to read next character as all characters have been consumed.");
        } 

        /// <summary>
        /// Attempts to consume the specified character.
        /// </summary>
        /// <param name="characterToConsume">The character to consume at the current position.</param>
        /// <exception cref="ParserConsumptionFailedException">
        /// When the character specified cannot be consumed at the current position.
        /// </exception>
        public void Consume(char characterToConsume)
        {
            if (!Matches(c => c == characterToConsume))
            {
                throw new ParserConsumptionFailedException(
            $"Unable to consume character {characterToConsume} at current position.");
            }
            
            _position++;
        }
        
        /// <summary>
        /// Moves to the next character.
        /// </summary>
        /// <returns>The character read from the previous position.</returns>
        /// <exception cref="ParserOutOfBoundsException">
        /// When the next character cannot be read as all characters have been consumed.
        /// </exception>
        public char Next()
        {
            var c = Peek();
            
            Consume(c);

            return c;
        }

        /// <summary>
        /// Determines whether the next character matches a given predicate.
        /// </summary>
        /// <param name="predicate">A predicate to execute against the next character.</param>
        /// <returns>
        /// <see langword="true"/> when the next character satisfies the predicate; <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ParserOutOfBoundsException">
        /// When the next character cannot be read as all characters have been consumed.
        /// </exception>
        public bool Matches(Func<char, bool> predicate) => predicate(Peek());

        /// <summary>
        /// Resets the context.
        /// </summary>
        public void Reset()
        {
            _position = 0;
        }
    }
}