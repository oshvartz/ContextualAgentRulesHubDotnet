using AgentRulesHub.Models;

namespace AgentRulesHub.Interfaces;

public interface IRuleParser
{
    Task<AgentRule> ParseRuleAsync(string filePath, CancellationToken cancellationToken = default);
}
