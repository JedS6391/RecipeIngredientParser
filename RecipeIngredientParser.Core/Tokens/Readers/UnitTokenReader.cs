using System.Collections.Generic;
using System.Text;
using RecipeIngredientParser.Core.Parser.Context;
using RecipeIngredientParser.Core.Parser.Extensions;
using RecipeIngredientParser.Core.Tokens.Abstract;

namespace RecipeIngredientParser.Core.Tokens.Readers
{
    /// <summary>
    /// A token reader responsible for the {unit} token type.
    /// </summary>
    public class UnitTokenReader : ITokenReader
    {
        private static readonly Dictionary<string, UnitType> DefaultUnitMappings = new Dictionary<string, UnitType>
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
            { "g", UnitType.Gram },
            { "handful", UnitType.Handful },
            { "ounce", UnitType.Ounce },
            { "ounces", UnitType.Ounce },
            { "oz", UnitType.Ounce },
            { "oz.", UnitType.Ounce },
            { "can", UnitType.Can },
            { "cans", UnitType.Can }
        };
        
        private readonly IDictionary<string, UnitType> _unitMappings;

        /// <summary>
        /// Initialises a new instance of the <see cref="UnitTokenReader"/> class
        /// that will use the default unit mappings.
        /// </summary>
        public UnitTokenReader()
        {
            _unitMappings = DefaultUnitMappings;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UnitTokenReader"/> class
        /// that will use the supplied unit mappings.
        /// </summary>
        /// <param name="unitMappings">A lookup for raw unit values (e.g. grams) to a <see cref="UnitType"/>.</param>
        public UnitTokenReader(IDictionary<string, UnitType> unitMappings)
        {
            _unitMappings = unitMappings;
        }        
        
        /// <inheritdoc/>
        public string TokenType => "{unit}";

        /// <inheritdoc/>
        public bool TryReadToken(ParserContext context, out IToken token)
        {
            var rawUnit = new StringBuilder();
            
            while (context.Buffer.HasNext() && 
                   (context.Buffer.IsLetter() || context.Buffer.Matches(c => c == '.')))
            {
                var c = context.Buffer.Next();
                
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

            return new UnitToken()
            {
                Unit = rawUnit,
                Type = GetUnitType(rawUnit)
            };
        }

        private UnitType GetUnitType(string rawUnit)
        {
            return _unitMappings.TryGetValue(rawUnit, out var unitType) ? 
                unitType : 
                UnitType.Unknown;
        }
    }
}