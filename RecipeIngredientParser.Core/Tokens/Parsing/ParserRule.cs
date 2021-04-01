using System;
using System.Collections.Generic;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Parsing
{
    /// <summary>
    /// Defines a rule for parsing an <see cref="InputBuffer"/>.
    /// </summary>
    /// <remarks>
    /// The parse rule may optionally extract a <see cref="IToken"/> from the <see cref="InputBuffer"/>
    /// to populate the <see cref="ParserRuleResult.Token"/>.
    /// </remarks>
    public abstract class ParserRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParserRule"/> class.
        /// </summary>
        protected ParserRule() { }

        /// <summary>
        /// Attempts to parse an <see cref="InputBuffer"/> according to the rule.
        /// </summary>
        /// <param name="buffer">The input buffer to parse.</param>
        /// <returns>A <see cref="ParserRuleResult"/> indicating the result of parsing.</returns>
        public abstract ParserRuleResult Execute(InputBuffer buffer);
    }

    /// <summary>
    /// Represents the result of executing a <see cref="ParserRule"/>.
    /// </summary>
    public class ParserRuleResult
    {
        /// <summary>
        /// Gets a value indicating whether the parsing was successful.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets the token that was extracted when <see cref="Succeeded"/> is <see langword="true"/> if any.
        /// </summary>
        public IToken Token { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserRuleResult"/> class.
        /// </summary>
        /// <param name="succeeded">A value indicating whether parsing was successful.</param>
        /// <param name="token">An optional token extracted from parsing.</param>
        private ParserRuleResult(bool succeeded, IToken token)
        {
            Succeeded = succeeded;
            Token = token;
        }        

        /// <summary>
        /// Creates a new <see cref="ParserRuleResult"/> indicating a failed parse.
        /// </summary>        
        public static ParserRuleResult Fail() => new(succeeded: false, token: null);

        /// <summary>
        /// Creates a new <see cref="ParserRuleResult"/> indicating a successful parse.
        /// </summary>  
        public static ParserRuleResult Success() => new(succeeded: true, token: null);

        /// <summary>
        /// Creates a new <see cref="ParserRuleResult"/> indicating a successful parse with the given <see cref="IToken"/>.
        /// </summary>  
        public static ParserRuleResult Success(IToken token) => new(succeeded: true, token: token);
    }

    /// <summary>
    /// Defines a <see cref="ParserRule"/> that determines if a particular condition holds true
    /// when applied to the <see cref="InputBuffer"/>.
    /// </summary>
    /// <remarks>
    /// Useful for checking if the next character in the buffer matches a particular character but 
    /// no token should be generated when the condition succeeds.
    /// </remarks>
    public class ConditionParserRule : ParserRule
    {
        private readonly Func<InputBuffer, bool> _condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionParserRule"/> class.
        /// </summary>
        /// <param name="condition">A condition to apply against the input buffer.</param>
        internal ConditionParserRule(Func<InputBuffer, bool> condition)
            : base()
        {
            _condition = condition;
        }
        
        /// <inheritdoc/>
        public override ParserRuleResult Execute(InputBuffer buffer)
        {
            return _condition(buffer) ?
                ParserRuleResult.Success(token: null) :
                ParserRuleResult.Fail();
        }
    }

    /// <summary>
    /// Defines a delegate for parsing a <see cref="IToken"/> from an <see cref="InputBuffer"/>.
    /// </summary>
    /// <typeparam name="T">The type of token to parse.</typeparam>
    /// <param name="buffer">The buffer to parse the token from.</param>
    /// <param name="token">The parsed token.</param>
    /// <returns>
    /// <see langword="true"/> if parsing of the token succeeded; <see langword="false"/> otherwise.
    /// </returns>
    public delegate bool TokenParserRuleDelegate<T>(InputBuffer buffer, out T token) where T : IToken;

    /// <summary>
    /// Defines a <see cref="ParserRule"/> that attempts to parse a <see cref="IToken"/> from the <see cref="InputBuffer"/>.
    /// </summary>
    /// <remarks>
    /// Useful for checking if a particular token can be read from the buffer.
    /// </remarks>
    /// <typeparam name="T">The type of token to parse.</typeparam>
    public class TokenParserRule<T> : ParserRule
        where T : IToken
    {
        private readonly TokenParserRuleDelegate<T> _delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenParserRule{T}"/> class.
        /// </summary>
        /// <param name="delegate">A delegate to apply against the input buffer.</param>
        internal TokenParserRule(TokenParserRuleDelegate<T> @delegate)
            : base()
        {
            _delegate = @delegate;
        }

        /// <inheritdoc/>
        public override ParserRuleResult Execute(InputBuffer buffer)
        {
            var succeeded = _delegate(buffer, out var token);

            return succeeded ?
                ParserRuleResult.Success(token) :
                ParserRuleResult.Fail();
        }
    }

    /// <summary>
    /// Defines a <see cref="ParserRule"/> which applies a sequence of <see cref="ParserRule"/>
    /// instances to the <see cref="InputBuffer"/>.
    /// </summary>
    /// <remarks>
    /// Useful for build a single rule that has a sequence of inner rules (e.g. token + condition + token).
    ///
    /// If any of the inner rules fail, then the whole rule will be considered as failed.
    /// 
    /// When all inner rules succeed, a mapping function will be called to build a <see cref="ParserRuleResult"/>
    /// from the collection of tokens parsed (if any).
    /// </remarks>
    public class SequenceParserRule : ParserRule
    {
        private readonly IEnumerable<ParserRule> _rules;
        private readonly Func<List<IToken>, ParserRuleResult> _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceParserRule"/> class.
        /// </summary>
        /// <param name="rules">A collection of rules to be executed sequentially against the input buffer.</param>
        /// <param name="mapper">
        /// A mapper for building a <see cref="ParserRuleResult"/> instance when all rules succeed.
        /// </param>
        internal SequenceParserRule(
            IEnumerable<ParserRule> rules,
            Func<List<IToken>, ParserRuleResult> mapper)
            : base()
        {
            _rules = rules;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public override ParserRuleResult Execute(InputBuffer buffer)
        {
            var tokens = new List<IToken>();

            foreach (var rule in _rules)
            {
                var result = rule.Execute(buffer);

                // All rules must succeed for this rule to pass through to the mapping stage.
                if (!result.Succeeded)
                {
                    return ParserRuleResult.Fail();
                }

                if (result.Token != null)
                {
                    tokens.Add(result.Token);
                }
            }

            return _mapper(tokens);
        }
    }   
}
