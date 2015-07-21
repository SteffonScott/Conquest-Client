using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;


namespace cqClient
{
    [XmlRoot("Conquest")]
    public class oGameCommand
    {
        public string _user { get; set; }
        public string _token { get; set; }
        public string _command { get; set; }
        public string[] _parameters { get; set; }
        public XDocument xmlDoc { get; set; }
        public string xmlString { get; set; }

        public oGameCommand(string user, string command, string[] parameters = null, string token = "0")
        {
            if (token != "0")
                _token = "0123456789012345678901234567890";
            else
                _token = token;
            if (user == null)
                _user = "Player";
            else
                _user = user;
            _command = command;
            _parameters = parameters;

            // Create XML string
            xmlDoc = new XDocument(
                new XDeclaration("1.0", null, null),
                new XElement("Conquest",
                    new XElement("User", _user),
                    new XElement("Token", _token),
                    new XElement("Command", _command),
                    new XElement("Parameter", new XAttribute("Id", 1), _parameters[0]),
                    new XElement("Parameter", new XAttribute("Id", 2), _parameters[1])
                    )
                );
            using (var sw = new StringWriter())
            {
                xmlDoc.Save(sw);
                xmlString = sw.GetStringBuilder().ToString().Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("encoding=\"utf-16\"", "".Replace("\\", ""));
            }

        }
        public oGameCommand() { }
    }
}
