using AgentRulesHub.Models;

namespace AgentRulesHub.Interfaces;

public interface IRuleLoader
{
    Task<IEnumerable<AgentRule>> LoadRulesAsync(string folderPath, CancellationToken cancellationToken = default);
}
