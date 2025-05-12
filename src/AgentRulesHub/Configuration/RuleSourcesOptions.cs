using System.Collections.Generic;
using AgentRulesHub.Configuration; // Added for RuleSourceOptions

namespace AgentRulesHub.Configuration;

public class RuleSourcesOptions
{
    public const string SectionName = "RuleSources";
    required public List<RuleSourceOptions> Sources { get; set; }
}
