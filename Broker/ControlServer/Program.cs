using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Message;
using System.Text.Json;

namespace ControlServer
{
    internal class Program
    {

        // TODO:  Come up with a way to no hard code the parameters
        private const int Port = 42069;
        private const string Ip = "127.0.0.1";


        private static readonly List<int>
            HitList = new List<int>(); // "hit list" aka list of processes that have a kill request

        private static readonly List<int>
            ProcessList = new List<int>(); // process that we think are running


        public static void Write2Con(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var conString = DateTime.Now.ToString("HH:mm:ss") + ">" + msg;

            Console.WriteLine(conString.ToUpper());
        }

        private static void Main()
        {
            Write2Con("ezMessageBroker Server Started!");

            TcpListener server = null;
            try
            {
                const int port = Port;
                var localAdd = IPAddress.Parse(Ip);

                server = new TcpListener(localAdd, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                var bytes = new byte[256];

                // some additional setting up
                Console.ForegroundColor = ConsoleColor.Green;

                // Enter the listening loop.
                while (true)
                {
                    var client = server.AcceptTcpClient();

                    var stream = client.GetStream();

                    // Loop to receive all the data sent by the client.
                    while (stream.Read(bytes, 0, bytes.Length) != 0)
                    {
                        ///var curProcess = JsonSerializer.Deserialize<ProcessMessage>(bytes);
                        var sequence = new Utf8JsonReader(bytes);
                        var curProcess = JsonSerializer.Deserialize<ProcessMessage>(ref sequence);

                        // run logic on the process object, and determine appropriate response
                        var data = ProcessLogic(curProcess);

                        var msg = Encoding.UTF8.GetBytes(data);

                        stream.Write(msg, 0, msg.Length);
                    }

                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Write2Con("SocketException: " + e);
            }
            finally
            {
                server?.Stop();
            }

            Write2Con("\nHit enter to continue...");
            Console.Read();
        }

        private static string ProcessLogic(ProcessMessage curProcess)
        {
            string data;

            switch (curProcess.MessageType)
            {
                case 0:
                    // msg 0 -- just a "Ping" to see if the server is running
                    data = "1"; //  respond with the process id


                    break;


                case 1:

                    // just a process "run report"
                    Write2Con("Received message from process id: " + curProcess.ProcId);

                    if (HitList.Contains(curProcess.ProcId))
                    {
                        data = "0"; // response = "0" means there is a kill request for this process, and is a signal to shut down
                    }
                    else
                    {
                        if (!ProcessList.Contains(curProcess.ProcId)) ProcessList.Add(curProcess.ProcId);
                        data = "1"; //response = "1"  -- go about your business
                    }

                    break;
                case 2:

                    // type 2: a kill request from the controller gui
                    Write2Con("Received request to kill process id: " + curProcess.ProcId);
                    HitList.Add(curProcess.ProcId); // add to "hit list" -- next time we see this process we will tell it to shut down
                    data = "1";
                    break;

                case 3:

                    // type 3: the controller checking if the process is running
                    data = ProcessList.Contains(curProcess.ProcId) ? "1" : "0";
                    break;

                case 4:

                    // type 4: remove the process from the list of currently running processes
                    Write2Con("process id " + curProcess.ProcId + " has been killed.");
                    if (ProcessList.Contains(curProcess.ProcId))
                    {
                        HitList.Remove(curProcess.ProcId);
                        ProcessList.Remove(curProcess.ProcId);
                    }

                    data = "0";
                    break;
                case 5:

                    // type 4: remove the process from the list of currently running processes
                    Write2Con("Goodbye Cruel World");

                    data = "0";
                    Environment.Exit(0);
                    break;

                default:

                    //if we reach default case that means the message to the server is invalid, so we will return something invalid :)
                    data = "-999";
                    break;
            }

            return data;
        }
    }
}