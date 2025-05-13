using AgentRulesHub.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgentRulesHub.Interfaces;

public interface IRuleMetadataIndexRepository
{
    Task AddRuleMetadataAsync(AgentRule rule, CancellationToken cancellationToken = default);
    Task AddRulesMetadataAsync(IEnumerable<AgentRule> rules, CancellationToken cancellationToken = default);
    Task<AgentRule?> GetRuleMetadataByIdAsync(string ruleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgentRule>> GetAllRulesMetadataAsync(CancellationToken cancellationToken = default);
    // Consider adding FindRulesAsync(Expression<Func<AgentRule, bool>> predicate, CancellationToken cancellationToken = default);
    // For more complex querying in the future.
}
