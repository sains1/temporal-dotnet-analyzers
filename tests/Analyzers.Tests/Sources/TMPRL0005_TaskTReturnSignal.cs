using System.Threading.Tasks;

using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class TaskTReturnSignal
{
    [WorkflowSignal]
    public Task<string> Signal()
    {
        return Task.FromResult("hello");
    }
}