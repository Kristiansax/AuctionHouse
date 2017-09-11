using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse
{
    class Program
    {
        static private List<Auction> AuctionList = new List<Auction>();
        static private List<Socket> Clients = new List<Socket>();

        private void ClientThread(Socket klient)
        {
            NetworkStream stream = new NetworkStream(klient);
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            IPEndPoint remoteIpEndPoint = klient.RemoteEndPoint as IPEndPoint;
            IPEndPoint localIpEndPoint = klient.LocalEndPoint as IPEndPoint;

            if (remoteIpEndPoint != null)
            {
                Console.WriteLine("I am connected to " + remoteIpEndPoint.Address + " on port number " + remoteIpEndPoint.Port);
            }


            writer.WriteLine("Ready");
            while (true)
            {
                string input = reader.ReadLine();
                switch (input.ToLower())
                {
                    case "test":
                        writer.WriteLine("test returned");
                        break;
                    case "bid":
                        writer.WriteLine();
                        break;
                    case "auctions":
                        foreach (Auction auction in AuctionList)
                        {
                            writer.WriteLine(auction.name);
                            writer.WriteLine(auction.price);
                            writer.WriteLine();
                        }
                        break;
                }
            }
        }
        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 0);
            server.Start();
            while (true)
            {
                while (true)
                {
                    Program program = new Program();
                    Socket client = server.AcceptSocket();
                    Clients.Add(client);
                    Thread t = new Thread(() => program.ClientThread(client));
                    t.Start();

                    //new Thread(program.ClientThread(klient)).Start(klient); old
                }
            }
        }
        private void SendMessageToAllClients(string message) //Burde måske være en gavel metode i Auction.cs?
        {
            foreach(Socket client in Clients)
            {
                SendMessage(message, client);
            }
        }

        private static void SendMessage(string message, Socket client)
        {
            NetworkStream stream = new NetworkStream(client);
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            writer.WriteLine(message);
        }
    }
}
