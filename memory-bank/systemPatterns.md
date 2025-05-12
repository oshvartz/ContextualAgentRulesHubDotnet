# System Patterns: AgentRulesHub

## Architecture Overview

### Core Data Models
```mermaid
classDiagram
    class AgentRule {
        +string RuleId
        +string Description
        +string? Language
        +List~string~ Tags
        +RuleSource Source
    }

    class RuleSource {
        <<abstract>>
        +string SourceType
    }

    class FileSource {
        +string FilePath
        // YamlContent is not directly stored here; content is fetched by GetRuleContentAsync
    }

    namespace Configuration {
        class RuleSourceOptions {
            +string LoaderType
            +Dictionary~string,object~ Settings
        }

        class RuleSourcesOptions {
            +List~RuleSourceOptions~ Sources
        }
    }

    class IRuleLoader {
        <<interface>>
        +string LoaderType
        +CanHandle(loaderType string) bool
        +LoadRulesAsync(options RuleSourceOptions) Task~IEnumerable~AgentRule~~
    }
    class YamlRuleLoader {
        +string LoaderType {"YamlFile"}
    }

    class IRuleLoaderOrchestrator {
        <<interface>>
        +LoadRulesAsync(sourcesOptions) Task~IEnumerable~AgentRule~~
    }
    class RuleLoaderOrchestrator {
        -IEnumerable~IRuleLoader~ _loaders
    }

    class IRuleRepository {
        <<interface>>
        +AddRulesAsync(rules)
        +GetRuleByIdAsync(ruleId) AgentRule
        +GetAllRulesAsync() IEnumerable~AgentRule~
    }
    class InMemoryRuleRepository {
        -ConcurrentDictionary~string,AgentRule~ _rules
    }

    AgentRule --> RuleSource
    RuleSource <|-- FileSource

    IRuleLoaderOrchestrator <|.. RuleLoaderOrchestrator
    RuleLoaderOrchestrator o--> IRuleLoader : uses
    RuleLoaderOrchestrator ..> Configuration.RuleSourceOptions : uses
    Configuration.RuleSourcesOptions o--> Configuration.RuleSourceOptions : contains list of
    IRuleLoader <|.. YamlRuleLoader
    YamlRuleLoader ..> IRuleParser : uses

    IRuleRepository <|.. InMemoryRuleRepository

    class IHostedService {
        <<interface>>
    }
    class RuleInitializationService {
        -IRuleLoaderOrchestrator _orchestrator
        -IRuleRepository _repository
    }
    IHostedService <|.. RuleInitializationService
    RuleInitializationService o--> IRuleLoaderOrchestrator : uses
    RuleInitializationService o--> IRuleRepository : uses
```

### New System Patterns Introduced

#### 1. Background Service Initialization Pattern
- **Purpose**: To initialize application-critical services, like loading rules into the repository, automatically when the application starts.
- **Mechanism**:
    - `RuleInitializationService` implements `IHostedService`.
    - It's registered in the DI container in `Program.cs` via `AddHostedService<RuleInitializationService>()`.
    - On application start, the `StartAsync` method of `RuleInitializationService` is called.
    - Inside `StartAsync`, it uses the injected `IRuleLoaderOrchestrator` to load all rules and then uses the injected `IRuleRepository` to store them.
- **Benefits**:
    - Ensures that rules are loaded and the repository is populated as soon as the application is ready.
    - Decouples the initialization logic from `Program.cs`'s `Main` method, leading to cleaner startup code.
    - Leverages the .NET Generic Host's lifecycle management for services.

#### 2. Rule Loader Orchestrator Pattern
- **Purpose**: To manage and coordinate multiple `IRuleLoader` implementations.
- **Mechanism**:
    - `IRuleLoaderOrchestrator` defines the contract for orchestration.
    - `RuleLoaderOrchestrator` implements this, taking a collection of `IRuleLoader` instances.
    - `Configuration.RuleSourcesOptions` (plural) is a configuration model (from `AgentRulesHub.Configuration` namespace), typically bound from `appsettings.json`, containing a list of `Configuration.RuleSourceOptions` (singular).
    - Each `Configuration.RuleSourceOptions` provides configuration for a specific source, specifying `LoaderType` and loader-specific `Settings`.
    - The orchestrator iterates through the `List<Configuration.RuleSourceOptions>` and, for each, selects the appropriate `IRuleLoader` by calling its `CanHandle(options.LoaderType)` method, then delegates rule loading to it.
- **Benefits**:
    - Rule source configurations are externalized to `appsettings.json`, improving maintainability.
    - Decouples rule loading logic from specific loader implementations.
    - Provides a robust mechanism for loader selection using `CanHandle`.
    - Allows easy addition of new rule sources (e.g., database, different file types) by creating new `IRuleLoader` implementations without modifying the orchestrator.
    - Centralizes the process of loading rules from multiple, potentially varied, sources.

#### 2. In-Memory Repository Pattern
- **Purpose**: To provide a centralized, in-memory store for `AgentRule` metadata.
- **Mechanism**:
    - `IRuleRepository` defines the contract for rule storage and retrieval.
    - `InMemoryRuleRepository` implements this using a `ConcurrentDictionary` for thread-safe storage.
    - Provides methods to add rules and retrieve them (e.g., by ID, get all).
- **Benefits**:
    - Fast access to rule metadata.
    - Decouples rule consumers from the loading process.
    - The actual rule content (script/text) is still loaded on-demand via `AgentRule.Source.GetRuleContentAsync()`, keeping the repository lightweight.

## Key Technical Decisions
1.  **Background Service for Initialization**: Using `IHostedService` for rule loading ensures rules are available early in the application lifecycle.

### 1. Abstract Source Pattern
- Used abstract RuleSource class to allow different source implementations
- Enables future expansion to other storage mechanisms
- FileSource as initial implementation for YAML-based storage

### 2. Nullable Language Property
- Language property is optional (nullable string)
- Allows rules to be either language-specific or language-agnostic

### 3. Tag-Based Categorization
- Rules can have multiple tags
- Facilitates flexible rule organization and retrieval
- Implemented as List<string> for simple manipulation

### 4. YAML Content Storage
- YAML chosen for rule content due to:
  - Human-readable format
  - Good structure support
  - Easy version control
  - Wide tool support

## Component Relationships
1.  **AgentRule**: Primary entity representing rule metadata. Holds a `RuleSource` for on-demand content retrieval.
2.  **RuleSource**: Abstract base for different types of rule content sources. `FileSource` is an implementation.
3.  **YamlRuleContent**: DTO for deserializing YAML file content, including the rule text.
4.  **IRuleParser (`YamlRuleParser`)**: Responsible for parsing a rule file (e.g., YAML) into an `AgentRule` object (metadata only, with `FileSource` pointing to the origin).
5.  **IRuleLoader (`YamlRuleLoader`)**: Has a `LoaderType` property and a `CanHandle(string loaderType)` method. Responsible for discovering rule files in a given location (e.g., folder) and using `IRuleParser` to parse each one. It takes an individual `Configuration.RuleSourceOptions`.
6.  **Configuration.RuleSourceOptions**: Configuration object (from `AgentRulesHub.Configuration` namespace) for a single rule source, specifying the `LoaderType` (e.g., "YamlFile") and loader-specific `Settings` (e.g., "Path" for `YamlRuleLoader`). Instances of this are typically part of `Configuration.RuleSourcesOptions`.
7.  **Configuration.RuleSourcesOptions**: A container model (from `AgentRulesHub.Configuration` namespace), typically bound from `appsettings.json` (e.g., from a "RuleSources" section). It holds a `List<Configuration.RuleSourceOptions>` representing all configured rule sources.
8.  **IRuleLoaderOrchestrator (`RuleLoaderOrchestrator`)**: Manages multiple `IRuleLoader` instances. It receives a `List<Configuration.RuleSourceOptions>` (obtained from `Configuration.RuleSourcesOptions` after configuration binding in `Program.cs`). For each `Configuration.RuleSourceOptions` in the list, it uses `options.LoaderType` by calling `loader.CanHandle(options.LoaderType)` on available loaders to select the correct one and initiate the loading process.
9.  **IRuleRepository (`InMemoryRuleRepository`)**: Stores the `AgentRule` metadata. Provides methods for adding and retrieving rules.
10. **RuleInitializationService**: An `IHostedService` that orchestrates the initial loading of rules into the `IRuleRepository` at application startup using the `IRuleLoaderOrchestrator`.

## Critical Implementation Paths
1.  **Rule Metadata Loading (via Background Service)**:
    Application Start -> `IHostedService` (`RuleInitializationService`) -> `IRuleLoaderOrchestrator` -> (uses `IOptions<Configuration.RuleSourcesOptions>`) -> (for each `Configuration.RuleSourceOptions`, selects using `IRuleLoader.CanHandle(options.LoaderType)`) `IRuleLoader` (e.g., `YamlRuleLoader`) -> `IRuleParser` (e.g., `YamlRuleParser`) -> `AgentRule` (metadata + `FileSource`) -> `IRuleRepository`.
2.  **Rule Content Retrieval (On-Demand)**:
    Client requests rule from `IRuleRepository` (already populated) -> Gets `AgentRule` -> Calls `agentRule.Source.GetRuleContentAsync()` -> `FileSource.GetRuleContentAsync()` reads and deserializes the specific rule content from the YAML file.
