using System;
using System.Collections.Generic;
using System.Management;

namespace DTMUtil
{
    public class Networking
    {
        private ManagementClass mc;
        private ManagementObjectCollection moc;

        public Networking()
        {
            mc = new ManagementClass("Win32_NetworkAdapter");
            moc = mc.GetInstances();
        }

        // Get all enabled network adapters
        public string[] GetNetworks()
        {
            List<string> list = new List<string>();

            try
            {
                foreach (ManagementObject mo in moc)
                {
                    // Ensure we're only working with network adapters that are enabled
                    if ((bool)mo["NetEnabled"])
                    {
                        string description = mo["Description"].ToString();
                        Console.WriteLine($"Network Adapter: {description}"); // Debugging line
                        list.Add(description);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while retrieving network adapters: {ex.Message}");
            }

            return list.ToArray();
        }

        // Set the static IP, Subnet, and Gateway for a given adapter
        public void SetIP(string nicName, string[] IpAddresses, string[] SubnetMasks, string Gateway)
        {
            try
            {
                foreach (ManagementObject mo in moc)
                {
                    // Find the matching network adapter based on description
                    if (mo["Description"].Equals(nicName))
                    {
                        Console.WriteLine("Found matching NIC: " + nicName); // Debugging line

                        // Parameters for setting static IP
                        ManagementBaseObject newIP = mo.GetMethodParameters("EnableStatic");
                        ManagementBaseObject newGate = mo.GetMethodParameters("SetGateways");

                        // Set Gateway
                        newGate["DefaultIPGateway"] = new string[] { Gateway };
                        newGate["GatewayCostMetric"] = new int[] { 1 };

                        // Set IP Address and Subnet Mask
                        newIP["IPAddress"] = IpAddresses;
                        newIP["SubnetMask"] = SubnetMasks;

                        // Apply settings
                        mo.InvokeMethod("EnableStatic", newIP, null);
                        mo.InvokeMethod("SetGateways", newGate, null);

                        Console.WriteLine("IP, Subnet, and Gateway set successfully.");
                        break; // Exit after setting the first matching adapter
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while setting IP settings: {ex.Message}");
            }
        }
    }
}
