using AgentRulesHub.Interfaces;
using AgentRulesHub.Models;
using AgentRulesHub.Configuration; // Added for RuleSourcesOptions
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

        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
        builder.Services.AddOptions<RuleSourcesOptions>()
          .Bind(builder.Configuration.GetSection(RuleSourcesOptions.SectionName))
          .ValidateDataAnnotations()
          .ValidateOnStart();

        // Register individual rule loaders
        builder.Services.AddSingleton<IRuleParser, YamlRuleParser>();
        builder.Services.AddSingleton<IRuleLoader, YamlRuleLoader>(); // YamlRuleLoader now implements IRuleLoader

        builder.Services.AddSingleton<IRuleLoaderOrchestrator, RuleLoaderOrchestrator>();

        builder.Services.AddSingleton<IRuleRepository, InMemoryRuleRepository>();

        // Register the background service for rule initialization
        builder.Services.AddHostedService<RuleInitializationService>();

        var host = builder.Build();

        // Start the host, which will run the IHostedService instances
        await host.StartAsync();

        // --- Application logic/demonstration after services have started ---
        // The RuleInitializationService should have loaded the rules by now.
        Console.WriteLine("Application started. Rule initialization service should have run.");
        Console.WriteLine("Attempting to retrieve rules from repository...");
        Console.WriteLine(new string('-', 50));

        try
        {
            var repository = host.Services.GetRequiredService<IRuleRepository>();

            // Retrieve and display all rules from repository
            var allRulesFromRepo = await repository.GetAllRulesAsync();
            if (allRulesFromRepo != null && allRulesFromRepo.Any())
            {
                Console.WriteLine($"Total rules in repository: {allRulesFromRepo.Count()}");
                foreach (var rule in allRulesFromRepo)
                {
                    Console.WriteLine($"Rule ID: {rule.RuleId}, Description: {rule.Description}, Language: {rule.Language ?? "N/A"}");
                if (rule.Source is FileSource fileSource)
                {
                    Console.WriteLine($"  Source File: {fileSource.FilePath}");
                }
            }
            }
            else
            {
                Console.WriteLine("No rules found in the repository. Initialization might have failed or found no rules.");
            }
            Console.WriteLine(new string('-', 50));

            // Example: Get a specific rule and its content
            string testRuleId = "csharp-standards-rule"; // Assuming this rule ID exists from your sample data
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
            Console.WriteLine($"An error occurred while demonstrating repository access: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            // Stop the host
            await host.StopAsync();
            Console.WriteLine("Application stopped.");
        }
    }
}
