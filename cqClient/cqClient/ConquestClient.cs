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
            string buffer = String.Format("<Conquest><User>"+"{0}"+"</User><Token>"+"{1}"+"</Token><Command>"+"{2}"+"</Command>", _playerName, _token, _command);
            if (_parameters != null)
            {
                foreach (string param in _parameters)
                {
                    if (param.Length < 1 || param == null)
                        break;
                    buffer = String.Format("{0}" + "<Parameter Id=" + "{1}" + ">" + "{2}" + "</Parameter>", buffer, id, param);
                    id += 1;
                }
            }
            buffer = String.Format("{0}"+"</Conquest>", buffer) + Environment.NewLine;
            return buffer;
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
                    // Console.WriteLine(stream.Read(oneByte, 0, 1));
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
            SendCommand("validate", new string[] { password });
            string response = ReadData();
            _loggedIn = true;
            Console.WriteLine(response);
            return true;
        }
    }
}
