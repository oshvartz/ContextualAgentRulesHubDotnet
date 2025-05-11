using AgentRulesHub.Interfaces;
using AgentRulesHub.Models;
using AgentRulesHub.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Setup dependency injection
        var services = new ServiceCollection();

        // Register individual rule loaders
        services.AddSingleton<IRuleParser, YamlRuleParser>();
        services.AddSingleton<IRuleLoader, YamlRuleLoader>(); // YamlRuleLoader now implements IRuleLoader

        // Register all IRuleLoader implementations for the orchestrator
        // This allows the orchestrator to discover all available loaders.
        // For this example, we only have YamlRuleLoader.
        services.AddSingleton<IRuleLoaderOrchestrator>(sp =>
        {
            var allLoaders = sp.GetServices<IRuleLoader>();
            return new RuleLoaderOrchestrator(allLoaders);
        });

        services.AddSingleton<IRuleRepository, InMemoryRuleRepository>();

        var serviceProvider = services.BuildServiceProvider();

        // --- Orchestrator and Repository Usage Example ---
        var orchestrator = serviceProvider.GetRequiredService<IRuleLoaderOrchestrator>();
        var repository = serviceProvider.GetRequiredService<IRuleRepository>();

        // Define rule source configurations
        var ruleSources = new List<RuleSourceOptions>
        {
            new RuleSourceOptions
            {
                LoaderType = "YamlFile", // This must match YamlRuleLoader.LoaderType
                Settings = new Dictionary<string, object>
                {
                    { "Path", "C:\\git\\AgentRules" } // Relative path to the rules folder
                }
            }
            // Add other RuleSourceOptions here for different loaders or paths
        };

        Console.WriteLine("Loading rules using orchestrator...");
        try
        {
            var loadedRules = await orchestrator.LoadRulesAsync(ruleSources);
            await repository.AddRulesAsync(loadedRules);

            Console.WriteLine($"Successfully loaded {loadedRules.Count()} rules into the repository.");
            Console.WriteLine(new string('-', 50));

            // Retrieve and display all rules from repository
            var allRulesFromRepo = await repository.GetAllRulesAsync();
            Console.WriteLine($"Total rules in repository: {allRulesFromRepo.Count()}");
            foreach (var rule in allRulesFromRepo)
            {
                Console.WriteLine($"Rule ID: {rule.RuleId}, Description: {rule.Description}, Language: {rule.Language ?? "N/A"}");
                if (rule.Source is FileSource fileSource)
                {
                    Console.WriteLine($"  Source File: {fileSource.FilePath}");
                }
            }
            Console.WriteLine(new string('-', 50));

            // Example: Get a specific rule and its content
            string testRuleId = "csharp-standards-rule";
            var specificRule = await repository.GetRuleByIdAsync(testRuleId);
            if (specificRule != null)
            {
                Console.WriteLine($"Found rule '{testRuleId}': {specificRule.Description}");
                Console.WriteLine("Attempting to load its content...");
                try
                {
                    string ruleContent = await specificRule.Source.GetRuleContentAsync(default);
                    Console.WriteLine($"--- Rule Content for {testRuleId} ---");
                    Console.WriteLine(ruleContent);
                    Console.WriteLine($"--- End of Rule Content for {testRuleId} ---");
                }
                catch (Exception contentEx)
                {
                    Console.WriteLine($"Error loading content for rule '{testRuleId}': {contentEx.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Rule with ID '{testRuleId}' not found in the repository.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
