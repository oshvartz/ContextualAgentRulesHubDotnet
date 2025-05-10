# Technical Context: AgentRulesHub

## Technologies Used

### Core Framework
- .NET 8.0
- C# 11
- Nullable reference types enabled

### Project Structure
```
src/AgentRulesHub/
├── Models/
│   ├── AgentRule.cs      # Main rule entity
│   ├── RuleSource.cs     # Abstract base for sources
│   └── FileSource.cs     # YAML file implementation
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
- (Future) YAML parsing library to be added

## Tool Usage Patterns
1. Source Control:
   - Git for version control
   - Feature branch workflow recommended

2. Development:
   - Use nullable reference types
   - Initialize collections in constructors
   - Follow C# coding conventions
