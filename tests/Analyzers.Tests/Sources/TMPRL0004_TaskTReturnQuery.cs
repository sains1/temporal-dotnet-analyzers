using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class TaskTReturnQuery
{
    [WorkflowQuery]
    public Task<string> QueryTask()
    {
        return Task.FromResult("hello");
    }
}