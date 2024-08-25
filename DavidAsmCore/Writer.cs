using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;

namespace DavidAsmCore
{
    public class Writer
    {
        private readonly TextWriter _ouput;
        
        public Writer(TextWriter ouput)
        {
            _ouput = ouput;
        }

        public void WriteComment(string comment)
        {
            _ouput.WriteLine($"// {comment}");
        }

        public void WriteBlankLine()
        {
            _ouput.WriteLine();
        }

        public void WritePaddingByte()
        {
            this.WriteByte(0);
        }

        public void WriteOp(Opcode opcode)
        {
            this.WriteByte((byte) opcode);
        }

        public void WriteReg(Register reg)
        {
            this.WriteByte((byte)reg.Value);
        }

        public void WriteI16(Int16 num)
        {
            this.WriteByte((byte)(num >> 8));
            this.WriteByte((byte)(num & 0xFF));
        }

        public void WriteByte(byte b)
        {
            string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');

            // Emit byte as binary. 
            _ouput.WriteLine(binaryString);
        }
    }
}
