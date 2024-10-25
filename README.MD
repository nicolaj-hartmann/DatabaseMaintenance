# DatabaseMaintenance

[![NuGet Package](https://img.shields.io/nuget/v/DatabaseMaintenance.svg)](https://www.nuget.org/packages/DatabaseMaintenance)
[![codecov](https://codecov.io/github/nicolaj-hartmann/DatabaseMaintenance/graph/badge.svg?token=G468V9HDZW)](https://codecov.io/github/nicolaj-hartmann/DatabaseMaintenance)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fnicolaj-hartmann%2FDatabaseMaintenance.svg?type=shield&issueType=license)](https://app.fossa.com/projects/git%2Bgithub.com%2Fnicolaj-hartmann%2FDatabaseMaintenance?ref=badge_shield&issueType=license)

DatabaseMaintenance is a .NET library designed for managing and optimizing database maintenance tasks, including index optimization and script execution. This library provides a flexible interface for configuring and running various maintenance operations with ease.

## Kudos

A special thanks to [Ola Hallengren](https://ola.hallengren.com) for his exceptional work on SQL Server maintenance solutions. His scripts serve as the foundation for this library, which aims to simplify and enhance database maintenance tasks.

## Why This Library?

In modern applications, maintaining database performance is crucial. As databases grow and evolve, regular maintenance tasks such as index optimization become necessary to ensure efficient data access and query performance. This library encapsulates common maintenance tasks, providing developers with an easy-to-use interface to manage these processes programmatically.

## Quick Start Guide

1. **Installation**

   You can install the `DatabaseMaintenance` library via NuGet Package Manager:

   ```bash
   dotnet add package DatabaseMaintenance
   ```

2. **Usage**

   Here’s a basic example of how to use the DatabaseClient and IndexOptions classes to perform index optimization:

   ```csharp
   using DatabaseMaintenance;

    var options = new IndexOptions
    {
    FragmentationLevel2 = 50, // Customize your options here
    Databases = "YourDatabaseName"
    };

    var dbClient = new DatabaseClient("YourConnectionString");

    // Optional: Create a CancellationTokenSource for canceling long-running tasks
    var cts = new CancellationTokenSource();

    try
    {
    await dbClient.ExecuteIndexOptimizeProcedure(options, Console.WriteLine, cts.Token);
    }
    catch (OperationCanceledException)
    {
    Console.WriteLine("Operation was canceled.");
    }
    catch (Exception ex)
    {
    Console.WriteLine($"An error occurred: {ex.Message}");
    }
   ```

## Key Features

- **Customizable Options**:

  - Set various parameters for index optimization through the `IndexOptions` object, allowing you to tailor maintenance operations to your specific database needs.

- **Cancellation Tokens**:

  - Easily manage long-running operations with `CancellationToken`. This feature enables you to cancel the execution of maintenance tasks if they exceed your desired time limits, providing flexibility in managing long-running queries.

- **No Connection Timeouts**:

  - All connection timeouts have been removed, ensuring that operations can wait indefinitely for completion without unexpected interruptions, allowing for smoother execution of database maintenance tasks.

- **Easy Integration**:

  - Simple installation via NuGet and straightforward usage patterns make it easy to incorporate into existing .NET applications, enhancing your database management capabilities with minimal effort.

- **Logging Capabilities**:
  - Output messages and logs during execution can be captured and displayed, facilitating debugging and monitoring of maintenance tasks. For now you need to parse the result yourselves.

---

## Conclusion

The `DatabaseMaintenance` library provides a robust solution for efficiently managing SQL Server maintenance tasks. By leveraging the well-established work of Ola Hallengren, this library simplifies the complex process of ensuring optimal database performance. With its customizable options, cancellation capabilities, and seamless integration into .NET applications, this library serves as an essential tool for developers looking to maintain their databases effectively and reliably.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.


[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fnicolaj-hartmann%2FDatabaseMaintenance.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Fnicolaj-hartmann%2FDatabaseMaintenance?ref=badge_large)