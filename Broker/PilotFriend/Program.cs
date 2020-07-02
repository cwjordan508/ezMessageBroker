using System;
using System.Threading;
using ControlClient;

namespace PilotFriend
{
    internal static class Program
    {
        private static Client _client;
        private static int _procId;

        private static void Main(string[] args)
        {
            if(args[0]!=null)
            {
                _procId = int.Parse(args[0]);
                _client = new Client(_procId);
            }


            if (_client != null)
            {
                var i = 0;

                while (_client.ProcessRunning())
                {
                    i += 1;
                    Console.WriteLine(i.ToString());
                    Thread.Sleep(2000);
                }
               
            } else
            {
                Console.WriteLine("Did not instantiate controller client object.");
                Console.ReadLine();
            }

            _client?.ReportKill();
        }
    }
}
