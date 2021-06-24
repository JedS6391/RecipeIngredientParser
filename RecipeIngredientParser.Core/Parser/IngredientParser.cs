using System.Collections.Generic;
using System.Linq;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Parser.Exceptions;
using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Provides the ability to parse an ingredient based on a set of templates.
    /// </summary>
    public class IngredientParser
    {
        private readonly IEnumerable<Template> _templates;
        private readonly IEnumerable<IInputSanitizationRule> _sanitizationRules;
        private readonly IParserStrategy _parserStrategy;

        /// <summary>
        /// Initialises a new instance of the <see cref="IngredientParser"/> class.
        /// </summary>
        /// <param name="templates">The set of templates the parser will attempt parsing with.</param>
        /// <param name="sanitizationRules">A set of rules the parser will use to sanitize the input.</param>
        /// <param name="parserStrategy">A strategy that will be used for parsing.</param>
        private IngredientParser(
            IEnumerable<Template> templates,
            IEnumerable<IInputSanitizationRule> sanitizationRules,
            IParserStrategy parserStrategy)
        {
            _templates = templates;
            _sanitizationRules = sanitizationRules;
            _parserStrategy = parserStrategy;
        }
        
        /// <summary>
        /// Attempts to parse a raw ingredient according to the configured templates and parsing strategy.
        /// </summary>
        /// <param name="ingredient">An ingredient string to parse (e.g. 1 bag vegan sausages).</param>
        /// <param name="parseResult">
        /// When this method returns, contains the result of parsing if the parse operation
        /// succeeded, or <see langword="null"/> if the parse failed.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the parsing succeeded; <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="InvalidParserInputException">When the parser is provided an input that is not valid.</exception>
        public bool TryParseIngredient(string ingredient, out ParseResult parseResult)
        {
            if (string.IsNullOrEmpty(ingredient))
            {
                throw new InvalidParserInputException("Input is not able to be parsed.");
            }
         
            var sanitizedIngredient = Sanitize(ingredient);

            var context = new ParserContext(sanitizedIngredient);

            return _parserStrategy.TryParseIngredient(context, _templates, out parseResult);
        }

        private string Sanitize(string input)
        {
            return _sanitizationRules.Aggregate(
                input, 
                (current, rule) => rule.Apply(current));
        }
        
        #region Builder
        
        /// <summary>
        /// Provides the ability to build a <see cref="IngredientParser"/> instance.
        /// </summary>
        public class Builder
        {
            private string[] _templateDefinitions;
            private IInputSanitizationRule[] _sanitizationRules;
            private ITokenReaderFactory _tokenReaderFactory;
            private IParserStrategy _parserStrategy;
            
            /// <summary>
            /// Gets a new builder instance to start the construction process.
            /// </summary>
            public static Builder New => new Builder();

            /// <summary>
            /// Gets a value indicating whether the builder is in a valid state.
            /// </summary>
            public bool IsValid => 
                _templateDefinitions != null && 
                _sanitizationRules != null &&
                _tokenReaderFactory != null &&
                _parserStrategy != null;
            
            /// <summary>
            /// Configures the parser to use the specified template definitions.
            /// </summary>
            /// <param name="templateDefinitions">
            /// A collection of template definitions (e.g. {amount} {unit} {ingredient}).
            /// </param>
            /// <returns>A <see cref="Builder"/> instance with the template definitions configured.</returns>
            public Builder WithTemplateDefinitions(params string[] templateDefinitions)
            {
                _templateDefinitions = templateDefinitions;

                return this;
            }

            /// <summary>
            /// Configures the parser to use the specified sanitization rules.
            /// </summary>
            /// <param name="sanitizationRules">A collection of sanitization rules.</param>
            /// <returns>A <see cref="Builder"/> instance with the sanitization rules configured.</returns>  
            public Builder WithSanitizationRules(params IInputSanitizationRule[] sanitizationRules)
            {
                _sanitizationRules = sanitizationRules;

                return this;
            }
            
            /// <summary>
            /// Configures the parser to use the specified <see cref="ITokenReaderFactory"/>.
            /// </summary>
            /// <remarks>
            /// The token reader factory will be used when constructing the templates that the parser uses.
            /// </remarks>
            /// <param name="tokenReaderFactory">A <see cref="ITokenReaderFactory"/> instance.</param>
            /// <returns>A <see cref="Builder"/> instance with the token reader factory configured.</returns>
            public Builder WithTokenReaderFactory(ITokenReaderFactory tokenReaderFactory)
            {
                _tokenReaderFactory = tokenReaderFactory;

                return this;
            }

            /// <summary>
            /// Configures the parser to use the specified <see cref="IParserStrategy"/>.
            /// </summary>
            /// <param name="parserStrategy">A <see cref="IParserStrategy"/> instance.</param>
            /// <returns>A <see cref="Builder"/> instance with the parser strategy configured.</returns>
            public Builder WithParserStrategy(IParserStrategy parserStrategy)
            {
                _parserStrategy = parserStrategy;

                return this;
            }

            /// <summary>
            /// Builds a <see cref="IngredientParser"/> instance based on the builders configuration.
            /// </summary>
            /// <returns>A <see cref="IngredientParser"/> instance.</returns>
            /// <exception cref="ParserBuilderException">
            /// When the builder is unable to build a <see cref="IngredientParser"/> in its current state.
            /// </exception>
            public IngredientParser Build()
            {
                if (!IsValid)
                {
                    throw new ParserBuilderException("Unable to build parser in current state.");
                }
                
                var templates = _templateDefinitions.Select(CreateTemplate);

                return new IngredientParser(
                    templates, 
                    _sanitizationRules,
                    _parserStrategy);
            }

            private Template CreateTemplate(string definition)
            {
                var builder = Template
                    .Builder
                    .New
                    .WithTemplateDefinition(definition)
                    .WithTokenReaderFactory(_tokenReaderFactory);

                return builder.Build();
            }
        }
        
        #endregion
    }
}