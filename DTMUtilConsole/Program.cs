using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTMUtil;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace DTMUtilConsole
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Initializing Load Substation Form");
            LoadSubstation loadSubstation = new LoadSubstation();

            Console.WriteLine("Subscribing to Load Substation Form Events");
            loadSubstation.OnFormClosed += BeforeFormClosed;

            Console.WriteLine("Starting Load Substation Form");
            loadSubstation.StartGUI();

            Console.WriteLine("End Load Substation Form Test Program");
        }

        
        static string GetFileByExtention(string path, string extention)
        {

            // Get all the files with the specified extension in the directory
            string[] files = Directory.GetFiles(path, "*" + extention);

            return files[0];
        }

        private static void BeforeFormClosed(object sender, FormClosingEventArgs args)
        {
            Console.WriteLine($"Sender Type: {sender} | Arguement Type: {args.GetType()}");
            if (sender is LoadSubstationGUI loadSubstation)
            {
                Console.WriteLine("Ayeeeee");
                Console.WriteLine($"{loadSubstation.data.lastFolderSelected}");

                SCDReader reader = new SCDReader(GetFileByExtention(loadSubstation.data.lastFolderSelected, ".scd"));

                Console.WriteLine($"NODE NAME: {reader.GetSourceInfo()[0].nodeName}");

                // GetFileByExtention(loadSubstation.data.lastFolderSelected, ".scd")
                object[] objs = new
                    object[5];
                objs[0] = GetFileByExtention(loadSubstation.data.lastFolderSelected, ".pcd");
                objs[1] = "HMI";
                objs[2] = "EPAC";
                objs[3] = "CHGR";
                objs[4] = "ETM";

                ControlTestInfoInterChange controlTestInfo = new ControlTestInfoInterChange(objs);

                for (int i = 0; i < controlTestInfo.Count(); ++i)
                {
                    var test = controlTestInfo.Current();
                    Console.WriteLine($"Control Test: {test.control_name}");
                    controlTestInfo.MoveNext();

                    if (test.control_name.Contains("SETTINGS"))
                    {
                        Console.WriteLine($"Control Test: {test.control_name}");
                    }
                }

                string[] ipAddresses = new string[1];
                ipAddresses[0] = "10.81.20.128";

                string[] subnets = new string[1];
                subnets[0] = "255.255.254.0";

                loadSubstation.ParentClass.SetIP(ipAddresses, subnets, "10.81.20.9");

                string scdPath = GetFileByExtention(loadSubstation.data.lastFolderSelected, ".scd");
                SCDReader scdReader = new SCDReader(scdPath);
                var iedInfos = scdReader.GetIEDDeviceInfos();
                for (int i = 0; i < iedInfos.Length; i++)
                {
                    string name = iedInfos[i].name;
                    Console.WriteLine(name);
                }

                Console.WriteLine("Source Info------------------------------------------------");
                var sourceInfo = reader.GetSourceInfo();
                for (int i = 0; i < sourceInfo.Length; ++i)
                {
                    Console.WriteLine($"{sourceInfo[i].source}: {sourceInfo[i].flow}");
                }
                Console.WriteLine("-----------------------------------------------------------");
            }
            else
            {
                Console.WriteLine("form not of correct type");
            }

            //System.Threading.Thread.Sleep(10000);    
            Console.ReadLine();
        }
    }
}
