using System;
using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Parser.Exceptions;

namespace RecipeIngredientParser.Core.Parser.Context
{
    /// <summary>
    /// Responsible for managing the input buffer during parsing.
    /// </summary>
    public class InputBuffer
    {
        /// <summary>
        /// Controls the current position in the buffer.
        /// </summary>
        /// <remarks>The position acts as a cursor for where to read from the source buffer.</remarks>
        private int _position;

        /// <summary>
        /// Contains the source buffer being managed.
        /// </summary>
        private readonly char[] _source;

        /// <summary>
        /// Manages checkpoints during procesing of the input buffer. 
        /// </summary>
        private readonly Stack<InputBufferCheckpoint> _checkpoints;

        /// <summary>
        /// Initialises a new instance of the <see cref="InputBuffer"/> class.
        /// </summary>
        /// <param name="source">The source string the buffer will manage.</param>
        public InputBuffer(string source)
        {
            _position = 0;
            _source = source.ToCharArray();
            _checkpoints = new Stack<InputBufferCheckpoint>();
        }
        
        /// <summary>
        /// Determines whether there is another character available for parsing.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when there is another character; <see langword="false"/> otherwise.
        /// </returns>
        public bool HasNext() => _position < _source.Length;

        /// <summary>
        /// Reads the next character without consuming it.
        /// </summary>
        /// <returns>The next character available for parsing.</returns>
        /// <exception cref="InputBufferOutOfBoundsException">
        /// When the next character cannot be read as all characters have been consumed.
        /// </exception>
        public char Peek()
        {
            if (_position < _source.Length)
            {
                return _source[_position];
            }

            throw new InputBufferOutOfBoundsException(
                "Unable to read next character as all characters have been consumed.");
        }

        /// <summary>
        /// Reads the next <paramref name="count"/> characters without consuming them, respecting the provided <paramref name="offset"/>.
        /// </summary>
        /// <remarks>
        /// When there are not enough characters to meet the requested count, the returned <see cref="IEnumerable{T}"/> 
        /// will contain as many characters as can be read.       
        /// </remarks>
        /// <param name="offset">The number of characters from the current position to start reading.</param>
        /// <param name="count">The number of characters to read.</param>
        /// <returns></returns>
        public IEnumerable<char> Peek(int offset, int count)
        {
            foreach (var position in Enumerable.Range(_position + offset, count))
            {
                if (position < _source.Length)
                {
                    yield return _source[position];
                }
                else
                {
                    yield break;
                }                
            }
        }

        /// <summary>
        /// Consumes the specified character.
        /// </summary>
        /// <param name="characterToConsume">The character to consume at the current position.</param>
        /// <exception cref="InputBufferConsumptionFailedException">
        /// When the character specified cannot be consumed at the current position.
        /// </exception>
        public void Consume(char characterToConsume)
        {
            if (!Matches(c => c == characterToConsume))
            {
                throw new InputBufferConsumptionFailedException(
                    $"Unable to consume character {characterToConsume} at current position.");
            }
            
            _position++;
        }

        /// <summary>
        /// Attempts to consume the specified character.
        /// </summary>
        /// <param name="characterToConsume">The character to consume at the current position.</param>
        /// <returns><see langword="true"/> if the character was consumed; <see langword="false"/> otherwise.</returns>
        public bool TryConsume(char characterToConsume)
        {
            if (!HasNext())
            {
                return false;
            }

            try
            {
                Consume(characterToConsume);

                return true;
            }
            catch (InputBufferConsumptionFailedException)
            {
                return false;
            }
        }

        /// <summary>
        /// Moves to the next character.
        /// </summary>
        /// <returns>The character read from the previous position.</returns>
        /// <exception cref="InputBufferOutOfBoundsException">
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
        /// <exception cref="InputBufferOutOfBoundsException">
        /// When the next character cannot be read as all characters have been consumed.
        /// </exception>
        public bool Matches(Func<char, bool> predicate) => predicate(Peek());

        /// <summary>
        /// Creates a checkpoint at the current position that can be 'rewinded' to. 
        /// </summary>
        /// <returns>
        /// A <see cref="InputBufferCheckpoint"/> instance that can be used to either commit
        /// the changes after the checkpoint, or backtrack to the position when the checkpoint
        /// was created.
        /// </returns>
        public InputBufferCheckpoint Checkpoint()
        {
            var checkpoint = new InputBufferCheckpoint(this, _position);

            _checkpoints.Push(checkpoint);

            return checkpoint;
        }

        /// <summary>
        /// Resets the context.
        /// </summary>
        public void Reset()
        {
            _position = 0;
        }

        /// <summary>
        /// Provides a means for managing a checkpoint of an <see cref="InputBuffer"/>.
        /// </summary>
        /// <remarks>
        /// An <see cref="InputBufferCheckpoint"/> can be treated like a handle for managing a checkpoint.
        ///
        /// When the checkpointed position is ready to be abandoned, the <see cref="InputBufferCheckpoint"/>
        /// instance can be disposed to automatically perform the appropriate action (either commit the changes
        /// or backtrack to the checkpointed position). 
        /// </remarks>
        /// <example>
        /// Commiting changes after a checkpoint with an <see cref="InputBufferCheckpoint"/> instance:
        /// <code>
        /// using (var checkpoint = buffer.Checkpoint())
        /// {       
        ///     buffer.Consume('a');
        ///     buffer.Consume('b');
        ///     buffer.Consume('c');
        ///
        ///     checkpoint.Commit();
        /// }        
        /// </code>
        /// </example>
        /// <example>
        /// Rewinding changes after a checkpoint with an <see cref="InputBufferCheckpoint"/> instance:
        /// <code>
        /// using (var checkpoint = buffer.Checkpoint())
        /// {       
        ///     buffer.Consume('a');
        ///     buffer.Consume('b');
        ///     buffer.Consume('c');
        ///
        ///     // No commit - changes will be undone.
        /// }        
        /// </code>
        /// </example>
        public class InputBufferCheckpoint : IDisposable
        {
            private bool _committed;
            private readonly InputBuffer _buffer;
            private readonly int _position;

            /// <summary>
            /// Initializes a new instance of the <see cref="InputBufferCheckpoint"/> class.
            /// </summary>
            /// <param name="buffer">The buffer this checkpoint was created for.</param>
            /// <param name="position">The position of the buffer when the checkpoint was created.</param>
            internal InputBufferCheckpoint(InputBuffer buffer, int position)
            {
                _committed = false;
                _buffer = buffer;
                _position = position;
            }

            /// <summary>
            /// Marks the checkpoint as committed.
            /// </summary>
            /// <remarks>
            /// A committed checkpoint won't be rewinded when the <see cref="InputBufferCheckpoint"/>
            /// </remarks>
            public void Commit()
            {
                _committed = true;
            }

            /// <summary>
            /// Handles the backtrack to the checkpointed position if the checkpoint remains uncommitted.
            /// </summary>
            public void Dispose()
            {
                var lastCheckpoint = _buffer._checkpoints.Peek();

                if (lastCheckpoint != this)
                {
                    throw new InvalidOperationException(
                        "The last checkpoint must be ended before this checkpoint can be ended.");
                }

                _buffer._checkpoints.Pop();

                if (!_committed)
                {
                    // Need to backtrack to the checkpointed position.
                    _buffer._position = _position;
                }
            }
        }
    }
}