# Active Context: AgentRulesHub

## Current Work Focus
- Modified `RuleProviderTools.cs`: removed `GetRulesByLanguageAsync` and ensured `GetAllRulesMetadataAsync` (for retrieving the entire rule index) and `GetRuleContentByIdAsync` are present.

## Recent Changes
1.  **Modified MCP Tools**:
    *   Removed `GetRulesByLanguageAsync` from `RuleProviderTools.cs`.
    *   Ensured `GetAllRulesMetadataAsync(IRuleMetadataIndexRepository ruleRepository)` is present in `RuleProviderTools.cs`.
2.  **Previously**: Added New MCP Tool `GetAllRulesMetadataAsync`.
3.  **Previously**: Renamed `IRuleRepository` to `IRuleMetadataIndexRepository` and updated all its usages and method names.
    *   `IRuleRepository` renamed to `IRuleMetadataIndexRepository`.
    *   Methods like `AddRuleAsync` renamed to `AddRuleMetadataAsync`, `GetRuleByIdAsync` to `GetRuleMetadataByIdAsync`, etc.
    *   Updated `InMemoryRuleRepository`, `RuleInitializationService`, `RuleProviderTools`, and `Program.cs` to reflect these changes.
4.  **Previously**: Added MCP Server Functionality:
    *   Added `ModelContextProtocol` NuGet package to `AgentRulesHub.csproj`.
    *   Created `src/AgentRulesHub/Mcp/RuleProviderTools.cs` with initial tools.
    *   Methods in `RuleProviderTools` use `[FromServices]` attribute for `IRuleMetadataIndexRepository` injection.
5.  **Previously**: Updated `Program.cs` for MCP Hosting:
    *   Configured MCP logging to `stderr`.
    *   Registered MCP server services using `AddMcpServer()`, `WithStdioServerTransport()`, and `WithToolsFromAssembly()`.
    *   Removed previous console demonstration logic.
    *   Application now runs as a persistent server using `await builder.Build().RunAsync();`.
4.  **Previous**: Implemented background service for rule initialization (`RuleInitializationService`).
5.  **Previous**: Moved configuration-related models to a dedicated `Configuration` folder.
6.  **Previous**: Implemented loading `RuleSourceOptions` from `appsettings.json`.
7.  **Previous**: Ensured rule content (actual script/text) is loaded on-demand.

## Active Decisions
1.  **Repository Naming**: The repository interface is now `IRuleMetadataIndexRepository` to better reflect its purpose of indexing rule metadata.
2.  **MCP Tool Implementation**: Tools are static methods in `RuleProviderTools` class, using `[McpServerToolType]` and `[McpServerTool]` attributes. `IRuleMetadataIndexRepository` is injected via `[FromServices]`.
3.  **MCP Hosting**: Using .NET Generic Host with `StdioServerTransport`. Tools are discovered from the assembly.
4.  **Rule Initialization**: Continues to be handled by `RuleInitializationService` at startup, using the renamed repository.
4.  **Configuration Model Location**: `RuleSourceOptions` and `RuleSourcesOptions` remain in `AgentRulesHub.Configuration`.
5.  **Configuration Source**: Rule source configurations are still managed in `appsettings.json`.

## Project Insights
- The existing architecture of AgentRulesHub (DI, service separation) facilitated a smooth integration of MCP server capabilities.
- The `ModelContextProtocol` library provides a straightforward way to expose C# methods as tools.

## Next Steps
1.  **Update Memory Bank**:
    *   `progress.md`: Reflect repository renaming.
2.  **Testing**:
    *   Manually test the MCP server by sending tool requests (outside the scope of this agent's direct actions, but a logical next step for the user).
    *   Add unit tests for `RuleProviderTools` methods (mocking `IRuleMetadataIndexRepository`).
3.  **Refine Error Handling/Logging**: Ensure robust logging within MCP tools if complex logic were added.
4.  **Documentation**: Ensure MCP tools are well-described using the `Description` attribute.

## Important Patterns and Preferences
1.  **Code Organization**:
    *   MCP-specific code (e.g., `RuleProviderTools.cs`) is placed in a dedicated `Mcp` sub-namespace/folder.
    *   DI is leveraged for MCP tool dependencies.
    *   Interface names should clearly reflect their purpose (e.g., `IRuleMetadataIndexRepository`).
2.  **Coding Standards**:
    *   Consistent use of `async`/`await` for I/O-bound operations in tools.
