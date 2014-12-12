using System;

using CalculatorInterface;
using RemotingFacade;

namespace Client
{
    class Program
    {
        void Run()
        {
            using (
                RemotingClient<ICalculator, IRemotedFactory<ICalculator>>
                    client =
                        new RemotingClient<ICalculator, IRemotedFactory<ICalculator>>(
                            Formatter.Binary,
                            Protocol.Http, "localhost", 65101, "ChannelPortName",
                            "CalculatorService", ClientActivation.Server)
                        )
            {
                ICalculator c1 = client.GetObject();

                Use(c1);

                ICalculator c2 = client.GetObject();

                Use(c2);
            }
        }

        private void Use(ICalculator c)
        {
            int result = c.Add(1, 2);
            Console.WriteLine(result);

            result = c.Add(2, 2);
            Console.WriteLine(result);
            
            result = c.Add(3, 2);
            Console.WriteLine(result);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Client is waiting for server to start.");
            Console.WriteLine("Press 'Enter' when server is ready ...");

            Console.ReadLine();

            try
            {
                new Program().Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}
