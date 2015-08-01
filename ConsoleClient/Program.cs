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
            
            // Insantiate the client and populate properties with the server's IP, port and the character name to use.
            ConquestClient client = new ConquestClient(IPAddress.Parse("99.7.194.3"), 9999, "ArchElf");

            // Attempt connection with server and report success or failure.
            if (client.Connect())
                Console.WriteLine("Connection to game server established.");
            else { Console.WriteLine("Connection to game server failed."); }

            // Login to the server and report success or failure.
            if (client.Login("Gemstone3"))
                Console.WriteLine("Login with current credentials succeeded.");
            else { Console.WriteLine("Login with current credentials failed."); }

            // Retrieve player stats and populate the "_player" property with current values.
            client.Person();

            

            Console.ReadLine();
        }
    }
}