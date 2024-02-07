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
                ied.name = node.Attributes["name"].Value;
                ied.type = node.Attributes["type"].Value;
                ied.manufacturer = node.Attributes["manufacturer"].Value;
                ied.configVersion = node.Attributes["configVersion"].Value;
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
        XmlDocument xmlDoc = null;
        private XmlNamespaceManager nsMgr = null;
    }
}
