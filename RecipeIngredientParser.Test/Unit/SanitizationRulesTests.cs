using NUnit.Framework;
using RecipeIngredientParser.Core.Parser.Sanitization;

namespace RecipeIngredientParser.Test.Unit
{
    public class SanitizationRulesTests
    {
        [Test]
        [TestCase("1 CUP FLOUR", "1 cup flour")]
        [TestCase("2 CuPs MiLk", "2 cups milk")]
        public void ConvertToLowerCaseRule_AppliedToString_ShouldLowerCaseAllCharacters(
            string unsanitizedValue,
            string expectedSanitizedValue)
        {
            var rule = new ConvertToLowerCaseRule();

            var sanitizedValue = rule.Apply(unsanitizedValue);

            Assert.AreEqual(expectedSanitizedValue, sanitizedValue);
        }

        [Test]
        [TestCase("1 to 2 cups flour", "1-2 cups flour")]
        [TestCase("4 to 5 grams butter", "4-5 grams butter")]
        [TestCase("1/3 cup milk to adjust consistency", "1/3 cup milk to adjust consistency")]
        public void RangeSubstitutionRule_AppliedToString_ReplacesRanges(
            string unsanitizedValue,
            string expectedSanitizedValue)
        {
            var rule = new RangeSubstitutionRule();

            var sanitizedValue = rule.Apply(unsanitizedValue);

            Assert.AreEqual(expectedSanitizedValue, sanitizedValue);
        }

        [Test]
        [TestCase("1 white onion or two small shallots", "1 white onion")]
        public void RemoveAlternateIngredientsRule_AppliedToString_RemovesAlternates(
            string unsanitizedValue,
            string expectedSanitizedValue)
        {
            var rule = new RemoveAlternateIngredientsRule();

            var sanitizedValue = rule.Apply(unsanitizedValue);

            Assert.AreEqual(expectedSanitizedValue, sanitizedValue);
        }

        [Test]
        [TestCase("1/3 cup milk (to adjust consistency)", "1/3 cup milk ")]
        public void RemoveBracketedTextRule_AppliedToString_RemovesBrackedText(
            string unsanitizedValue,
            string expectedSanitizedValue)
        {
            var rule = new RemoveBracketedTextRule();

            var sanitizedValue = rule.Apply(unsanitizedValue);

            Assert.AreEqual(expectedSanitizedValue, sanitizedValue);
        }

        [Test]
        [TestCase("1/3  cup milk", "1/3 cup milk")]
        [TestCase("1  large   white    onion", "1 large white onion")]
        public void RemoveExtraneousSpacesRule_AppliedToString_RemovesExtraSpaces(
            string unsanitizedValue,
            string expectedSanitizedValue)
        {
            var rule = new RemoveExtraneousSpacesRule();

            var sanitizedValue = rule.Apply(unsanitizedValue);

            Assert.AreEqual(expectedSanitizedValue, sanitizedValue);
        }

        [Test]
        [TestCase("½ cup flour", "1/2 cup flour")]
        [TestCase("1 ½ cup flour", "1 1/2 cup flour")]
        [TestCase("¼ cup milk", "1/4 cup milk")]
        [TestCase("¾ cup milk", "3/4 cup milk")]
        public void ReplaceUnicodeFractionsRule_AppliedToString_ShouldRemoveUnicodeFraction(
            string unsanitizedValue,
            string expectedSanitizedValue)
        {
            var rule = new ReplaceUnicodeFractionsRule();

            var sanitizedValue = rule.Apply(unsanitizedValue);

            Assert.AreEqual(expectedSanitizedValue, sanitizedValue);
        }
    }
}
