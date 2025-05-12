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
        [McpServerTool, Description("Gets rule metadata for a specific language.")]
        public static async Task<IEnumerable<AgentRule>> GetRulesByLanguageAsync(
            string language,
            IRuleRepository ruleRepository)
        {
            var allRules = await ruleRepository.GetAllRulesAsync();
            return allRules.Where(rule => 
                !string.IsNullOrEmpty(rule.Language) && 
                rule.Language.Equals(language, System.StringComparison.OrdinalIgnoreCase));
        }

        [McpServerTool, Description("Gets the content of a specific rule by its ID.")]
        public static async Task<string?> GetRuleContentByIdAsync(
            string ruleId,
            IRuleRepository ruleRepository)
        {
            var rule = await ruleRepository.GetRuleByIdAsync(ruleId);
            if (rule?.Source != null)
            {
                return await rule.Source.GetRuleContentAsync(default);
            }
            return null;
        }
    }
}
