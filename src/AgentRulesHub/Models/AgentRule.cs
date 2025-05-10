namespace AgentRulesHub.Models;

public class AgentRule
{
    required public string RuleId { get; set; }
    required public string Description { get; set; }
    public string? Language { get; set; }
    public List<string> Tags { get; set; } = new();
    public RuleSource Source { get; set; } = null!;
}
