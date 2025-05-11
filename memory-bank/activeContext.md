# Active Context: AgentRulesHub

## Current Work Focus
- Implemented Rule Loader Orchestrator pattern.
- Implemented In-Memory Repository pattern for rule metadata.
- Ensured rule content (actual script/text) is loaded on-demand and not stored directly in `AgentRule` or the repository.

## Recent Changes
1.  **Created `RuleSourceOptions` model**: To configure different rule loaders.
2.  **Created `IRuleLoaderOrchestrator` interface and `RuleLoaderOrchestrator` service**: To manage multiple rule loaders.
3.  **Updated `IRuleLoader` interface and `YamlRuleLoader`**: To integrate with the orchestrator using `RuleSourceOptions`.
4.  **Created `IRuleRepository` interface and `InMemoryRuleRepository` service**: To store and retrieve rule metadata.
5.  **Updated `Program.cs`**: To demonstrate DI setup and usage of the new orchestrator and repository.
6.  **Added `sample-rule.yaml`**: For testing the loading mechanism.
7.  **Confirmed `AgentRule` does not store rule content**: The `Source` property (`FileSource`) handles on-demand loading of rule content via `GetRuleContentAsync`. The `YamlRuleParser` deserializes the full `YamlRuleContent` (including the rule string) temporarily, but only metadata and the `FileSource` (with `FilePath`) are passed to the `AgentRule` instance.

## Active Decisions
1.  **Rule Content Loading**: Rule content is loaded on-demand via `AgentRule.Source.GetRuleContentAsync()`. This keeps `AgentRule` instances and the `InMemoryRuleRepository` lightweight, containing only metadata. The temporary loading of the full rule string within `YamlRuleParser` into `YamlRuleContent` during initial parsing is deemed acceptable as it's not persisted in the `AgentRule` object.
2.  **Loader Configuration**: `RuleSourceOptions` with a `LoaderType` string and a `Settings` dictionary provides a flexible way to configure different loaders.
3.  **Error Handling in Loaders/Orchestrator**:
    *   `YamlRuleLoader`: If a configured directory in `RuleSourceOptions.Settings["Path"]` is not found, it logs a message and returns an empty list for that source, allowing the orchestrator to continue with other sources.
    *   `RuleLoaderOrchestrator`: If no loader is found for a given `LoaderType`, it logs a message and skips that source. If a loader throws an exception, it's caught, logged, and the orchestrator continues.
4.  **Repository Storage**: `InMemoryRuleRepository` uses a `ConcurrentDictionary` for basic thread safety.

## Project Insights
- The Orchestrator and Repository patterns significantly improve the flexibility and extensibility of the rule loading and management system.
- Separating metadata from content loading is effectively achieved.
- The system is now better prepared for adding new types of rule sources.

## Next Steps
1.  **Unit Tests**: Add comprehensive unit tests for:
    *   `YamlRuleParser`
    *   `YamlRuleLoader`
    *   `RuleLoaderOrchestrator`
    *   `InMemoryRuleRepository`
    *   `FileSource` (especially `GetRuleContentAsync`)
2.  **Refine Error Handling/Logging**: Implement more robust logging (e.g., using `ILogger`) instead of `Console.WriteLine`.
3.  **Configuration Management**: Explore loading `RuleSourceOptions` from a configuration file (e.g., `appsettings.json`) instead of hardcoding in `Program.cs`.
4.  **Advanced Repository Features**: Consider adding more advanced querying capabilities to `IRuleRepository` if needed (e.g., filtering by tags, language).
5.  **Documentation**: Ensure all public APIs are well-documented with XML comments.

## Important Patterns and Preferences
1.  **Code Organization**:
    *   Clear separation into `Interfaces`, `Models`, and `Services` namespaces.
    *   Dependency Injection is used for service resolution.
2.  **Coding Standards**:
    *   Use of nullable reference types.
    *   Asynchronous programming (`async`/`await`) for I/O-bound operations.
    *   Defensive programming (null checks, argument validation).
    *   Favoring interfaces for service contracts.
