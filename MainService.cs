using System;

public interface IMainService
{
	void Run();
}

public class MainService : IMainService
{
	private ILoggerService _logger;
	private IOtherService _other;
	public IPropService Proper { get; set; }

	public MainService()
	{
		Console.WriteLine("> MainService created by default constructor!");
	}

	public MainService(int value, ILoggerService logger, IOtherService other)
	{
		_logger = logger;
		_other = other;
		// Comment to create automatically. Uncomment to create manually
		// Proper = new ProperService();
		Console.WriteLine("> Service created (value = {0})", value);
	}

	public void Run()
	{
		Console.WriteLine("MainService called");
		_logger.Run();
		Proper.Run();
		_other.Run();
	}
}