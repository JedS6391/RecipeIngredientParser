# Recipe ingredient parser

> *A parser for recipe ingredients.*

[![nuget][nuget-image]][nuget-url]
[![codecov][codecov-image]][codecov-url]

## About 

This project provides a parser that can be used when attempting to parse ingredients of a recipe.

The general approach is to define a set of templates for ingredients and try to parse a raw ingredient according to that template.

For example, a template could be `{amount} {unit} {form} {ingredient}` which would match raw ingredient strings such as:

  - 2 cups grated cheese
  - 4 grams chopped onion

## Getting started

Following the instructions below to build and run the project in a local development environment.

### Prerequisites

  - [.NET core](https://dotnet.microsoft.com/download) (v3.1)

### Building

After cloning the source code to a destination of your choice, run the following command to build the project:

```
dotnet build
```

A single DLL (`RecipeIngredientParser.Core.dll`) will be produced and provides everything needed to get started.

### Tests

The test suite can be run using the following command:

```
dotnet test
```

### Example

An example console application is provided that can be used to test the parser out. Run the following command to start the example:

```
dotnet run --project RecipeIngredientParser.Example
```

[nuget-image]: https://img.shields.io/nuget/v/RecipeIngredientParser.Core?style=flat-square
[nuget-url]: https://www.nuget.org/packages/RecipeIngredientParser.Core/
[codecov-image]: https://img.shields.io/codecov/c/github/JedS6391/RecipeIngredientParser?style=flat-square&token=P5A6PMGOBA
[codecov-url]: https://codecov.io/gh/JedS6391/RecipeIngredientParser