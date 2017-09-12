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


            
            string name;
            name = reader.ReadLine();
            writer.WriteLine("Ready");

            while (true)
            {
                string input = reader.ReadLine();
                switch (input.ToLower())
                {
                    case "test":
                        writer.WriteLine("test returned.");
                        writer.WriteLine();
                        break;
                    case "bid":
                        writer.WriteLine("Enter auction name:");
                        string a = reader.ReadLine();
                        writer.WriteLine("Enter bid:");
                        double b = Convert.ToDouble(reader.ReadLine());
                        foreach (Auction auction in AuctionList)
                        {
                            if (a == auction.name && b > auction.currentbid)
                            {
                                auction.currenthighestbidder = name;
                                auction.currentbid = b;
                                SendMessageToAllClients(name + " has the highest bid on " + auction);
                            }
                            else
                            {
                                writer.WriteLine("Auction not found, try again.");
                            }
                        }
                        writer.WriteLine();
                        break;
                    case "auctions":
                        foreach (Auction auction in AuctionList)
                        {
                            writer.WriteLine("Auction: " + auction.name);
                            writer.WriteLine("Current highest bidder: " + auction.currenthighestbidder + "with a bid of" + auction.currentbid);
                            writer.WriteLine();
                        }
                        break;
                }
            }
        }
        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 11000);
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
