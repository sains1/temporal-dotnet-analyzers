using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class TaskDelayWorkflow
{
    [WorkflowRun]
    public async Task RunAsync(string name)
    {
        await Task.Delay(1000);
    }
}