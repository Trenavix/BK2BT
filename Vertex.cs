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
using static OpenTK.GLControl;
using System.Drawing;

public struct Vertex
{
    public static readonly int MaxInt16 = 0x7FFF;
    Int16 X;
    Int16 Y;
    Int16 Z;
    float U;
    float V;
    float R;
    float G;
    float B;
    float A;

    Vertex(Int16 X, Int16 Y, Int16 Z, float U, float V, float R, float G, float B, float A)
    {
        this.X = X;
        this.Y = Y;
        this.Z = Z;
        this.U = U;
        this.V = V;
        this.R = R;
        this.G = G;
        this.B = B;
        this.A = A;
    }

    public static Vertex getVertex(int segAddr, BTBinFile Binfile)
    {
        Int32 VTXAddr = Binfile.getVTXAddr() + segAddr;
            Vertex NewVert = new Vertex
            (
            Binfile.ReadTwoBytes(VTXAddr),
            Binfile.ReadTwoBytes(VTXAddr + 2),
            Binfile.ReadTwoBytes(VTXAddr + 4),
            (float)(Binfile.ReadTwoBytes(VTXAddr + 8)) / (0x800 * Textures.S_Scale),
            (float)(Binfile.ReadTwoBytes(VTXAddr + 10)) / (0x800 * Textures.T_Scale),
            (float)Binfile.getByte(VTXAddr + 12) / 255,
            (float)Binfile.getByte(VTXAddr + 13) / 255,
            (float)Binfile.getByte(VTXAddr + 14) / 255,
            (float)Binfile.getByte(VTXAddr + 15) / 255
            );
        return NewVert;
    }

    public Vector3 getCoordVector()
    {
        return new Vector3(X, Y, Z);
    }

    public Vector2 getUVVector()
    {
        return new Vector2(U, V);
    }

    public Vector4 getRGBAVector()
    {
        return new Vector4(R, G, B, A);
    }

    public Color4 getRGBAColor()
    {
        return new Color4(R, G, B, A);
    }
}