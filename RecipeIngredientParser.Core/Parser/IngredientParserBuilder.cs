using System.Linq;
using RecipeIngredientParser.Core.Parser.Exceptions;
using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Provides the ability to build a <see cref="IngredientParser"/> instance.
    /// </summary>
    public class IngredientParserBuilder
    {
        private string[] _templateDefinitions;
        private IInputSanitizationRule[] _sanitizationRules;
        private ITokenReaderFactory _tokenReaderFactory;
        private IParserStrategy _parserStrategy;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientParserBuilder"/> class.
        /// </summary>
        private IngredientParserBuilder()
        {
        }

        /// <summary>
        /// Gets a new builder instance to start the construction process.
        /// </summary>
        public static IngredientParserBuilder New => new IngredientParserBuilder();

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
        /// <returns>An <see cref="IngredientParserBuilder"/> instance with the template definitions configured.</returns>
        public IngredientParserBuilder WithTemplateDefinitions(params string[] templateDefinitions)
        {
            _templateDefinitions = templateDefinitions;

            return this;
        }

        /// <summary>
        /// Configures the parser to use the specified sanitization rules.
        /// </summary>
        /// <param name="sanitizationRules">A collection of sanitization rules.</param>
        /// <returns>An <see cref="IngredientParserBuilder"/> instance with the sanitization rules configured.</returns>  
        public IngredientParserBuilder WithSanitizationRules(params IInputSanitizationRule[] sanitizationRules)
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
        /// <returns>An <see cref="IngredientParserBuilder"/> instance with the token reader factory configured.</returns>
        public IngredientParserBuilder WithTokenReaderFactory(ITokenReaderFactory tokenReaderFactory)
        {
            _tokenReaderFactory = tokenReaderFactory;

            return this;
        }

        /// <summary>
        /// Configures the parser to use the specified <see cref="IParserStrategy"/>.
        /// </summary>
        /// <param name="parserStrategy">A <see cref="IParserStrategy"/> instance.</param>
        /// <returns>An <see cref="IngredientParserBuilder"/> instance with the parser strategy configured.</returns>
        public IngredientParserBuilder WithParserStrategy(IParserStrategy parserStrategy)
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
}