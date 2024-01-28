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

        _ = Guid.NewGuid();

        return result;
    }

    [WorkflowQuery]
    public void QueryVoid()
    {
    }

    [WorkflowQuery]
    public Task QueryTask()
    {
        return Task.CompletedTask;
    }

    [WorkflowQuery]
    public Task<string> QueryTaskT()
    {
        return Task.FromResult("hello");
    }

    [WorkflowSignal]
    public void SignalVoid()
    {
    }

    [WorkflowSignal]
    public Task<string> SignalTaskT()
    {
        return Task.FromResult("hello");
    }

    [WorkflowSignal]
    public string SignalString()
    {
        return "hello";
    }
}

public class SampleActivities
{
    public Task<string> SayHello(string name)
    {
        return Task.FromResult($"Hello, {name}!");
    }
}