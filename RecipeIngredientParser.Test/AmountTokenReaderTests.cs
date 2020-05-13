using System;
using NUnit.Framework;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Test
{
    public class AmountTokenReaderTests
    {
        private static readonly Random Random = new Random();
        private AmountTokenReader _amountTokenReader;
        
        [SetUp]
        public void Setup()
        {
            _amountTokenReader = new AmountTokenReader();
        }

        [Test]
        public void AmountTokenReader_TryReadLiteralAmount_ShouldReadTokenSuccessfully()
        {
            var amount = Random.Next(0, 20);
            var rawIngredient = $"{amount} cups grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _amountTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<LiteralAmountToken>(token);
            Assert.AreEqual(((LiteralAmountToken) token).Amount, amount);
        }
        
        [Test]
        public void AmountTokenReader_TryReadFractionalAmount_ShouldReadTokenSuccessfully()
        {
            var numerator = Random.Next(0, 20);
            var denominator = Random.Next(0, 20);
            var rawIngredient = $"{numerator}/{denominator} cups grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _amountTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<FractionalAmountToken>(token);
            Assert.AreEqual(((FractionalAmountToken) token).Numerator, numerator);
            Assert.AreEqual(((FractionalAmountToken) token).Denominator, denominator);
        }
        
        [Test]
        public void AmountTokenReader_TryReadRangeAmount_ShouldReadTokenSuccessfully()
        {
            var lowerBound = Random.Next(0, 20);
            var upperBound = Random.Next(0, 20);
            var rawIngredient = $"{lowerBound}-{upperBound} cups grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _amountTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<RangeAmountToken>(token);
            Assert.AreEqual(((RangeAmountToken) token).LowerBound, lowerBound);
            Assert.AreEqual(((RangeAmountToken) token).UpperBound, upperBound);
        }
        
        [Test]
        public void AmountTokenReader_TryReadInvalidAmount_ShouldNotReadTokenSuccessfully()
        {
            var rawIngredient = $"test cups grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _amountTokenReader.TryReadToken(context, out var token);
            
            Assert.IsFalse(result);
            Assert.IsNull(token);
        }
    }
}