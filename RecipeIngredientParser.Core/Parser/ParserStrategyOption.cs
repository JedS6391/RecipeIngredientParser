namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Defines the different options for the parsing strategy.
    /// </summary>
    public enum ParserStrategyOption
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