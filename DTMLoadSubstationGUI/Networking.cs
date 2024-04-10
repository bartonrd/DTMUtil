using System;
using System.Collections.Generic;
using System.Management;
namespace DTMUtil
{

    public class Networking
    {
        public Networking()
        {
            mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            moc = mc.GetInstances();
        }
        public string[] GetNetworks()
        {
            List<string> list = new List<string>();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"])
                {
                    list.Add(mo["Description"].ToString());
                }
            }
            return list.ToArray();
        }

        public void SetIP(string nicName, string[] IpAddresses,
            string[] SubnetMasks, string Gateway)
        {
            foreach (ManagementObject mo in moc)
            {
                if (mo["Description"].Equals(nicName))
                {
                    Console.WriteLine("Inside NicName block");
                    ManagementBaseObject newIP =
                        mo.GetMethodParameters("EnableStatic");
                    ManagementBaseObject newGate =
                        mo.GetMethodParameters("SetGateways");

                    newGate["DefaultIPGateway"] = new string[] { Gateway };
                    newGate["GatewayCostMetric"] = new int[] { 1 };

                    newIP["IPAddress"] = IpAddresses;
                    newIP["SubnetMask"] = SubnetMasks;

                    ManagementBaseObject setIP = mo.InvokeMethod(
                        "EnableStatic", newIP, null);
                    ManagementBaseObject setGateways = mo.InvokeMethod(
                        "SetGateways", newGate, null);
                    break;
                }

            }
        }

        ManagementClass mc;
        ManagementObjectCollection moc;
    }
}
