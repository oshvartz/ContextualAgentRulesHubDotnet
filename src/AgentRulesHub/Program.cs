using AgentRulesHub.Interfaces;
using AgentRulesHub.Models;
using AgentRulesHub.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        builder.Services.AddOptions<RuleSourcesOptions>()
          .BindConfiguration(nameof(RuleSourcesOptions.SectionName))
          .ValidateDataAnnotations()
          .ValidateOnStart();

        // Register individual rule loaders
        builder.Services.AddSingleton<IRuleParser, YamlRuleParser>();
        builder.Services.AddSingleton<IRuleLoader, YamlRuleLoader>(); // YamlRuleLoader now implements IRuleLoader

        builder.Services.AddSingleton<IRuleLoaderOrchestrator, RuleLoaderOrchestrator>();

        builder.Services.AddSingleton<IRuleRepository, InMemoryRuleRepository>();

        var host = builder.Build();

        // --- Orchestrator and Repository Usage Example ---
        var orchestrator = host.Services.GetRequiredService<IRuleLoaderOrchestrator>();
        var repository = host.Services.GetRequiredService<IRuleRepository>();

        Console.WriteLine("Loading rules using orchestrator...");
        try
        {
            var loadedRules = await orchestrator.LoadRulesAsync();
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
