namespace RecipeIngredientParser.Core.Parser
{
    /// <summary>
    /// Defines the different options for the parsing strategy.
    /// </summary>
    public enum ParserStrategyOption
    {
        /// <summary>
        /// The first full match will be accepted - partial and non-matches will be discarded.
        /// </summary>
        AcceptFirstFullMatch,
        
        /// <summary>
        /// The best full match will be accepted - partial and non-matches will be discarded.
        /// </summary>
        AcceptBestFullMatch,
        
        /// <summary>
        /// The best partial match will be accepted, if there is one.
        /// </summary>
        AcceptBestPartialMatch
    }
}