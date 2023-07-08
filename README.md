# Recipe ingredient parser

> *A parser for recipe ingredients.*

[![nuget][nuget-image]][nuget-url]
[![codecov][codecov-image]][codecov-url]

## About

This project provides a parser that can be used when attempting to parse ingredients of a recipe.

The general approach is to define a set of templates for ingredients and try to parse a raw ingredient according to that template.

For example, a template could be `{amount} {unit} {form} {ingredient}` which would match raw ingredient strings such as:

* 2 cups grated cheese
* 4 grams chopped onion

## Getting started

Following the instructions below to build and run the project in a local development environment.

### Prerequisites

* [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

### Building

After cloning the source code to a destination of your choice, run the following command to build the project:

```shell
dotnet build
```

### Tests

The test suite can be run using the following command:

```shell
dotnet test
```

### Usage

To create an `IngredientParser` instance a builder interface is provided. The default configuration is intended to support a wide range of ingredient formats.

The builder can be used to customise the parser as required. For more details on customising the parser, refer to the [docs](./docs/).

```csharp
var parser = IngredientParser
    .Builder
    .New
    .WithDefaultConfiguration()
    .Build();
```

From here, the `TryParseIngredient` method can be used to parse an ingredient string.

```csharp
if (parser.TryParseIngredient("2 cups grated cheese", out ParseResult parseResult))
{
    // Use parsed result
    ...
}
```

### Example

An example console application is provided that can be used to test the parser out. Run the following command to start the example:

```shell
dotnet run --project RecipeIngredientParser.Example
```

[nuget-image]: https://img.shields.io/nuget/v/RecipeIngredientParser.Core?style=flat-square
[nuget-url]: https://www.nuget.org/packages/RecipeIngredientParser.Core/
[codecov-image]: https://img.shields.io/codecov/c/github/JedS6391/RecipeIngredientParser?style=flat-square&token=P5A6PMGOBA
[codecov-url]: https://codecov.io/gh/JedS6391/RecipeIngredientParser
