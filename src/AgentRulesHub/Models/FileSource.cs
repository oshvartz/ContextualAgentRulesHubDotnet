using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AgentRulesHub.Models;

public class FileSource : RuleSource
{
    public override string SourceType { get; } = "File"; 
    public string FilePath { get; set; } = string.Empty;

    // Example YAML file structure:
    /*
    # rule.yaml
    rule: |
        // Your rule content here
        function validateInput(input) {
            if (!input) throw new Error("Input required");
            return input.trim();
        }
    description: Validates user input by checking for null/empty and trimming whitespace
    language: javascript
    tags:
        - validation
        - input-handling
        - security
    */
    public override Task<string> GetRuleContentAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(FilePath))
            throw new InvalidOperationException("FilePath is not set");

        using var fileStream = File.OpenText(FilePath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var yamlContent = deserializer.Deserialize<Dictionary<string,string>>(fileStream);
        return Task.FromResult(yamlContent["rule"]);
    }
}
