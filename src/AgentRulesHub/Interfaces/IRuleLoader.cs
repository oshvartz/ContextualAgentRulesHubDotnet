using AgentRulesHub.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgentRulesHub.Interfaces;

public interface IRuleLoader
{
    string LoaderType { get; } // Used by the orchestrator to select the correct loader
    bool CanHandle(string loaderType);
    Task<IEnumerable<AgentRule>> LoadRulesAsync(RuleSourceOptions options, CancellationToken cancellationToken = default);
}
