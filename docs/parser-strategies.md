# Parser strategies

The parser supports the usage of different parsing strategies by delegating to a particular `IParserStrategy` 
implementation. This allows further control of the parsing operation.

A parser strategy defines how to transform a raw ingredient into a parsed ingredient based on the set of templates
provided to it. 

There are a set of built-in parser strategies that are available.

## `FirstFullMatchParserStrategy`

This strategy will attempt to match the raw ingredient input to each of the provided templates, stopping on the first
full match. Partial and non-matches will be ignored. If there is no full match found, then the parse is considered
unsuccessful.

Upon a successful match, the strategy will create a `ParserTokenVisitor` to visit each of the tokens extracted by
the matching template.

## `BestFullMatchParserStrategy`

This strategy is similar to `FirstFullMatchParserStrategy`, the difference being that this strategy will not stop on
the first full match and will instead collect all full matches and allow a *best* match to be chosen.

Which match is considered the *best* is controllable through a `BestMatchHeuristic`. For example, the `WeightedTokenHeuristic`
could be used to give each extracted token a weighting, depending on which tokens are considered more important. By default,
the strategy will treat the result with the greatest number of tokens as the best.

## `BestPartialMatchParserStrategy`

This strategy is the most lenient of the available built-in strategies. It will allow the *best* partial match to be deemed
a successful parse, in the case where no full match is found. If a full match is found, then that match has the higher priority.

Similar to the `BestFullMatchParserStrategy`, a `BestMatchHeuristic` can be used to control what the *best* match is.

## Demo

```cs --region "parser-strategies" --source-file ../RecipeIngredientParser.Documentation/Program.cs --project ../RecipeIngredientParser.Documentation/RecipeIngredientParser.Documentation.csproj
```