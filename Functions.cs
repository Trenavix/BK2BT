using OpenTK;
using System;

public class Functions
{
    public static byte[] ConvertShortArraytoByteArray(short[] shorts)
    {
        byte[] bytes = new byte[shorts.Length * 2];
        for (int i = 0; i < shorts.Length; i++)
        {
            bytes[i * 2] = (byte)(shorts[i] >> 8);
            bytes[i * 2 + 1] = (byte)(shorts[i] & 0xFF);
        }
        return bytes;
    }

    public static Int64[] ConvertByteArrayToInt64(byte[] bytes)
    {
        Int64[] Bits64 = new Int64[bytes.Length / 8];
        for (int i = 1; i < Bits64.Length; i++)
        {
            int Bytesidx = i * 8;
            Bits64[i] = bytes[Bytesidx + 7] << 56;
            Bits64[i] |= bytes[Bytesidx + 6] << 48;
            Bits64[i] |= bytes[Bytesidx + 5] << 40;
            Bits64[i] |= bytes[Bytesidx + 4] << 32;
            Bits64[i] |= bytes[Bytesidx + 3] << 24;
            Bits64[i] |= bytes[Bytesidx + 2] << 16;
            Bits64[i] |= bytes[Bytesidx + 1] << 8;
            Bits64[i] |= bytes[Bytesidx];
        }
        return Bits64;
    }

    public static Int64[] ConvertShortArrayToInt64(short[] shorts)
    {
        Int64[] Bits64 = new Int64[shorts.Length / 4];
        for (int i = 1; i < Bits64.Length; i++)
        {
            int Shortsidx = i * 4;
            Bits64[i] = shorts[Shortsidx + 3] << 48;
            Bits64[i] |= shorts[Shortsidx + 2] << 32;
            Bits64[i] |= shorts[Shortsidx + 1] << 16;
            Bits64[i] |= shorts[Shortsidx];
        }
        return Bits64;
    }

    public static void Copy64BitsToArray(Int64[] SrcArray, int SrcOffset, Int64[] DstArray, int DstOffset, int size)
    {
        for (int i = 0; i < size; i++)
        {
            DstArray[DstOffset + i] = SrcArray[SrcOffset + i];
        }
    }

    public static void CopyBytes(byte[] SrcArray, int SrcOffset, byte[] DstArray, int DstOffset, int size)
    {
        for (int i = 0; i < size; i++)
        {
            DstArray[DstOffset + i] = SrcArray[SrcOffset + i];
        }
    }

    public byte readByte(byte[] bytes, int offset)
    {
        return bytes[offset];
    }
}