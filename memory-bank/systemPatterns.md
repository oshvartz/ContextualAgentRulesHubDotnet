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

    class RuleSourceOptions {
        +string LoaderType
        +Dictionary~string,object~ Settings
    }

    class IRuleLoader {
        <<interface>>
        +string LoaderType
        +LoadRulesAsync(options) Task~IEnumerable~AgentRule~~
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
    RuleLoaderOrchestrator ..> RuleSourceOptions : uses
    IRuleLoader <|.. YamlRuleLoader
    YamlRuleLoader ..> IRuleParser : uses

    RuleLoaderOrchestrator --> IRuleRepository : loads into
    IRuleRepository <|.. InMemoryRuleRepository
```

### New System Patterns Introduced

#### 1. Rule Loader Orchestrator Pattern
- **Purpose**: To manage and coordinate multiple `IRuleLoader` implementations.
- **Mechanism**:
    - `IRuleLoaderOrchestrator` defines the contract for orchestration.
    - `RuleLoaderOrchestrator` implements this, taking a collection of `IRuleLoader` instances.
    - `RuleSourceOptions` provides configuration for each source, specifying `LoaderType` and loader-specific `Settings`.
    - The orchestrator selects the appropriate `IRuleLoader` based on `LoaderType` from `RuleSourceOptions` and delegates rule loading to it.
- **Benefits**:
    - Decouples rule loading logic from specific loader implementations.
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
5.  **IRuleLoader (`YamlRuleLoader`)**: Responsible for discovering rule files in a given location (e.g., folder) and using `IRuleParser` to parse each one. It now takes `RuleSourceOptions`.
6.  **RuleSourceOptions**: Configuration object specifying the `LoaderType` (e.g., "YamlFile") and loader-specific `Settings` (e.g., "Path" for `YamlRuleLoader`).
7.  **IRuleLoaderOrchestrator (`RuleLoaderOrchestrator`)**: Manages multiple `IRuleLoader` instances. It uses `RuleSourceOptions` to select the correct loader and initiate the loading process for configured sources.
8.  **IRuleRepository (`InMemoryRuleRepository`)**: Stores the `AgentRule` metadata loaded by the orchestrator. Provides methods for adding and retrieving rules.

## Critical Implementation Paths
1.  **Rule Metadata Loading**:
    `RuleSourceOptions` -> `RuleLoaderOrchestrator` -> (selects) `IRuleLoader` (e.g., `YamlRuleLoader`) -> `IRuleParser` (e.g., `YamlRuleParser`) -> `AgentRule` (metadata + `FileSource`) -> `IRuleRepository`.
2.  **Rule Content Retrieval (On-Demand)**:
    Client requests rule from `IRuleRepository` -> Gets `AgentRule` -> Calls `agentRule.Source.GetRuleContentAsync()` -> `FileSource.GetRuleContentAsync()` reads and deserializes the specific rule content from the YAML file.
