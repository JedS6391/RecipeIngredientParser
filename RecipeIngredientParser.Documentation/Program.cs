using System;
using System.Linq;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Parser.Sanitization;
using RecipeIngredientParser.Core.Parser.Sanitization.Abstract;
using RecipeIngredientParser.Core.Parser.Strategy;
using RecipeIngredientParser.Core.Templates;
using RecipeIngredientParser.Core.Tokens;
using RecipeIngredientParser.Core.Tokens.Abstract;
using RecipeIngredientParser.Core.Tokens.Readers;

namespace RecipeIngredientParser.Documentation
{
    /// <summary>
    /// Contains samples for the documentation.
    /// </summary>
    public class Program
    {
        public static int Main(
            string region = null,
            string project = null,
            string[] args = null)
        {
            return region switch 
            {
                "ingredient-parser" => IngredientParserExample(),
                "template" => TemplateExample(),
                "amount-token" => AmountTokenExample(),
                "unit-token" => UnitTokenExample(),
                "form-token" => FormTokenExample(),
                "ingredient-token" => IngredientTokenExample(),
                "sanitization-rules" => SanitizationRulesExample(),
                "parser-strategies" => ParserStrategiesExample(),
                _ => throw new ArgumentException($"Unexpected region provided {region}")
            };
        }
        
        private static int IngredientParserExample()
        {
            #region ingredient-parser
            
            var templateDefinitions = new string[] 
            {
                TemplateDefinitions.AmountUnitFormIngredient,
                TemplateDefinitions.AmountUnitIngredientForm
            };
            
            var tokenReaders = new ITokenReader[] 
            {
                new AmountTokenReader(),
                new UnitTokenReader(),
                new FormTokenReader(),
                new IngredientTokenReader()
            };
            
            var parserStrategy = new FirstFullMatchParserStrategy();

            var sanitizationRules = new IInputSanitizationRule[] 
            {
                new RemoveExtraneousSpacesRule(), 
                new RangeSubstitutionRule(), 
                new RemoveBracketedTextRule(), 
                new RemoveAlternateIngredientsRule(), 
                new ReplaceUnicodeFractionsRule(), 
                new RemoveExtraneousSpacesRule(), 
                new ConvertToLowerCaseRule()
            };

            var builder = IngredientParser
                .Builder
                .New
                .WithTemplateDefinitions(templateDefinitions)
                .WithTokenReaderFactory(new TokenReaderFactory(tokenReaders))
                .WithParserStrategy(parserStrategy)
                .WithSanitizationRules(sanitizationRules);

            var parser = builder.Build();
            
            if (parser.TryParseIngredient("1 cup grated cheese", out var parseResult)) 
            {
                Console.WriteLine("Parse successful.");
                Console.WriteLine();
                Console.WriteLine($"\tAmount: {parseResult.Details.Amount}");
                Console.WriteLine($"\tUnit: {parseResult.Details.Unit}");
                Console.WriteLine($"\tForm: {parseResult.Details.Form}");
                Console.WriteLine($"\tIngredient: {parseResult.Details.Ingredient}");
            }
            else 
            {
                Console.WriteLine("Parse not successful.");
            }

            #endregion

            return 0;
        }

        private static int TemplateExample()
        {
            #region template

            var templateDefinition = "{amount} {unit} {ingredient}";
            var tokenReaderFactory = new TokenReaderFactory(new ITokenReader[]
            {
                new AmountTokenReader(),
                new UnitTokenReader(),
                new IngredientTokenReader()
            });

            var builder = Template
                .Builder
                .New
                .WithTemplateDefinition(templateDefinition)
                .WithTokenReaderFactory(tokenReaderFactory);

            var template = builder.Build();
            
            var context = new ParserContext("1 cup vegetable stock");
            
            var result = template.TryReadTokens(context, out var tokens);

            switch (result)
            {
                case TemplateMatchResult.NoMatch:
                    Console.WriteLine("No match.");
                    break;
                
                case TemplateMatchResult.PartialMatch:
                    Console.WriteLine("Partial match.");
                    break;

                case TemplateMatchResult.FullMatch:
                    Console.WriteLine("Full match.");
                    Console.WriteLine();

                    foreach (var token in tokens)
                    {
                        Console.WriteLine($"\t- {token.GetType().Name}");
                    }
                    
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected result {result}.");
            }
            
            #endregion
            
            return 0;
        }

        private static int AmountTokenExample()
        {
            #region amount-token

            var context = new ParserContext("1/2");
            var amountTokenReader = new AmountTokenReader();

            if (amountTokenReader.TryReadToken(context, out var amountToken))
            {
                Console.WriteLine("Successfully read amount token.");
                Console.WriteLine();

                switch (amountToken)
                {
                    case FractionalAmountToken fractionalAmountToken:
                        Console.WriteLine($"Value = {fractionalAmountToken.Numerator}/{fractionalAmountToken.Denominator}");
                        break;
                    
                    case RangeAmountToken rangeAmountToken:
                        Console.WriteLine($"Value = {rangeAmountToken.LowerBound}-{rangeAmountToken.UpperBound}");
                        break;
                    
                    case LiteralAmountToken literalAmountToken:
                        Console.WriteLine($"Value: {literalAmountToken.Amount}");
                        break;
                    
                    default:
                        throw new ArgumentException($"Unexpected amount token.");
                }
            }
            else
            {
                Console.WriteLine("Failed to read amount token.");
            }

            #endregion

            return 0;
        }

        private static int UnitTokenExample()
        {
            #region unit-token
        
            var context = new ParserContext("gram");
            var unitTokenReader = new UnitTokenReader();

            if (unitTokenReader.TryReadToken(context, out var unitToken))
            {
                Console.WriteLine("Successfully read unit token.");
                Console.WriteLine();
                
                Console.WriteLine($"Unit type: {((UnitToken) unitToken).Type}");
            }
            else
            {
                Console.WriteLine("Failed to read unit token.");
            }
            
            #endregion

            return 0;
        }

        private static int FormTokenExample()
        {
            #region form-token
        
            var context = new ParserContext("grated");
            var formTokenReader = new FormTokenReader();

            if (formTokenReader.TryReadToken(context, out var formToken))
            {
                Console.WriteLine("Successfully read form token.");
                Console.WriteLine();
                
                Console.WriteLine($"Form: {((FormToken) formToken).Form}");
            }
            else
            {
                Console.WriteLine("Failed to read form token.");
            }
            
            #endregion

            return 0;
        }

        private static int IngredientTokenExample()
        {
            #region ingredient-token
        
            var context = new ParserContext("carrot");
            var ingredientTokenReader = new IngredientTokenReader();

            if (ingredientTokenReader.TryReadToken(context, out var ingredientToken))
            {
                Console.WriteLine("Successfully read ingredient token.");
                Console.WriteLine();
                
                Console.WriteLine($"Form: {((IngredientToken) ingredientToken).Ingredient}");
            }
            else
            {
                Console.WriteLine("Failed to read ingredient token.");
            }
            
            #endregion

            return 0;
        }

        private static int SanitizationRulesExample()
        {
            #region sanitization-rules

            var inputs = new string[]
            {
                "2  cups  grated  cheese",
                "2 to 3",
                "8 whole wheat tortillas (about 8” in diameter)",
                "1 bunch of broccoli or 1 small head of cauliflower",
                "¼ teaspoon ground cumin",
                "1 CUP Grated Cheese"
            };

            var rules = new IInputSanitizationRule[]
            {
                new RemoveExtraneousSpacesRule(),
                new RangeSubstitutionRule(), 
                new RemoveBracketedTextRule(),
                new RemoveAlternateIngredientsRule(),
                new ReplaceUnicodeFractionsRule(),
                new ConvertToLowerCaseRule()
            };

            foreach (var (input, rule) in inputs.Zip(rules))
            {
                var sanitizedInput = rule.Apply(input);
                
                Console.WriteLine($"{rule.GetType().Name}(\"{input}\") => {sanitizedInput}");
            }

            #endregion

            return 0;
        }

        private static int ParserStrategiesExample()
        {
            #region parser-strategies

            static decimal ResolveTokenWeight(IToken token)
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
            }

            var tokenReaderFactory = new TokenReaderFactory(new ITokenReader[]
            {
                new AmountTokenReader(),
                new UnitTokenReader(),
                new FormTokenReader(),
                new IngredientTokenReader()
            });
            var context = new ParserContext("1 cup grated carrot");
            var templates = new Template[]
            {
                Template
                    .Builder
                    .New
                    .WithTemplateDefinition("{amount} {unit} {form} {ingredient}")
                    .WithTokenReaderFactory(tokenReaderFactory)
                    .Build()
            };

            var strategy = new BestFullMatchParserStrategy(
                BestMatchHeuristics.WeightedTokenHeuristic(ResolveTokenWeight));

            if (strategy.TryParseIngredient(context, templates, out var parseResult))
            {
                Console.WriteLine("Parse successful.");
                Console.WriteLine();
                Console.WriteLine($"\tAmount: {parseResult.Details.Amount}");
                Console.WriteLine($"\tUnit: {parseResult.Details.Unit}");
                Console.WriteLine($"\tForm: {parseResult.Details.Form}");
                Console.WriteLine($"\tIngredient: {parseResult.Details.Ingredient}");
            }
            else 
            {
                Console.WriteLine("Parse not successful.");
            }
            
            #endregion

            return 0;
        }
    }
}