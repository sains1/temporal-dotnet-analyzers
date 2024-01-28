using Temporalio.Workflows;

// ReSharper disable once CheckNamespace
[Workflow]
public class StringReturnSignal
{
    [WorkflowSignal]
    public string Signal()
    {
        return "hello";
    }
}