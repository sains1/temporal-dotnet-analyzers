# Temporal .NET Analyzers

Available analyzers:

-

[Workflow constraints](https://github.com/temporalio/sdk-dotnet?tab=readme-ov-file#workflow-logic-constraints)

- Perform IO (network, disk, stdio, etc)
- Access/alter external mutable state
- Do any threading
- Do anything using the system clock (e.g. DateTime.Now)
- This includes .NET timers (e.g. Task.Delay or Thread.Sleep)
- Make any random calls
- Make any not-guaranteed-deterministic calls (e.g. iterating over a dictionary)

## TODO

- Do not use Task.Run
  - Use Task.Factory.StartNew or instantiate the Task and run Task.Start on it.
- Do not use Task.ConfigureAwait(false)
- Do not use Task.Delay, Task.Wait, timeout-based CancellationTokenSource
  - Workflow.DelayAsync, Workflow.WaitConditionAsync, or non-timeout-based cancellation token source is suggested
- Do not use Task.WhenAny
  - Use Workflow.WhenAnyAsync instead
- suggested code fixes
  - delays to use wf delay
  - guid to use wf new guid
- workflow constraints
  - run method on exactly one method
  - run method returns task or taskT
  - query methods cannot be async, and must not be void
  - update method returns task or taskT

Prior Art:

-

## Contributing

Commit message guide - https://gist.github.com/parmentf/035de27d6ed1dce0b36a
