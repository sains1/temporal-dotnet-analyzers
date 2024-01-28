# Temporal .NET Analyzers

A collection of roslyn analyzers to catch common mistakes in Temporal workflows.

Most of the analyzers are intended to help catch mistakes that break determinism in workflows. See [Temporal .NET SDK Workflow logic constrants](https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#workflow-logic-constraints) for more detail.

## 1. Installation

```sh
dotnet add package sains1.Temporalio.RosylnAnalyzers --version 0.1.1
```

> To test the analyzers are working, try adding a workflow with a common mistake such as using Task.Delay(1000). After a build you should see a warning highlighting the error.

## 2. Available analyzers

> Note: The analyzers are still a work in progress, see [future development](#-🚧-future-development-🚧-) for more detail.

### TMPRL0001: Do not use .NET timers in workflows

#### Reason:

Timers use the system clock which breaks determinism. To use a timer in a workflow, use the Temporal SDK's Workflow.DelayAsync method.

Example error:

```csharp
[Workflow]
public class TimerWorkflow
{
    [WorkflowRun]
    public async Task RunAsync(string name)
    {
        await Task.Delay(1000);
        Thread.Sleep(1000);
    }
}
```

> Note: see [SDK docs - timers and conditions](https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#timers-and-conditions) for detail on using timers in workflows

### TMPRL0002: Do not use System clock in workflows

#### Reason:

Use of system clock breaks determinism in workflows. To use the current time in a workflow use the Temporal SDK's Workflow.Now method.

Example error:

```csharp
[Workflow]
public class DateTimeNowWorkflow
{
    [WorkflowRun]
    public Task RunAsync(string name)
    {
        _ = DateTime.Now;
        _ = DateTime.UtcNow;
        _ = DateTimeOffset.Now;
        _ = DateTimeOffset.UtcNow;
        return Task.CompletedTask;
    }
}
```

> Note: see [SDK docs - workflow utilities](https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#workflow-utilities) for detail on accessing the current time in workflows

### TMPRL0003: Do not use Guid Generation in workflows

#### Reason:

Use of Guid generation breaks determinism in workflows. To generate a Guid in a workflow use the Temporal SDK's Workflow.NewGuid method.

Example error:

```csharp
[Workflow]
public class GuidNewGuidWorkflow
{
    [WorkflowRun]
    public Task RunAsync(string name)
    {
        _ = Guid.NewGuid();
        return Task.CompletedTask;
    }
}
```

> Note: see [SDK docs - workflow utilities](https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#workflow-utilities) for detail on generating guids in workflows

### TMPRL0004: Queries must not return `Task`, `Task<T>`, or `void`

#### Reason:

Queries should not be async, and should return a value.

Example error:

```csharp
[Workflow]
public class TaskReturnQuery
{
    [WorkflowQuery]
    public void QueryVoid()
    {
        return Task.CompletedTask;
    }

    [WorkflowQuery]
    public Task QueryTask()
    {
        return Task.CompletedTask;
    }

    [WorkflowQuery]
    public Task<string> QueryTaskT()
    {
        return Task.CompletedTask;
    }
}
```

> Note: see [SDK docs - workflows](https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#workflows) for detail on workflow queries

### TMPRL0005: Signals must return Task

#### Reason:

The only valid return type for a signal method is Task.

Example error:

```csharp
[Workflow]
public class StringReturnSignal
{
    [WorkflowSignal]
    public string Signal()
    {
        return "hello";
    }

    [WorkflowSignal]
    public Task<string> Signal()
    {
        return "hello";
    }
}
```

> Note: see [SDK docs - workflows](https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#workflows) for detail on workflow signals

### TMPRL0006 Do not use `ConfigureAwait(false)` in workflows

#### Reason:

Using ConfigureAwait(false) will not use the current context which breaks determinism in workflows.

Example error:

```csharp
[Workflow]
public class ConfigureAwaitFalseWorkflow
{
    [WorkflowRun]
    public async Task RunAsync()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }
}
```

> Note: see [SDK docs - .NET task determinism](https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#net-task-determinism) for details on the temporal task scheduler in workflows

## 3. Configuring Severity

Severity of the analyzers can be configured in the .editorconfig file.

```.editorconfig
# turn off a rule
dotnet_diagnostic.TMPRL0001.severity = none

# set a rule to error
dotnet_diagnostic.TMPRL0001.severity = error
```

## 4. 🚧 Future Development 🚧

Potential future analyzers:

The Temporal .NET SDK describes a number of [logic constraints in workflows](https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#workflow-logic-constraints) that I haven't written analyzers for yet, such as:

- Performing IO (network, disk, stdio, etc)
- Access/alter external mutable state
- Do any threading
- Make any random calls
- Make any not-guaranteed-deterministic calls (e.g. iterating over a dictionary)

Other improvements:

- Suggested codefixes where possible (e.g. Use Workflow.DelayAsync instead of Task.Delay)

## 5. Performance

I haven't yet benchmarked the analyzers, but I followed a few of the [common tips described on the roslyn repo](https://github.com/dotnet/roslyn/issues/25259#issuecomment-376116587) such as:

- use of syntax trees instead of semantic models where possible
- enabled concurrent execution of analyzers

I've also added a root analyzer that performs common filtering steps once and then passes the filtered syntax trees to each of the individual analyzers, although I'm unsure how effective this will be without benchmarking.

## 6. Contributing

[Commit message guide :)](https://gist.github.com/parmentf/035de27d6ed1dce0b36a)

### 6.1 Debugging the analyzers

You can debug the `src/Analyzers` project from your IDE which will use the `src/Analyzers.Samples` Project as its source. Alternatively, debugging a test also works as expected.
