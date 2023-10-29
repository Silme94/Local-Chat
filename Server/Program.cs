using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

class Server
{
    public static List<Socket> clients = new List<Socket>();
    public static void Main(string[] args)
    {
        using (Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            server.Bind(new IPEndPoint(IPAddress.Parse(args[0]), int.Parse(args[1])));
            server.Listen(10);

            Console.WriteLine($"Server Chat is listening at {args[0]}:{args[1]}");

            while (true)
            {
                Socket client = server.Accept();
                Console.WriteLine(client.RemoteEndPoint + " has joined the chat.");
                clients.Add(client);

                Thread handle = new Thread(() => { HandleClient(client); });
                handle.Start();
            }
        }
    }

    public static void HandleClient(Socket client)
    {
        try
        {
            while (true)
            {
                byte[] buffer = new byte[2048];
                int bytesRead = client.Receive(buffer);
                if (bytesRead == 0)
                {
                    Console.WriteLine(client.RemoteEndPoint + " has left the chat.");
                    clients.Remove(client);
                    client.Close(); 
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine(client.RemoteEndPoint + " > " + message);

                foreach (Socket cl in clients)
                {
                    if (cl != client)
                    {
                        cl.Send(Encoding.ASCII.GetBytes(client.RemoteEndPoint + " > " + message));
                    }
                }
            }
        }
        catch
        {
            Console.WriteLine(client.RemoteEndPoint + " has left the chat.");
            clients.Remove(client);
            client.Close();
        }
    }

}