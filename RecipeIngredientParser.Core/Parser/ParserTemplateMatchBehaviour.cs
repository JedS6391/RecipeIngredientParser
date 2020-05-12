namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Defines the different parser template matching options.
    /// </summary>
    public enum ParserTemplateMatchBehaviour
    {
        /// <summary>
        /// Only full matches will be accepted - partial and non-matches will be discarded.
        /// </summary>
        OnlyAcceptFullMatch,
        
        /// <summary>
        /// The best partial match will be accepted (if there is one).
        /// </summary>
        AcceptBestPartialMatch
    }
}