using System;
using System.IO;

public class BinManager
{
    public static BTBinFile MainBin;

    public static void LoadBIN(String BinDirectory)
    {
        using (FileStream fs = new FileStream(BinDirectory, FileMode.Open, FileAccess.Read))
        {
            byte[] binFile = new byte[fs.Length];
            fs.Read(binFile, 0, (int)fs.Length);
            BTBinFile NewBTBin = new BTBinFile(binFile);
            MainBin = NewBTBin;
            fs.Close();
        }
    }

    public static void ConvertBKtoBT()
    {
        F3DEXtoF3DEX2();
        TextureHeaderBK2BT();
        useCommonVertexHeader();
        newGeoLayout();//MUST BE LAST, space allocation
    }

    public static void F3DEXtoF3DEX2()
    {
        int CommandsLength = MainBin.F3DCommandsLength();
        Int32 F3DSetupAddr = MainBin.ReadFourBytes(0x0C);
        byte[][] DisplayList = new byte[CommandsLength][];
        int newCommandAddr = F3DSetupAddr+0x08;
        //Copy DL into 2D Byte array with double loop 
        for (int i = 0; i < CommandsLength; i++)
        {
            DisplayList[i] = new byte[8];
            for (int j = 0; j < 8; j++)
            {
                DisplayList[i][j] = MainBin.getByte(newCommandAddr+(i*8)+j);
            }
        }
        //change or convert commands
        byte MaxVertIndex = 0x00;
        bool firstVTX = true;
        int previousVTX = 0;
        for (int i = 0; i < DisplayList.Length; i++)
        {
            switch(DisplayList[i][0])
            {
                case 0x01:
                    DisplayList[i][0] = 0xDA;
                    break;
                case 0x03:
                    DisplayList[i][0] = 0xDC;
                    break;
                case 0x04:
                    DisplayList[i][0] = 0x01;
                    Int16 numvert = (Int16)(DisplayList[i][2] / 4);
                    DisplayList[i][1] = (byte)(numvert >> 4);
                    DisplayList[i][2] = (byte)(numvert << 4);
                    numvert = (short)(numvert * 2);
                    DisplayList[i][3] = (byte)(numvert);

                    if (firstVTX) { firstVTX = false; previousVTX = i; break; }
                    byte MaxVertNum = (byte)((MaxVertIndex / 2) + 1);
                    DisplayList[previousVTX][1] = (byte)(MaxVertNum >> 4);
                    DisplayList[previousVTX][2] = (byte)(MaxVertNum << 4);
                    DisplayList[previousVTX][3] = (byte)(MaxVertIndex+2);
                    MaxVertIndex = 0x00;
                    previousVTX = i;
                    break;
                case 0x06:
                    DisplayList[i][0] = 0xDE;
                    break;
                case 0xB1:
                    DisplayList[i][0] = 0x06;
                    MaxVertIndex = MaxVertexCheck(MaxVertIndex, DisplayList, i);
                    break;
                case 0xB2:
                    DisplayList[i][0] = 0x02;
                    break;
                case 0xB3:
                    DisplayList[i][0] = 0xF1;
                    break;
                case 0xB4:
                    DisplayList[i][0] = 0xE1;
                    break;
                case 0xB5:
                    DisplayList[i][0] = 0x07;
                    MaxVertIndex = MaxVertexCheck(MaxVertIndex, DisplayList, i);
                    break;
                case 0xB6:
                    DisplayList[i][0] = 0xD9;
                    if (DisplayList[i+1][5] == 0x08 && DisplayList[i+1][6] == 0x22 && DisplayList[i+1][7] == 0x04) //VertRGBABackCull, i+1 for SETGEO
                    {
                    DisplayList[i][1] = 0xC0;
                    DisplayList[i][2] = 0xFF;
                    DisplayList[i][3] = 0xFB;
                    }
                    else if (DisplayList[i+1][5] == 0x08 && DisplayList[i+1][6] == 0x02 && DisplayList[i+1][7] == 0x04) //VertRGBANoCull, i+1 for SETGEO
                    {
                        DisplayList[i][1] = 0xC0;
                        DisplayList[i][2] = 0xF9;
                        DisplayList[i][3] = 0xFB;
                    }
                    DisplayList[i][5] = 0x00; //NOP Clear for now
                    DisplayList[i][6] = 0x00;
                    DisplayList[i][7] = 0x00;
                    break;
                case 0xB7:
                    DisplayList[i][0] = 0xD9;
                    DisplayList[i][1] = 0xFF; //NOP Set for now
                    DisplayList[i][2] = 0xFF;
                    DisplayList[i][3] = 0xFF;
                    DisplayList[i][5] = 0x20; //VertRGBA only for now (later env mapping flag)
                    DisplayList[i][6] = 0x00;
                    DisplayList[i][7] = 0x04;
                    break;
                case 0xB9:
                    DisplayList[i][0] = 0xE2;
                    break;
                case 0xBA:
                    DisplayList[i][0] = 0xE3;
                    break;
                case 0xBB:
                    DisplayList[i][0] = 0xD7;
                    break;
                case 0xBC:
                    DisplayList[i][0] = 0xDB;
                    break;
                case 0xBD:
                    DisplayList[i][0] = 0xD8;
                    break;
                case 0xBF:
                    DisplayList[i][0] = 0x05;
                    DisplayList[i][1] = DisplayList[i][5];
                    DisplayList[i][2] = DisplayList[i][6];
                    DisplayList[i][3] = DisplayList[i][7];
                    for (int j = 5; j < 8; j++) { DisplayList[i][j] = 0x00; }
                    MaxVertIndex = MaxVertexCheck(MaxVertIndex, DisplayList, i);
                    break;
                case 0xF0:
                    if (DisplayList[i][4] == 0x01) { DisplayList[i][4] = 0x06; }
                    break;
                case 0xF5:
                    if (DisplayList[i][4] == 0x01) { DisplayList[i][4] = 0x06; }
                    break;
                case 0xB8:
                    DisplayList[i][0] = 0xDF;
                    if (i != DisplayList.Length - 1) { DisplayList[i][0] = 0x00; }
                    break;

            }
            

        }
        //Load converted DL back into bin
        for (int i = 0; i < CommandsLength; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                MainBin.changeByte((newCommandAddr + (i * 8) + j), DisplayList[i][j]);
            }
        }
    }

    public static byte MaxVertexCheck(byte MaxVertIndex, byte[][] DisplayList, int currentCommand)
    {
        byte CommandMax = 0;
        for (int j = 1; j < 8; j++)
        {
            byte VertIndex = (DisplayList[currentCommand][j]);
            if (VertIndex > MaxVertIndex) CommandMax = VertIndex;
        }
        return CommandMax;
    }


    public static void TextureHeaderBK2BT()
    {
        
        Int16 SetupAddr = MainBin.getTextureSetupAddr();
        MainBin.copyBytes(SetupAddr, 0x50, MainBin.endBinAddr()-SetupAddr); //Shift bin file so TexSetup is at 0x50
        MainBin.WriteTwoBytes(0x08, 0x0050);
        for (int i = 0; i < 4; i++) { MainBin.WriteEightBytes(SetupAddr-8 + (i * 8), 0); } //fill ext texture addr with 0
        SetupAddr = MainBin.getTextureSetupAddr();
        if (MainBin.getByte(SetupAddr+0x06) == 01) { return; } //External texture flag
        Int16 numTextures = MainBin.ReadTwoBytes(SetupAddr + 4);
        int BKCommandBeginAddr = SetupAddr+8;
        int BTCommandBeginAddr = SetupAddr + 8;
        for (int i = 0; i < numTextures; i++) //copy 128bit commands into 64bit commands
        {
            MainBin.copyBytes(BKCommandBeginAddr, BTCommandBeginAddr, 6);//TexAddr & Type
            MainBin.copyBytes(BKCommandBeginAddr+8, BTCommandBeginAddr+6, 2);//XX and YY grid
            BTCommandBeginAddr += 0x08;
            BKCommandBeginAddr += 0x10;
        }
        MainBin.copyBytes(BKCommandBeginAddr, BTCommandBeginAddr, MainBin.endBinAddr()-BKCommandBeginAddr);//shift all data back
        MainBin.changeEndBinAddr(MainBin.endBinAddr() - (numTextures * 8));//remove data at end after backshift
        int AddressesShift = (0x18 - (numTextures * 8));
        Int32 TexLoadBytes = MainBin.ReadFourBytes(0x50)-(numTextures*8);
        Int32 NewGeoAddr = MainBin.ReadFourBytes(0x04) + AddressesShift;//declare new addresses after shift
        Int32 NewF3DEX2Addr = MainBin.ReadFourBytes(0x0C) + AddressesShift;
        Int32 NewVertAddr = MainBin.ReadFourBytes(0x10) + AddressesShift;
        Int32 NewCollisionAddr = MainBin.ReadFourBytes(0x1C) + AddressesShift;
        MainBin.WriteFourBytes(0x50, TexLoadBytes);
        MainBin.WriteFourBytes(0x04, NewGeoAddr);
        MainBin.WriteFourBytes(0x0C, NewF3DEX2Addr);
        MainBin.WriteFourBytes(0x10, NewVertAddr);
        MainBin.WriteFourBytes(0x1C, NewCollisionAddr);
        MainBin.changeByte(0x0B, 0x00);
    }
    public static void newGeoLayout()
    {
        int GeoAddress = MainBin.ReadFourBytes(0x04);
        MainBin.writeByteArray(GeoAddress, BTGeoLayout());
        MainBin.changeEndBinAddr(GeoAddress+0x1C);
    }
    public static void useCommonVertexHeader()
    {
        Int32 VertexSetupAddr = MainBin.ReadFourBytes(0x10);
        MainBin.writeByteArray(VertexSetupAddr, BTVertexHeader());
    }

    private enum CMD
    {
        F3DEX2_NOOP = 0x00,
        F3DEX2_MTX = 0xDA,
        F3DEX2_MOVEMEM = 0xDC,
        F3DEX2_VTX = 0x01,
        F3DEX2_DL = 0xDE,
        F3DEX2_GEOMETRYMODE = 0xD9,
        F3DEX2_ENDDL = 0xDF,
        F3DEX2_SETOTHERMODE_L = 0xE2,
        F3DEX2_SETOTHERMODE_H = 0xE3,
        F3DEX2_TEXTURE = 0xD7,
        F3DEX2_MOVEWORD = 0xDB,
        F3DEX2_POPMTX = 0xD8,
        F3DEX2_CULLDL = 0x03,
        F3DEX2_TRI1 = 0x05,
        F3DEX2_TRI2 = 0x06,
        G_TEXRECT = 0xE4,
        G_TEXRECTFLIP = 0xE5,
        G_RDPLOADSYNC = 0xE6,
        G_RDPPIPESYNC = 0xE7,
        G_RDPTILESYNC = 0xE8,
        G_RDPFULLSYNC = 0xE9,
        G_SETKEYGB = 0xEA,
        G_SETKEYR = 0xEB,
        G_SETCONVERT = 0xEC,
        G_SETSCISSOR = 0xED,
        G_SETPRIMDEPTH = 0xEE,
        G_RDPSETOTHERMODE = 0xEF,
        G_LOADTLUT = 0xF0,
        G_SETTILESIZE = 0xF2,
        G_LOADBLOCK = 0xF3,
        G_SETTILE = 0xF5,
        G_FILLRECT = 0xF6,
        G_SETFILLCOLOR = 0xF7,
        G_SETFOGCOLOR = 0xF8,
        G_SETBLENDCOLOR = 0xF9,
        G_SETPRIMCOLOR = 0xFA,
        G_SETENVCOLOR = 0xFB,
        G_SETCOMBINE = 0xFC,
        G_SETTIMG = 0xFD,
        G_SETZIMG = 0xFE,
        G_SETCIMG = 0xFF
    }

    public static byte[] BTGeoLayout()
    {
        byte[] geolayout = new byte[0x1C];
        geolayout[0] = 0x00; geolayout[1] = 0x00; geolayout[2] = 0x00; geolayout[3] = 0x03;
        geolayout[4] = 0x00; geolayout[5] = 0x00; geolayout[6] = 0x00; geolayout[7] = 0x10;
        geolayout[8] = 0x00; geolayout[9] = 0x00; geolayout[0x0a] = 0x00; geolayout[0x0b] = 0x30;
        geolayout[0x0c] = 0x00; geolayout[0x0d] = 0x00; geolayout[0x0e] = 0x00; geolayout[0x0f] = 0x00;
        geolayout[0x10] = 0x00; geolayout[0x11] = 0x00; geolayout[0x12] = 0x00; geolayout[0x13] = 0x03;
        geolayout[0x14] = 0x00; geolayout[0x15] = 0x00; geolayout[0x16] = 0x00; geolayout[0x17] = 0x00;
        geolayout[0x18] = 0x00; geolayout[0x19] = 0x00; geolayout[0x1A] = 0x00; geolayout[0x1B] = 0x30;
        return geolayout;
    }

    public static byte[] BTVertexHeader()
    {
        int VertexStartAddr = MainBin.ReadFourBytes(0x10) + 0x18;
        Int16 numVerts = ((Int16)((MainBin.ReadFourBytes(0x1C) - VertexStartAddr)/0x10));
        byte numVertsByte1;
        byte numVertsByte2;
        FromShortToByte(numVerts, out numVertsByte1, out numVertsByte2);
        byte[] geolayout = new byte[24];
        geolayout[0] = 0xBC; geolayout[1] = 0x47; geolayout[2] = 0xED; geolayout[3] = 0xAE;
        geolayout[4] = 0xC1; geolayout[5] = 0x40; geolayout[6] = 0x3E; geolayout[7] = 0x01;
        geolayout[8] = 0x37; geolayout[9] = 0x58; geolayout[0x0a] = 0x3D; geolayout[0x0b] = 0x2A;
        geolayout[0x0c] = 0xFD; geolayout[0x0d] = 0x24; geolayout[0x0e] = 0x12; geolayout[0x0f] = 0x83;
        geolayout[0x10] = 0xFF; geolayout[0x11] = 0x35; geolayout[0x12] = 0x53; geolayout[0x13] = 0x9B;
        geolayout[0x14] = 0xA0; geolayout[0x15] = 0xC0; geolayout[0x16] = numVertsByte1; geolayout[0x17] = numVertsByte2;
        return geolayout;
    }
    public static void FromShortToByte(short number, out byte byte1, out byte byte2)
    {
        byte1 = (byte)(number >> 8);
        byte2 = (byte)(number & 255);
    }
}
