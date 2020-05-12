using System;

namespace RecipeIngredientParser.Core.Parser.Extensions
{
    public static class ParserContextExtensions
    {
        public static bool IsDigit(this ParserContext context) => context.Matches(char.IsDigit);
        public static bool IsLetter(this ParserContext context) => context.Matches(char.IsLetter);
        public static bool IsWhitespace(this ParserContext context) => context.Matches(char.IsWhiteSpace);
    }
}