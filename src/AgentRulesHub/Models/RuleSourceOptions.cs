using System.Collections.Generic;

namespace AgentRulesHub.Models;

public class RuleSourceOptions
{
    public string LoaderType { get; set; } = string.Empty; // e.g., "YamlFile", "Database"
    public Dictionary<string, object> Settings { get; set; } = new(); // Loader-specific settings, e.g., "Path" for FileLoader
}
