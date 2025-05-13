using AgentRulesHub.Interfaces;
using AgentRulesHub.Models;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AgentRulesHub.Mcp
{
    [McpServerToolType]
    public static class RuleProviderTools
    {
        [McpServerTool, Description("Gets the content of a specific rule by its ID.")]
        public static async Task<string?> GetRuleContentByIdAsync(
            string ruleId,
            IRuleMetadataIndexRepository ruleRepository)
        {
            var rule = await ruleRepository.GetRuleMetadataByIdAsync(ruleId);
            if (rule?.Source != null)
            {
                return await rule.Source.GetRuleContentAsync(default);
            }
            return null;
        }

        [McpServerTool, Description("Gets the metadata for all rules in the index.")]
        public static async Task<IEnumerable<AgentRule>> GetAllRulesMetadataAsync(
            IRuleMetadataIndexRepository ruleRepository)
        {
            return await ruleRepository.GetAllRulesMetadataAsync();
        }
    }
}
