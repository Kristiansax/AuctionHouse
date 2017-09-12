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
            writer.WriteLine();
            PrintAllAuctions(writer);
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

                        Auction key = null;
                        foreach (Auction auction in AuctionList)
                        {
                            if (a == auction.name)
                            {
                                key = auction;
                                writer.WriteLine("Enter bid:");
                                double b = Convert.ToDouble(reader.ReadLine());

                                if (b > auction.currentbid)
                                {
                                    auction.currenthighestbidder = name;
                                    auction.currentbid = b;
                                    SendMessageToAllClients(name + " has the highest bid on " + auction.name);
                                }
                                else
                                {
                                    writer.WriteLine("Bid is too low");
                                }
                            }
                        }

                        if (key == null)
                        {
                            writer.WriteLine("Auction does not exist");
                        }

                        writer.WriteLine();
                        break;
                    case "auctions":
                        PrintAllAuctions(writer);
                        break;
                    default:
                        writer.WriteLine("Invalid input");
                        writer.WriteLine();
                        break;
                }
            }
        }

        private static void PrintAllAuctions(StreamWriter writer)
        {
            foreach (Auction auction in AuctionList)
            {
                writer.WriteLine("Auction: " + auction.name);

                if (auction.currenthighestbidder == null)
                {
                    writer.WriteLine("No bidders yet, minimum bid is " + auction.currentbid);
                    writer.WriteLine();
                }
                else
                {
                    writer.WriteLine("Current highest bidder: " + auction.currenthighestbidder + "with a bid of" + auction.currentbid);
                    writer.WriteLine();
                }
            }
        }

        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 11000);
            server.Start();

            //Pupulating auctionlist
            Auction Hat = new Auction("Hat", 20, null);
            Auction Sko = new Auction("Sko", 10, null);
            Auction Hammer = new Auction("Hammer", 30, null);
            AuctionList.Add(Hat);
            AuctionList.Add(Sko);
            AuctionList.Add(Hammer);

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
            foreach (Socket client in Clients)
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



        /*public class HClient
        {
            public static bool newHighestBid = false;
            private TcpClient clientSocket;
            static int _currentBid = 0;
            private string cli1Nu;
            static List<int> _previousBid = new List<int>(); //Vi vil gerne have vores bud på en liste der kan søges i, sortes og manipuleres,
            public static List<HClient> _clientList = new List<HClient>();
            private object _lock;

            internal HClient(TcpClient inClientSocket, int clientNo)
            {
                this.clientSocket = inClientSocket;
                cli1Nu = clientNo.ToString();
            }
            internal void StartClient()
            {
                Thread newClient = new Thread(ClientHandler);
                newClient.Start();
            }

            internal void ClientHandler()
            {
                try
                {
                    while (true)
                    {
                        IPEndPoint remoteIpEndPoint = clientSocket.Client.RemoteEndPoint as IPEndPoint;
                        IPEndPoint localIpEndPoint = clientSocket.Client.LocalEndPoint as IPEndPoint;

                        NetworkStream stream = new NetworkStream(clientSocket.Client);
                        StreamReader reader = new StreamReader(stream);
                        StreamWriter writer = new StreamWriter(stream);
                        writer.AutoFlush = true;

                        if (remoteIpEndPoint != null)
                        {
                            Console.WriteLine("I am connected to " + remoteIpEndPoint.Address + " on port number " +
                                              remoteIpEndPoint.Port);
                        }

                        if (clientSocket.Connected)
                        {
                            Thread.CurrentThread.Name = reader.ReadLine();
                            Console.WriteLine(Thread.CurrentThread.Name);
                            writer.WriteLine("You can now bid on a special one of a kind authentic Mcnugget shaped like a Mcnugget");
                            while (clientSocket.Connected)
                            {
                                int i = int.Parse(reader.ReadLine());
                                Monitor.Enter(_lock);
                                if (i > _currentBid)
                                {

                                    newHighestBid = true;
                                    _previousBid.Add(_currentBid);
                                    _currentBid = i;
                                    Console.WriteLine(i + " is the highest bid ");

                                }
                                else
                                {
                                    writer.WriteLine(_currentBid);
                                }
                                Monitor.Exit(_lock);
                            }
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("An IOException occurred: " + e);
                }
            }
            public static void Gavel()
            {
                for (int i = 10; i > 0; i--)
                {
                    if (newHighestBid)
                    {
                        i = 10;
                        foreach (HClient item in _clientlist)
                        {
                            item.GavelMessage(i);
                        }
                        newHighestBid = false;
                    }
                    if (i == 5 || i == 3 || i == 1)
                    {
                        foreach (HClient item in _clientlist)
                        {
                            item.GavelMessage(i);
                        }
                    }
                    Thread.Sleep(500);
                }
            }

            internal void GavelMessage(int i)
            {
                NetworkStream stream = new NetworkStream(clientSocket.Client);
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);
                writer.AutoFlush = true;
                writer.WriteLine();
                if (i == 10)
                {
                    writer.WriteLine("NEW BID - GAVEL IS STARTED");
                }
                if (i == 5)
                {
                    writer.WriteLine("FIRST - FIRST!");
                }
                if (i == 3)
                {
                    writer.WriteLine("SECOND - SECOND!");
                }
                if (i == 1)
                {
                    writer.WriteLine("SOLD! TO THE HIGHEST BID ");
                }
            }
        }*/
    }
}