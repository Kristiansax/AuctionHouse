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
        List<Auction> Auctions = new List<Auction>();
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
                    Socket klient = server.AcceptSocket();
                    Thread t = new Thread(() => program.ClientThread(klient));
                    t.Start();

                    //new Thread(program.ClientThread(klient)).Start(klient);
                }
            }
        }




    }
}
