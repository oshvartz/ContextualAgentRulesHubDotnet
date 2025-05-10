using AgentRulesHub.Interfaces;
using AgentRulesHub.Models;

namespace AgentRulesHub.Services;

public class YamlRuleLoader : IRuleLoader
{
    private readonly IRuleParser _ruleParser;

    public YamlRuleLoader(IRuleParser ruleParser)
    {
        _ruleParser = ruleParser;
    }

    public async Task<IEnumerable<AgentRule>> LoadRulesAsync(string folderPath, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {folderPath}");
        }

        var yamlFiles = Directory.GetFiles(folderPath, "*.yaml", SearchOption.TopDirectoryOnly);
        var rules = new List<AgentRule>();

        foreach (var file in yamlFiles)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                var rule = await _ruleParser.ParseRuleAsync(file, cancellationToken);
                rules.Add(rule);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing rule file {file}: {ex.Message}");
                // Continue with next file instead of failing the entire operation
                continue;
            }
        }

        return rules;
    }
}
