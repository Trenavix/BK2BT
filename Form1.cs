using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BTBinFile;


namespace BK2BT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.  
            OpenFileDialog OpenBIN = OFD();
            OpenBIN.Filter = "BIN Files|*.bin";
            OpenBIN.Title = "Select a BIN File";
            if (OpenBIN.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StatusLabel.Text = OpenBIN.FileName;
                BinManager.LoadBIN(OpenBIN.FileName);
                saveBinForBTToolStripMenuItem.Enabled = true;

            }
        }
        private static OpenFileDialog OFD() => new OpenFileDialog();



        private void saveBinForBTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {

                
                // Displays a SaveFileDialog so the user can save the bin  
                SaveFileDialog saveBin = new SaveFileDialog();
                saveBin.Filter = "Bin File|*.bin";
                saveBin.Title = "Save a new bin file.";
                saveBin.ShowDialog();

                // If the file name is not an empty string open it for saving.  
                if (saveBin.FileName != "")
                {
                    // Saves the Image via a FileStream created by the OpenFile method.  
                    BinManager.ConvertBKtoBT();
                    File.WriteAllBytes(saveBin.FileName, BinManager.MainBin.getCurrentBin());
                }
            }


        }

        private void StatusLabel_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This programme is an early alpha of a Banjo-Kazooie\n" +
                "to Banjo-Tooie model bin file converter.\n" +
                "Its purpose is to convert two microcodes,Fast3DEX(Kazooie)\n" +
                "and Fast3DEX2(Tooie) along with adjust several parameters for the\n" +
                "model such as geometry layouts, texture setups, and draw distance, \n" +
                "all of which have new formats.\n\n" +
                "This programme was coded entirely by Trenavix, and is far from finished.\nYouTube.com/Trenavix\nPatreon.com/Trenavix", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
