namespace AgentRulesHub.Models;

public abstract class RuleSource
{
    public abstract string SourceType { get; }
    public abstract Task<string> GetRuleContentAsync(CancellationToken cancellationToken);
}
