using AgentRulesHub.Interfaces;
using AgentRulesHub.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AgentRulesHub.Services;

public class InMemoryRuleRepository : IRuleRepository
{
    private readonly ConcurrentDictionary<string, AgentRule> _rules = new();

    public Task AddRuleAsync(AgentRule rule, CancellationToken cancellationToken = default)
    {
        if (rule == null)
        {
            throw new ArgumentNullException(nameof(rule));
        }
        if (string.IsNullOrWhiteSpace(rule.RuleId))
        {
            throw new ArgumentException("RuleId cannot be null or whitespace.", nameof(rule));
        }

        _rules[rule.RuleId] = rule;
        return Task.CompletedTask;
    }

    public Task AddRulesAsync(IEnumerable<AgentRule> rules, CancellationToken cancellationToken = default)
    {
        if (rules == null)
        {
            throw new ArgumentNullException(nameof(rules));
        }

        foreach (var rule in rules)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            if (rule == null)
            {
                // Log or decide how to handle null rules in a collection
                Console.WriteLine("Encountered a null rule in the collection. Skipping.");
                continue;
            }
            if (string.IsNullOrWhiteSpace(rule.RuleId))
            {
                // Log or decide how to handle rules with invalid RuleId
                Console.WriteLine($"Encountered a rule with invalid RuleId. Description: {rule.Description}. Skipping.");
                continue;
            }
            _rules[rule.RuleId] = rule;
        }
        return Task.CompletedTask;
    }

    public Task<AgentRule?> GetRuleByIdAsync(string ruleId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ruleId))
        {
            // Consider throwing ArgumentException or returning null based on desired contract
            return Task.FromResult<AgentRule?>(null);
        }
        _rules.TryGetValue(ruleId, out var rule);
        return Task.FromResult(rule);
    }

    public Task<IEnumerable<AgentRule>> GetAllRulesAsync(CancellationToken cancellationToken = default)
    {
        // Returning a snapshot to prevent modification of the internal collection
        return Task.FromResult(_rules.Values.ToList().AsEnumerable());
    }
}
