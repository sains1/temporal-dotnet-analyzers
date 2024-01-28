using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class ConfigureAwaitFalseWorkflow
{
    [WorkflowRun]
    public async Task RunAsync()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }
}