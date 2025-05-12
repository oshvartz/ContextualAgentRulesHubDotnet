using AgentRulesHub.Interfaces;
using AgentRulesHub.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AgentRulesHub.Services;

public class RuleLoaderOrchestrator : IRuleLoaderOrchestrator
{
    private readonly IEnumerable<IRuleLoader> _loaders;
    private readonly RuleSourcesOptions _ruleSourcesOptions;

    public RuleLoaderOrchestrator(IEnumerable<IRuleLoader> loaders,IOptions<RuleSourcesOptions> ruleSourcesOptions)
    {
        _loaders = loaders ?? throw new ArgumentNullException(nameof(loaders));

        // Get rule source configurations from the bound object
        List<RuleSourceOptions>? ruleSourceOptionsList = ruleSourcesOptions.Value?.Sources ?? new List<RuleSourceOptions>();

        if (!ruleSourceOptionsList.Any())
        {
           throw new InvalidOperationException ("No rule sources configured in appsettings.json. Exiting.");
        }
        _ruleSourcesOptions = ruleSourcesOptions.Value!;
    }

    public async Task<IEnumerable<AgentRule>> LoadRulesAsync(CancellationToken cancellationToken = default)
    {

        if (_ruleSourcesOptions == null)
        {
            throw new ArgumentNullException(nameof(_ruleSourcesOptions));
        }

        var allRules = new List<AgentRule>();

        foreach (var options in _ruleSourcesOptions.Sources)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var loader = _loaders.FirstOrDefault(l => l.CanHandle(options.LoaderType));

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
