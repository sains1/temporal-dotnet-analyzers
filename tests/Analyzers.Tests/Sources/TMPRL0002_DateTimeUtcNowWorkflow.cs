using System;
using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class DateTimeUtcNowWorkflow
{
    [WorkflowRun]
    public Task RunAsync(string name)
    {
        _ = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}