namespace RecipeIngredientParser.Core.Templates
{
    /// <summary>
    /// Defines the different possibilities when matching an ingredient to a <see cref="Template"/>.
    /// </summary>
    public enum TemplateMatchResult
    {
        /// <summary>
        /// The ingredient does not match the template.
        /// </summary>
        NoMatch,
        
        /// <summary>
        /// The ingredient partially matches the template.
        /// </summary>
        PartialMatch,
        
        /// <summary>
        /// The ingredient fully matches the template.
        /// </summary>
        FullMatch
    }
}