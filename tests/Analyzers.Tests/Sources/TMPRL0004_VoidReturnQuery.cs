using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class VoidReturnQuery
{
    [WorkflowQuery]
    public void QueryVoid()
    {
    }
}