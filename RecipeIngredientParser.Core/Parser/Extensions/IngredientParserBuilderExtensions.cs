using RecipeIngredientParser.Core.Parser.Sanitization;
using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;
using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens.Abstract;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Core.Parser.Extensions
{
    /// <summary>
    /// Defines a set of extensions for building an <see cref="IngredientParser"/>.
    /// </summary>
    public static class IngredientParserBuilderExtensions
    {
        /// <summary>
        /// Configures an <see cref="IngredientParser.Builder"/> instance with a set of default configurations.
        /// </summary>
        /// <remarks>
        /// The configuration applied is as follows:
        /// <list type="bullet">
        ///     <item>
        ///        <description>
        ///             Template definitions: <see cref="TemplateDefinitions.DefaultTemplateDefinitions"/>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///            Token readers:
        ///                 <see cref="AmountTokenReader"/>, <see cref="UnitTokenReader"/>,
        ///                 <see cref="FormTokenReader"/>, <see cref="IngredientTokenReader"/>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///        <description>Parser strategy: <see cref="FirstFullMatchParserStrategy"/></description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             Sanitization rules:
        ///                <see cref="RemoveExtraneousSpacesRule"/>, <see cref="RangeSubstitutionRule"/>,
        ///                <see cref="RemoveBracketedTextRule"/>, <see cref="RemoveAlternateIngredientsRule"/>,
        ///                <see cref="ReplaceUnicodeFractionsRule"/>, <see cref="ConvertToLowerCaseRule"/>.
        ///         </description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="builder">A <see cref="IngredientParser.Builder"/> instance to configure with defaults.</param>
        /// <returns>A <see cref="IngredientParser.Builder"/> instance with the defaults configured.</returns>
        public static IngredientParser.Builder WithDefaultConfiguration(this IngredientParser.Builder builder)
        {
            return builder
                .WithTemplateDefinitions(TemplateDefinitions.DefaultTemplateDefinitions)
                .WithTokenReaderFactory(new TokenReaderFactory(new ITokenReader[]
                {
                    new AmountTokenReader(),
                    new UnitTokenReader(),
                    new FormTokenReader(),
                    new IngredientTokenReader()
                }))
                .WithParserStrategy(new FirstFullMatchParserStrategy())
                .WithSanitizationRules(new IInputSanitizationRule[]
                {
                    new RemoveExtraneousSpacesRule(), 
                    new RangeSubstitutionRule(), 
                    new RemoveBracketedTextRule(), 
                    new RemoveAlternateIngredientsRule(), 
                    new ReplaceUnicodeFractionsRule(), 
                    new RemoveExtraneousSpacesRule(), 
                    new ConvertToLowerCaseRule()
                });
        }
    }
}