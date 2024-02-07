using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTMUtil
{
    public struct IEDDeviceInfo
    {
        public string name;
        public string type;
        public string manufacturer;
        public string ipAddress;
        public string ipMask;
        public string configVersion;
        public string gateway;
        override public string ToString()
        {
            return $"name: {name}\n\ttype: {type}\n\tmanufacturer: {manufacturer}\n\tipAddress: {ipAddress}\n\tipMask: {ipMask}\n\tConfigVersion: {configVersion}\n";
        }
    }
}
