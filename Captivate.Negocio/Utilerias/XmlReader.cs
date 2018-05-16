using Captivate.Comun.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Captivate.Negocio.Utilerias
{
    public class XmlFileReader : IFileReader
    {
        public string GetContent(string path)
        {
            //Lee el xml y lo devuelve en un string
            string xmlstring = System.IO.File.ReadAllText(path);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlstring);
            return doc.InnerText;
        }
        public string GetNodeContent(String path, Queue<String> nodeNames)
        {
            //Lee el xml
            string xmlstring = System.IO.File.ReadAllText(path);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlstring);
            //Dado el parámetro nodeNames, obtiene el contenido del nodo en específico
            String nodeRoute = "/" + String.Join("/", nodeNames);
            XmlNode messageNode = doc.DocumentElement.SelectSingleNode(nodeRoute);

            return messageNode.InnerText;
        }
    }
}
