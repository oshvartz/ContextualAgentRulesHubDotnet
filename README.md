# AgentRulesHub MCP Server

AgentRulesHub is an MCP (Model Context Protocol) server designed to manage and provide contextual rules for AI agents. It allows agents to dynamically retrieve rules based on various criteria, such as programming language or specific rule identifiers.

## Features

-   **Dynamic Rule Retrieval**: Access rules based on language or ID.
-   **YAML-based Rule Storage**: Rules are defined in YAML files.
-   **Configurable Rule Sources**: Specify locations for rule files via configuration.
-   **MCP Integration**: Exposes functionality as tools consumable by MCP clients.

## Configuration

To use the AgentRulesHub MCP server, you need to configure it in your MCP client's settings file (e.g., `cline_mcp_settings.json` for the Cline VS Code extension).

Add the following configuration block to your MCP settings:

```json
{
  "mcp_servers": {
    "rules-hub": {
      "autoApprove": [],
      "disabled": false,
      "timeout": 300,
      "command": "dotnet",
      "args": [
        "C:/git/AgentRulesHub/src/AgentRulesHub/bin/Release/net8.0/AgentRulesHub.dll"
      ],
      "env": {
        "RuleSources:Sources:0:LoaderType": "YamlFile",
        "RuleSources:Sources:0:Settings:Path": "c:\\git\\AgentRules"
      },
      "transportType": "stdio"
    }
  }
}
```

**Explanation of Configuration Fields:**

*   `"rules-hub"`: A unique name for this MCP server instance.
*   `"command"`: The executable to run the server. For this .NET project, it's `dotnet`.
*   `"args"`: Arguments passed to the command. The primary argument is the path to the `AgentRulesHub.dll`.
    *   **Important**: Ensure the path `C:/git/AgentRulesHub/src/AgentRulesHub/bin/Release/net8.0/AgentRulesHub.dll` correctly points to the compiled DLL on your system.
*   `"env"`: Environment variables for the server process.
    *   `"RuleSources:Sources:0:LoaderType"`: Specifies the type of rule loader. `YamlFile` indicates rules are loaded from YAML files.
    *   `"RuleSources:Sources:0:Settings:Path"`: The directory path where the YAML rule files are located.
        *   **Important**: Ensure the path `c:\\git\\AgentRules` correctly points to your rule definitions directory.
*   `"transportType"`: The communication protocol. `stdio` is used for local IPC.

## Usage Examples

Once configured, you can use MCP tools to interact with the AgentRulesHub server.

### 1. Get Rule Content by ID

This tool retrieves the content of a specific rule using its ID.

**Tool Name**: `GetRuleContentById`
**Server Name**: `rules-hub` (or the name you configured)

**Example MCP Tool Call (conceptual):**

```json
{
  "server_name": "rules-hub",
  "tool_name": "GetRuleContentById",
  "arguments": {
    "ruleId": "my-sample-rule"
  }
}
```

This would return the content of the rule identified by `my-sample-rule`.

### 2. Get Rules by Language

This tool retrieves metadata for all rules associated with a specific programming language.

**Tool Name**: `GetRulesByLanguage`
**Server Name**: `rules-hub` (or the name you configured)

**Example MCP Tool Call (conceptual):**

```json
{
  "server_name": "rules-hub",
  "tool_name": "GetRulesByLanguage",
  "arguments": {
    "language": "csharp"
  }
}
```

This would return a list of rule metadata (ID, description, tags, etc.) for all rules tagged with the "csharp" language.

## Development

### Prerequisites
- .NET 8.0 SDK or higher

### Building the Server
1. Navigate to the `src/AgentRulesHub` directory.
2. Run `dotnet build -c Release`.

This will produce the `AgentRulesHub.dll` in the `src/AgentRulesHub/bin/Release/net8.0/` directory.

### Rule File Format
Rules are defined in YAML files (e.g., `my-rule.yaml`) within the directory specified by `RuleSources:Sources:0:Settings:Path`.

Example `sample-rule.yaml`:
```yaml
RuleId: sample-rule
Description: A sample rule for demonstration.
Language: csharp
Tags:
  - example
  - demo
Content: |
  // This is the content of the sample C# rule.
  public class SampleRuleClass {
      public void DoSomething() {
          Console.WriteLine("Executing sample rule.");
      }
  }
