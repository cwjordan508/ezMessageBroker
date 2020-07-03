using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Message;

namespace ControlClient
{
    /// <summary>
    /// </summary>
    public class Client
    {
        // TODO:  Come up with a way to no hard code the parameters
        private const int Port = 42069;
        private const string Ip = "127.0.0.1";

        private readonly int _procId;

        /// <summary>
        ///     Provide a unique process ID for any process you wish to control.  If you are instantiating from a controller GUI,
        ///     the ID can be arbitrary.
        /// </summary>
        /// <param name="procId"></param>
        public Client(int procId)
        {
            _procId = procId;
        }

        /// <summary>
        ///     IsListening can be used to determine if the server is already running.
        ///     This is useful in the GUI when deciding to start the server or not
        /// </summary>
        public bool IsListening()
        {
            bool returnVal;
            try
            {
                returnVal = true;

                var pStatus = new ProcessMessage {MessageType = 0};

                var byteArray = JsonSerializer.SerializeToUtf8Bytes(pStatus);

                var response = Connect(byteArray);

                if (response == "0" || response == null) returnVal = false;
            }
            catch
            {
                returnVal = false;
            }

            return returnVal;
        }

        /// <summary>
        ///     ProcessRunning is to be used as a conditional check to determine if the process should continue running or not.
        ///     It will return true when there is not kill request for the process, and false when there is.
        ///     It can be used to terminate or break out of long, nested, or indefinite running loops.
        /// </summary>
        public bool ProcessRunning()
        {
            var keepRunning = true;

            // create message object, serialize and send to server
            var pStatus = new ProcessMessage {ProcId = _procId, MessageType = 1};
            //  var byteArray = JsonSerializer.Serialize(pStatus);
            var byteArray = JsonSerializer.SerializeToUtf8Bytes(pStatus);
            // pass object and read response
            var response = Connect(byteArray);

            // if response = 0, that means there has been a kill request.  return false to the application so it knows to stop running.
            if (response == "0") keepRunning = false;

            return keepRunning;
        }


        /// <summary>
        ///     This method is intended to be called via the controller GUI.
        ///     It sends a request to kill an application based on the assigned process ID.
        /// </summary>
        public void RequestKill(int target)
        {
            // create message object, serialize and send to server
            var pStatus = new ProcessMessage {ProcId = target, MessageType = 2};
            //  var byteArray = JsonSerializer.Serialize(pStatus);
            var byteArray = JsonSerializer.SerializeToUtf8Bytes(pStatus);
            // pass to server.  we aren't doing anything with the server's response right here, maybe we should.
            Connect(byteArray);
        }


        /// <summary>
        ///     This method takes the current process out of the running processes list.
        ///     This should be the final call before application exit.
        /// </summary>
        public void ReportKill()
        {
            // create message object, serialize and send to server
            var pStatus = new ProcessMessage {ProcId = _procId, MessageType = 4};
            //  var byteArray = JsonSerializer.Serialize(pStatus);
            var byteArray = JsonSerializer.SerializeToUtf8Bytes(pStatus);
            // pass object and read response -- we don't need a response from this
            Connect(byteArray);
        }

        /// <summary>
        ///     CheckProcessStatus will return true if the supplied process id is still running
        /// </summary>
        public bool CheckProcessStatus(int target)
        {
            var running = false;

            // create message object, serialize and send to server
            var pStatus = new ProcessMessage {ProcId = target, MessageType = 3};
            //  var byteArray = JsonSerializer.Serialize(pStatus);
            var byteArray = JsonSerializer.SerializeToUtf8Bytes(pStatus);
            // pass byteArray to TCP server and parse response.  A response of "1" means the process is running.
            var response = Connect(byteArray);
            if (response == "1") running = true;

            return running;
        }

        /// <summary>
        ///     This method sends the server a shutdown request.
        /// </summary>
        public void KillServer()
        {
            // create message object, serialize and send to server
            var pStatus = new ProcessMessage {MessageType = 5};
            //   var byteArray = JsonSerializer.Serialize(pStatus);
            var byteArray = JsonSerializer.SerializeToUtf8Bytes(pStatus);
            // pass object and read response -- we don't need a response from this
            Connect(byteArray);
        }


        /// <summary>
        ///     Connect is a private method that connects and sends a message to the TCP server.
        /// </summary>
        private string Connect(byte[] data)
        {
            string response;
            try
            {
                // Create a TcpClient.
                var client = new TcpClient(Ip, Port);

                var stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                // Buffer to store the response bytes.
                data = new byte[256];

                // String to store the response UTF8 representation.

                // Read the first batch of the TcpServer response bytes.
                var bytes = stream.Read(data, 0, data.Length);
                var responseData = Encoding.UTF8.GetString(data, 0, bytes);
                response = responseData;

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch
            {
                response = null;
            }

            return response;
        }
    }
}