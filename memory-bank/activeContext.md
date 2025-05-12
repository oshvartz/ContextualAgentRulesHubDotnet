# Active Context: AgentRulesHub

## Current Work Focus
- Implemented loading `RuleSourceOptions` from `appsettings.json`.
- Ensured rule content (actual script/text) is loaded on-demand.

## Recent Changes
1.  **Created `RuleSourcesOptions` model**: To hold a list of `RuleSourceOptions` for configuration binding.
2.  **Created `appsettings.json`**: To store rule source configurations.
3.  **Updated `Program.cs`**:
    *   To read configuration from `appsettings.json`.
    *   To bind the "RuleSources" section to `RuleSourcesOptions`.
    *   To use the bound options for the `RuleLoaderOrchestrator`.
    *   To adjust relative paths from configuration to absolute paths.
4.  **Updated `AgentRulesHub.csproj`**:
    *   Added `Microsoft.Extensions.Configuration.Json` and `Microsoft.Extensions.Configuration.Binder` NuGet packages.
    *   Configured `appsettings.json` and `*.yaml` files in the `rules` folder to be copied to the output directory.
5.  **Previous**: Created `RuleSourceOptions` model.
6.  **Previous**: Created `IRuleLoaderOrchestrator` interface and `RuleLoaderOrchestrator` service.
7.  **Previous**: Updated `IRuleLoader` interface and `YamlRuleLoader`.
8.  **Previous**: Added `CanHandle(string loaderType)` method to `IRuleLoader`.
9.  **Previous**: Implemented `CanHandle` in `YamlRuleLoader`.
10. **Previous**: Updated `RuleLoaderOrchestrator` to use `CanHandle`.
11. **Previous**: Created `IRuleRepository` interface and `InMemoryRuleRepository` service.
12. **Previous**: Added `sample-rule.yaml`.
13. **Confirmed `AgentRule` does not store rule content**: The `Source` property (`FileSource`) handles on-demand loading.

## Active Decisions
1.  **Configuration Source**: Rule source configurations are now managed in `appsettings.json` and loaded via `Microsoft.Extensions.Configuration`.
2.  **Path Handling**: Paths specified in `appsettings.json` for `YamlFile` loader are treated as relative to the application's base directory and converted to absolute paths in `Program.cs`.
3.  **Rule Content Loading**: Remains on-demand via `AgentRule.Source.GetRuleContentAsync()`.
4.  **Loader Configuration and Selection**: `RuleSourceOptions` (now sourced from config) with `LoaderType` and `Settings` is used. `RuleLoaderOrchestrator` uses `IRuleLoader.CanHandle(string loaderType)`.
5.  **Error Handling in Loaders/Orchestrator**:
    *   `YamlRuleLoader`: If a configured directory is not found, logs and returns an empty list.
    *   `RuleLoaderOrchestrator`: If no loader `CanHandle` a `LoaderType`, logs and skips. Catches loader exceptions.
6.  **Repository Storage**: `InMemoryRuleRepository` uses `ConcurrentDictionary`.

## Project Insights
- Configuration-driven rule sources enhance flexibility and maintainability.
- The system is robust in handling different rule source types and configurations.
- Clear separation of concerns between configuration, loading, and storage.

## Next Steps
1.  **Unit Tests**: Add comprehensive unit tests for:
    *   Configuration loading in `Program.cs`.
    *   `YamlRuleParser`
    *   `YamlRuleLoader` (with various path configurations)
    *   `RuleLoaderOrchestrator`
    *   `InMemoryRuleRepository`
    *   `FileSource` (especially `GetRuleContentAsync`)
2.  **Refine Error Handling/Logging**: Implement more robust logging (e.g., using `ILogger`) throughout, especially for configuration and path resolution issues.
3.  **Advanced Repository Features**: Consider adding more advanced querying capabilities to `IRuleRepository` if needed (e.g., filtering by tags, language).
4.  **Documentation**: Ensure all public APIs are well-documented with XML comments.

## Important Patterns and Preferences
1.  **Code Organization**:
    *   Clear separation into `Interfaces`, `Models`, and `Services` namespaces.
    *   Dependency Injection is used for service resolution.
2.  **Coding Standards**:
    *   Use of nullable reference types.
    *   Asynchronous programming (`async`/`await`) for I/O-bound operations.
    *   Defensive programming (null checks, argument validation).
    *   Favoring interfaces for service contracts.
