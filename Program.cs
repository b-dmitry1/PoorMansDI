using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using PoorMans.DI;

class Program
{
	static void Main(string[] args)
	{
		var services = new ServiceCollection();

		// Register services
		services
			.AddSingleton<ILoggerService, LoggerService>()
			.AddSingleton<IOtherService, OtherService>()
			.AddSingleton<IPropService, PropService>()
			.AddSingleton<IMainService, MainService>();

		Console.WriteLine("Running MainService");
		services.GetService<IMainService>().Run();

		Console.WriteLine("Running MainService again");
		services.GetService<IMainService>().Run();

		Console.ReadKey();
	}
}
