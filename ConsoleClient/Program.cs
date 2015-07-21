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
            Console.WriteLine("Sending " + client.BuildCommand("validate", new string[] { "Gemstone3" }) + " to 99.7.194.3 on port 9999");
            if (client.Connect())
                Console.WriteLine("Connection Successful!");
            else
                Console.WriteLine("Connection failed!");
            if (client.Login("gemstone3"))
            {

            }
            /* while (true)
                Console.WriteLine(client.ReadData()); */
            Console.ReadLine();
        }
    }
}
