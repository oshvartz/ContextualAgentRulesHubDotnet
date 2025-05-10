using YamlDotNet.Serialization;

namespace AgentRulesHub.Models;

public class YamlRuleContent
{
    [YamlMember(Alias = "id")]
    public string Id { get; set; } = string.Empty;

    [YamlMember(Alias = "description")]
    public string Description { get; set; } = string.Empty;

    [YamlMember(Alias = "language")]
    public string? Language { get; set; }

    [YamlMember(Alias = "tags")]
    public List<string>? Tags { get; set; }

    [YamlMember(Alias = "rule")]
    public string Rule { get; set; } = string.Empty;
}
