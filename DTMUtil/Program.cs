using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTMUtil
{
    internal static class Program
    {
        static void Main()
        {
            LoadSubstation loadSubstation = new LoadSubstation();
            loadSubstation.StartGUI();
            loadSubstation.OnFormClosed += BeforeFormClosed;
        }

        private static void BeforeFormClosed(object sender, FormClosingEventArgs args)
        {
            if(sender is LoadSubstation loadSubstation)
            {
                Console.WriteLine("Ayeeeee");
            }
            else
            {
                Console.WriteLine("form not of correct type");
            }
        }
    }
}

