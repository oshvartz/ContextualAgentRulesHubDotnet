using Microsoft.Extensions.DependencyInjection;
using AgentRulesHub.Interfaces;
using AgentRulesHub.Services;

// Setup dependency injection
var services = new ServiceCollection();
services.AddScoped<IRuleLoader, YamlRuleLoader>();
services.AddScoped<IRuleParser, YamlRuleParser>();

var serviceProvider = services.BuildServiceProvider();

// Get the rule loader service
var ruleLoader = serviceProvider.GetRequiredService<IRuleLoader>();

try
{
    // Load rules from the specified directory
    var rules = await ruleLoader.LoadRulesAsync("C:\\git\\AgentRules");

    // Display the loaded rules
    foreach (var rule in rules)
    {
        Console.WriteLine($"Rule: {rule.RuleId}");
        Console.WriteLine($"Description: {rule.Description}");
        Console.WriteLine($"Language: {rule.Language}");
        Console.WriteLine($"Tags: {string.Join(", ", rule.Tags)}");
        Console.WriteLine(new string('-', 50));
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
