# Poor Man's DI

A very simple Dependency Injection library for .net 2.0

Windows 2000 should have DI too!

### Service example

```csharp
public interface IMainService
{
  void Run();
}

public class MainService : IMainService
{
  // We want LoggerService to be instantiated automatically by Property Injection mechanism
  public ILoggerService Logger { get; set; }

  // The next dependencies will be injected through a constructor
  private IDownloadService _download;
  private IUploadService _upload;

  // The constructor
  public MainService(IDownloadService download, IUploadService upload)
  {
    _download = download;
    _upload = upload;
  }

  public void Run()
  {
    // Here we can use IDownloadService, IUploadService, and ILoggerService
    _download.Download();
    _upload.Upload();
    Logger.Log("Ok");
  }
}
```

### Service usage example

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
