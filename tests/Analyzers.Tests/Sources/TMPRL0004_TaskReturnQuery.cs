using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class TaskReturnQuery
{
    [WorkflowQuery]
    public Task QueryTask()
    {
        return Task.CompletedTask;
    }
}