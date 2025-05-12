using AgentRulesHub.Interfaces;
using AgentRulesHub.Configuration;
using AgentRulesHub.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging; // For MCP logging
using ModelContextProtocol.Server; // For MCP
using System.IO;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args); // Pass args

        // Configure MCP logging (as per example)
        builder.Logging.ClearProviders(); // Optional: clear existing providers if any
        builder.Logging.AddConsole(consoleLogOptions =>
        {
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        // Existing AgentRulesHub Configuration and Services
        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
        // Ensure appsettings.json is added if not already implicitly by CreateApplicationBuilder
        // builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        builder.Services.AddOptions<RuleSourcesOptions>()
          .Bind(builder.Configuration.GetSection(RuleSourcesOptions.SectionName))
          .ValidateDataAnnotations()
          .ValidateOnStart();

        builder.Services.AddSingleton<IRuleParser, YamlRuleParser>();
        builder.Services.AddSingleton<IRuleLoader, YamlRuleLoader>();
        builder.Services.AddSingleton<IRuleLoaderOrchestrator, RuleLoaderOrchestrator>();
        builder.Services.AddSingleton<IRuleRepository, InMemoryRuleRepository>();
        builder.Services.AddHostedService<RuleInitializationService>();

        // Add MCP Server services
        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly(); // This will discover RuleProviderTools

        // Build and run the host
        await builder.Build().RunAsync();
    }
}
