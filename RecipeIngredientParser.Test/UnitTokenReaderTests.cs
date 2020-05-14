using NUnit.Framework;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Test
{
    public class UnitTokenReaderTests
    {
        private UnitTokenReader _unitTokenReader;
        
        [SetUp]
        public void Setup()
        {
            _unitTokenReader = new UnitTokenReader();
        }

        [Test, Sequential]
        public void UnitTokenReader_TryReadTeaspoon_ShouldReadTokenSuccessfully(
            [Values("tsp", "t.", "t", "teaspoon", "teaspoons")]
            string unit)
        {
            var rawIngredient = $"{unit} grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _unitTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<UnitToken>(token);
            Assert.AreEqual(((UnitToken) token).Unit, unit);
            Assert.AreEqual(((UnitToken) token).Type, UnitType.Teaspoon);
        }
        
        [Test, Sequential]
        public void UnitTokenReader_TryReadTablespoon_ShouldReadTokenSuccessfully(
            [Values("tbl", "tbsp.", "tbsp", "tablespoon", "tablespoons")]
            string unit)
        {
            var rawIngredient = $"{unit} grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _unitTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<UnitToken>(token);
            Assert.AreEqual(((UnitToken) token).Unit, unit);
            Assert.AreEqual(((UnitToken) token).Type, UnitType.Tablespoon);
        }
        
        [Test, Sequential]
        public void UnitTokenReader_TryReadCup_ShouldReadTokenSuccessfully(
            [Values("cup", "cups", "c.", "c")]
            string unit)
        {
            var rawIngredient = $"{unit} grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _unitTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<UnitToken>(token);
            Assert.AreEqual(((UnitToken) token).Unit, unit);
            Assert.AreEqual(((UnitToken) token).Type, UnitType.Cup);
        }
        
        [Test, Sequential]
        public void UnitTokenReader_TryReadGram_ShouldReadTokenSuccessfully(
            [Values("gram", "grams", "g.", "g")]
            string unit)
        {
            var rawIngredient = $"{unit} grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _unitTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<UnitToken>(token);
            Assert.AreEqual(((UnitToken) token).Unit, unit);
            Assert.AreEqual(((UnitToken) token).Type, UnitType.Gram);
        }
        
        [Test, Sequential]
        public void UnitTokenReader_TryReadUnknown_ShouldReadTokenSuccessfully(
            [Values("unknown", "test", "blah")]
            string unit)
        {
            var rawIngredient = $"{unit} grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _unitTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<UnitToken>(token);
            Assert.AreEqual(((UnitToken) token).Unit, unit);
            Assert.AreEqual(((UnitToken) token).Type, UnitType.Unknown);
        }
        
        [Test]
        public void UnitTokenReader_TryReadInvalidUnit_ShouldReadTokenSuccessfully()
        {
            var rawIngredient = $"4 grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _unitTokenReader.TryReadToken(context, out var token);
            
            Assert.IsFalse(result);
            Assert.IsNull(token);
        }
    }
}