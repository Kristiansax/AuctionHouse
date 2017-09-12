using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouseClient
{
    class Program
    {
        static TcpClient server;
        static Program CProgram = new Program();
        
        static void Main(string[] args)
        {
            Program CProgram = new Program();
            server = new TcpClient("localhost", 11000);
            CProgram.Run();
        }
        private void Run()
        {
            NetworkStream stream = server.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            Console.WriteLine("ENTER NICKNAME"); //Seadog

            new Thread(read => CProgram.Read(reader)).Start();
            new Thread(write => CProgram.Write(writer)).Start();
        }

        private void Write(StreamWriter writer)
        {
            while (true)
            {
                string input = Console.ReadLine();
                writer.WriteLine(input);
            }
        }

        private void Read(StreamReader reader)
        {
            while (true)
            {
                string output = reader.ReadLine();
                Console.WriteLine(output);
            }
        }
    }
}