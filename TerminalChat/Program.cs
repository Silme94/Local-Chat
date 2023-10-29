using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Server
{
    public static async Task Main(string[] args)
    {
        Console.Write("Enter IP server => ");
        string ip = Console.ReadLine();

        Console.Write("Enter PORT server => ");
        int port = int.Parse(Console.ReadLine());

        using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            Console.WriteLine("Connecting...");
            client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            Console.WriteLine("Connected!");

            _ = Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        if (client.Connected)
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead = client.Receive(buffer);

                            if (bytesRead > 0)
                            {
                                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                                Console.WriteLine(message);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Disconnected : " + e.Message);
                }
            });

            while (true)
            {
                Console.Write(">> ");
                string message = Console.ReadLine();
                client.Send(Encoding.ASCII.GetBytes(message));
            }
        }
    }
}
