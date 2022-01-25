# Poor Man's DI

A very simple Dependency Injection library for .net 2.0

Windows 2000 should have DI too!


### Example

```csharp

using PoorMans.DI;

var services = new ServiceCollection();

services
  .AddSingleton<ILoggerService, LoggerService>()
  .AddSingleton<IDownloadService, DownloadService>()
  .AddSingleton<IUploadService, UploadService>()
  .AddSingleton<IMainService, MainService>();

services
  .GetService<IMainService>()
  .Run();

```
