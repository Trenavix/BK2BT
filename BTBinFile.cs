using System;
using System.IO;

public class BTBinFile
{
    private static byte[] CurrentBin;

	public BTBinFile(byte[] newBin)
	{
        CurrentBin = newBin;
    }

    public Int32 getF3DEX2SetupAddr()
    {
        return ReadFourBytes(0x0C);
    }
    public void updateF3DEX2SetupAddr(Int32 newAddr)
    {
        WriteFourBytes(getF3DEX2SetupAddr(), newAddr);
    }
    public Int16 getTextureSetupAddr()
    {
        return ReadTwoBytes(0x08);
    }


    public int F3DCommandsLength()
    {
        return ReadFourBytes(getF3DEX2SetupAddr());
    }
    public byte[] getF3DSegment()
    {
        byte[] F3DSeg = new byte[F3DCommandsLength()];
        for (int i = 0; i < F3DCommandsLength()*8; i++)
        {
            F3DSeg[i] = CurrentBin[getF3DEX2SetupAddr()+8+i];
        }
        return F3DSeg;
    }

    public byte[] getCurrentBin()
    {
        return CurrentBin;
    }

    public int endBinAddr()
    {
        return CurrentBin.Length-1;
    }

    public Int16 ReadTwoBytes(int offset)
    {
        Int16 value = getByte(offset);
        for (int i = offset; i < offset+2; i++)
        {
            value = (Int16)((value << 8) | CurrentBin[i]);
        }
        return value;
    }
    public Int32 ReadFourBytes(int offset)
    {
        Int32 value = getByte(offset);
        for (int i = offset; i < offset + 4; i++)
        {
            value = (value << 8) | CurrentBin[i];
        }
        return value;
    }

    public Int64 ReadEightBytes(int offset)
    {
        Int64 value = getByte(offset);
        for (int i = offset; i < offset+8; i++)
        {
            value = (value << 8) | CurrentBin[i];
        }
        return value;
    }
    public void WriteFourBytes(int offset, Int32 bytes)
    {
        byte[] currentbyte = BitConverter.GetBytes(bytes);
        for (int i = offset; i > offset - 4; i--)
        {
            CurrentBin[i + 3] = currentbyte[offset - i];
        }
    }
    public void WriteTwoBytes(int offset, Int16 bytes)
    {
        byte[] currentbyte = BitConverter.GetBytes(bytes);
        for (int i = offset; i > offset - 2; i--)
        {
            CurrentBin[i+1] = currentbyte[offset-i];
        }
    }
    public void WriteEightBytes(int offset, Int64 bytes)
    {
        byte[] currentbyte = BitConverter.GetBytes(bytes);
        for (int i = offset; i < offset + 8; i++)
        {
            CurrentBin[i] = currentbyte[i-offset];
        }
    }
    public byte getByte(int offset)
    {
        return CurrentBin[offset];
    }
    public void changeByte(int offset, byte newbyte)
    {
        if (offset > endBinAddr())
        {
            Array.Resize(ref CurrentBin, offset+1);
        }
        CurrentBin[offset] = newbyte;
    }
    public void copyBytes(int srcAddr, int destAddr, int size)
    {
        byte[] tempbuffer = new byte[size];
        for (int i = 0; i < size; i++)
        {
            tempbuffer[i] = CurrentBin[srcAddr+i];
        }
        for (int i = 0; i < size; i++)
        {
            changeByte(destAddr+i, tempbuffer[i]);
        }
    }
    public void writeByteArray(int offset, byte[] array)
    {
        for (int i=0; i < array.Length; i++)
        {
            changeByte(offset+i, array[i]);
        }
    }
    public void changeEndBinAddr(int newsize)
    {
        Array.Resize(ref CurrentBin, newsize);
    }
}
