using System;
using System.Linq;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Example
{
    /// <summary>
    /// An example program to demonstrate the ingredient parser.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
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

                    var totalScore = parseResult
                        .Metadata
                        .Tokens
                        .Sum(t => TokenWeightResolver.Invoke(t));
                    
                    Console.WriteLine($"Total score: {totalScore}");

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
            return IngredientParser
                .Builder
                .New
                .WithDefaultConfiguration()
                .WithParserStrategy(new BestFullMatchParserStrategy(
                    BestMatchHeuristics.WeightedTokenHeuristic(TokenWeightResolver)))
                .Build();
        }
        
        private static Func<IToken, decimal> TokenWeightResolver => token =>
        {
            switch (token)
            {
                case LiteralToken literalToken:
                    // Longer literals score more - the assumption being that
                    // a longer literal means a more specific value.
                    return 0.1m * literalToken.Value.Length;
                    
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
                
                FractionalAmountToken fractionalAmountToken => string.Format(valueInfo, GetRepresentation(fractionalAmountToken)),
                
                RangeAmountToken rangeAmountToken => string.Format(valueInfo, GetRepresentation(rangeAmountToken)),

                UnitToken unitToken => string.Format(valueInfo, unitToken.Unit),

                FormToken formToken => string.Format(valueInfo, formToken.Form),

                IngredientToken ingredientToken => string.Format(valueInfo, ingredientToken.Ingredient),
                
                _ => string.Empty
            };
            
            var output = $"-> {tokenInfo}\n\t{valueInfo}\n\t{scoreInfo}";

            Console.WriteLine(output);
        }

        private static string GetRepresentation(FractionalAmountToken fraction) =>
            fraction.WholeNumber != null ?
                $"{fraction.WholeNumber.Amount} {fraction.Numerator.Amount}/{fraction.Denominator.Amount}" :
                $"{fraction.Numerator.Amount}/{fraction.Denominator.Amount}";

        private static string GetRepresentation(RangeAmountToken range)
        {
            return (range.LowerBound, range.UpperBound) switch
            {
                (LiteralAmountToken lowerBound, LiteralAmountToken upperBound) =>
                    $"{lowerBound.Amount}-{upperBound.Amount}",

                (FractionalAmountToken lowerBound, FractionalAmountToken upperBound) =>
                    $"{GetRepresentation(lowerBound)}-{GetRepresentation(upperBound)}",

                _ => throw new InvalidOperationException($"Unexpected range lower bound token type: {range.LowerBound.Type}")
            };
        }
    }
}