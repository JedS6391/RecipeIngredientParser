using System;

namespace RecipeIngredientParser.Core.Templates.Exceptions
{
    /// <summary>
    /// An exception given when a <see cref="Template"/> cannot be built by a <see cref="Template.Builder"/>
    /// </summary>
    public class TemplateBuilderException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TemplateBuilderException"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public TemplateBuilderException(string message)
            : base(message)
        {}
    }
}