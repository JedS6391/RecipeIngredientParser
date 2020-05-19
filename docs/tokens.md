# Tokens

A token (defined by the `IToken` interface) represents a unit of a template definition
that can be extracted from a raw string. An `ITokenReader` instance is responsible for
defining how to extract a token of a specific type.

## Token readers

There are a set of built-in token reader implementations which can be used to extract the
main token types. It is possible to build custom token readers as required though.

### Amount tokens

An `AmountToken` is linked to the `{amount}` token type tag. The `AmountTokenReader` is responsible
for reading amount tokens, and supports the following forms:

- 1/2 => `FractionalAmountToken`
- 1-2 => `RangeAmountToken`
- 1   => `LiteralAmountToken`

```cs --region "amount-token" --source-file ../RecipeIngredientParser.Documentation/Program.cs --project ../RecipeIngredientParser.Documentation/RecipeIngredientParser.Documentation.csproj
```

### Unit tokens

The `UnitToken` is linked to the `{unit}` token type tag. To read unit tokens, the `UnitTokenReader` can be used. There are a number
of built in units supported by the reader.

```cs --region "unit-token" --source-file ../RecipeIngredientParser.Documentation/Program.cs --project ../RecipeIngredientParser.Documentation/RecipeIngredientParser.Documentation.csproj
```

### Form tokens

A `FormToken` is linked to the `{form}` token type tag. To read unit tokens, the `FormTokenReader` can be used. There are a number
of built in forms supported by the reader, or custom forms can be provided

```cs --region "form-token" --source-file ../RecipeIngredientParser.Documentation/Program.cs --project ../RecipeIngredientParser.Documentation/RecipeIngredientParser.Documentation.csproj
```

### Ingredient tokens

An `IngredientToken` is linked to the `{ingredient}` token type tag. To read ingredient tokens, the `IngredientTokenReader` can be used. 
The ingredient token type is the hardest to define without a full list of ingredients, so the implementation is simple and tries to eagerly match text.

```cs --region "ingredient-token" --source-file ../RecipeIngredientParser.Documentation/Program.cs --project ../RecipeIngredientParser.Documentation/RecipeIngredientParser.Documentation.csproj
```

### Literal tokens

A `LiteralToken` is an internal concept used to match tokens in the template definition that don't have a specific token type tag (e.g. whitespace).

## Parser token visitor

Each token has the ability to accept a `ParserTokenVisitor` which can be used to allow the token to modify the `ParseResult`
that the parser has created. 