using System.Collections.Generic;

namespace AgentRulesHub.Models
{
    public class RuleSourcesOptions
    {
        public const string SectionName = "RuleSources";
        public List<RuleSourceOptions> Sources { get; set; } = [];
    }
}
