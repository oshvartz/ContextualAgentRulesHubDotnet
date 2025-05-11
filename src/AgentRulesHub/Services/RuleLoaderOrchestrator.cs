using AgentRulesHub.Interfaces;
using AgentRulesHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AgentRulesHub.Services;

public class RuleLoaderOrchestrator : IRuleLoaderOrchestrator
{
    private readonly IEnumerable<IRuleLoader> _loaders;

    public RuleLoaderOrchestrator(IEnumerable<IRuleLoader> loaders)
    {
        _loaders = loaders ?? throw new ArgumentNullException(nameof(loaders));
    }

    public async Task<IEnumerable<AgentRule>> LoadRulesAsync(IEnumerable<RuleSourceOptions> sourcesOptions, CancellationToken cancellationToken = default)
    {
        if (sourcesOptions == null)
        {
            throw new ArgumentNullException(nameof(sourcesOptions));
        }

        var allRules = new List<AgentRule>();

        foreach (var options in sourcesOptions)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var loader = _loaders.FirstOrDefault(l => l.LoaderType.Equals(options.LoaderType, StringComparison.OrdinalIgnoreCase));

            if (loader == null)
            {
                Console.WriteLine($"No loader found for type: {options.LoaderType}. Skipping.");
                // Or throw an exception, depending on desired behavior
                // throw new InvalidOperationException($"No loader found for type: {options.LoaderType}");
                continue;
            }

            try
            {
                var rulesFromSource = await loader.LoadRulesAsync(options, cancellationToken);
                allRules.AddRange(rulesFromSource);
            }
            catch (Exception ex)
            {
                // Log the exception and continue with other sources
                Console.WriteLine($"Error loading rules from source with type {options.LoaderType}: {ex.Message}");
                // Depending on requirements, might re-throw or handle more gracefully
                continue;
            }
        }

        return allRules;
    }
}
