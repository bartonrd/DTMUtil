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

        public void SetIP(string nicName, string[] IpAddresses, string[] SubnetMasks, string Gateway)
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (mo["Description"].Equals(nicName))
                {
                    Console.WriteLine("Inside NicName block");

                    // Ensure IP and Subnet are correctly formatted
                    if (IpAddresses == null || IpAddresses.Length == 0 || SubnetMasks == null || SubnetMasks.Length == 0)
                    {
                        Console.WriteLine("IP Addresses or Subnet Masks are empty!");
                        return;
                    }

                    // Validate the Gateway format
                    if (string.IsNullOrEmpty(Gateway) || !IsValidIPAddress(Gateway))
                    {
                        Console.WriteLine("Invalid Gateway format.");
                        return;
                    }

                    Console.WriteLine($"Setting IP: {IpAddresses[0]}, Subnet: {SubnetMasks[0]}, Gateway: {Gateway}");

                    // Prepare parameters for static IP and gateway
                    ManagementBaseObject newIP = mo.GetMethodParameters("EnableStatic");
                    ManagementBaseObject newGate = mo.GetMethodParameters("SetGateways");

                    // Assign the IP and Subnet mask
                    newIP["IPAddress"] = IpAddresses;
                    newIP["SubnetMask"] = SubnetMasks;

                    // Assign the default gateway
                    newGate["DefaultIPGateway"] = new string[] { Gateway };
                    newGate["GatewayCostMetric"] = new int[] { 1 };

                    // Invoke the method to set static IP
                    ManagementBaseObject setIP = mo.InvokeMethod("EnableStatic", newIP, null);
                    uint returnValue = Convert.ToUInt32(setIP.Properties["ReturnValue"].Value);
                    Console.WriteLine($"EnableStatic Return Value: {returnValue}");

                    if (returnValue == 0)
                    {
                        Console.WriteLine("Static IP set successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to set static IP. Error code: {returnValue}");
                    }

                    // Invoke the method to set gateway
                    ManagementBaseObject setGateways = mo.InvokeMethod("SetGateways", newGate, null);
                    uint returnGateValue = Convert.ToUInt32(setGateways.Properties["ReturnValue"].Value);
                    Console.WriteLine($"SetGateways Return Value: {returnGateValue}");

                    if (returnGateValue == 0)
                    {
                        Console.WriteLine("Gateway set successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to set gateway. Error code: {returnGateValue}");
                    }

                    break;
                }
            }
        }

        // Helper function to validate IP address format
        private bool IsValidIPAddress(string ip)
        {
            System.Net.IPAddress address;
            return System.Net.IPAddress.TryParse(ip, out address);
        }




        ManagementClass mc;
        ManagementObjectCollection moc;
    }
}
