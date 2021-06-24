using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Templates.Exceptions;
using RecipeIngredientParser.Core.Tokens.Abstract;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Core.Templates
{
    /// <summary>
    /// Defines a template that can be used to tokenize a raw ingredient string. 
    /// </summary>
    /// <remarks>
    /// A template is described by a template definition (e.g. {amount} {unit} {ingredient}), which
    /// can be used to provide instructions for tokenizing a raw ingredient string.
    ///
    /// Each token type in the definition (specified like {token-type}) can be mapped to a <see cref="ITokenReader"/>
    /// which is responsible for consuming the <see cref="ParserContext"/> until it can extract a <see cref="IToken"/>.
    /// </remarks>
    public class Template
    {
        /// <summary>
        /// Matches a string like '{tokenType}'.
        /// </summary>
        private static readonly Regex TemplateRegex = new Regex(@"(\{[a-z]+\})");
        
        private readonly ITokenReaderFactory _tokenReaderFactory;
        private readonly Lazy<IEnumerable<ITokenReader>> _tokenReaders;

        /// <summary>
        /// Initialises a new instance of the <see cref="Template"/> class.
        /// </summary>
        /// <param name="templateDefinition">The template definition this template describes.</param>
        /// <param name="tokenReaderFactory">A factory for providing <see cref="ITokenReader"/> instances.</param>
        private Template(
            string templateDefinition,
            ITokenReaderFactory tokenReaderFactory)
        {
            Definition = templateDefinition;
            _tokenReaderFactory = tokenReaderFactory;
            _tokenReaders = new Lazy<IEnumerable<ITokenReader>>(InitialiseTokenReaders);
        }

        /// <summary>
        /// Gets the definition this template represents.
        /// </summary>
        public string Definition { get; }

        /// <summary>
        /// Attempts to tokenize the provided context according to this templates definition.
        /// </summary>
        /// <param name="context">The context to read the tokens from.</param>
        /// <param name="tokens">
        /// When this method returns, contains the set of tokens that were extracted from the context if
        /// the tokenization succeeded, or <see langword="null"/> if the tokenization failed.
        /// </param>
        /// <returns>A value indicating the result of the match against the template.</returns>
        public TemplateMatchResult TryReadTokens(ParserContext context, out IEnumerable<IToken> tokens)
        {
            var tokensRead = new List<IToken>();
            
            foreach (var tokenReader in _tokenReaders.Value)
            {
                // Attempt to read the next token based on the definition.
                if (tokenReader.TryReadToken(context, out var token))
                {
                    // Record this token and move to the next.
                    tokensRead.Add(token);
                    
                    continue;
                }

                // Not able to read the next token - bail out.
                tokens = tokensRead;
                    
                return tokensRead.Any() ? 
                    TemplateMatchResult.PartialMatch : 
                    TemplateMatchResult.NoMatch;
            }

            // All tokens were able to be read according to this definition.
            tokens = tokensRead;

            return TemplateMatchResult.FullMatch;
        }
        
        private IEnumerable<ITokenReader> InitialiseTokenReaders()
        {
            var tokenTypes = TemplateRegex.Split(Definition);

            // If there is no token reader for a token type then we
            // fall back to the literal token reader.
            return tokenTypes.Select(t => 
                _tokenReaderFactory.GetTokenReader(t) ??
                    new LiteralTokenReader(t)).ToList();
        }

        #region Builder

        /// <summary>
        /// Provides the ability to build a <see cref="Template"/> instance.
        /// </summary>
        public class Builder
        {
            private string _templateDefinition;
            private ITokenReaderFactory _tokenReaderFactory;
            
            /// <summary>
            /// Gets a new builder instance to start the construction process.
            /// </summary>
            public static Builder New => new Builder();
            
            /// <summary>
            /// Gets a value indicating whether the builder is in a valid state.
            /// </summary>
            public bool IsValid => 
                _templateDefinition != null && 
                _tokenReaderFactory != null;

            /// <summary>
            /// Configures the template to use the specified template definition.
            /// </summary>
            /// <remarks>
            /// The template definition provides instructions on how to tokenize an ingredient string.
            /// </remarks>
            /// <param name="templateDefinition">A definition for the template (e.g. {amount} {unit} {ingredient}).</param>
            /// <returns>A <see cref="Builder"/> instance with the template definition configured.</returns>
            public Builder WithTemplateDefinition(string templateDefinition)
            {
                _templateDefinition = templateDefinition;

                return this;
            }

            /// <summary>
            /// Configures the template to use the specified <see cref="ITokenReaderFactory"/>.
            /// </summary>
            /// <remarks>
            /// The token reader factory will be used to create a set of <see cref="ITokenReader"/> instances
            /// that adhere to the template definition provided.
            /// </remarks>
            /// <param name="tokenReaderFactory">A <see cref="ITokenReaderFactory"/> instance.</param>
            /// <returns>A <see cref="Builder"/> instance with the token reader factory configured.</returns>
            public Builder WithTokenReaderFactory(ITokenReaderFactory tokenReaderFactory)
            {
                _tokenReaderFactory = tokenReaderFactory;

                return this;
            }
            
            /// <summary>
            /// Builds a <see cref="Template"/> instance based on the builders configuration. 
            /// </summary>
            /// <returns>A <see cref="Template"/> instance.</returns>
            /// <exception cref="TemplateBuilderException">
            /// When the builder is unable to build a <see cref="Template"/> in its current state.
            /// </exception>
            public Template Build()
            {
                if (!IsValid)
                {
                    throw new TemplateBuilderException("Unable to build template in current state.");
                }
                
                return new Template(_templateDefinition, _tokenReaderFactory);
            }
        }

        #endregion
    }
}