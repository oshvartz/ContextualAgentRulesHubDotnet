# Product Context: AgentRulesHub

## Purpose
AgentRulesHub serves as a foundational component for MCP servers to provide contextual rules to AI agents based on their current tasks. It enables dynamic rule retrieval that adapts to the specific needs of each task, improving agent performance and consistency.

## Problems Solved
1. **Dynamic Rule Access**: Enables agents to access relevant rules based on their current task context
2. **Language-Specific Guidance**: Provides specialized rules for different programming languages
3. **Flexible Organization**: Tags allow rules to be categorized and retrieved in multiple ways
4. **Content Source Abstraction**: Separates rule content from storage mechanism

## How It Works
1. Rules are stored with metadata including:
   - Unique identifier
   - Description
   - Optional language specification
   - Tags for categorization
2. Rules content is stored in YAML files, accessible through the FileSource implementation
3. The system can be queried to retrieve rules based on:
   - Programming language
   - Specific tags
   - Combinations of criteria

## User Experience Goals
- Simple and efficient rule retrieval
- Clear organization of rules through tags
- Easy extension for new rule sources
- Straightforward rule content management through YAML files
