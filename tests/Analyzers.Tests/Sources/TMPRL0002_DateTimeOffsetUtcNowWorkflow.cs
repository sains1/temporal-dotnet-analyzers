using System;
using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class DateTimeOffsetUtcNowWorkflow
{
    [WorkflowRun]
    public Task RunAsync(string name)
    {
        _ = DateTimeOffset.UtcNow;
        return Task.CompletedTask;
    }
}