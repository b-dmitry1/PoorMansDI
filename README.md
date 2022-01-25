# Poor Man's DI

A very simple Dependency Injection library for .net 2.0

Windows 2000 should have DI too!


### Example

```csharp

using PoorMans.DI;

// Create a service collection
var services = new ServiceCollection();

// Register services
services
  .AddSingleton<ILoggerService, LoggerService>()
  .AddSingleton<IDownloadService, DownloadService>()
  .AddSingleton<IUploadService, UploadService>()
  .AddTransient<IMainService, MainService>();

// Create a service provider
var provider = services.BuildServiceProvider();

// Request and call a service
// All the dependencies will be resolved automatically
provider
  .GetService<IMainService>()
  .Run();

```
