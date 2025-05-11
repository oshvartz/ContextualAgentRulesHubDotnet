using AgentRulesHub.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgentRulesHub.Interfaces;

public interface IRuleRepository
{
    Task AddRuleAsync(AgentRule rule, CancellationToken cancellationToken = default);
    Task AddRulesAsync(IEnumerable<AgentRule> rules, CancellationToken cancellationToken = default);
    Task<AgentRule?> GetRuleByIdAsync(string ruleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgentRule>> GetAllRulesAsync(CancellationToken cancellationToken = default);
    // Consider adding FindRulesAsync(Expression<Func<AgentRule, bool>> predicate, CancellationToken cancellationToken = default);
    // For more complex querying in the future.
}
