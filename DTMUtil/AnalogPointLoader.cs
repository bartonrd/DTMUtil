using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DTMUtil
{
    public class AnalogPointLoader
    {
        // xml files
        private XElement hmi_config;
        private XElement scd;
        // namespaces for each xml files
        // maybe make this dynamic and move to constructor in the future
        private XNamespace scdIedNamespace = @"http://www.iec.ch/61850/2003/SCL";
        private XNamespace hmiNamespace = @"http://www.sce.com/SEMT/61850/HMI";

        public AnalogPointLoader(string scdPath, string hmiConfigPath)
        {
            // this needs to be changed to look in the active directory
            hmi_config = XElement.Load(hmiConfigPath);
            scd = XElement.Load(scdPath);
        }

        // Given the iedName and the unit we are interested in
        // This function returns the mms multiplier
        public float FetchMmsMulti(string iedName, string unit)
        {
            string manufacturer;
            string type;
            IEDMakeModel(out manufacturer, out type, iedName);
            Console.WriteLine("manufacturer: " + manufacturer);
            Console.WriteLine("type: " + type);
            // we search for the mms value now that we have the manufacturer and ied type
            // we first search for the manufacturer name... if we don't find it we use the catch all name
            // we then search in that block for the ied type.. if we don't find it we use the catch all type
            // then we find the unit in that block and return the mmsMult cast to a float
            // POTENTIAL ISSUE WITH THIS METHOD:
            // this approach assumes all units are contained within their most specific container
            // i.e. if the type "D60" exists but does not contain the unit Volt
            // then that is an error with the hmi config rather than the unit Volt being defined by the less specific container "all"
            // this appears to be the case looking at the hmi config


            // find manufacturer block to search in
            var make_list = hmi_config.Descendants(hmiNamespace + "manufacturer");
            XElement manufacturer_block = make_list.Single(el => el.Attribute("name").Value == "all");
            foreach (var i in make_list)
            {
                if (i.Attribute("name").Value == manufacturer)
                {
                    manufacturer_block = i;
                    break;
                }
            }

            // find type block to search in
            var type_list = manufacturer_block.Descendants(hmiNamespace + "type");
            XElement type_block = type_list.Single(el => el.Attribute("name").Value == "all");
            foreach (var i in type_list)
            {
                if (i.Attribute("name").Value == type)
                {
                    type_block = i;
                    break;
                }
            }

            // There is another container for logical devices that is currently not used
            // this is left here in case that changes in the future
            var LD_list = type_block.Descendants(hmiNamespace + "LDevice");
            XElement LD_block = LD_list.Single(el => el.Attribute("inst").Value == "all");
            /*foreach (var i in LD_list)
            {
                if (i.Attribute("inst").Value == LD)
                {
                    LD_block = i;
                    break;
                }
            }*/

            var units = (from n in LD_block.Elements(hmiNamespace + "unit") where n.Attribute("name").Value == unit select n).ToArray();
            if (units.Length == 0)
            {
                throw new Exception("unit missing from hmi config file");
            }
            if (units.Length != 1)
            {
                throw new Exception("Duplicate unit instances found in hmi config file");
            }

            return float.Parse(units[0].Attribute("mmsMult").Value);
        }

        // Given an IED name like S311L_66HS1
        // This function will return the IED type - S311L
        // And the manufacturer - SEL
        // This is needed to find the mms multiplier in the HMI config
        private void IEDMakeModel(out string manufacturer, out string type, string iedName)
        {
            var ied = (from n in scd.Elements(scdIedNamespace + "IED") where n.Attribute("name").Value == iedName select n).ToArray();
            if (ied.Length == 0)
            {
                throw new Exception("IED missing from scd file");
            }
            if (ied.Length != 1)
            {
                throw new Exception("Duplicate IED's found in scd file");
            }
            manufacturer = ied[0].Attribute("manufacturer").Value;
            type = ied[0].Attribute("type").Value;
        }



    }
}