using System;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Parser.Strategy.Abstract;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Example
{
    /// <summary>
    /// An example program to demonstrate the ingredient parser.
    /// </summary>
    public class Program
    {
        private static void Main(string[] args)
        {
            var parser = CreateParser();
            string input;
            
            while ((input = GetInput()) != "exit")
            {
                if (parser.TryParseIngredient(input, out var parseResult))
                {
                    Console.WriteLine("Successfully parsed provided ingredient:");
                    Console.WriteLine($"\tAmount: {parseResult.Details.Amount}");
                    Console.WriteLine($"\tUnit: {parseResult.Details.Unit}");
                    Console.WriteLine($"\tForm: {parseResult.Details.Form}");
                    Console.WriteLine($"\tIngredient: {parseResult.Details.Ingredient}");

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
            var parserStrategies = new IParserStrategy[]
            {
                new BestFullMatchParserStrategy(BestMatchHeuristics.WeightedTokenHeuristic(TokenWeightResolver))
            };

            return IngredientParser
                .Builder
                .New
                .WithDefaultConfiguration()
                .WithParserStrategy(ParserStrategyOption.AcceptBestFullMatch)
                .WithParserStrategyFactory(new ParserStrategyFactory(parserStrategies))
                .Build();
        }
        
        private static Func<IToken, decimal> TokenWeightResolver => token =>
        {
            switch (token)
            {
                case LiteralToken _:
                    return 0.0m;
                    
                case LiteralAmountToken _:
                case FractionalAmountToken _:
                case RangeAmountToken _:
                    return 1.0m;
                    
                case UnitToken unitToken:
                    return unitToken.Type == UnitType.Unknown ?
                        // Punish unknown unit types
                        -1.0m :
                        1.0m;
                    
                case FormToken _:
                    return 1.0m;
                    
                case IngredientToken _:
                    return 2.0m;
            }

            return 0.0m;
        };

        private static void DisplayToken(IToken token)
        {
            var score = TokenWeightResolver.Invoke(token);
            var tokenInfo = $"{token.GetType().Name}";
            var valueInfo = "[value = {0}]";
            var scoreInfo = $"[score = {score}]";
            
            valueInfo = token switch
            {
                LiteralToken literalToken => string.Format(valueInfo, $"'{literalToken.Value}'"),

                LiteralAmountToken literalAmountToken => string.Format(valueInfo, literalAmountToken.Amount),

                FractionalAmountToken fractionalAmountToken => string.Format(
                    valueInfo,
                    $"{fractionalAmountToken.Numerator}/{fractionalAmountToken.Denominator}"),

                RangeAmountToken rangeAmountToken => string.Format(
                    valueInfo,
                    $"{rangeAmountToken.LowerBound}-{rangeAmountToken.UpperBound}"),

                UnitToken unitToken => string.Format(valueInfo, unitToken.Unit),

                FormToken formToken => string.Format(valueInfo, formToken.Form),

                IngredientToken ingredientToken => string.Format(valueInfo, ingredientToken.Ingredient),
                
                _ => string.Empty
            };
            
            var output = $"-> {tokenInfo}\n\t{valueInfo}\n\t{scoreInfo}";

            Console.WriteLine(output);
        }
    }
}