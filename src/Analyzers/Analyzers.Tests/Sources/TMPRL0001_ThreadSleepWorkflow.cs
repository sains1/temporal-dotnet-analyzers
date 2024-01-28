using System.Threading;
using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class ThreadSleepWorkflow
{
    [WorkflowRun]
    public Task RunAsync(string name)
    {
        Thread.Sleep(1000);
        return Task.CompletedTask;
    }
}