using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Message
{
    public class ProcessMessage
    {
        public int ProcId { get; set; }
        public int MessageType { get; set; } // 0= server(listener) status, check 1 = run report, 2 = kill request, 3 = status check

    }

    public static class MessageMethods
    {
        public static T DeserializeMessage<T>(byte[] data) where T : class
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
                return JsonSerializer.Create().Deserialize(reader, typeof(T)) as T;
        }
    }
   
}
