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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

public class F3DEX
{
    static UInt32 GeoMode;
    public static bool RenderEdges = false;
    public static bool Culling = true;
    static bool LightingEnabled = false;
    static bool EnvMapping = false;

    public static void ParseF3DEXDL(BTBinFile BinFile, uint BinNum, bool ColourBuffer)
    {
        UInt32 DLAddr = BinFile.getDLAddr();
        uint CommandsLength = BinFile.F3DCommandsLength();
        UInt32 F3DSetupAddr = BinFile.ReadFourBytes(0x0C);
        byte[][] DisplayList = new byte[CommandsLength][];
        uint newCommandAddr = F3DSetupAddr + 0x08;
        uint AllVTXAddr = BinFile.getVTXAddr();
        Vertex[] GVTXArray = new Vertex[32];
        int TextureCount = BinFile.getTextureCount();
        int CICount = 0;
        int TextureIndex = 0;
        Textures.ShortstoLoad = 0;

        Textures.TextureAddrArray[BinNum] = new uint[TextureCount+1];
        for (int i = 0; i < TextureCount; i++)
        {
            Textures.TextureAddrArray[BinNum][i] = BinFile.ReadFourBytes((uint)(BinFile.getTextureSetupAddr() + 8 + (i * 0x10))); //Load all texture addresses into array
        }
        if(TextureCount != 0) Textures.TextureAddrArray[BinNum][TextureCount] = Textures.TextureAddrArray[BinNum][TextureCount-1]+0x500; //allow last item to always be greater than last addr

        //Copy DL into 2D Byte array with double loop 
        for (uint i = 0; i < CommandsLength; i++)
        {
            DisplayList[i] = new byte[8];
            for (int j = 0; j < 8; j++)
            {
                DisplayList[i][j] = BinFile.getByte((uint)(newCommandAddr + (i * 8) + j));
            }
        }
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        if (ColourBuffer || RenderEdges) GL.Disable(EnableCap.Blend);
        for (int i = 0; i < DisplayList.Length; i++)
        {
            byte[] CMD = DisplayList[i];
            if (!ColourBuffer) switch (DisplayList[i][0])
            {
                case 0x01:
                    break;
                case 0x03: //movemem
                    if (RenderEdges) break;
                    if (CMD[1] == 0x86)
                    {
                        float[] light0_diffuse = new float[4];
                        for (uint j = 0; j < 4; j++)
                        {
                            light0_diffuse[j] = (float)0xFF / 255f;
                        }
                        GL.Light(LightName.Light0, LightParameter.Diffuse, light0_diffuse);
                        GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);
                    }
                    else if (CMD[1] == 0x88)
                    {
                        float[] light0_ambient = new float[4];
                        for (uint j = 0; j < 4; j++)
                        {
                            light0_ambient[j] = (((float)0x3F / 255f) - 0.2f) * 1.25f;
                        }
                        GL.Light(LightName.Light0, LightParameter.Ambient, light0_ambient);
                        GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Ambient);
                    }
                    GL.Enable(EnableCap.ColorMaterial);
                    break;
                case 0x04:
                    UInt32 VTXStart = readSegmentAddr(CMD);
                    short numVerts = (short)(CMD[2] / 0x04);
                    int bufferIndex = CMD[1] / 0x02;
                    for (int j = 0; j < numVerts; j++)
                    {
                        GVTXArray[bufferIndex + j] = Vertex.getVertex((uint)(VTXStart+(j*0x10)), BinFile, (byte)BinNum);
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
                            int VertIndex = CMD[j] / 0x02;
                            if (!EnvMapping) GL.TexCoord2(GVTXArray[VertIndex].getUVVector());
                            if (LightingEnabled) //Normals
                            {
                                Vector3 normals = new Vector3(GVTXArray[VertIndex].getRGBColor());
                                if (!EnvMapping) { GL.Normal3(Vector3.Normalize(normals)); }
                                else
                                {
                                    Vector4 normals4 = new Vector4(normals, 1f);
                                    normals4 = Vector4.Normalize(Renderer.projection * normals4);
                                    Vector3 newnorms = new Vector3(normals4.X, normals4.Y, normals4.Z);
                                    GL.Normal3(newnorms);
                                }
                            }
                            else GL.Color4(GVTXArray[VertIndex].getRGBAColor()); //RGBA
                            if (RenderEdges) GL.Color4(0, 0, 0, 0xFF);
                            GL.Vertex3(GVTXArray[VertIndex].getCoordVector());
                        }
                        if (!RenderEdges) Renderer.TriCount++;
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
                case 0xB6: //ClearGeoMode
                    if (RenderEdges) break;
                    UInt32 ClearBits = (UInt32)((CMD[4] << 24) | (CMD[5] << 16) | (CMD[6] << 8) | CMD[7]);
                    GeoMode &= ~ClearBits;
                    SetGeoMode(ColourBuffer);
                    break;
                case 0xB7: //SetGeoMode   
                    if (RenderEdges) break;
                    UInt32 SetBits = (UInt32)((CMD[4] << 24) | (CMD[5] << 16) | (CMD[6] << 8) | CMD[7]);
                    GeoMode |= SetBits;
                    SetGeoMode(ColourBuffer);
                    break;
                case 0xB9:
                    break;
                case 0xBA:
                    break;
                case 0xBB:
                    Textures.S_Scale *= 0x10000 / (float)(CMD[4] * 0x100 + CMD[5]); //0x10000/
                    Textures.T_Scale *= 0x10000 / (float)(CMD[6] * 0x100 + CMD[7]);
                    if (CMD[3] == 0x01 && Renderer.TextureEnabler) { GL.Enable(EnableCap.Texture2D); }
                    else
                    {
                        GL.Disable(EnableCap.Texture2D);
                        Textures.T_Scale = 1f; Textures.S_Scale = 1f;
                    }
                    if (DisplayList[i][2] == 0x12)
                    {
                        Textures.MipMapping = true;
                        Textures.T_Scale = 32f; Textures.S_Scale = 32f;//Revert mipmapping for now
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
                            int VertIndex = CMD[j] / 0x02;
                            if (!EnvMapping) GL.TexCoord2(GVTXArray[VertIndex].getUVVector());
                            if (LightingEnabled) //Normals
                            {
                                Vector3 normals = new Vector3(GVTXArray[VertIndex].getRGBColor());
                                if (!EnvMapping) { GL.Normal3(Vector3.Normalize(normals)); }
                                else
                                {
                                    Vector4 normals4 = new Vector4(normals, 1f);
                                    normals4 = Vector4.Normalize(Renderer.projection * normals4);
                                    Vector3 newnorms = new Vector3(normals4.X, normals4.Y, normals4.Z);
                                    GL.Normal3(newnorms);
                                }
                            }
                            else GL.Color4(GVTXArray[VertIndex].getRGBAColor()); //RGBA
                            if (RenderEdges) GL.Color4(0, 0, 0, 0xFF);
                            GL.Vertex3(GVTXArray[VertIndex].getCoordVector());
                        }
                        if (!RenderEdges) Renderer.TriCount++;
                        GL.End();
                        break;
                case 0xF0:
                    CICount = (((CMD[5] << 4) + ((CMD[6] & 0xF0) >> 4))>>2)+1;
                    Textures.currentPalette = Textures.LoadRGBA16TextureData(CICount, BinFile);
                    break;
                case 0xF2: //Settilesize
                    Textures.S_Scale = Convert.ToSingle((((CMD[5] << 4) | ((CMD[6] & 0xF0) >> 4)) >> 2) + 1);
                    Textures.T_Scale = Convert.ToSingle(((((CMD[6] & 0x0F) << 8) | CMD[7]) >> 2) + 1);
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
                        Textures.TextureArray[BinNum][TextureIndex] = (uint)Textures.LoadTexture(BinFile);
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
                    if (Textures.MODE == Textures.CIMODE) Textures.TextureArray[BinNum][TextureIndex] = (uint)Textures.LoadCITexture(BinFile);
                    else if (Textures.MODE == Textures.RGBAMODE && Textures.BitSize ==16) Textures.TextureArray[BinNum][TextureIndex] = (uint)Textures.LoadRGBA16Texture(BinFile);
                    else if (Textures.MODE == Textures.RGBAMODE && Textures.BitSize == 32) Textures.TextureArray[BinNum][TextureIndex] = (uint)Textures.LoadRGBA32Texture(BinFile);
                    else if (Textures.MODE == Textures.IAMODE) Textures.TextureArray[BinNum][TextureIndex] = (uint)Textures.LoadIA8Texture(BinFile);
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
            else switch (DisplayList[i][0])
                {

                    case 0x04:
                        UInt32 VTXStart = readSegmentAddr(CMD);
                        short numVerts = (short)(CMD[2] / 0x04);
                        int bufferIndex = CMD[1] / 0x02;
                        for (int j = 0; j < numVerts; j++)
                        {
                            GVTXArray[bufferIndex + j] = Vertex.getVertex((uint)(VTXStart + (j * 0x10)), BinFile, (byte)(BinNum));
                        }
                        Renderer.VertexCount += (uint)numVerts;
                        break;
                    case 0xB6:
                        if (RenderEdges) break;
                        UInt32 ClearBits = (UInt32)((CMD[4] << 24) | (CMD[5] << 16) | (CMD[6] << 8) | CMD[7]);
                        GeoMode &= ~ClearBits;
                        SetGeoMode(ColourBuffer);
                        break;
                    case 0xB7:
                        if (RenderEdges) break;
                        UInt32 SetBits = (UInt32)((CMD[4] << 24) | (CMD[5] << 16) | (CMD[6] << 8) | CMD[7]);
                        GeoMode |= SetBits;
                        SetGeoMode(ColourBuffer);
                        break;
                    case 0xB1:
                        if (LightingEnabled) break; //Disable painting on non-RGBA meshes
                        for (uint j = 0; j < 2; j++)
                        {
                            Vertex[] Triangle2 = new Vertex[3];
                            Color4[] colour2 = new Color4[3];
                            for (int k = 1; k < 4; k++)
                            {
                                int idx = (int)(k + (j * 4)); //bytes 1-3, and 5-8
                                int VertIndex = CMD[idx] / 0x02;
                                Triangle2[k-1] = GVTXArray[VertIndex];
                                UInt32 Addr = Triangle2[k - 1].getAddr();
                                colour2[k - 1] = new Color4((byte)((Addr >> 24) | (BinNum<<7)), (byte)((Addr >> 16) & 0xFF), (byte)((Addr >> 8) & 0xFF), (byte)(Addr & 0xFF)); //Addr to RGBA

                            }
                            RenderColourBufferQuads(Triangle2, colour2);
                            //if(BinNum > 0)throw new Exception(((byte)(colour2[0].B)).ToString("x"));
                        }
                        
                        break;
                    case 0xBF:
                        if (LightingEnabled) break; //Disable painting on non-RGBA meshes
                        Vertex[] Triangle = new Vertex[3];
                        Color4[] colour = new Color4[3];
                        for (int j = 5; j < 8; j++)
                        {
                            int VertIndex = CMD[j] / 0x02;
                            Triangle[j - 5] = GVTXArray[VertIndex];
                            UInt32 Addr = Triangle[j - 5].getAddr();
                            colour[j - 5] = new Color4((byte)((Addr >> 24)|(BinNum<<7)), (byte)((Addr >> 16) & 0xFF), (byte)((Addr >> 8) & 0xFF), (byte)(Addr & 0xFF)); //Addr to RGBA
                        }
                        RenderColourBufferQuads(Triangle, colour);
                        break;
                    case 0xB8:
                        break;
                }
        }
        Textures.FirstTexLoad[BinNum] = false;
        GL.Disable(EnableCap.Texture2D);
    }

    static void RenderColourBufferQuads(Vertex[] Triangle, Color4[] colour)
    {
        Vector3 CentreCoord = new Vector3
            (
            (Triangle[0].getCoordVector().X + Triangle[1].getCoordVector().X + Triangle[2].getCoordVector().X) / 3,
            (Triangle[0].getCoordVector().Y + Triangle[1].getCoordVector().Y + Triangle[2].getCoordVector().Y) / 3,
            (Triangle[0].getCoordVector().Z + Triangle[1].getCoordVector().Z + Triangle[2].getCoordVector().Z) / 3
            );
        Vector3 OneTwoAVG = new Vector3
            (
            (Triangle[0].getCoordVector().X + Triangle[1].getCoordVector().X) / 2,
            (Triangle[0].getCoordVector().Y + Triangle[1].getCoordVector().Y) / 2,
            (Triangle[0].getCoordVector().Z + Triangle[1].getCoordVector().Z) / 2
            );
        Vector3 TwoThreeAVG = new Vector3
            (
            (Triangle[1].getCoordVector().X + Triangle[2].getCoordVector().X) / 2,
            (Triangle[1].getCoordVector().Y + Triangle[2].getCoordVector().Y) / 2,
            (Triangle[1].getCoordVector().Z + Triangle[2].getCoordVector().Z) / 2
            );
        Vector3 OneThreeAVG = new Vector3
            (
            (Triangle[0].getCoordVector().X + Triangle[2].getCoordVector().X) / 2,
            (Triangle[0].getCoordVector().Y + Triangle[2].getCoordVector().Y) / 2,
            (Triangle[0].getCoordVector().Z + Triangle[2].getCoordVector().Z) / 2
            );

        GL.Begin(BeginMode.Quads);
        GL.Color4(colour[0]); //Vert 1 quad 
        GL.Vertex3(Triangle[0].getCoordVector());
        GL.Vertex3(OneTwoAVG);
        GL.Vertex3(CentreCoord);
        GL.Vertex3(OneThreeAVG);

        GL.Color4(colour[1]); //Vert 2 quad
        GL.Vertex3(Triangle[1].getCoordVector());
        GL.Vertex3(TwoThreeAVG);
        GL.Vertex3(CentreCoord);
        GL.Vertex3(OneTwoAVG);

        GL.Color4(colour[2]); //Vert 3 quad
        GL.Vertex3(Triangle[2].getCoordVector());
        GL.Vertex3(OneThreeAVG);
        GL.Vertex3(CentreCoord);
        GL.Vertex3(TwoThreeAVG);
        GL.End();
    }

    static void SetGeoMode(bool ColourBuffer)
    {
        byte CullMode = (byte)((GeoMode & 0x3000) >> 12);
        if (!Culling) CullMode = 0;
        GL.Enable(EnableCap.CullFace);
        switch (CullMode)
        {
            case 0:
                GL.Disable(EnableCap.CullFace);
                break;
            case 1:
                GL.CullFace(CullFaceMode.Front);
                break;
            case 2:
                GL.CullFace(CullFaceMode.Back);
                break;
            case 3:
                GL.CullFace(CullFaceMode.FrontAndBack);
                break;
        }
        if (ColourBuffer)
        {
            if ((GeoMode & 0x020000) >> 17 == 1) LightingEnabled = true;
            else LightingEnabled = false;
            return;
        }
        if (RenderEdges) return;
        if ((GeoMode & 0x010000) >> 16 == 1) GL.Enable(EnableCap.Fog);
        else GL.Disable(EnableCap.Fog);
        if ((GeoMode & 0x020000) >> 17 == 1) EnableLighting();
        else DisableLighting();
        if ((GeoMode & 0x040000) >> 18 == 1) SetEnvironmentMapping();
        else RemoveEnvMapping();
        //if ((SetBits & 0x080000) >> 19 == 1) SetLinearTexMap();
    }

    static void ClearGeoMode(UInt32 ClearBits)
    {
        byte CullMode = (byte)((ClearBits & 0x3000) >> 12);
        GL.Enable(EnableCap.CullFace);
        switch (CullMode)
        {
            case 1:
                GL.CullFace(CullFaceMode.Back);
                break;
            case 2:
                GL.CullFace(CullFaceMode.Front);
                break;
            case 3:
                GL.Disable(EnableCap.CullFace);
                break;
        }
    }

    static void EnableLighting()
    {
        GL.Color4(1f, 1f, 1f, 1f);
        float[] light0_position = { Math.Abs(Renderer.cam.CamRotation.X), Math.Abs(Renderer.cam.CamRotation.Y), Math.Abs(Renderer.cam.CamRotation.Z), 0.0f };
        GL.ShadeModel(ShadingModel.Smooth);
        GL.Light(LightName.Light0, LightParameter.Position, light0_position);
        GL.Enable(EnableCap.Lighting);
        GL.Enable(EnableCap.Light0);
        GL.Enable(EnableCap.Normalize);
        LightingEnabled = true;
    }

    static void DisableLighting()
    {
        GL.Disable(EnableCap.Lighting);
        LightingEnabled = false;
    }

    private static void SetEnvironmentMapping()
    {
        GL.TexGen(TextureCoordName.S, TextureGenParameter.TextureGenMode, (float)TextureGenMode.SphereMap);
        GL.TexGen(TextureCoordName.T, TextureGenParameter.TextureGenMode, (float)TextureGenMode.SphereMap);
        GL.Enable(EnableCap.TextureGenS);
        GL.Enable(EnableCap.TextureGenT);
        EnvMapping = true;
        //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
    }

    private static void RemoveEnvMapping()
    {
        GL.Disable(EnableCap.TextureGenS);
        GL.Disable(EnableCap.TextureGenT);
        EnvMapping = false;
    }

    static UInt32 readSegmentAddr(byte[] cmd)
    {
        UInt32 value = 0;
        for (int i = 5; i < 8; i++)
        {
            value = (value << 8) | cmd[i];
        }
        return value;
    }
}


