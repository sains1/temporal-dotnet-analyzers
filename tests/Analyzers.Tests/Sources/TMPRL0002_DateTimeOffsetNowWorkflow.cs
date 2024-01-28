using System;
using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class DateTimeOffsetNowWorkflow
{
    [WorkflowRun]
    public Task RunAsync(string name)
    {
        _ = DateTimeOffset.Now;
        return Task.CompletedTask;
    }
}