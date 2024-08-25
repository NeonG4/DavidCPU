using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    public enum Opcode : byte
    {
        Val = 0x40,
        Add = 0x10,
        JumpIf = 0x22,
        Exit = 0x20,
    }

    /// <summary>
    /// Emit Opcodes. 
    /// </summary>
    public class OpEmitter
    {
        private readonly Writer _writer;

        public OpEmitter()
        {
            _writer = new Writer();
        }
                
        public void WriteToFile(TextWriter output)
        {
            _writer.WriteToFile(output);
        }

        public void Exit()
        {
            _writer.WriteComment("Exit");
            _writer.WriteOp(Opcode.Val);
            _writer.WritePaddingByte();
            _writer.WritePaddingByte();
            _writer.WritePaddingByte();
        }


        public void MarkLabel(Label label)        
        {
            _writer.WriteBlankLine();
            _writer.WriteComment($"{label}");

            _writer.MarkLabel(label);
        }

        // Offset is unknown, will need 
        public void JumpIf(Label label, Register reg)
        {
            _writer.WriteComment($"Jmp to {label} if {reg} > 0");

            _writer.WriteOp(Opcode.JumpIf);

            _writer.WriteLabel(label, TouchupKind.Relative);


            _writer.WriteReg(reg);

            _writer.WriteBlankLine();
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

        // Mov [addressSrc] --> RegDest
        public void MoveMemToReg(int addressSrc, Register regDest)
        {
        }

        // Mov [regSrc] --> regDst
        public void MoveMemToReg(Register regAddressSrc, Register regDest)
        {

        }


        // Mov regSrc --> [AddressDest]
        public void MoveRegToMem(Register regSrc, int addressDest)
        {
        }

        // Mov regSrc --> [regDst]
        public void MoveRegToMem(Register regSrc, Register regAddressDest)
        {
        }

 

        // Arithmetic operations 
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
