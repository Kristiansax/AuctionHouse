using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient server = new TcpClient("localhost", 20001);
            NetworkStream stream = server.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            Console.WriteLine(reader.ReadLine());
            while (true)
            {
                string input = Console.ReadLine();
                writer.WriteLine(input);
                string output = reader.ReadLine();
                Console.WriteLine(output);
            }
        }
    }
}
