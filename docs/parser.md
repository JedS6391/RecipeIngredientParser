#  Parser

`IngredientParser` is the main component that can be used to parse a raw ingredient string.

To create an `IngredientParser` instance, the `IngredientParser.Builder` class can be used.

```cs
var builder = IngredientParser.Builder.New;

// Configure builder
...

var parser = builder.Build();
```

The builder provides a set of methods that allow us to configure how the parser should behave.

## Templates

An `IngredientParser` takes a set of `Template` instances that the parser will attempt to
match the raw ingredient string against. The builder can be used to configure a collection of 
template definitions which will be transformed into `Template` instances for the parser to use.

```cs
var templateDefinitions = new string[] 
{
    "{amount} {unit} {form} {ingredient}",
    "{amount} {unit} {ingredient}, {form}"
};

builder.WithTemplateDefinitions(templateDefinitions);
```

There are also a set of built-in template definitions for common ingredient structures:

```cs
var templateDefinitions = new string[] 
{
    TemplateDefinitions.AmountUnitFormIngredient,
    TemplateDefinitions.AmountUnitIngredientForm
};

builder.WithTemplateDefinitions(templateDefinitions);
```

*For more information on templates, see the [templates section](./templates.md).*

## Token readers

The template definitions provided consist of tags (in the form `{tag}`), that instruct the parser
to use a specific `ITokenReader` when attempting to parse that component of the template. For example,
a `{amount}` tag will instruct the parser to use a `AmountTokenReader` to extract an `AmountToken` from
the raw ingredient string.

The token readers that the parser uses will be provided by an `ITokenReaderFactory`, which can
be configured when building a parser instance. For example, to configure the parser with the default
`ITokenReaderFactory` implementation:

```cs
var tokenReaders = new ITokenReader[] 
{
    new AmountTokenReader(),
    new UnitTokenReader(),
    new FormTokenReader(),
    new IngredientTokenReader()
};

builder.WithTokenReaderFactory(new TokenReaderFactory(tokenReaders));
```

*For more information on token readers, see the [tokens section](./tokens.md).*

## Parser strategy

When the parser attempts to parse a raw ingredient string, it will do so according to a provided
`IParserStrategy`. The builder can be used to configure the strategy that the parser will use:

```cs
var parserStrategy = new FirstFullMatchParserStrategy();

builder.WithParserStrategy(parserStrategy);
```

*For more information on parser strategies, see the [parser strategies section](./parser-strategies.md).*

## Sanitization rules

Before parsing occurs, it can be useful to sanitize the input, to better prepare it for the parsing
operation. A set of built-in sanitization rules are provided, but custom rules can be defined.

The santization rules the parser will use can be configured via the builder:

```cs
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

builder.WithSanitizationRules(sanitizationRules);
```

*For more information on sanitization rules, see the [sanitization rules section](./sanitization-rules.md).*

## Usage

The `IngredientParser` provides the `TryParseIngredient` method which will attempt to parse a raw ingredient. 
A `ParseResult` will be set in the `out` parameter, while the return value of the method can be used to check
whether the parse operation was successful or not.

```cs
var parser = ...;

var successful = parser.TryParseIngredient("1 cup grated cheese", out var parseResult);
```

## Demo

```cs --region "ingredient-parser" --source-file ../RecipeIngredientParser.Documentation/Program.cs --project ../RecipeIngredientParser.Documentation/RecipeIngredientParser.Documentation.csproj
```