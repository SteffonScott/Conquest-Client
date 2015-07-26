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
        private Connection _connection { get; set; }
        private List<XMLResponse> _serverMessages { get; set; }

        public ConquestClient(IPAddress gameIp, int port, string playerName = "Player")
        {
            _playerName = playerName;
            _token = "0123456789012345678901234567890";
            _connection = new Connection(gameIp, port);
            _loggedIn = false;
            _serverMessages = new List<XMLResponse>();

            //Initialize the events
            _connection.DataReceived += new Connection.delDataReceived(connection_DataReceived);
            _connection.ConnectionStatusChanged += new Connection.delConnectionStatusChanged(connection_ConnectionStatusChanged);
        }

        public bool Connect()
        {

                _connection.Connect();
                do
                {
                    var _delayTimer = new System.Timers.Timer();
                    _delayTimer.Interval = 5000;
                    _delayTimer.Start();
                }
                while (_connection.ConnectionState != Connection.ConnectionStatus.Connected);

            return (_connection.ConnectionState == Connection.ConnectionStatus.Connected);
        }

        /// <summary>
        /// Build XML command using string and send to server if connection is established.
        /// </summary>
        public void SendCommand(string _command, string[] _parameters = null)
        {
            string xmlCommand = BuildCommand(_command, _parameters);
            Byte[] data = System.Text.UTF8Encoding.ASCII.GetBytes(xmlCommand);

            if (this._connection.ConnectionState != Connection.ConnectionStatus.Connected)
                return;
            _connection.Send(xmlCommand);
        }
        /// <summary>
        /// Build XML object and format the string output to server specification
        /// </summary>
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
            return String.Format(conquest.ToString(SaveOptions.DisableFormatting) + Environment.NewLine);
        }
        public bool Login(string password)
        {
            XMLResponse response = new XMLResponse();
            SendCommand("Validate", new string[] { password });
            do
            {
                response = FindResponse("Validate");
            }
            while (response == null);
            if (response.message[0].id == "46")
            {
                _loggedIn = true;
                return true;
            }
            return false;
        }
        public void Person()
        {
            SendCommand("Person");
        }
        public XMLResponse FindResponse(string command)
        {
            XMLResponse response = _serverMessages.FirstOrDefault(r => r.command == command);
            return response;
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
        void connection_ConnectionStatusChanged(Connection sender, Connection.ConnectionStatus status)
        {
            Console.WriteLine("Connection: " + status.ToString() + Environment.NewLine);
        }
        void connection_DataReceived(Connection sender, object data)
        {
            //Interpret the received data object as XMLReponse
            XMLResponse response = ParseResponse(data as string);
            _serverMessages.Add(response);
        }
    }
}

