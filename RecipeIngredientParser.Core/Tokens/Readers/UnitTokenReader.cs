using System.Collections.Generic;
using System.Text;
using RecipeIngredientParser.Core.Parser;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    public class UnitTokenReader : ITokenReader
    {
        private static readonly Dictionary<string, UnitType> UnitMappings = new Dictionary<string, UnitType>
        {
            { "tsp", UnitType.Teaspoon },
            { "t.", UnitType.Teaspoon },
            { "t", UnitType.Teaspoon },
            { "teaspoon", UnitType.Teaspoon },
            { "teaspoons", UnitType.Teaspoon },
            { "tbl", UnitType.Tablespoon },
            { "tbsp.", UnitType.Tablespoon },
            { "tbsp", UnitType.Tablespoon },
            { "tablespoon", UnitType.Tablespoon },
            { "tablespoons", UnitType.Tablespoon },
            { "cup", UnitType.Cup },
            { "cups", UnitType.Cup },
            { "c.", UnitType.Cup },
            { "c", UnitType.Cup },
            { "gram", UnitType.Gram },
            { "grams", UnitType.Gram },
            { "g.", UnitType.Gram },
            { "g", UnitType.Gram }
        };
        
        public string TokenType => "{unit}";

        public bool TryReadToken(ParserContext context, out IToken token)
        {
            var rawUnit = new StringBuilder();
            
            while (context.HasNext() && 
                   (context.IsLetter() || context.Matches(c => c == '.')))
            {
                var c = context.Next();
                
                rawUnit.Append(c);
            }
            
            token = GenerateToken(rawUnit.ToString());

            return token != null;
        }

        private UnitToken GenerateToken(string rawUnit)
        {
            if (string.IsNullOrEmpty(rawUnit))
            {
                return null;
            }
            
            if (UnitMappings.TryGetValue(rawUnit, out var unitType))
            {
                return new UnitToken()
                {
                    Unit = rawUnit,
                    Type = unitType
                };
            }

            return new UnitToken()
            {
                Unit = rawUnit,
                Type = UnitType.Unknown
            };
        }
    }
}