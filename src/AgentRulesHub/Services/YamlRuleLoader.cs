using AgentRulesHub.Interfaces;
using AgentRulesHub.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AgentRulesHub.Services;

public class YamlRuleLoader : IRuleLoader
{
    private readonly IRuleParser _ruleParser;
    public string LoaderType => "YamlFile";

    public YamlRuleLoader(IRuleParser ruleParser)
    {
        _ruleParser = ruleParser ?? throw new ArgumentNullException(nameof(ruleParser));
    }

    public bool CanHandle(string loaderType)
    {
        return !string.IsNullOrWhiteSpace(loaderType) && loaderType.Equals(LoaderType, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<IEnumerable<AgentRule>> LoadRulesAsync(RuleSourceOptions options, CancellationToken cancellationToken = default)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (!options.Settings.TryGetValue("Path", out var pathObject) || pathObject is not string folderPath || string.IsNullOrWhiteSpace(folderPath))
        {
            throw new ArgumentException("Path setting is missing, not a string, or empty in RuleSourceOptions for YamlFile loader.", nameof(options));
        }

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
