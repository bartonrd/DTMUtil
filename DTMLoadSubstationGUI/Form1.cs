using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTMUtil
{
    public partial class LoadSubstationGUI : Form
    {
        public LoadSubstation ParentClass { get; internal set; }
        public LoadSubstationGUI(LoadSubstationData data)
        {
            InitializeComponent();
            this.data = data;
            var objForCombo = data.networkAdapters.Cast<Object>().ToArray();
            this.comboBox1.Items.AddRange(objForCombo);
            this.comboBox1.SelectedIndex = this.comboBox1.FindStringExact(data.lastAdapterSelected) == -1 ? 0 : this.comboBox1.FindStringExact(data.lastAdapterSelected);
        }

        // User presses folder button
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = data.lastFolderSelected;
            DialogResult = folderBrowserDialog1.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                data.folderLocation = folderBrowserDialog1.SelectedPath;
                textBox1.Text = folderBrowserDialog1.SelectedPath;

            }
        }

        // User presses continue
        private void button2_Click(object sender, EventArgs e)
        {
            if (data.folderLocation != textBox1.Text)
                data.folderLocation = textBox1.Text;
            data.selectedAdapter = this.comboBox1.Text;
            data.userWantsToContinue = true;
            data.lastAdapterSelected = this.comboBox1.Text;
            data.lastFolderSelected = folderBrowserDialog1.SelectedPath;
            this.Close();
        }

        //User presses exit
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private LoadSubstationData data;

        

        private void LoadSubstationGUI_Load(object sender, EventArgs e)
        {

        }
    }
}
