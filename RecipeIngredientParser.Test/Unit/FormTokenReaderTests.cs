using NUnit.Framework;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Test.Unit
{
    public class FormTokenReaderTests
    {
        private FormTokenReader _formTokenReader;
        
        [SetUp]
        public void Setup()
        {
            _formTokenReader = new FormTokenReader();
        }
        
        [Test, Sequential]
        public void FormTokenReader_TryReadKnownForm_ShouldReadTokenSuccessfully(
            [Values("grated", "drained", "chopped", "shredded")]
            string form)
        {
            var rawIngredient = $"{form} ingredient";
            var context = new ParserContext(rawIngredient);

            var result = _formTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<FormToken>(token);
            Assert.AreEqual(((FormToken) token).Form, form);
        }
        
        [Test, Sequential]
        public void FormTokenReader_TryReadUnknown_ShouldNotReadTokenSuccessfully(
            [Values("test", "blah", "whatever")]
            string form)
        {
            var rawIngredient = $"{form} ingredient";
            var context = new ParserContext(rawIngredient);

            var result = _formTokenReader.TryReadToken(context, out var token);
            
            Assert.IsFalse(result);
            Assert.IsNull(token);
        }
    }
}