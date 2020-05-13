using NUnit.Framework;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Test
{
    public class IngredientTokenReaderTests
    {
        private IngredientTokenReader _ingredientTokenReader;
        
        [SetUp]
        public void Setup()
        {
            _ingredientTokenReader = new IngredientTokenReader();
        }
        
        [Test, Sequential]
        public void IngredientTokenReader_TryReadIngredient_ShouldReadTokenSuccessfully(
            [Values("onion", "cheese", "vegan sausages")]
            string ingredient)
        {
            var rawIngredient = $"{ingredient}";
            var context = new ParserContext(rawIngredient);

            var result = _ingredientTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<IngredientToken>(token);
            Assert.AreEqual(((IngredientToken) token).Ingredient, ingredient);
        }
        
        [Test, Sequential]
        public void IngredientTokenReader_TryReadIngredientWithPunctuation_ShouldReadTokenSuccessfully(
            [Values("onion:", "cheese: ", "vegan sausages; ")]
            string ingredientInput,
            [Values("onion", "cheese", "vegan sausages")]
            string ingredientOutput)
        {
            var rawIngredient = $"{ingredientInput}";
            var context = new ParserContext(rawIngredient);

            var result = _ingredientTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<IngredientToken>(token);
            Assert.AreEqual(((IngredientToken) token).Ingredient, ingredientOutput);
        }
    }
}