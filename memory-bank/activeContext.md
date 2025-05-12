# Active Context: AgentRulesHub

## Current Work Focus
- Implemented MCP server capabilities for AgentRulesHub.

## Recent Changes
1.  **Added MCP Server Functionality**:
    *   Added `ModelContextProtocol` NuGet package to `AgentRulesHub.csproj`.
    *   Created `src/AgentRulesHub/Mcp/RuleProviderTools.cs` with two tools:
        *   `GetRulesByLanguageAsync(string language, IRuleRepository ruleRepository)`: Retrieves rule metadata by language.
        *   `GetRuleContentByIdAsync(string ruleId, IRuleRepository ruleRepository)`: Retrieves rule content by ID.
    *   Methods in `RuleProviderTools` use `[FromServices]` attribute for `IRuleRepository` injection.
2.  **Updated `Program.cs` for MCP Hosting**:
    *   Configured MCP logging to `stderr`.
    *   Registered MCP server services using `AddMcpServer()`, `WithStdioServerTransport()`, and `WithToolsFromAssembly()`.
    *   Removed previous console demonstration logic.
    *   Application now runs as a persistent server using `await builder.Build().RunAsync();`.
3.  **Previous**: Implemented background service for rule initialization (`RuleInitializationService`).
4.  **Previous**: Moved configuration-related models to a dedicated `Configuration` folder.
5.  **Previous**: Implemented loading `RuleSourceOptions` from `appsettings.json`.
6.  **Previous**: Ensured rule content (actual script/text) is loaded on-demand.

## Active Decisions
1.  **MCP Tool Implementation**: Tools are static methods in `RuleProviderTools` class, using `[McpServerToolType]` and `[McpServerTool]` attributes. `IRuleRepository` is injected via `[FromServices]`.
2.  **MCP Hosting**: Using .NET Generic Host with `StdioServerTransport`. Tools are discovered from the assembly.
3.  **Rule Initialization**: Continues to be handled by `RuleInitializationService` at startup.
4.  **Configuration Model Location**: `RuleSourceOptions` and `RuleSourcesOptions` remain in `AgentRulesHub.Configuration`.
5.  **Configuration Source**: Rule source configurations are still managed in `appsettings.json`.

## Project Insights
- The existing architecture of AgentRulesHub (DI, service separation) facilitated a smooth integration of MCP server capabilities.
- The `ModelContextProtocol` library provides a straightforward way to expose C# methods as tools.

## Next Steps
1.  **Update Memory Bank**:
    *   `systemPatterns.md`: Add MCP Server pattern.
    *   `techContext.md`: Update dependencies and project structure.
    *   `progress.md`: Reflect MCP server implementation.
2.  **Testing**:
    *   Manually test the MCP server by sending tool requests (outside the scope of this agent's direct actions, but a logical next step for the user).
    *   Add unit tests for `RuleProviderTools` methods (mocking `IRuleRepository`).
3.  **Refine Error Handling/Logging**: Ensure robust logging within MCP tools if complex logic were added.
4.  **Documentation**: Ensure MCP tools are well-described using the `Description` attribute.

## Important Patterns and Preferences
1.  **Code Organization**:
    *   MCP-specific code (e.g., `RuleProviderTools.cs`) is placed in a dedicated `Mcp` sub-namespace/folder.
    *   DI is leveraged for MCP tool dependencies.
2.  **Coding Standards**:
    *   Consistent use of `async`/`await` for I/O-bound operations in tools.
