# Active Context: AgentRulesHub

## Current Work Focus
- Implemented background service for rule initialization.
- Moved configuration-related models to a dedicated `Configuration` folder.

## Recent Changes
1.  **Created `RuleInitializationService`**: An `IHostedService` that uses `RuleLoaderOrchestrator` to load rules and initialize `InMemoryRuleRepository` on application startup.
2.  **Updated `Program.cs`**:
    *   Registered `RuleInitializationService` as a hosted service.
    *   Removed manual rule loading logic, now handled by the background service.
    *   Updated `using` statements for moved configuration models.
3.  **Moved Configuration Models**:
    *   `RuleSourceOptions.cs` moved from `Models/` to `Configuration/`.
    *   `RuleSourcesOptions.cs` moved from `Models/` to `Configuration/`.
    *   Namespaces updated accordingly in these files and their usages.
4.  **Previous**: Implemented loading `RuleSourceOptions` from `appsettings.json`.
5.  **Previous**: Ensured rule content (actual script/text) is loaded on-demand.
6.  **Previous**: Created `RuleSourcesOptions` model (now in `Configuration` folder).
7.  **Previous**: Created `appsettings.json`.
8.  **Previous**: Updated `Program.cs` for configuration binding.
9.  **Previous**: Updated `AgentRulesHub.csproj` for configuration packages and file copying.
10. **Previous**: Created `RuleSourceOptions` model (now in `Configuration` folder).
11. **Previous**: Created `IRuleLoaderOrchestrator` interface and `RuleLoaderOrchestrator` service.
12. **Previous**: Updated `IRuleLoader` interface and `YamlRuleLoader`.
13. **Previous**: Added `CanHandle(string loaderType)` method to `IRuleLoader`.
14. **Previous**: Implemented `CanHandle` in `YamlRuleLoader`.
15. **Previous**: Updated `RuleLoaderOrchestrator` to use `CanHandle`.
16. **Previous**: Created `IRuleRepository` interface and `InMemoryRuleRepository` service.
17. **Previous**: Added `sample-rule.yaml`.
18. **Confirmed `AgentRule` does not store rule content**: The `Source` property (`FileSource`) handles on-demand loading.

## Active Decisions
1.  **Rule Initialization**: Rules are now initialized at application startup via the `RuleInitializationService` background service.
2.  **Configuration Model Location**: `RuleSourceOptions` and `RuleSourcesOptions` are now located in the `AgentRulesHub.Configuration` namespace and `Configuration/` folder.
3.  **Configuration Source**: Rule source configurations are still managed in `appsettings.json`.
4.  **Path Handling**: Paths in `appsettings.json` remain relative and are resolved by the application.
5.  **Rule Content Loading**: Remains on-demand.
6.  **Loader Configuration and Selection**: `RuleSourceOptions` (from `Configuration` namespace) is used.
7.  **Error Handling**: `RuleInitializationService` includes basic error logging. Further refinement is a next step.
8.  **Repository Storage**: `InMemoryRuleRepository` continues to use `ConcurrentDictionary`.

## Project Insights
- Background service initialization simplifies `Program.cs` and ensures rules are ready early.
- Dedicated `Configuration` folder improves project organization.
- The system remains robust and flexible.

## Next Steps
1.  **Unit Tests**: Add comprehensive unit tests for:
    *   `RuleInitializationService`.
    *   Configuration loading in `Program.cs`.
    *   `YamlRuleParser`.
    *   `YamlRuleLoader`.
    *   `RuleLoaderOrchestrator`.
    *   `InMemoryRuleRepository`.
    *   `FileSource`.
2.  **Refine Error Handling/Logging**: Implement more robust logging (e.g., using `ILogger` consistently) throughout, especially in `RuleInitializationService`.
3.  **Advanced Repository Features**: Consider adding more advanced querying capabilities to `IRuleRepository`.
4.  **Documentation**: Ensure all public APIs are well-documented with XML comments.

## Important Patterns and Preferences
1.  **Code Organization**:
    *   Clear separation into `Interfaces`, `Models`, `Services`, and `Configuration` namespaces/folders.
    *   Dependency Injection is used for service resolution, including `IHostedService`.
2.  **Coding Standards**:
    *   Use of nullable reference types.
    *   Asynchronous programming (`async`/`await`) for I/O-bound operations.
    *   Defensive programming (null checks, argument validation).
    *   Favoring interfaces for service contracts.
