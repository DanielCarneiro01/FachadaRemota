using System;

using CalculatorInterface;
using RemotingFacade;

namespace Server
{
    class Program
    {
        private static void Run()
        {
            using (
                RemotingServer<Calculator, CalculatorFactory> server =
                    new RemotingServer<Calculator, CalculatorFactory>(
                        Formatter.Binary,
                        Protocol.Http, 65101, "ChannelPortName",
                        "CalculatorService", ServerActivation.ServerSingleCall
                    )
            )
            {
                Console.WriteLine("Calculator service is ready");
                Console.WriteLine("Press 'Enter' to end...");
     
                // Wait for requests

                Console.ReadLine();
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press 'Enter' to end...");
            Console.WriteLine("Press 'Enter' to end...");
        }
    }
}
