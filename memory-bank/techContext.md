# Technical Context: AgentRulesHub

## Technologies Used

### Core Framework
- .NET 8.0
- C# 11
- Nullable reference types enabled

### Project Structure
```
src/AgentRulesHub/
├── Configuration/          # New folder for configuration models
│   ├── RuleSourceOptions.cs
│   └── RuleSourcesOptions.cs
├── Interfaces/
│   ├── IRuleLoader.cs
│   ├── IRuleParser.cs
│   ├── IRuleLoaderOrchestrator.cs
│   └── IRuleRepository.cs
├── Models/
│   ├── AgentRule.cs
│   ├── RuleSource.cs
│   ├── FileSource.cs
│   └── YamlRuleContent.cs
├── Services/
│   ├── YamlRuleParser.cs
│   ├── YamlRuleLoader.cs
│   ├── RuleLoaderOrchestrator.cs
│   ├── InMemoryRuleRepository.cs
│   └── RuleInitializationService.cs # New background service
├── rules/                  # Example folder for rule files
│   └── sample-rule.yaml
└── Program.cs
```

## Development Setup
- Visual Studio Code / Visual Studio 2022
- .NET SDK 8.0 or higher
- Git for version control

## Technical Constraints
1. All string properties initialized to empty string to avoid null
2. Nullable reference types enabled for better null safety
3. Abstract RuleSource class for source implementations
4. YAML format required for file-based rules

## Dependencies
- .NET 8.0 Runtime
- YamlDotNet (for YAML parsing, implicitly used by `FileSource` and `YamlRuleParser`)
- Microsoft.Extensions.DependencyInjection (used in `Program.cs` for DI setup)
- Microsoft.Extensions.Hosting (used for `IHostedService` and `RuleInitializationService`)
- Microsoft.Extensions.Logging (used by `RuleInitializationService`)
- Microsoft.Extensions.Options.ConfigurationExtensions (for binding configuration)

## Tool Usage Patterns
1. Source Control:
   - Git for version control
   - Feature branch workflow recommended

2. Development:
   - Use nullable reference types
   - Initialize collections in constructors
   - Follow C# coding conventions
