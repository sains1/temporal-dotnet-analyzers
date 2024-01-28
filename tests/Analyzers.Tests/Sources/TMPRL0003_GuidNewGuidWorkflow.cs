using System;
using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class GuidNewGuidWorkflow
{
    [WorkflowRun]
    public Task RunAsync(string name)
    {
        _ = Guid.NewGuid();
        return Task.CompletedTask;
    }
}