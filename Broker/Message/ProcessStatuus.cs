namespace Message
{
    public class ProcessMessage
    {
        public int ProcId { get; set; }
        public int MessageType { get; set; } // 0= server(listener) status, check 1 = run report, 2 = kill request, 3 = status check

    }
   
}
