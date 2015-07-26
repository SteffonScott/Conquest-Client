using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using cqClient;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WELCOME TO CONQUEST!");
            
            ConquestClient client = new ConquestClient(IPAddress.Parse("99.7.194.3"), 9999, "ArchElf");

            client.Connect();
            
            client.Login("Gemstone3");
            client.Person();

            

            Console.ReadLine();
        }
    }
}