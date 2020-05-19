# Sanitization rules

As recipe ingredients come in all different forms, it can be useful to apply sanitization rules
to ensure the input is in a somewhat consistent format before attempting to match to a template.

The parser can be configured with a set of `IInputSanitizationRule` which will be applied to the raw
ingredient string before template matching occurs. A collection of default rules are provided for common
sanitization processes:

## `RemoveExtraneousSpacesRule`

Removes extra spaces characters so that the string will only ever have a single consecutive space character.

```cs
var input = "2  cups  grated  cheese";
var rule = new RemoveExtraneousSpacesRule();

var sanitizedInput = rule.Apply(input); // '2 cups grated cheese'
```

## `RangeSubsitutionRule`

The `AmountTokenReader` supports reading ranges when they are in the form 'x-y', so this rule will attempt
to convert instances of 'x to y' to 'x-y' so that the token reader can read them successfully.

```cs
var input = "2 to 3";
var rule = new RangeSubsitutionRule();

var sanitizedInput = rule.Apply(input); // '2-3'
```

## `RemoveBracketedTextRule`

Ingredients will often contain non-essential information in brackets. This rule will remove this usually non-essential
information to help get a cleaner parse result.

```cs
var input = "8 whole wheat tortillas (about 8” in diameter)";
var rule = new RemoveBracketedTextRule();

var sanitizedInput = rule.Apply(input); // '8 whole wheat tortillas '
```

## `RemoveAlternateIngredientsRule`

It is common for ingredients to contain alternatives. This rule takes the stance that the alternative is not essential
and will remove the alternative.

```cs
var input = "1 bunch of broccoli or 1 small head of cauliflower";
var rule = new RemoveAlternateIngredientsRule();

var sanitizedInput = rule.Apply(input); // '1 bunch of broccoli'
```

## `ReplaceUnicodeFractionsRule`

Sometimes ingredients will use unicode fraction symbols instead of plain-text alternatives. This rule converts
any unicode fractions to their non-unicode alterative.

```cs
var input = "¼ teaspoon ground cumin";
var rule = new ReplaceUnicodeFractionsRule();

var sanitizedInput = rule.Apply(input); // '1/4 teaspoon ground cumin'
```

## `ConvertToLowerCaseRule`

To ease the parsing effort, it can be useful to ensure the input string is in lower case.

```cs
var input = "1 CUP Grated Cheese";
var rule = new ConvertToLowerCaseRule();

var sanitizedInput = rule.Apply(input); // '1 cup grated cheese'
```

## Demo

```cs --region "sanitization-rules" --source-file ../RecipeIngredientParser.Documentation/Program.cs --project ../RecipeIngredientParser.Documentation/RecipeIngredientParser.Documentation.csproj
```