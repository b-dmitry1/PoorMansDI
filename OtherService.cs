using System;

public interface IOtherService
{
	void Run();
}

public class OtherService : IOtherService
{
	public OtherService()
	{
		Console.WriteLine("> OtherService created by default constructor");
	}

	public void Run()
	{
		Console.WriteLine("  OtherService called");
	}
}
