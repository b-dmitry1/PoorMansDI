using System;

public interface IPropService
{
	void Run();
}

public class PropService : IPropService
{
	public PropService()
	{
		Console.WriteLine("> PropService created by default constructor");
	}

	public void Run()
	{
		Console.WriteLine("  PropService called");
	}
}
