using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    public enum Opcode : byte
    {
        Val = 0x40,
        Add = 0x10,
        Exit = 0x20,
    }

    public class OpEmitter
    {
        private readonly Writer _writer;

        public OpEmitter(Writer writer)
        {
            _writer = writer;
        }

        public void Exit()
        {
            _writer.WriteComment("Exit");
            _writer.WriteOp(Opcode.Val);
            _writer.WritePaddingByte();
            _writer.WritePaddingByte();
            _writer.WritePaddingByte();
        }

        // Save constant to register
        public void LoadConstant(Int16 constant, Register reg)
        {
            _writer.WriteComment($"val {constant} --> {reg}");
            _writer.WriteOp(Opcode.Val);
            _writer.WriteI16(constant);
            _writer.WriteReg(reg);

            _writer.WriteBlankLine();
        }

        public void Add(Register in1, Register in2, Register output)
        {
            _writer.WriteComment($"add {in1}, {in2} --> {output}");
            _writer.WriteOp(Opcode.Add);
            _writer.WriteReg(in1);
            _writer.WriteReg(in2);
            _writer.WriteReg(output);

            _writer.WriteBlankLine();
        }

    }
}
