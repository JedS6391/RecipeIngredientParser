using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RecipeIngredientParser.Core.Parser.Exceptions;
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
        private static readonly Regex WhitespaceRegex = new Regex(@"[ ]{2,}", RegexOptions.Compiled);

        private readonly IEnumerable<Template> _templates;
        private readonly ParserStrategyOption _strategyOption;
        private readonly IParserStrategyFactory _parserStrategyFactory;

        /// <summary>
        /// Initialises a new instance of the <see cref="IngredientParser"/> class.
        /// </summary>
        /// <param name="templates">The set of templates the parser will attempt parsing with.</param>
        /// <param name="strategyOption">The strategy option to use for parsing.</param>
        /// <param name="parserStrategyFactory">A factory for parsing strategies.</param>
        private IngredientParser(
            IEnumerable<Template> templates,
            ParserStrategyOption strategyOption,
            IParserStrategyFactory parserStrategyFactory)
        {
            _templates = templates;
            _strategyOption = strategyOption;
            _parserStrategyFactory = parserStrategyFactory;
        }
        
        /// <summary>
        /// Attempts to parse a raw ingredient according to the configured templates and parsing strategy.
        /// </summary>
        /// <param name="rawIngredient">A raw ingredient string to parse (e.g. 1 bag vegan sausages).</param>
        /// <param name="parseResult">
        /// When this method returns, contains the result of parsing if the parse operation
        /// succeeded, or <see langword="null"/> if the parse failed.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the parsing succeeded; <see langword="false"/> otherwise.
        /// </returns>
        public bool TryParseIngredient(string rawIngredient, out ParseResult parseResult)
        {
            // TODO: Input validation.
            rawIngredient = NormalizeRawIngredient(rawIngredient);
            var context = new ParserContext(rawIngredient);

            var parserStrategy = _parserStrategyFactory.GetStrategy(_strategyOption);
            
            return parserStrategy.TryParseIngredient(context, _templates, out parseResult);
        }

        private string NormalizeRawIngredient(string rawIngredient)
        {
            return WhitespaceRegex
                .Replace(rawIngredient, " ")
                .ToLower();
        }

        #region Builder
        
        /// <summary>
        /// Provides the ability to build a <see cref="IngredientParser"/> instance.
        /// </summary>
        public class Builder
        {
            private string[] _templateDefinitions;
            private ITokenReaderFactory _tokenReaderFactory;
            private IParserStrategyFactory _parserStrategyFactory;

            private ParserStrategyOption _strategyOption =
                ParserStrategyOption.AcceptFirstFullMatch;
            
            /// <summary>
            /// Gets a new builder instance to start the construction process.
            /// </summary>
            public static Builder New => new Builder();

            /// <summary>
            /// Gets a value indicating whether the builder is in a valid state.
            /// </summary>
            public bool IsValid => 
                _templateDefinitions != null && 
                _tokenReaderFactory != null &&
                _parserStrategyFactory != null;
            
            /// <summary>
            /// Configures the parser to use the specified template definitions
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
            /// Configures the parser to use the specified parsing strategy option.
            /// </summary>
            /// <remarks>
            /// By default, <see cref="ParserStrategyOption.AcceptFirstFullMatch"/> will be used
            /// if not explicitly set during construction.
            /// </remarks>
            /// <param name="strategyOption">A parsing strategy option to use.</param>
            /// <returns>A <see cref="Builder"/> instance with the parsing strategy option configured.</returns>
            public Builder WithParserStrategy(ParserStrategyOption strategyOption)
            {
                _strategyOption = strategyOption;

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
            /// Configures the parser to use the specified <see cref="IParserStrategyFactory"/>.
            /// </summary>
            /// <remarks>
            /// The parser strategy factory will be used to get the correct parsing strategy based on
            /// the configured <see cref="ParserStrategyOption"/>.
            /// </remarks>
            /// <param name="parserStrategyFactory">A <see cref="IParserStrategyFactory"/> instance.</param>
            /// <returns>A <see cref="Builder"/> instance with the parser strategy factory configured.</returns>
            public Builder WithParserStrategyFactory(IParserStrategyFactory parserStrategyFactory)
            {
                _parserStrategyFactory = parserStrategyFactory;

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
                
                var templates = BuildTemplates();

                return new IngredientParser(
                    templates, 
                    _strategyOption,
                    _parserStrategyFactory);
            }

            private IEnumerable<Template> BuildTemplates()
            {
                return _templateDefinitions.Select(CreateTemplate);
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