using System;

public interface ILoggerService
{
	void Run();
}

public class LoggerService : ILoggerService
{
	public LoggerService()
	{
		Console.WriteLine("> LoggerService created by default constructor");
	}

	public void Run()
	{
		Console.WriteLine("  Logger called");
	}
}
