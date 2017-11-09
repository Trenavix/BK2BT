/****************************************************************************
*                                                                           *
* BK2BT - An N64 Graphics Microcode Converter                               *
* https://www.YouTube.com/Trenavix/                                         *
* Copyright (C) 2017 Trenavix. All rights reserved.                         *
*                                                                           *
* License:                                                                  *
* GNU/GPLv2 http://www.gnu.org/licenses/gpl-2.0.html                        *
*                                                                           *
****************************************************************************/

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
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using static Renderer;
using System.Windows.Media;

namespace BK2BT
{
    public partial class Form1 : Form
    {
        Camera cam = new Camera();
        Vector2 oldXYDelta = new Vector2(0, 0);
        Vector2 XYEnd = new Vector2(0,0);
        String currentMainBinPath;
        String currentAlphaBinPath;
        bool isBTBin = false;

        public Form1()
        {
            InitializeComponent();
            panel1.Dock = DockStyle.Fill;
            OpenTK.Toolkit.Init();
            lastMousePos.X = (Bounds.Left + Bounds.Width / 2);
            panel1.MouseDown += new MouseEventHandler(panel1_MouseDown);
            panel1.MouseMove += new MouseEventHandler (panel1_MouseMove);
            panel1.MouseUp += new MouseEventHandler(panel1_MouseUp);
            panel1.KeyDown += new KeyEventHandler(panel1_KeyDown);
            panel1.KeyUp += new KeyEventHandler(panel1_KeyUp);
            panel1.Resize += new EventHandler(panel1_Resize);
            panel1.Paint += new PaintEventHandler(panel1_Paint);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void bKBinToolStripMenuItem_Click(object sender, EventArgs e)//LoadMainBin
        {
            // Displays an OpenFileDialog so the user can select a Cursor.  
            OpenFileDialog OpenBIN = OFD();
            OpenBIN.Filter = "BIN Files|*.bin";
            OpenBIN.Title = "Select a BIN File";
            if (OpenBIN.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool isBinFile = false;
                using (FileStream fs = new FileStream(OpenBIN.FileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] Header = new byte[16]; fs.Read(Header, 0, 16); //Load header into bytearray
                    if (Header[3] == 0x0B && Header[0] == 0x00) { isBinFile = true; } //Check if header is correct
                    if (Header[9] == 0x50) isBTBin = true;
                    else isBTBin = false;
                    fs.Close();
                }
                if (isBinFile)
                {
                    if (isBTBin) { DisplayBTBinMessage(); return; } //BT Not supported YET
                    BinManager.LoadBIN(OpenBIN.FileName);
                    SaveBinMenu.Enabled = true; SaveBinAs.Enabled = true;
                    LoadBKAlphaBin.Enabled = true;
                    Renderer.Render(ClientRectangle, Width, Height, panel1);
                    currentMainBinPath = OpenBIN.FileName;
                    UpdateStatusText();
                }
                else { MessageBox.Show("File is not a BK Model Bin! Please try again.", "Invalid File!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void bKAlphaBinToolStripMenuItem_Click(object sender, EventArgs e)//LoadAlphaBin
        {
            OpenFileDialog OpenAlphaBIN = OFD();
            OpenAlphaBIN.Filter = "BIN Files|*.bin";
            OpenAlphaBIN.Title = "Select a BIN File";
            if (OpenAlphaBIN.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool isAlphaBinFile = false;
                using (FileStream fs = new FileStream(OpenAlphaBIN.FileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] Header = new byte[4]; fs.Read(Header, 0, 4); //Load header into bytearray
                    if (Header[3] == 0x0B && Header[0] == 0x00) { isAlphaBinFile = true; } //Check if header is correct
                    fs.Close();
                }
                if (isAlphaBinFile)
                {
                    BinManager.LoadAlphaBIN(OpenAlphaBIN.FileName);
                    Renderer.Render(ClientRectangle, Width, Height, panel1);
                    SaveAlphaBinAs.Enabled = true;
                    currentAlphaBinPath = OpenAlphaBIN.FileName;
                    UpdateStatusText();
                }
                else { MessageBox.Show("Alpha File is not a BK Model Bin! Please try again.", "Invalid File!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void bKBinToolStripMenuItem1_Click(object sender, EventArgs e)//Save MainBin
        {
            // Displays a SaveFileDialog so the user can save the bin  
            SaveFileDialog saveBin = new SaveFileDialog();
            saveBin.Filter = "BT Bin File|*.bin|BK Bin File|*.bin";
            saveBin.Title = "Save a new bin file.";
            saveBin.ShowDialog();
            if (saveBin.FileName != "") // If the file name is not an empty string open it for saving.  
                switch (saveBin.FilterIndex)
            {
                case 1:
                        BinManager.ConvertBKtoBT(BinManager.MainBin);
                        File.WriteAllBytes(saveBin.FileName, BinManager.MainBin.getCurrentBin());
                        ReloadMainBin(); //BK was converted to BT, must convert back or renderer can't read it!
                        break;
                case 2: 
                        BinManager.ConvertBTtoBK(BinManager.MainBin);
                        File.WriteAllBytes(saveBin.FileName, BinManager.MainBin.getCurrentBin());
                        break;
            }
        }

        private void SaveAlphaBinAs_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the bin  
            SaveFileDialog saveBin = new SaveFileDialog();
            saveBin.Filter = "BT Alpha Bin File|*.bin|BK Alpha Bin File|*.bin";
            saveBin.Title = "Save a new bin file.";
            saveBin.ShowDialog();
            if (saveBin.FileName != "") // If the file name is not an empty string open it for saving.  
                switch (saveBin.FilterIndex)
                {
                    case 1:
                        BinManager.ConvertBKtoBT(BinManager.AlphaBin);
                        File.WriteAllBytes(saveBin.FileName, BinManager.AlphaBin.getCurrentBin());
                        ReloadMainBin();//BK was converted to BT, must convert back or renderer can't read it!
                        break;
                    case 2:
                        BinManager.ConvertBTtoBK(BinManager.AlphaBin);
                        File.WriteAllBytes(saveBin.FileName, BinManager.AlphaBin.getCurrentBin());
                        break;
                }
        }

        private static OpenFileDialog OFD() => new OpenFileDialog();

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This program is an early alpha of a Banjo-Kazooie\n" +
                "to Banjo-Tooie model bin file converter.\n" +
                "Its purpose is to convert two microcodes, Fast3DEX(Kazooie)\n" +
                "and Fast3DEX2(Tooie) along with adjust several parameters for the\n" +
                "model such as geometry layouts, texture setups, and draw distance, \n" +
                "all of which have new formats.\n\n" +
                "This program was coded entirely by Trenavix, and is far from finished.\n"+
                "It is licensed by GPL and is available at: \nGitHub.com/Trenavix/BK2BT\n"+
                "Author's pages:\nYouTube.com/Trenavix\nPatreon.com/Trenavix", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void controlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("W/S: Move Forward/Backward\n" +
                "A/S: Move Left/Right\n" +
                "Q/E: Move Up/Down\n" +
                "Mouse Click & Drag: Rotate Camera\n" +
                "Left Shift: Double Movement Speed\n" +
                "T: Toggle Textures\n" +
                "F: Toggle WireFrame\n" +
                "R: Reload Bin Files (If Modified)",
                "Controls", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (this.ContainsFocus) { Renderer.cam.WASDMoveMent(); }
            MouseState state = Mouse.GetState();
            Control control = sender as Control;
            Point pt = control.PointToClient(Control.MousePosition);
            //StatusLabel.Text = "CAMERA X: " + Renderer.cam.Orientation.X + ", CAMERA Y: " + Renderer.cam.Orientation.Y + ", CAMERA Z: " + Renderer.cam.Position.Z+", MOUSE: " + pt;
            if (state[MouseButton.Left] && pt.X > 0 && pt.Y > 0 && pt.X < panel1.Width && pt.Y < panel1.Height && this.ContainsFocus==true) //Left Click in GLControl & windowfocused
            {
                float movementX = oldXYDelta.X + ((Control.MousePosition.X - lastMousePos.X)); //last rotation + new rotation (Both are differences)
                float movementY = oldXYDelta.Y + ((Control.MousePosition.Y - lastMousePos.Y));
                if (-movementY * Renderer.cam.MouseSensitivity > Math.PI/2) { movementY = ((float)-Math.PI/2) / Renderer.cam.MouseSensitivity; }
                if (-movementY * Renderer.cam.MouseSensitivity < -Math.PI/2) { movementY = ((float)Math.PI/2) / Renderer.cam.MouseSensitivity; }
                Renderer.cam.AddRotation(movementX, movementY);
                XYEnd.X = movementX;
                XYEnd.Y = movementY;
                //StatusLabel.Text += ", MOVEMENTX: " + movementX.ToString() + ", MOVEMENTY: " + movementY.ToString();
            }
            else //When not clicking, track mouse position
            {
                lastMousePos.X = Control.MousePosition.X;
                lastMousePos.Y = Control.MousePosition.Y;
            }
            Renderer.Render(ClientRectangle, Width, Height, panel1);
        }

        void panel1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F)
            {
                Renderer.WireFrameMode = !Renderer.WireFrameMode;
            }
            if (e.KeyCode == Keys.T)
            {
                Renderer.TextureEnabler = !Renderer.TextureEnabler;
            }
            if (e.KeyCode == Keys.R && BinManager.MainBin != null)
            {
                ReloadMainBin();
                if (BinManager.AlphaBin == null) return;
                ReloadAlphaBin();
            }
        }

        void panel1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        void panel1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
                
        }

        void panel1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        void panel1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            oldXYDelta.X = XYEnd.X;
            oldXYDelta.Y = XYEnd.Y;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (panel1.IsIdle)
            {
                Renderer.Render(ClientRectangle, Width, Height, panel1);
            }
        }
       
        private void saveBinForBKToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void panel1_Load(object sender, EventArgs e)
        {

        }
        void panel1_Resize(object sender, EventArgs e)
        {

        }
        private void StatusLabel_Click(object sender, EventArgs e)
        {

        }
        private void openBinToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void DisplayBTBinMessage()
        {
            MessageBox.Show("Banjo-Tooie Bin Files are not yet supported. \nPlease only load BK Bins for now.", "BT Bin Detected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ReloadMainBin()
        {
            BinManager.LoadBIN(currentMainBinPath);
            Renderer.Render(ClientRectangle, Width, Height, panel1);
            UpdateStatusText();
        }

        private void ReloadAlphaBin()
        {
            BinManager.LoadAlphaBIN(currentAlphaBinPath);
            Renderer.Render(ClientRectangle, Width, Height, panel1);
            UpdateStatusText();
        }

        private void UpdateStatusText()
        {
            StatusLabel.Text = Renderer.TriCount.ToString() + " triangles (" + Renderer.VertexCount.ToString() + " vertices) have been loaded to the preview. (Banjo-Kazooie)";
        }

    }

    
}
