using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTMLoadSubstationGUI
{
    public class LoadSubstation
    {
        public LoadSubstation()
        {
            net = new Networking();
            data = new LoadSubstationData();
            data.networkAdapters = net.GetNetworks();
            LoadLastSelection(ref data);
            form = new LoadSubstationGUI(data);
        }
        public void StartGUI()
        {
            thread = new System.Threading.Thread(Run);
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
            thread.Join();
            WriteLastSelection();
        }
        public void SetIP(string[] ipAddresses, string[] subnetMasks, string gatewayIP)
        {
            net.SetIP(data.selectedAdapter, ipAddresses, subnetMasks, gatewayIP);
        }
        private void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(form);
        }

        private void LoadLastSelection(ref LoadSubstationData _data)
        {
            if (System.IO.File.Exists($"{Environment.CurrentDirectory}/NetworkAdapterChoice.txt"))
            {
                byte[] buffer = new byte[256];
                System.IO.FileStream file = System.IO.File.OpenRead($"{Environment.CurrentDirectory}/NetworkAdapterChoice.txt");
                try
                {
                    file.Read(buffer, 0, buffer.Length > file.Length ? (int)file.Length : buffer.Length);
                    var str = Encoding.ASCII.GetString(buffer);
                    var strs = str.Split(',');
                    _data.lastAdapterSelected = strs[0];
                    _data.lastFolderSelected = strs[1];
                }
                catch
                {
                    _data.lastAdapterSelected = "";
                    _data.lastFolderSelected = "";
                }
                finally
                {
                    file.Close();
                }
            }
            else
            {
                _data.lastAdapterSelected = "";
                _data.lastFolderSelected = "";
            }
        }
        private void WriteLastSelection()
        {
            System.IO.FileStream file = System.IO.File.OpenWrite($"{Environment.CurrentDirectory}/NetworkAdapterChoice.txt");
            try
            {
                byte[] buffer = new byte[256];
                buffer = Encoding.ASCII.GetBytes(data.lastAdapterSelected + "," + data.lastFolderSelected);
                file.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                file.Close();
            }
        }

        private System.Threading.Thread thread;
        private Networking net;
        private LoadSubstationGUI form;
        public LoadSubstationData data;
    }
    public class LoadSubstationData
    {
        public LoadSubstationData() { }
        public string folderLocation = "";
        public string[] networkAdapters;
        public string selectedAdapter = "";
        public string lastAdapterSelected = "";
        public bool userWantsToContinue = false;
        public string lastFolderSelected = "";
    }
}