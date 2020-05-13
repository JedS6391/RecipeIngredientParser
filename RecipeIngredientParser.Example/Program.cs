using System;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Example
{
    /// <summary>
    /// An example program to demonstrate the ingredient parser.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var parser = CreateParser();
            var input = string.Empty;
            
            while ((input = GetInput()) != "exit")
            {
                if (parser.TryParseIngredient(input, out var parseResult))
                {
                    Console.WriteLine("Successfully parsed provided ingredient:");
                    Console.WriteLine($"\tAmount: {parseResult.Ingredient.Amount}");
                    Console.WriteLine($"\tUnit: {parseResult.Ingredient.Unit}");
                    Console.WriteLine($"\tForm: {parseResult.Ingredient.Form}");
                    Console.WriteLine($"\tIngredient: {parseResult.Ingredient.Ingredient}");

                    Console.WriteLine();
                    Console.WriteLine("Matching template:");
                    Console.WriteLine(parseResult.Metadata.Template.Definition);
                    
                    Console.WriteLine();
                    Console.WriteLine("Tokens:");
                    
                    foreach (var token in parseResult.Metadata.Tokens)
                    {
                        DisplayToken(token);
                    }

                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Failed to parse ingredient provided.");
                }
            }
        }

        private static string GetInput()
        {
            Console.WriteLine("Enter recipe ingredient to parse (or type exit): ");
            
            var input = Console.ReadLine();

            return input.Trim().ToLower();
        }

        private static IngredientParser CreateParser()
        {
            var tokenReaders = new ITokenReader[]
            {
                new AmountTokenReader(),
                new UnitTokenReader(),
                new FormTokenReader(),
                new IngredientTokenReader()
            };

            var parserStrategies = new IParserStrategy[]
            {
                new FirstFullMatchParserStrategy()
            };

            return IngredientParser
                .Builder
                .New
                .WithDefaultConfiguration();
        }

        private static void DisplayToken(IToken token)
        {
            var tokenOutput = $"{token.GetType().Name}({{0}})";

            tokenOutput = token switch
            {
                LiteralToken literalToken =>
                    tokenOutput = string.Format(tokenOutput, $"'{literalToken.Value}'"),

                LiteralAmountToken literalAmountToken =>
                    tokenOutput = string.Format(tokenOutput, literalAmountToken.Amount),

                FractionalAmountToken fractionalAmountToken =>
                    tokenOutput = string.Format(
                        tokenOutput,
                        $"{fractionalAmountToken.Numerator}/{fractionalAmountToken.Denominator}"),

                RangeAmountToken rangeAmountToken =>
                    tokenOutput = string.Format(
                        tokenOutput,
                        $"{rangeAmountToken.LowerBound}-{rangeAmountToken.UpperBound}"),

                UnitToken unitToken =>
                    tokenOutput = string.Format(tokenOutput, unitToken.Unit),

                FormToken formToken =>
                    tokenOutput = string.Format(tokenOutput, formToken.Form),

                IngredientToken ingredientToken =>
                    tokenOutput = string.Format(tokenOutput, ingredientToken.Ingredient),
                
                _ => string.Empty
            };

            Console.WriteLine($"-> {tokenOutput}");
        }
    }
}