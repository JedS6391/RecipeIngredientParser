using System;
using NUnit.Framework;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Test.Unit
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
            Assert.AreEqual(amount, (token as LiteralAmountToken).Amount);
        }

        [Test]
        [TestCase("1.5")]
        [TestCase("0.25")]
        [TestCase(".5")]
        public void AmountTokenReader_TryReadDecimalLiteralAmount_ShouldReadTokenSuccessfully(string amount)
        {
            var rawIngredient = $"{amount} cups grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _amountTokenReader.TryReadToken(context, out var token);

            Assert.IsTrue(result);
            Assert.IsInstanceOf<LiteralAmountToken>(token);
            Assert.AreEqual(decimal.Parse(amount), (token as LiteralAmountToken).Amount);
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
            Assert.AreEqual(null, (token as FractionalAmountToken).WholeNumber);
            Assert.AreEqual(numerator, (token as FractionalAmountToken).Numerator.Amount);
            Assert.AreEqual(denominator, (token as FractionalAmountToken).Denominator.Amount);
        }

        [Test]
        public void AmountTokenReader_TryReadMixedNumberWithSpaceFractionalAmount_ShouldReadTokenSuccessfully()
        {
            var wholeNumber = Random.Next(0, 20);
            var numerator = Random.Next(0, 20);
            var denominator = Random.Next(0, 20);
            var rawIngredient = $"{wholeNumber} {numerator}/{denominator} cups grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _amountTokenReader.TryReadToken(context, out var token);

            Assert.IsTrue(result);
            Assert.IsInstanceOf<FractionalAmountToken>(token);
            Assert.AreEqual(wholeNumber, (token as FractionalAmountToken).WholeNumber.Amount);
            Assert.AreEqual(numerator, (token as FractionalAmountToken).Numerator.Amount);
            Assert.AreEqual(denominator, (token as FractionalAmountToken).Denominator.Amount);
        }

        [Test]
        public void AmountTokenReader_TryReadRangeAmountWithTwoLiteralBounds_ShouldReadTokenSuccessfully()
        {
            var lowerBound = Random.Next(0, 20);
            var upperBound = Random.Next(0, 20);
            var rawIngredient = $"{lowerBound}-{upperBound} cups grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _amountTokenReader.TryReadToken(context, out var token);
            
            Assert.IsTrue(result);
            Assert.IsInstanceOf<RangeAmountToken>(token);

            var lowerBoundToken = (token as RangeAmountToken).LowerBound;
            var upperBoundToken = (token as RangeAmountToken).UpperBound;

            Assert.IsInstanceOf<LiteralAmountToken>(lowerBoundToken);
            Assert.IsInstanceOf<LiteralAmountToken>(upperBoundToken);

            Assert.AreEqual(lowerBound, (lowerBoundToken as LiteralAmountToken).Amount);
            Assert.AreEqual(upperBound, (upperBoundToken as LiteralAmountToken).Amount);            
        }

        [Test]
        public void AmountTokenReader_TryReadRangeAmountWithTwoFractionalBounds_ShouldReadTokenSuccessfully()
        {
            const int numerator = 1;
            const int lowerBoundDenominator = 4;
            const int upperBoundDenominator = 3;

            var lowerBound = $"{numerator}/{lowerBoundDenominator}";
            var upperBound = $"{numerator}/{upperBoundDenominator}";
            var rawIngredient = $"{lowerBound}-{upperBound} cups grated carrot";
            var context = new ParserContext(rawIngredient);            

            var result = _amountTokenReader.TryReadToken(context, out var token);

            Assert.IsTrue(result);
            Assert.IsInstanceOf<RangeAmountToken>(token);

            var lowerBoundToken = (token as RangeAmountToken).LowerBound;
            var upperBoundToken = (token as RangeAmountToken).UpperBound;

            Assert.IsInstanceOf<FractionalAmountToken>(lowerBoundToken);
            Assert.IsInstanceOf<FractionalAmountToken>(upperBoundToken);

            Assert.AreEqual(numerator, (lowerBoundToken as FractionalAmountToken).Numerator.Amount);
            Assert.AreEqual(lowerBoundDenominator, (lowerBoundToken as FractionalAmountToken).Denominator.Amount);
            Assert.AreEqual(numerator, (upperBoundToken as FractionalAmountToken).Numerator.Amount);
            Assert.AreEqual(upperBoundDenominator, (upperBoundToken as FractionalAmountToken).Denominator.Amount);
        }

        [Test]
        [TestCase("test")]
        [TestCase("1..2")]
        [TestCase("1/t")]
        [TestCase("1-t")]
        [TestCase("1-1/t")]
        [TestCase("1/3-1")]
        public void AmountTokenReader_TryReadInvalidAmount_ShouldNotReadTokenSuccessfully(string amount)
        {
            var rawIngredient = $"{amount} cups grated carrot";
            var context = new ParserContext(rawIngredient);

            var result = _amountTokenReader.TryReadToken(context, out var token);
            
            Assert.IsFalse(result);
            Assert.IsNull(token);
        }
    }
}