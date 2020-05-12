using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Parser
{
    public class IngredientParser
    {
        private static readonly Regex WhitespaceRegex = new Regex(@"[ ]{2,}", RegexOptions.Compiled);

        private readonly IEnumerable<Template> _templates;
        private readonly ParserTemplateMatchBehaviour _templateMatchBehaviour;

        private IngredientParser(
            IEnumerable<Template> templates,
            ParserTemplateMatchBehaviour templateMatchBehaviour)
        {
            _templates = templates;
            _templateMatchBehaviour = templateMatchBehaviour;
        }
        
        public bool TryParseIngredient(string rawIngredient, out ParsedIngredient parsedIngredient)
        {
            rawIngredient = NormalizeRawIngredient(rawIngredient);
            var context = new ParserContext(rawIngredient);

            var partialMatches = new List<IEnumerable<IToken>>();
            
            foreach (var template in _templates)
            {
                var result = template.TryReadTokens(context, out var tokens);

                switch (result)
                {
                    case TemplateMatchResult.NoMatch:
                        // Always skip non-matches
                        continue;
                    
                    case TemplateMatchResult.PartialMatch:
                        if (_templateMatchBehaviour == ParserTemplateMatchBehaviour.AcceptBestPartialMatch)
                        {
                            // Keep a track of the partial match in case we need to choose the best.
                            partialMatches.Add(tokens);
                        }

                        continue;
                    
                    case TemplateMatchResult.FullMatch:
                        // Stop on the first full match
                        parsedIngredient = new ParsedIngredient()
                        {
                            RawIngredient = rawIngredient,
                            Metadata = new ParsedIngredient.ParseMetadata()
                            {
                                Template = template,
                                MatchResult = TemplateMatchResult.FullMatch,
                                Tokens =  tokens
                            }
                        };

                        foreach (var token in tokens)
                        {
                            token.Accept(new ParserTokenVisitor(parsedIngredient));
                        }

                        return true;
                    
                    default:
                        // TODO
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (_templateMatchBehaviour == ParserTemplateMatchBehaviour.AcceptBestPartialMatch &&
                partialMatches.Any())
            {
                var bestMatch = partialMatches
                    .OrderByDescending(m => m.Count())
                    .FirstOrDefault();
                
                parsedIngredient = new ParsedIngredient()
                {
                    RawIngredient = rawIngredient,
                    Metadata = new ParsedIngredient.ParseMetadata()
                    {
                        // TODO:
                        Template = null,
                        MatchResult = TemplateMatchResult.PartialMatch,
                        Tokens =  bestMatch
                    }
                };

                return true;
            }

            parsedIngredient = null;
            
            return false;
        }

        private string NormalizeRawIngredient(string rawIngredient)
        {
            return WhitespaceRegex
                .Replace(rawIngredient, " ")
                .ToLower();
        }

        public class Builder
        {
            private string[] _templateDefinitions;
            private ITokenReaderFactory _tokenReaderFactory;

            private ParserTemplateMatchBehaviour _templateMatchBehaviour =
                ParserTemplateMatchBehaviour.OnlyAcceptFullMatch;
            
            public static Builder New => new Builder();

            public bool IsValid => _templateDefinitions != null && _tokenReaderFactory != null;
            
            public Builder WithTemplateDefinitions(params string[] templateDefinitions)
            {
                _templateDefinitions = templateDefinitions;

                return this;
            }

            public Builder WithTemplateMatchBehaviour(ParserTemplateMatchBehaviour templateMatchBehaviour)
            {
                _templateMatchBehaviour = templateMatchBehaviour;

                return this;
            }

            public Builder WithTokenReaderFactory(ITokenReaderFactory tokenReaderFactory)
            {
                _tokenReaderFactory = tokenReaderFactory;

                return this;
            }

            public IngredientParser Build()
            {
                if (!IsValid)
                {
                    // TODO: Exception type
                    throw new Exception();
                }
                
                var templates = BuildTemplates();

                return new IngredientParser(templates, _templateMatchBehaviour);
            }

            private IEnumerable<Template> BuildTemplates()
            {
                var templates = new List<Template>();
                
                foreach (var definition in _templateDefinitions)
                {
                    var builder = Template
                        .Builder
                        .New
                        .WithTemplateDefinition(definition)
                        .WithTokenReaderFactory(_tokenReaderFactory);

                    var template = builder.Build();
                    
                    templates.Add(template);
                }

                return templates;
            }
        }
    }
}