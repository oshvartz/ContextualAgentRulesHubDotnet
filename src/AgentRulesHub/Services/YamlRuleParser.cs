using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using AgentRulesHub.Interfaces;
using AgentRulesHub.Models;

namespace AgentRulesHub.Services;

public class YamlRuleParser : IRuleParser
{
    public async Task<AgentRule> ParseRuleAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        using var reader = new StreamReader(filePath);
        var content = await reader.ReadToEndAsync(cancellationToken);
        var yamlContent = deserializer.Deserialize<YamlRuleContent>(content);

        var rule = new AgentRule
        {
            RuleId = yamlContent.Id,
            Description = yamlContent.Description,
            Language = yamlContent.Language,
            Tags = yamlContent.Tags ?? new List<string>(),
            Source = new FileSource { FilePath = filePath }
        };

        return rule;
    }
}
