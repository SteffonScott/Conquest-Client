using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Net.Sockets;
using System.IO;
using Microsoft.VisualBasic;

namespace cqClient
{
    public class XMLMessage
    {
        public XMLMessage()
        {
            body = new List<string>();
        }

        public string id { get; set; }
        public List<string> body { get; set; }
    }
    public class XMLResponse
    {
        public string user { get; set; }
        public string token { get; set; }
        public string command { get; set; }
        public List<XMLMessage> message { get; set; }

        public XMLResponse()
        {
            message = new List<XMLMessage>();
        }

    }
    public class ConquestClient
    {
        private string _playerName { get; set; }
        private IPAddress _gameIp { get; set; }
        private int _port { get; set; }
        private string _token { get; set; }
        public bool _loggedIn { get; set; }
        private System.Net.Sockets.TcpClient _clientSocket { get; set; }

        public ConquestClient(IPAddress gameIp, int port, string playerName = "Player")
        {
            _playerName = playerName;
            _token = "0123456789012345678901234567890";
            _gameIp = gameIp;
            _port = port;
            _clientSocket = new System.Net.Sockets.TcpClient();
        }

        public bool Connect()
        {
            _clientSocket.Connect(_gameIp, _port);
            return _clientSocket.Connected;
        }
        public void SendCommand(string _command, string[] _parameters = null)
        {
            NetworkStream activeStream;
            string xmlCommand = BuildCommand(_command, _parameters);
            Byte[] data = System.Text.UTF8Encoding.ASCII.GetBytes(xmlCommand);
  
            if (this._clientSocket.Connected == false)
                return;
            activeStream = _clientSocket.GetStream();
            if (activeStream.CanWrite)
                activeStream.Write(data, 0, data.Length);
        }
        public string BuildCommand(string _command, string[] _parameters = null)
        {
            int id = 1;

            XElement conquest = new XElement("Conquest");

            conquest.Add(new XElement("User", _playerName));

            conquest.Add(new XElement("Token", _token));

            conquest.Add(new XElement("Command", _command));
            if (_parameters != null)
            {
                foreach (string param in _parameters)
                {
                    XElement e_parameter = new XElement("Parameter", param);
                    e_parameter.Add(new XAttribute("Id", id));
                    conquest.Add(e_parameter);
                    id++;
                }
            }
            Console.WriteLine(conquest.ToString(SaveOptions.DisableFormatting));
            return String.Format(conquest.ToString(SaveOptions.DisableFormatting) + Environment.NewLine);
        }
        public string ReadData()
        {
            string buffer = null;
            byte[] oneByte = new byte[1];
            char oneChar;
            NetworkStream stream;
            int total = 0;

            if (!this._clientSocket.Connected)
                return null;
            stream = this._clientSocket.GetStream();

            if (stream.CanRead)
            {
                do
                {
                    total = stream.Read(oneByte, 0, 1);
                    oneChar = Convert.ToChar(System.Text.Encoding.ASCII.GetString(oneByte, 0, 1));
                    buffer = buffer + oneChar;
                    if (oneByte[0] == 10)
                        total = 0;
                }
                while (oneByte[0] != 10 || total != 0);
                if (buffer != null && buffer.IndexOf("</Conquest>") < 1)
                {
                    buffer = null;
                }
            }
            return buffer;
        }
        public bool Login(string password)
        {
            SendCommand("Validate", new string[] { password });
            XMLResponse response = ParseResponse(ReadData());
            _loggedIn = true;

            if (response.message[0].id == "46")
            {
                _loggedIn = true;
                Console.WriteLine("Login successful! Playing as {0}", _playerName);
                return true;
            }
            Console.WriteLine("Login failed! Please re-attempt validation.");
            return false;
        }
        public XMLResponse ParseResponse(string _response)
        {
            int bodyId = 0;
            int id = 0;
            XMLResponse response = new XMLResponse();
            XElement XML;

            XML = XElement.Parse(_response);

            response.user = XML.Element("User").Value;
            response.command = XML.Element("Command").Value;
            response.token = XML.Element("Token").Value;

            foreach (XElement msgElem in XML.Elements("Message"))
            {
                response.message.Add(new XMLMessage());
                response.message[id].id = msgElem.Attribute("Id").Value;

                foreach (XElement bodyElem in msgElem.Elements("Body"))
                {
                    response.message[id].body.Add(bodyElem.Value);
                    bodyId += 1;
                }

                bodyId = 0;
                id += 1;
                
            }

            return response;
        }
        public void testMethod1()
        {
            Console.WriteLine(BuildCommand("Person"));
        }
     
    }
}

