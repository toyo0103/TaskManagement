using System;
using NetMQ;
using NetMQ.Sockets;

namespace producer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new RequestSocket(">tcp://localhost:5556"))  // connect
            {
                for (var i = 0; i < 10; i++)
                {
                    // Send a message from the client socket
                    var msg = string.Format("{{\"JobName\": \"Job1\", \"Data\":\"{0}\"}}", Guid.NewGuid());
                    client.SendFrame(msg);
                    Console.WriteLine(msg);

                    // Receive the response from the client socket
                    string m2 = client.ReceiveFrameString();
                    while (m2 != "exit")
                    {
                        Console.WriteLine("From Server: {0}", m2);
                    }
                }
            }
        }
    }
}
