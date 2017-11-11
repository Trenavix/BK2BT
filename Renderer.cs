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

class Renderer
{
    public static readonly int MAINBIN = 0;
    public static readonly int ALPHABIN = 1;

    public static bool WireFrameMode = false;
    public static bool TextureEnabler = true;

    public static Camera cam = new Camera();
    public static Vector2 lastMousePos = new Vector2();
    public static uint TriCount = 0;
    public static uint VertexCount = 0;

    public static void InitialiseView()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadMatrix(ref modelview);
        GL.ClearColor(Color.Black);
        GL.Enable(EnableCap.DepthTest);
    }

    public static void Render(Rectangle ClientRectangle, int Width, int Height, GLControl panel1)
    {
        TriCount = 0;
        VertexCount = 0;
        GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        Matrix4 projection = cam.GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.0f, Width /(float)Height, 0.1f, 1000.0f);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref projection);
        InitialiseView();
        if(WireFrameMode) GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        else GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        GL.Scale(0.01, 0.01, 0.01);
        if (BinManager.MainBin != null) F3DEX.ParseF3DEXDL(BinManager.MainBin, 0);
        else DrawCube();
        if (BinManager.AlphaBin != null) F3DEX.ParseF3DEXDL(BinManager.AlphaBin, 1);
        panel1.SwapBuffers();
    }

    public static void DrawCube()
    {
        //GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.Begin(BeginMode.Quads);

        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-400.0f, -100.0f, -100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-400.0f, 100.0f, -100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-600.0f, 100.0f, -100.0f);
        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-600.0f, -100.0f, -100.0f);

        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-400.0f, -100.0f, -100.0f);
        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-600.0f, -100.0f, -100.0f);
        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-600.0f, -100.0f, 100.0f);
        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-400.0f, -100.0f, 100.0f);

        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-400.0f, -100.0f, -100.0f);
        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-400.0f, -100.0f, 100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-400.0f, 100.0f, 100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-400.0f, 100.0f, -100.0f);

        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-400.0f, -100.0f, 100.0f);
        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-600.0f, -100.0f, 100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-600.0f, 100.0f, 100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-400.0f, 100.0f, 100.0f);

        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-400.0f, 100.0f, -100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-400.0f, 100.0f, 100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-600.0f, 100.0f, 100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-600.0f, 100.0f, -100.0f);

        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-600.0f, -100.0f, -100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-600.0f, 100.0f, -100.0f);
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f); GL.Vertex3(-600.0f, 100.0f, 100.0f);
        GL.Color4(0.0f, 0.0f, 0.0f, 1.0f); GL.Vertex3(-600.0f, -100.0f, 100.0f);

        GL.End();
    }
    
}

public class Camera
{
    public Vector3 Position = Vector3.Zero;
    public Vector3 Orientation = new Vector3(0f, 0f, 0f);
    public float MoveSpeed = 0.2f;
    public float MouseSensitivity = 0.005f;

    public Matrix4 GetViewMatrix()
    {
        Vector3 lookat = new Vector3();

        lookat.X = (float)Math.Cos(((float)Orientation.X));
        lookat.Y = (float)(2*Math.Sin((float)Orientation.Y));
        lookat.Z = (float)Math.Sin(((float)Orientation.X));

        return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
    }

    public void Move(float x, float y, float z)
    {
        Vector3 offset = new Vector3();

        Vector3 forward = new Vector3((float)Math.Cos((float)Orientation.X), (float)(2 * Math.Sin((float)Orientation.Y)), (float)Math.Sin((float)Orientation.X));
        Vector3 right = new Vector3(-forward.Z, 0, forward.X);

        offset += x * right;
        offset += y * forward;
        offset.Y += z;

        offset.NormalizeFast();
        offset = Vector3.Multiply(offset, MoveSpeed);

        Position += offset;
    }

    public void AddRotation(float x, float y)
    {
        Orientation.X = ((x * MouseSensitivity));
        Orientation.Y = ((-y * MouseSensitivity));
    }

    public void WASDMoveMent()
    {
        KeyboardState state = Keyboard.GetState();
        if (state[Key.W] && state[Key.ShiftLeft]) Renderer.cam.Move(0f, 0.4f, 0f);
        if (state[Key.W]) Renderer.cam.Move(0f, 0.1f, 0f);
        if (state[Key.S] && state[Key.ShiftLeft]) Renderer.cam.Move(0f, -0.4f, 0f);
        if (state[Key.S]) Renderer.cam.Move(0f, -0.1f, 0f);
        if (state[Key.A] && state[Key.ShiftLeft]) Renderer.cam.Move(-0.4f, 0f, 0f);
        if (state[Key.A]) Renderer.cam.Move(-0.1f, 0f, 0f);
        if (state[Key.D] && state[Key.ShiftLeft]) Renderer.cam.Move(0.4f, 0f, 0f);
        if (state[Key.D]) Renderer.cam.Move(0.1f, 0f, 0f);
        if (state[Key.E] && state[Key.ShiftLeft]) Renderer.cam.Move(0f, 0f, 0.4f);
        if (state[Key.E]) Renderer.cam.Move(0f, 0f, 0.1f);
        if (state[Key.Q] && state[Key.ShiftLeft]) Renderer.cam.Move(0f, 0f, -0.4f);
        if (state[Key.Q]) Renderer.cam.Move(0f, 0f, -0.1f);
    }
}



