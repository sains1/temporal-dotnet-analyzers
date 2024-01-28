using System;
using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class DateTimeNowWorkflow
{
    [WorkflowRun]
    public Task RunAsync(string name)
    {
        _ = DateTime.Now;
        return Task.CompletedTask;
    }
}