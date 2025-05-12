using AgentRulesHub.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgentRulesHub.Interfaces;

public interface IRuleLoaderOrchestrator
{
    Task<IEnumerable<AgentRule>> LoadRulesAsync(CancellationToken cancellationToken = default);
}
