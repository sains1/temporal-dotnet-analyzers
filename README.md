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

Others:

- Single run method?

Prior Art:

-

## Contributing

Commit message guide - https://gist.github.com/parmentf/035de27d6ed1dce0b36a
