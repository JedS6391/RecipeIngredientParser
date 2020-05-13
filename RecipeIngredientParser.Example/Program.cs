using System;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = CreateParser();
            
            while (true)
            {
                Console.WriteLine("Enter recipe ingredient to parse (or type exit): ");
                var input = Console.ReadLine();

                if (input.Trim().ToLower() == "exit")
                {
                    Environment.Exit(0);
                }

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
                .WithTemplateDefinitions(
                    "{amount} {unit} {form} {ingredient}",
                    "{amount} {unit} {ingredient}")
                .WithTokenReaderFactory(new TokenReaderFactory(tokenReaders))
                .WithParserStrategy(ParserStrategyOption.AcceptFirstFullMatch)
                .WithParserStrategyFactory(new ParserStrategyFactory(parserStrategies))
                .Build();
        }

        private static void DisplayToken(IToken token)
        {
            var tokenOutput = $"{token.GetType().Name}({{0}})";

            switch (token)
            {
                case LiteralToken literalToken:
                    tokenOutput = string.Format(tokenOutput, $"'{literalToken.Value}'");
                    break;
                
                case LiteralAmountToken literalAmountToken:
                    tokenOutput = string.Format(tokenOutput, literalAmountToken.Amount);
                    break;
                
                case FractionalAmountToken fractionalAmountToken:
                    tokenOutput = string.Format(tokenOutput, $"{fractionalAmountToken.Numerator}/{fractionalAmountToken.Denominator}");
                    break;
                
                case RangeAmountToken rangeAmountToken:
                    tokenOutput = string.Format(tokenOutput, $"{rangeAmountToken.LowerBound}-{rangeAmountToken.UpperBound}");
                    break;  
                
                case UnitToken unitToken:
                    tokenOutput = string.Format(tokenOutput, unitToken.Unit);
                    break;
                
                case FormToken formToken:
                    tokenOutput = string.Format(tokenOutput, formToken.Form);
                    break;
                
                case IngredientToken ingredientToken:
                    tokenOutput = string.Format(tokenOutput, ingredientToken.Ingredient);
                    break;
            }

            Console.WriteLine($"-> {tokenOutput}");
        }
    }
}