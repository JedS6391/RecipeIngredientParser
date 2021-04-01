using System;
using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Parsing
{
    /// <summary>
    /// Provides mechanisms for configuring and building a <see cref="ParserRule"/> instance.
    /// </summary>
    public class ParserRuleBuilder
    {
        private readonly List<ParserRule> _rules;
        private Func<List<IToken>, ParserRuleResult> _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserRuleBuilder"/> class.
        /// </summary>
        private ParserRuleBuilder()
        {
            _rules = new List<ParserRule>();
            _mapper = null;
        }

        /// <summary>
        /// Creates a new <see cref="ParserRuleBuilder"/> instance.
        /// </summary>        
        public static ParserRuleBuilder New() => new();

        /// <summary>
        /// Builds a <see cref="ParserRule"/> from the configured options.
        /// </summary>
        /// <returns>
        /// A <see cref="ParserRule"/> instance reflecting the options configured against the builder.
        /// </returns>
        public ParserRule Build()
        {
            if (!_rules.Any())
            {
                throw new InvalidOperationException("One or more rules must be configured.");
            }

            if (_mapper == null)
            {
                throw new InvalidOperationException("A mapper must be configured.");
            }

            return new SequenceParserRule(_rules, _mapper);
        }

        /// <summary>
        /// Configures the <see cref="ParserRuleBuilder"/> with a rule to match the specified <paramref name="condition"/>.
        /// </summary>
        /// <param name="condition">A condition to apply against the input buffer.</param>
        /// <returns>A <see cref="ParserRuleBuilder"/> with the condition rule configured.</returns>
        public ParserRuleBuilder Condition(Func<InputBuffer, bool> condition)
        {
            var rule = new ConditionParserRule(condition);

            Rule(rule);

            return this;
        }

        /// <summary>
        /// Configures the <see cref="ParserRuleBuilder"/> with a rule to match a particular token
        /// using the specified <paramref name="delegate"/>.
        /// </summary>
        /// <param name="delegate">A delegate to apply against the input buffer.</param>
        /// <returns>A <see cref="ParserRuleBuilder"/> with the token rule configured.</returns>
        public ParserRuleBuilder Token<T>(TokenParserRuleDelegate<T> @delegate) where T : IToken
        {
            var rule = new TokenParserRule<T>(@delegate);

            Rule(rule);

            return this;
        }

        /// <summary>
        /// Configures the <see cref="ParserRuleBuilder"/> with the provided <paramref name="rule"/>.
        /// </summary>
        /// <param name="rule">A rule to apply against the input buffer.</param>
        /// <returns>A <see cref="ParserRuleBuilder"/> with the rule configured.</returns>
        public ParserRuleBuilder Rule(ParserRule rule)
        {
            _rules.Add(rule);

            return this;
        }

        /// <summary>
        /// Configures the <see cref="ParserRuleBuilder"/> with the provided <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">
        /// A mapper for building a <see cref="ParserRuleResult"/> instance when all rules succeed.
        /// </param>
        /// <returns>A <see cref="ParserRuleBuilder"/> with the mapper configured.</returns>
        public ParserRuleBuilder Map(Func<List<IToken>, ParserRuleResult> mapper)
        {
            _mapper = mapper;

            return this;
        }
    }
}
