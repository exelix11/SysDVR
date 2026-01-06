using System;

namespace SysDVR.Client.App
{
    internal class TestStubClient : BaseStandaloneClient
	{
		CommandLineOptions cmd;

		public TestStubClient(CommandLineOptions cmd) 
		{
			this.cmd = cmd;
		}

        public override void Entrypoint()
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

        public override void Initialize()
        {
            
        }
	}
}
