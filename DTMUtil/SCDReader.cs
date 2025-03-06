using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DTMUtil
{
    public class SCDReader
    {
        public SCDReader() { }
        XmlDocument xmlDoc = null;
        private XmlNamespaceManager nsMgr = null;
        public SCDReader(string filePath)
        {
            LoadFile(filePath);
        }
        public void LoadFile(string filePath)
        {
            xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.Load(filePath);
            nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsMgr.AddNamespace("ns", @"http://www.iec.ch/61850/2003/SCL");
            nsMgr.AddNamespace("gse", @"http://www.selinc.com/2006/61850");
            nsMgr.AddNamespace("header", @"http://www.sce.com/SEMT/61850/SEMT_Header");
        }

        public IEDDeviceInfo[] GetIEDDeviceInfos()
        {
            List<IEDDeviceInfo> list = new List<IEDDeviceInfo>();
            if (xmlDoc == null)
                throw new XMLReaderException("Xml Document is null");
            var networkList = xmlDoc.SelectNodes("ns:SCL/ns:Communication/ns:SubNetwork/ns:ConnectedAP", nsMgr);
            var iedList = xmlDoc.SelectNodes("ns:SCL/ns:IED", nsMgr);

            foreach (XmlNode node in iedList)
            {
                IEDDeviceInfo ied = new IEDDeviceInfo();
                ied.name = GetAttributeValue(node, "name"); // node.Attributes["name"].Value;
                ied.type = GetAttributeValue(node, "type"); // node.Attributes["type"].Value;
                ied.manufacturer = GetAttributeValue(node, "manufacturer"); // node.Attributes["manufacturer"].Value;
                ied.configVersion = GetAttributeValue(node, "configVersion");
                ied.ipAddress = "";
                ied.ipMask = "";
                foreach (XmlNode networkDevice in networkList)
                {
                    if (networkDevice.Attributes["iedName"].Value == ied.name)
                    {
                        var ipNode = networkDevice.SelectSingleNode("ns:Address/ns:P[@type=\"IP\"]", nsMgr);
                        ied.ipAddress = ipNode.InnerText;
                        var ipMaskNode = networkDevice.SelectSingleNode("ns:Address/ns:P[@type=\"IP-SUBNET\"]", nsMgr);
                        ied.ipMask = ipMaskNode.InnerText;
                        var gateway = networkDevice.SelectSingleNode("ns:Address/ns:P[@type=\"IP-GATEWAY\"]", nsMgr);
                        ied.gateway = gateway.InnerText;
                    }
                }
                list.Add(ied);
            }
            return list.ToArray();
        }

        private string GetXMLAttributeValue(XmlNode node, string attributeName)
        {
            if (node == null)
                return string.Empty;

            var attribute = node.Attributes[attributeName];
            if (attribute == null)
                return string.Empty;
            
            return attribute.Value;
        }

        public SourceInfo[] GetSourceInfo()
        {
            List<SourceInfo> list = new List<SourceInfo>();
            if (xmlDoc == null)
                throw new XMLReaderException("Xml Document is null");

            var privateNodeList = xmlDoc.SelectNodes("//ns:SCL/ns:Private", nsMgr);

            foreach (XmlNode privateNode in privateNodeList)
            {
                var sourcesParentNode = privateNode.SelectNodes("header:Sources", nsMgr);
                foreach (XmlNode sources in sourcesParentNode)
                {
                    var sourcesList = sources.SelectNodes("Source", nsMgr);
                    foreach(XmlNode sourceNode in sourcesList)
                    {
                        SourceInfo source = new SourceInfo();
                        source.model = GetAttributeValue(sourceNode,"model");
                        source.network = GetAttributeValue(sourceNode, "network");  
                        source.source = GetAttributeValue(sourceNode, "source");  
                        source.nodeName = GetAttributeValue(sourceNode, "nodeName"); 
                        source.routingAdd = GetAttributeValue(sourceNode, "routingAdd");  
                        source.xmlns = GetAttributeValue(sourceNode, "xmlns");  


                        source.ansiDevs = GetAttributeValue(sourceNode, "ansiDevs"); 
                        source.rackPosNo = GetAttributeValue(sourceNode, "rackPosNo");  
                        source.pod = GetAttributeValue(sourceNode, "pod");  
                        source.ncs = GetAttributeValue(sourceNode, "ncs"); 
                        source.logic = GetAttributeValue(sourceNode, "logic"); 
                        source.flow = GetAttributeValue(sourceNode, "flow");  
                        source.operatorTag = GetAttributeValue(sourceNode, "operatorTag");  

                        list.Add(source);
                    }
                }
                
            }
            return list.ToArray();
        }

        private string GetAttributeValue(XmlNode node, string prop)
        {
            // sourceNode.Attributes["operatorTag"].Value;
            var attr = node.Attributes[prop];
            if(attr != null)
            {
                return attr.Value;
            }

            return "";
        }

    }
}
