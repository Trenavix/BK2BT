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
using System;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

public class F3DEX
{
    

    public static void ParseF3DEXDL(BTBinFile BinFile, int BinNum)
    {
        Int32 DLAddr = BinFile.getDLAddr();
        int CommandsLength = BinFile.F3DCommandsLength();
        Int32 F3DSetupAddr = BinFile.ReadFourBytes(0x0C);
        byte[][] DisplayList = new byte[CommandsLength][];
        int newCommandAddr = F3DSetupAddr + 0x08;
        int AllVTXAddr = BinFile.getVTXAddr();
        Vertex[] GVTXArray = new Vertex[32];
        int TextureCount = BinFile.getTextureCount();
        int CICount = 0;
        int TextureIndex = 0;
        Textures.ShortstoLoad = 0;

        Textures.TextureAddrArray[BinNum] = new int[TextureCount+1];
        for (int i = 0; i < TextureCount; i++)
        {
            Textures.TextureAddrArray[BinNum][i] = BinFile.ReadFourBytes(BinFile.getTextureSetupAddr() + 8 + (i * 0x10)); //Load all texture addresses into array
        }
        if(TextureCount != 0) Textures.TextureAddrArray[BinNum][TextureCount] = Textures.TextureAddrArray[BinNum][TextureCount-1]+0x500; //allow last item to always be greater than last addr

        //Copy DL into 2D Byte array with double loop 
        for (int i = 0; i < CommandsLength; i++)
        {
            DisplayList[i] = new byte[8];
            for (int j = 0; j < 8; j++)
            {
                DisplayList[i][j] = BinFile.getByte(newCommandAddr + (i * 8) + j);
            }
        }
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

        for (int i = 0; i < DisplayList.Length; i++)
        {
            byte[] CMD = DisplayList[i];
            switch (DisplayList[i][0])
            {
                case 0x01:
                    break;
                case 0x03:
                    break;
                case 0x04:
                    Int32 VTXStart = readSegmentAddr(CMD);
                    short numVerts = (short)(CMD[2] / 0x04);
                    int bufferIndex = CMD[1] / 0x02;
                    for (int j = 0; j < numVerts; j++)
                    {
                        GVTXArray[bufferIndex + j] = Vertex.getVertex(VTXStart+(j*0x10), BinFile);
                    }
                    Renderer.VertexCount += (uint)numVerts;
                    break;
                case 0x06:
                    break;
                case 0xB1:
                    GL.Begin(BeginMode.Triangles);
                    for (int j = 1; j < 8; j++)
                    {
                        if (j == 4) j++;
                        int VertIndex = CMD[j] / 2;
                        GL.TexCoord2(GVTXArray[VertIndex].getUVVector());
                        GL.Color4(GVTXArray[VertIndex].getRGBAColor());
                        GL.Vertex3(GVTXArray[VertIndex].getCoordVector());
                    }
                    Renderer.TriCount+=2;
                    GL.End();
                    break;
                case 0xB2:
                    break;
                case 0xB3:
                    break;
                case 0xB4:
                    break;
                case 0xB5:
                    break;
                case 0xB6:
                    break;
                case 0xB7:
                    if (CMD[6] >> 4 == 0) { GL.Disable(EnableCap.CullFace); }
                    else if (CMD[6] >> 4 == 2) { GL.Enable(EnableCap.CullFace);  GL.CullFace(CullFaceMode.Back); }
                    else if (CMD[6] >> 4 == 1) { GL.Enable(EnableCap.CullFace); GL.CullFace(CullFaceMode.Front); }
                    if ((CMD[5] & 0x0F) == 6)
                    {
                        //env mapping
                    }
                        break;
                case 0xB9:
                    break;
                case 0xBA:
                    break;
                case 0xBB:
                    Textures.S_Scale *= 0x10000/(float)(CMD[4] * 0x100 + CMD[5]); //0x10000/
                    Textures.T_Scale *= 0x10000 / (float)(CMD[6] * 0x100 + CMD[7]) ;
                    if (CMD[3] == 0x01 && Renderer.TextureEnabler) { GL.Enable(EnableCap.Texture2D); }
                    else
                    {
                        GL.Disable(EnableCap.Texture2D);
                        Textures.T_Scale = 1f; Textures.S_Scale = 1f;
                    }
                    if (DisplayList[i][2] == 0x12)
                    {
                        Textures.MipMapping = true;
                        Textures.T_Scale = 1f; Textures.S_Scale = 1f;//Revert mipmapping for now
                    } 
                    break;
                case 0xBC:
                    break;
                case 0xBD:
                    break;
                case 0xBF:
                    GL.Begin(BeginMode.Triangles);
                    for (int j = 5; j < 8; j++)
                    {
                        int VertIndex = DisplayList[i][j] / 2;
                        GL.TexCoord2(GVTXArray[VertIndex].getUVVector());
                        GL.Color4(GVTXArray[VertIndex].getRGBAColor());
                        GL.Vertex3(GVTXArray[VertIndex].getCoordVector());
                    }
                    Renderer.TriCount++;
                    GL.End();
                    break;
                case 0xF0:
                    CICount = (((CMD[5] << 4) + ((CMD[6] & 0xF0) >> 4))>>2)+1;
                    Textures.currentPalette = Textures.LoadRGBA16TextureData(CICount, BinFile);
                    break;
                case 0xF2:
                    Textures.S_Scale = ((CMD[5])*0x10 + ((CMD[6] & 0xF0) >> 4))/124f;//124 = 0x7C = scale 1.0
                    Textures.T_Scale = ((CMD[6] & 0x0F)*0x100 + CMD[7]) / 124f;//0x7C
                    break;

                    //To avoid TMEM emulation, we will run a hybrid of 0xF3 and 0xF5 in this section to accurately interpret texture sizes

                case 0xF3:
                    TextureIndex = 0;
                    CICount = Textures.currentPalette.Length;
                    for (int j = 0; j < TextureCount; j++) // Compare currentTexAddr to our texture address array
                    {
                        if (Textures.currentTexAddr >= Textures.TextureAddrArray[BinNum][j] && Textures.currentTexAddr < Textures.TextureAddrArray[BinNum][j+1])
                        { TextureIndex = j; }
                    }
                    if (Textures.FirstTexLoad[BinNum])
                    {
                        int TexLoadRange = ((CMD[6] & 0x0F) * 0x100 + CMD[7]);
                        Textures.ShortstoLoad = (CMD[5]) * 0x10 + ((CMD[6] & 0xF0) >> 4) + 1;
                        Textures.BytestoLoad = Textures.ShortstoLoad * 2;
                        byte Mode = Textures.MODE;

                        if (Mode == Textures.CIMODE && CICount == 16)
                        {
                            Textures.Width = TexLoadRange / 8;
                            Textures.Height = (Textures.BytestoLoad * 2) / Textures.Width;//4bpp
                        }
                        else if (Mode == Textures.CIMODE && CICount == 256)
                        {
                            Textures.Width = TexLoadRange / 16;
                            Textures.Height = (Textures.BytestoLoad) / Textures.Width;//8bpp
                        }
                        else if (Mode == Textures.RGBAMODE && Textures.BitSize == 16)
                        {
                            if (Textures.MipMapping) Textures.ShortstoLoad -= 0x200;
                            Textures.Width = TexLoadRange / 8;
                            Textures.Height = (Textures.ShortstoLoad) / Textures.Width;//16bpp
                        }
                        else if (Mode == Textures.IAMODE && Textures.BitSize == 16)
                        {
                            Textures.Width = TexLoadRange / 4;
                            Textures.Height = (Textures.BytestoLoad) / Textures.Width;//16bpp
                        }
                        else if (Textures.MODE == Textures.RGBAMODE && Textures.BitSize == 32)
                        {
                            Textures.Width = TexLoadRange / 4;
                            Textures.Height = (Textures.ShortstoLoad) / Textures.Width;//32bpp
                        }
                        else { return; }
                        Textures.TextureArray[BinNum][TextureIndex] = Textures.LoadTexture(BinFile);
                    }
                    GL.BindTexture(TextureTarget.Texture2D, Textures.TextureArray[BinNum][TextureIndex]);
                    break;
                case 0xF5:
                    Textures.MODE = (byte)(CMD[1] >> 5);
                    Textures.BitSize = (byte)(4 * Math.Pow(2, ((CMD[1] >> 3) & 3)));
                    Textures.TFlags = (CMD[5] >> 2) & 3;
                    Textures.SFlags = CMD[6] & 3;
                    int WidthBits = (((CMD[1] & 3) << 7) + CMD[2] >> 1)*64;
                    if (CMD[4] != 0 || !Textures.FirstTexLoad[BinNum]) break;
                    int HeightPower = ((CMD[5] & 3) << 2) | (CMD[6] >> 6);
                    int WidthPower = (CMD[7] >> 4);
                    Textures.Width = (int)Math.Pow(2, WidthPower);//(int)Math.Pow(2, WidthPower);
                    Textures.Height = (int)Math.Pow(2, HeightPower);
                    GL.DeleteTexture(Textures.TextureArray[BinNum][TextureIndex]);
                    if (Textures.MODE == Textures.CIMODE) Textures.TextureArray[BinNum][TextureIndex] = Textures.LoadCITexture(BinFile);
                    else if (Textures.MODE == Textures.RGBAMODE && Textures.BitSize ==16) Textures.TextureArray[BinNum][TextureIndex] = Textures.LoadRGBA16Texture(BinFile);
                    else if (Textures.MODE == Textures.RGBAMODE && Textures.BitSize == 32) Textures.TextureArray[BinNum][TextureIndex] = Textures.LoadRGBA32Texture(BinFile);
                    else if (Textures.MODE == Textures.IAMODE) Textures.TextureArray[BinNum][TextureIndex] = Textures.LoadIA8Texture(BinFile);
                    GL.BindTexture(TextureTarget.Texture2D, Textures.TextureArray[BinNum][TextureIndex]);
                    break;
                case 0xFD:
                    Textures.BitSize = (byte)(4 * Math.Pow(2, ((CMD[1] >> 3) & 3)));
                    Textures.MODE = (byte)(CMD[1] >> 5);
                    Textures.currentTexAddr = readSegmentAddr(CMD);
                    break;
                case 0xB8:
                    Textures.T_Scale = 1;
                    Textures.S_Scale = 1;
                    
                    break;
            }
        }
        Textures.FirstTexLoad[BinNum] = false;
        GL.Disable(EnableCap.Texture2D);
    }

    public static Int32 readSegmentAddr(byte[] cmd)
    {
        Int32 value = 0;
        for (int i = 5; i < 8; i++)
        {
            value = (value << 8) | cmd[i];
        }
        return value;
    }
}


