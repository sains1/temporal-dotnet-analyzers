using System;
using System.Threading;
using System.Threading.Tasks;

using Temporalio.Workflows;

namespace Analyzers.Sample;

[Workflow]
public class BadWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync(string name)
    {
        var result = await Workflow.ExecuteActivityAsync((SampleActivities act) => act.SayHello(name),
            new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(1) });

        await Task.Delay(100);

        Thread.Sleep(1000);

        _ = DateTimeOffset.Now;
        _ = DateTimeOffset.UtcNow;
        _ = DateTime.Now;
        _ = DateTime.UtcNow;

        return result;
    }
}

public class SampleActivities
{
    public Task<string> SayHello(string name)
    {
        return Task.FromResult($"Hello, {name}!");
    }
}