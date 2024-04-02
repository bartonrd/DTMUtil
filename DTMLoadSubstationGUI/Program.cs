using DTMLoadSubstationGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTMLoadSubstationGUI
{
    internal static class Program
    {
        static void Main()
        {
            LoadSubstation loadSubstation = new LoadSubstation();
            loadSubstation.StartGUI();
        }
    }
}

