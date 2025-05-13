using AgentRulesHub.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AgentRulesHub.Services
{
    public class RuleInitializationService : IHostedService
    {
        private readonly IRuleLoaderOrchestrator _ruleLoaderOrchestrator;
        private readonly IRuleMetadataIndexRepository _ruleRepository;
        private readonly ILogger<RuleInitializationService> _logger;

        public RuleInitializationService(
            IRuleLoaderOrchestrator ruleLoaderOrchestrator,
            IRuleMetadataIndexRepository ruleRepository,
            ILogger<RuleInitializationService> logger)
        {
            _ruleLoaderOrchestrator = ruleLoaderOrchestrator ?? throw new ArgumentNullException(nameof(ruleLoaderOrchestrator));
            _ruleRepository = ruleRepository ?? throw new ArgumentNullException(nameof(ruleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Rule Initialization Service starting.");

            try
            {
                var loadedRules = await _ruleLoaderOrchestrator.LoadRulesAsync();
                if (loadedRules != null && loadedRules.Any())
                {
                    await _ruleRepository.AddRulesMetadataAsync(loadedRules);
                    _logger.LogInformation($"Successfully loaded {loadedRules.Count()} rules into the repository via background service.");
                }
                else
                {
                    _logger.LogInformation("No rules were loaded by the orchestrator.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during rule initialization.");
                // Depending on the application's requirements, you might want to stop the application
                // or handle this more gracefully. For now, we just log.
            }

            _logger.LogInformation("Rule Initialization Service has completed its startup task.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Rule Initialization Service stopping.");
            return Task.CompletedTask;
        }
    }
}
