using System.Dynamic;
using RulesEngine.Extensions;
using RulesEngine.Models;

namespace rulesenginedemo.Components.common;

public class RulesEngineUtility
{
    public void evaluateRulesBasicExample(int counter)
    {
        
        dynamic data = new ExpandoObject();
        data.count = counter;
        var inputs = new dynamic[]
        {
            data
        };

        var rules = new List<Rule>();

        var rule = new Rule
        {
            RuleName = "Test Rule",
            SuccessEvent = "Count is within tolerance.",
            ErrorMessage = "Over expected.",
            Expression = "count < 3",
            RuleExpressionType = RuleExpressionType.LambdaExpression
        };
        
        rules.Add(rule);

        var workflows = new List<Workflow>();

        var exampleWorkflow = new Workflow
        {
            WorkflowName = "Example Workflow",
            Rules = rules
        };

        workflows.Add(exampleWorkflow);

        var bre = new RulesEngine.RulesEngine(workflows.ToArray());
        
        foreach (var workflow in workflows)
        {
            var resultList = bre.ExecuteAllRulesAsync(workflow.WorkflowName, inputs).Result;
            
            bool outcome = false;

            //Different ways to show test results:
            outcome = resultList.TrueForAll(r => r.IsSuccess);

            resultList.OnSuccess((eventName) => {
                Console.WriteLine($"Result '{eventName}' is as expected.");
                outcome = true;
            });

            resultList.OnFail(() => {
                outcome = false;
            });

            Console.WriteLine($"Test outcome: {outcome}.");
        }
    }
}