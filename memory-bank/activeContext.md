# Active Context: AgentRulesHub

## Current Work Focus
- Initial implementation of data models for rule management
- Setting up base architecture for rule source abstraction

## Recent Changes
1. Created core data models:
   - AgentRule with language and tags support
   - Abstract RuleSource base class
   - FileSource for YAML-based implementation

## Active Decisions
1. Using nullable string for Language to support both:
   - Language-specific rules
   - General-purpose rules
2. Implementing Tags as List<string> for flexible categorization
3. Choosing YAML as the initial file format for rule content

## Project Insights
- Abstract source pattern provides flexibility for future extensions
- Tag-based system allows multi-dimensional categorization
- Nullable language support accommodates various rule types

## Next Steps
1. Implement YAML parsing functionality
2. Create rule retrieval logic based on:
   - Language matching
   - Tag filtering
3. Add unit tests for core models
4. Create example rule YAML files
5. Implement rule loading mechanism

## Important Patterns and Preferences
1. Code Organization:
   - Models namespace for data structures
   - Clear separation of concerns between rule and source
2. Coding Standards:
   - Use of nullable reference types
   - Initialization of collections and strings
   - Abstract base class for extensibility
