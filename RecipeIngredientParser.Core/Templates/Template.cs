using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Tokens.Abstract;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Core.Templates
{
    public class Template
    {
        private static readonly Regex TemplateRegex = new Regex(@"(\{[a-z]+\})");
        
        private readonly string _templateDefinition;
        private readonly ITokenReaderFactory _tokenReaderFactory;
        private readonly Lazy<LinkedList<ITokenReader>> _tokenReaders;

        private Template(
            string templateDefinition,
            ITokenReaderFactory tokenReaderFactory)
        {
            _templateDefinition = templateDefinition;
            _tokenReaderFactory = tokenReaderFactory;
            _tokenReaders = new Lazy<LinkedList<ITokenReader>>(InitialiseTokenReaders);
        }

        public TemplateMatchResult TryReadTokens(ParserContext context, out IEnumerable<IToken> tokens)
        {
            var tokensRead = new List<IToken>();
            
            foreach (var tokenReader in _tokenReaders.Value)
            {
                if (tokenReader.TryReadToken(context, out var token))
                {
                    tokensRead.Add(token);

                    // Move to the next token.
                    continue;
                }
                
                // Not able to read the next token - bail out.
                tokens = tokensRead;
                    
                return tokensRead.Any() ? 
                    TemplateMatchResult.PartialMatch : 
                    TemplateMatchResult.NoMatch;
            }

            tokens = tokensRead;

            return TemplateMatchResult.FullMatch;
        }
        
        private LinkedList<ITokenReader> InitialiseTokenReaders()
        {
            var templateComponents = TemplateRegex.Split(_templateDefinition);
            var tokenReaders = new LinkedList<ITokenReader>();
            
            foreach (var component in templateComponents)
            {
                var tokenReader = _tokenReaderFactory.GetTokenReader(component) 
                                  ?? new LiteralTokenReader(component);

                tokenReaders.AddLast(tokenReader);
            }

            return tokenReaders;
        }

        #region Builder

        public class Builder
        {
            private string _templateDefinition;
            private ITokenReaderFactory _tokenReaderFactory;
            
            public static Builder New => new Builder();
            
            public bool IsValid => _templateDefinition != null && _tokenReaderFactory != null;

            public Builder WithTemplateDefinition(string templateDefinition)
            {
                _templateDefinition = templateDefinition;

                return this;
            }

            public Builder WithTokenReaderFactory(ITokenReaderFactory tokenReaderFactory)
            {
                _tokenReaderFactory = tokenReaderFactory;

                return this;
            }
            
            public Template Build()
            {
                if (!IsValid)
                {
                    // TODO: Exception type
                    throw new Exception();
                }
                
                return new Template(_templateDefinition, _tokenReaderFactory);
            }
        }

        #endregion
    }
}