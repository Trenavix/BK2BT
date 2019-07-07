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

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;
using static Renderer;

namespace BK2BT
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        Camera cam = new Camera(GameScale.X);
        Vector2 oldXYDelta = new Vector2(0, 0);
        Vector2 XYEnd = new Vector2(0,0);
        String currentMainBinPath;
        String currentAlphaBinPath;
        bool isBTBin = false;
        MouseState state; KeyboardState keystate;
        public readonly float MouseSensitivity = 0.005f;

        public MainForm()
        {
            InitializeComponent();
            RenderPanel.Dock = DockStyle.Fill;
            OpenTK.Toolkit.Init();
            lastMousePos.X = (Bounds.Left + Bounds.Width / 2);
            RenderPanel.MouseDown += new MouseEventHandler(panel1_MouseDown);
            RenderPanel.MouseMove += new MouseEventHandler (panel1_MouseMove);
            RenderPanel.MouseUp += new MouseEventHandler(panel1_MouseUp);
            RenderPanel.KeyDown += new KeyEventHandler(RenderPanel_KeyDown);
            RenderPanel.KeyUp += new KeyEventHandler(RenderPanel_KeyUp);
            RenderPanel.Resize += new EventHandler(panel1_Resize);
            RenderPanel.Paint += new PaintEventHandler(RenderPanel_Paint);
            RenderPanel.MouseWheel += new MouseEventHandler(RenderPanel_MouseWheel);
            Bitmap white = new Bitmap(16, 1); Graphics whiteGraphics = Graphics.FromImage(white);
            whiteGraphics.FillRectangle(System.Drawing.Brushes.White, 0, 0, 16, 1); ColourPreview.Image = white;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void OpenMainBin(object sender, EventArgs e)//LoadMainBin
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
                    Renderer.Render(ClientRectangle, Width, Height, RenderPanel);
                    currentMainBinPath = OpenBIN.FileName;
                    UpdateStatusText();
                    ControlPanel.Visible = true;
                    viewToolStripMenuItem.Visible = true;
                }
                else { MessageBox.Show("File is not a BK Model Bin! Please try again.", "Invalid File!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void OpenAlphaBin(object sender, EventArgs e)//LoadAlphaBin
        {
            OpenFileDialog OpenAlphaBIN = OFD();
            OpenAlphaBIN.Filter = "BIN Files|*.bin";
            OpenAlphaBIN.Title = "Select an Alpha BIN File";
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
                    Renderer.Render(ClientRectangle, Width, Height, RenderPanel);
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
                        if (BinManager.AlphaBin != null) ReloadAlphaBin();
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
                        if (BinManager.AlphaBin != null) ReloadAlphaBin();
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
                "Mouse Scroll: Adjust Movement Speed\n" +
                "T: Toggle Textures\n" +
                "F: Toggle WireFrame\n" +
                "Space: Toggle Edges\n" + 
                "Ctrl+(Shift)+O: Open (Alpha) Bin\n" +
                "Ctrl+Z: Undo Paint\n" +
                "Ctrl+Y: Redo Paint\n" +
                "C: Eye-dropper\n" + 
                "R: Toggle Control Panel\n" + 
                "Esc: Toggle fullscreen",
                "Controls", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RenderPanel.Invalidate();
        }
        private void RenderPanel_Paint(object sender, PaintEventArgs e)
        {
            if (this.ContainsFocus) Renderer.cam.WASDMoveMent();
            Control control = sender as Control;
            state = Mouse.GetState();
            keystate = Keyboard.GetState();
            Point pt = control.PointToClient(Control.MousePosition);
            Vector2 Boundaries = new Vector2(RenderPanel.Width, RenderPanel.Height);
            if (ControlPanel.Visible) Boundaries.X -= ControlPanel.Size.Width; //shorten boundaries if control panel is open
            if (keystate[Key.C])
            {
                Cursor.Current = Cursors.Cross;
                if (state[MouseButton.Left])
                {
                    byte[] color = new byte[4];
                    GL.ReadPixels(pt.X, RenderPanel.Height - pt.Y - 1, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, color);
                    UpdateColour(color[0], color[1], color[2], color[3]);
                }
            }
            else Cursor.Current = Cursors.Default;
            if (state[MouseButton.Left] && !keystate[Key.C])
            {
                if (pt.X > 0 && pt.Y > 0 && pt.X < Boundaries.X && pt.Y < Boundaries.Y && this.ContainsFocus == true)//Left Click in GLControl & windowfocused
                {
                    float movementX = oldXYDelta.X + ((Control.MousePosition.X - lastMousePos.X) * MouseSensitivity); //last rotation + new rotation (Both are differences)
                    float movementY = oldXYDelta.Y + ((Control.MousePosition.Y - lastMousePos.Y) * MouseSensitivity);
                    if (movementY > 1.57) movementY = 1.57f;// limit Y axis rotation
                    else if (movementY < -1.57) movementY = -1.57f;
                    Renderer.cam.AddRotation(movementX, movementY);
                    XYEnd.X = movementX;
                    XYEnd.Y = movementY;
                }
                if (ControlPanel.Visible) // Colour Picker
                {
                    Point cursor = new Point();
                    GetCursorPos(ref cursor);
                    Point PaletteLocation = VertexRGBA.PointToScreen(RGBPalette.Location);
                    Point AlphaPaletteLocation = VertexRGBA.PointToScreen(AlphaPalette.Location);
                    if (cursor.X > PaletteLocation.X && cursor.X < PaletteLocation.X + RGBPalette.Size.Width && cursor.Y > PaletteLocation.Y && cursor.Y < PaletteLocation.Y + RGBPalette.Size.Height)
                    {
                        System.Drawing.Color PxColor = GetColorAt(cursor);
                        UpdateColour(PxColor.R, PxColor.G, PxColor.B);
                        //AlphaNum.Value = PxColor.A; //All alpha here is 255 so don't copy it
                    }
                    else if (cursor.X >= AlphaPaletteLocation.X && cursor.X <= AlphaPaletteLocation.X + AlphaPalette.Size.Width && cursor.Y > AlphaPaletteLocation.Y && cursor.Y < AlphaPaletteLocation.Y + AlphaPalette.Size.Height)
                    {
                        int right = AlphaPaletteLocation.X + AlphaPalette.Size.Width;
                        int alpha = Convert.ToInt32((double)((double)(right - cursor.X) / AlphaPalette.Size.Width) * (double)255);
                        if (alpha >= 252) alpha = 255;
                        else if (alpha <= 4) alpha = 0;
                        AlphaNum.Value = alpha;
                    }
                }
            }
            else //When not clicking, track mouse position
            {
                lastMousePos.X = Control.MousePosition.X;
                lastMousePos.Y = Control.MousePosition.Y;
            }
            if (state[MouseButton.Right] && pt.X > 0 && pt.Y > 0 && pt.X < Boundaries.X && pt.Y < Boundaries.Y && this.ContainsFocus == true)
            {
                EditVertex
                (
                ClientRectangle,
                Width,
                Height,
                RenderPanel,
                pt,
                (byte)Math.Round(RedNum.Value * ((decimal)Brightness.Value / 100)),
                (byte)Math.Round(GreenNum.Value * ((decimal)Brightness.Value / 100)),
                (byte)Math.Round(BlueNum.Value * ((decimal)Brightness.Value / 100)),
                (byte)AlphaNum.Value,
                AlphaOnlyCheckbox.Checked
                );
                byte[] color2 = new byte[4];
                GL.ReadPixels(pt.X, RenderPanel.Height - pt.Y - 1, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, color2);
                UInt32 addrr = (uint)((color2[0] << 24) | (color2[1] << 16) | (color2[2] << 8) | (color2[3]));
            }
            Render(ClientRectangle, Width, Height, RenderPanel);
        }

        Bitmap screenPixel = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        public System.Drawing.Color GetColorAt(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }

        private void UpdateColour(byte R, byte G, byte B, int A = 256)
        {
            RedNum.Value = R;
            GreenNum.Value = G;
            BlueNum.Value = B;
            if (A < 256) AlphaNum.Value = A;
        }

        private void UpdatePreviewColour(object sender, EventArgs e)
        {
            Bitmap colour = new Bitmap(16, 1);
            byte A = (byte)AlphaNum.Value;
            byte R = (byte)Math.Round(RedNum.Value * ((decimal)Brightness.Value / 100));
            byte G = (byte)Math.Round(GreenNum.Value * ((decimal)Brightness.Value / 100));
            byte B = (byte)Math.Round(BlueNum.Value * ((decimal)Brightness.Value / 100));
            Graphics colourGraphics = Graphics.FromImage(colour);
            SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(A, R, G, B));
            colourGraphics.FillRectangle(brush, 0, 0, 16, 1);
            ColourPreview.Image = colour;
        }

        //PaletteBoxes
        private void PaletteBox_Click(object sender, EventArgs e) { PaletteSelection((PictureBox)sender, e); }
        private void PaletteSelection(PictureBox PaletteBoxNum, EventArgs e)
        {
            System.Windows.Forms.MouseEventArgs me = (System.Windows.Forms.MouseEventArgs)e;
            if (me.Button == MouseButtons.Left && PaletteBoxNum.Image != null)
            {
                Bitmap palettecolour = (Bitmap)PaletteBoxNum.Image;
                System.Drawing.Color colour = palettecolour.GetPixel(0, 0);
                RedNum.Value = colour.R;
                GreenNum.Value = colour.G;
                BlueNum.Value = colour.B;
                AlphaNum.Value = colour.A;
            }
            else if (me.Button == MouseButtons.Right)
            {
                Bitmap colour = new Bitmap(16, 1);
                byte A = (byte)AlphaNum.Value;
                byte R = (byte)Math.Round(RedNum.Value * ((decimal)Brightness.Value / 100));
                byte G = (byte)Math.Round(GreenNum.Value * ((decimal)Brightness.Value / 100));
                byte B = (byte)Math.Round(BlueNum.Value * ((decimal)Brightness.Value / 100));
                Graphics colourGraphics = Graphics.FromImage(colour);
                SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(A, R, G, B));
                colourGraphics.FillRectangle(brush, 0, 0, 16, 1);
                PaletteBoxNum.Image = colour;
            }
        }

        void RenderPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Renderer.cam.MoveSpeed > 0f) Renderer.cam.MoveSpeed += 0.00000005f * Renderer.GameScale.X * e.Delta;
            if (Renderer.cam.MoveSpeed <= 0f) Renderer.cam.MoveSpeed = 0.0000025f * Renderer.GameScale.X;
        }

        void RenderPanel_KeyUp(object sender, KeyEventArgs e)
        {
            keystate = Keyboard.GetState();
            switch (e.KeyCode)
            {
                case Keys.F:
                    Renderer.WireFrameMode = !Renderer.WireFrameMode;
                    if (Renderer.WireFrameMode)
                    {
                        EdgesOption.Checked = false;
                        Renderer.EdgesOption = false;
                    }
                    break;
                case Keys.T:
                    Renderer.TextureEnabler = !Renderer.TextureEnabler;
                    break;
                case Keys.R:
                    if (BinManager.MainBin == null || keystate[Key.ControlLeft]) break; //No control panel if rom unopened or ctrl held (Ctrl+R = reload rom)
                    ControlPanel.Visible = !ControlPanel.Visible;
                    if (ControlPanel.Visible) { RenderPanel.Width -= ControlPanel.Width; }
                    else RenderPanel.Width += ControlPanel.Width;
                    break;
                case Keys.Enter:
                    if (!keystate[Key.AltRight]) break;
                    ToggleFullscreen();
                    break;
                case Keys.Escape:
                    ToggleFullscreen();
                    break;
                case Keys.Space:
                    Renderer.WireFrameMode = false;
                    EdgesOption.Checked = !EdgesOption.Checked;
                    Renderer.EdgesOption = EdgesOption.Checked;
                    break;
            }
        }

        private void ToggleFullscreen()
        {
            if (FormBorderStyle == FormBorderStyle.None)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
                RenderPanel.Location = new Point(0, 0);
            }
            else
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                RenderPanel.Location = new Point(0, 44);
            }
            StatusLabel.Visible = !StatusLabel.Visible; statusStrip1.Visible = !statusStrip1.Visible;
            menuStrip1.Visible = !menuStrip1.Visible;
        }

        void RenderPanel_KeyDown(object sender, KeyEventArgs e)
        {
            keystate = Keyboard.GetState();
            switch (e.KeyCode)
            {
                case Keys.Z:
                    if (!keystate[Key.ControlLeft] || Vertex.OriginalVertexMem[0] == null) break;
                    for (int j = 29; j > 0; j--) { Vertex.EditedVertexMem[j] = Vertex.EditedVertexMem[j - 1]; } //shift all mem back one (make room for redo)
                    Vertex.EditedVertexMem[0] = new UInt32[Vertex.OriginalVertexMem[0].Length][];
                    for (int i = 0; i < Vertex.EditedVertexMem[0].Length; i++) { Vertex.EditedVertexMem[0][i] = new UInt32[2]; }//set new space to this undo
                    for (uint i = 0; i < Vertex.OriginalVertexMem[0].Length; i++)
                    {
                        uint OriginalVTXValue = Vertex.OriginalVertexMem[0][i][0];
                        Vertex.EditedVertexMem[0][i][0] = OriginalVTXValue;
                        var BinAndAddr = BinManager.AttainCorrectBin(OriginalVTXValue);
                        Vertex.EditedVertexMem[0][i][1] = BinAndAddr.Item1.ReadFourBytes(BinAndAddr.Item2); //Write @ addr this colour for all addr+rgba in collection
                    }
                    for (uint i = 0; i < Vertex.OriginalVertexMem[0].Length; i++)
                    {
                        var BinAndAddr = BinManager.AttainCorrectBin(Vertex.OriginalVertexMem[0][i][0]);
                        BinAndAddr.Item1.WriteFourBytes(BinAndAddr.Item2, Vertex.OriginalVertexMem[0][i][1]); //Write @ addr this colour for all addr+rgba in collection
                    }
                    for (uint i = 0; i < 29; i++) { Vertex.OriginalVertexMem[i] = Vertex.OriginalVertexMem[i + 1]; } //Shift all mem forward one (forget the undo)
                    break;
                case Keys.Y:
                    if (!keystate[Key.ControlLeft] || Vertex.EditedVertexMem[0] == null) break;
                    for (int j = 29; j > 0; j--) { Vertex.OriginalVertexMem[j] = Vertex.OriginalVertexMem[j - 1]; } //Shift all mem back one (make room for undo)
                    Vertex.OriginalVertexMem[0] = new UInt32[Vertex.EditedVertexMem[0].Length][];
                    for (int i = 0; i < Vertex.OriginalVertexMem[0].Length; i++) { Vertex.OriginalVertexMem[0][i] = new UInt32[2]; }//set new space to this undo
                    for (uint i = 0; i < Vertex.EditedVertexMem[0].Length; i++)
                    {
                        uint EditedVTXAddr = Vertex.EditedVertexMem[0][i][0];
                        Vertex.OriginalVertexMem[0][i][0] = EditedVTXAddr;
                        var BinAndAddr = BinManager.AttainCorrectBin(EditedVTXAddr);
                        Vertex.OriginalVertexMem[0][i][1] = BinAndAddr.Item1.ReadFourBytes(BinAndAddr.Item2); //Write @ addr this colour for all addr+rgba in collection
                    }
                    for (uint i = 0; i < Vertex.EditedVertexMem[0].Length; i++)
                    {
                        var BinAndAddr = BinManager.AttainCorrectBin(Vertex.EditedVertexMem[0][i][0]);
                        BinAndAddr.Item1.WriteFourBytes(BinAndAddr.Item2, Vertex.EditedVertexMem[0][i][1]); //Write @ addr this colour for all addr+rgba in collection
                    }
                    for (uint i = 0; i < 29; i++) { Vertex.EditedVertexMem[i] = Vertex.EditedVertexMem[i + 1]; } //Shift all mem forward one (forget the redo)
                    break;
                case Keys.S:
                    if (!keystate[Key.ControlLeft] || BinNullCheck()) break;
                    Cursor.Current = Cursors.WaitCursor;
                    File.WriteAllBytes(currentMainBinPath, BinManager.MainBin.getCurrentBin()); //Ctrl+S to save
                    if(!AlphaBinNullCheck())File.WriteAllBytes(currentMainBinPath, BinManager.MainBin.getCurrentBin()); //Ctrl+S to save
                    StatusLabel.Text = "ROM Saved to " + currentMainBinPath;
                    Cursor.Current = Cursors.Default;
                    break;
                case Keys.O:
                    if (!keystate[Key.ControlLeft]) break;
                    if (!keystate[Key.LShift]) OpenMainBin(sender, e); //Ctrl+O to open
                    else if (!BinNullCheck())OpenAlphaBin(sender, e); //if ctrl shift is held, it will open alpha bin
                    break;
                case Keys.R:
                    if (!keystate[Key.ControlLeft] || BinNullCheck()) break;
                    ReloadMainBin(); //Ctrl+O to open
                    if (!AlphaBinNullCheck()) ReloadAlphaBin();
                    break;
            }
        }

        bool AlphaBinNullCheck()
        {
            return BinManager.AlphaBin == null;
        }

        bool BinNullCheck()
        {
            return BinManager.MainBin == null;
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
            while (RenderPanel.IsIdle)
            {
                Renderer.Render(ClientRectangle, Width, Height, RenderPanel);
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
            Renderer.Render(ClientRectangle, Width, Height, RenderPanel);
            UpdateStatusText();
        }

        private void ReloadAlphaBin()
        {
            BinManager.LoadAlphaBIN(currentAlphaBinPath);
            Renderer.Render(ClientRectangle, Width, Height, RenderPanel);
            UpdateStatusText();
        }

        private void UpdateStatusText()
        {
            StatusLabel.Text = Renderer.TriCount.ToString() + " triangles (" + Renderer.VertexCount.ToString() + " vertices) have been loaded to the preview. (Banjo-Kazooie)";
        }

        private void EdgesOption_Click(object sender, EventArgs e)
        {
            Renderer.EdgesOption = !Renderer.EdgesOption;
        }

        private void BaseCoatButton_Click(object sender, EventArgs e)
        {
            for (uint j = 29; j >= 1 && j <= 29; j--) //Shift all mem back one
            {
                Vertex.OriginalVertexMem[j] = Vertex.OriginalVertexMem[j - 1];
            }
            Vertex.OriginalVertexMem[0] = new UInt32[Vertex.CurrentVertexList.Length][]; //Set up new undo level with all combos
            for (uint i = 0; i < Vertex.CurrentVertexList.Length; i++)
            {
                Vertex.OriginalVertexMem[0][i] = new UInt32[2];
                uint currentVTX = Vertex.CurrentVertexList[i];
                Vertex.OriginalVertexMem[0][i][0] = currentVTX + 12;
                var BinAndAddr = BinManager.AttainCorrectBin(currentVTX);
                Vertex.OriginalVertexMem[0][i][1] = BinAndAddr.Item1.ReadFourBytes(BinAndAddr.Item2 + 12); //Initial RGBA
            }
            byte R = (byte)Math.Round(RedNum.Value * ((decimal)Brightness.Value / 100));
            byte G = (byte)Math.Round(GreenNum.Value * ((decimal)Brightness.Value / 100));
            byte B = (byte)Math.Round(BlueNum.Value * ((decimal)Brightness.Value / 100));
            byte A = (byte)AlphaNum.Value;
            UInt32 colour = (uint)((R << 24) | (G << 16) | (B << 8) | A);
            for (int i = 0; i < Vertex.CurrentVertexList.Length; i++)
            {
                var BinAndAddr = BinManager.AttainCorrectBin(Vertex.CurrentVertexList[i]);
                BinManager.SetVertRGBA(BinAndAddr.Item1, BinAndAddr.Item2, colour, AlphaOnlyCheckbox.Checked);
            }
        }
    }

    
}
