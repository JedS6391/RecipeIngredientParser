namespace RecipeIngredientParser.Core.Parser.Sanitization.Abstract
{
    /// <summary>
    /// Defines a rule for sanitizing the parser input.
    /// </summary>
    public interface IInputSanitizationRule
    {
        /// <summary>
        /// Applies this sanitization rule to the provided input.
        /// </summary>
        /// <param name="input">An input to apply sanitization to.</param>
        /// <returns>The sanitized input.</returns>
        string Apply(string input);
    }
}