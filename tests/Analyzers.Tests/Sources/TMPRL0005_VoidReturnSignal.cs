using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class VoidReturnSignal
{
    [WorkflowSignal]
    public void Signal()
    {
    }
}