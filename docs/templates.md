# Templates

A `Template` is a primitive used by the parser as it attempts to parse a raw ingredient string.
The template will define the structure of an ingredient string and provide methods which can be 
used to extract the tokens that comprise that structure.

A `Template` instance can be created by using the `Template.Builder` class:

```cs
var builder = Template.Builder.New;

// Configure builder
...

var template = builder.Build();
```

## Template definitions

A `Template` requires a template definition which will provide instructions on how to tokenize
a raw ingredient string. For example, the template definition `{amount} {unit} {ingredient}` can
be interpreted as follows:

```
{amount} {unit} {ingredient} =>
    - Read an amount token
    - Read a unit token
    - Read an ingredient token
```

Internally, the template definition will be converted to a set of `ITokenReader` instances, which are
responsible for extracting the token according to the definition.

The template definition can be configured by the builder:

```cs
var templateDefinition = "{amount} {unit} {ingredient};

builder.WithTemplateDefinition(templateDefinition);
```

## Token reader factory

As the template definition will be converted to a set of `ITokenReader` instances, a `ITokenReaderFactory` is
needed in order to perform the token reader resolution process.

```cs
var tokenReaders = new ITokenReader[] 
{
    // Token readers that will be needed
    ...
};

var tokenReaderFactory = new TokenReaderFactory(tokenReaders);

builder.WithTokenReaderFactory(tokenReaderFactory);
```

## Usage

While it is not expected for the `Template` class to be used directly (instead an `IngredientParser`) would be used,
it is still possible to directly use a template to try and match an ingredient string.

A `TemplateMatchResult` will be returned to indicate the result of the template matching process. The extracted tokens
(when the result is `TemplateMatchResult.FullMatch` or `TemplateMatchResult.PartialMatch`) will be set in the `out` parameter.

```cs
var template = ...;
var context = new ParserContext("1 cup vegetable stock");

var result = template.TryReadTokens(context, out var tokens);
```

## Demo

```cs --region "template" --source-file ../RecipeIngredientParser.Documentation/Program.cs --project ../RecipeIngredientParser.Documentation/RecipeIngredientParser.Documentation.csproj
```