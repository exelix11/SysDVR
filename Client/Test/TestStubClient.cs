using System;

namespace SysDVR.Client.Test
{
	internal class TestStubClient : BaseStandaloneClient
	{
		CommandLineOptions cmd;

		public TestStubClient(CommandLineOptions cmd) 
		{
			this.cmd = cmd;
		}

		public void Run()
		{
			var manager = GetConnector(cmd).ConnectStub().GetAwaiter().GetResult();

			manager.Begin();

			Console.WriteLine("TestStubClient running");
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();

			manager.Stop().GetAwaiter().GetResult();
			manager.Dispose();

			GC.Collect();
		}
	}
}
