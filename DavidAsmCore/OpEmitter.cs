using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{
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
                
        public void WriteToFile(TextWriter output, bool compact = false)
        {
            _writer.WriteToFile(output, compact);
        }

        public void Exit()
        {
            _writer.WriteComment("Exit");
            _writer.WriteOp(Opcode.Exit);
            _writer.WritePaddingByte();
            _writer.WritePaddingByte();
            _writer.WritePaddingByte();
        }

        public void WriteComment(string comment)
        {
            _writer.WriteComment(comment);
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

        public void JumpReg(Register reg) 
        {
            _writer.WriteComment($"Jmp to address in {reg}");

            _writer.WriteOp(Opcode.JumpReg);
            _writer.WriteReg(reg);

            _writer.WritePaddingByte();
            _writer.WritePaddingByte();

            _writer.WriteBlankLine();
        }

        public void JumpLabel(Label label)
        {
            _writer.WriteComment($"Jmp to label '{label}'");

            _writer.WriteOp(Opcode.Jump);
            _writer.WriteLabel(label, TouchupKind.Relative);

            _writer.WritePaddingByte();
            
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

        // address can be specified by iether Constant or Register 
        // Mov [addressSrc] --> RegDest
        // Mov regSrc --> [AddressDest]
        public void MoveMemToReg(AddressSpec addrSource, Register regDest)
        {
            _writer.WriteComment($"Mem {addrSource} --> {regDest}");
            
            if (addrSource is ConstantAddressSpec c1)
            {
                _writer.WriteOp(Opcode.Mov_RCA); // Ram-->CPU
                _writer.WriteI16((Int16) c1.Address);
                _writer.WriteReg(regDest);
            } 
            else if (addrSource is RegisterAddressSpec r1)
            {
                _writer.WriteOp(Opcode.Mov_RCR); // Ram-->CPU
                _writer.WriteReg(r1.Register);
                _writer.WriteReg(regDest);
                _writer.WritePaddingByte();
            }
            else
            {
                throw new NotImplementedException($"Unrecognized address kind");
            }

            _writer.WriteBlankLine();
        }

        // Mov regSrc --> [AddressDest]
        // Mov regSrc --> [regDst]
        public void MoveRegToMem(Register regSource, AddressSpec addrDest)
        {
            _writer.WriteComment($"Mem {regSource} --> {addrDest}");

            if (addrDest is ConstantAddressSpec c2)
            {
                _writer.WriteOp(Opcode.Mov_CRA); // CPU --> Ram
                _writer.WriteReg(regSource);
                _writer.WriteI16((Int16)c2.Address);                
            }
            else if (addrDest is RegisterAddressSpec r2)
            {
                _writer.WriteOp(Opcode.Mov_CRR); // CPU --> Ram                
                _writer.WriteReg(regSource);
                _writer.WriteReg(r2.Register);
                _writer.WritePaddingByte();
            }
            else
            {
                throw new NotImplementedException($"Unrecognized address kind");
            }

            _writer.WriteBlankLine();
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

        public void Add(Register in1, int value, Register output)
        {
            _writer.WriteComment($"add {in1}, {value} --> {output}");
            
            _writer.WriteOp(Opcode.AddImmediate);
            _writer.WriteReg(in1);
            _writer.WriteI8(value);
            _writer.WriteReg(output);

            _writer.WriteBlankLine();
        }

        public void Sub(Register in1, Register in2, Register output)
        {
            _writer.WriteComment($"sub {in1}, {in2} --> {output}");
            _writer.WriteOp(Opcode.Sub);
            _writer.WriteReg(in1);
            _writer.WriteReg(in2);
            _writer.WriteReg(output);

            _writer.WriteBlankLine();
        }        
        public void Mul(Register in1, Register in2, Register output)
        {
            _writer.WriteComment($"mul {in1}, {in2} --> {output}");
            _writer.WriteOp(Opcode.Mul);
            _writer.WriteReg(in1);
            _writer.WriteReg(in2);
            _writer.WriteReg(output);

            _writer.WriteBlankLine();
        }
        public void Div(Register in1, Register in2, Register output)
        {
            _writer.WriteComment($"div {in1}, {in2} --> {output}");
            _writer.WriteOp(Opcode.Div);
            _writer.WriteReg(in1);
            _writer.WriteReg(in2);
            _writer.WriteReg(output);

            _writer.WriteBlankLine();
        }
        public void And(Register in1, Register in2, Register output)
        {
            _writer.WriteComment($"and {in1}, {in2} --> {output}");
            _writer.WriteOp(Opcode.And);
            _writer.WriteReg(in1);
            _writer.WriteReg(in2);
            _writer.WriteReg(output);

            _writer.WriteBlankLine();
        }
        public void Or(Register in1, Register in2, Register output)
        {
            _writer.WriteComment($"or {in1}, {in2} --> {output}");
            _writer.WriteOp(Opcode.Or);
            _writer.WriteReg(in1);
            _writer.WriteReg(in2);
            _writer.WriteReg(output);

            _writer.WriteBlankLine();
        }
        public void Not(Register in1, Register output)
        {
            _writer.WriteComment($"not {in1} --> {output}");
            _writer.WriteOp(Opcode.Not);
            _writer.WriteReg(in1);
            _writer.WriteReg(output);
            _writer.WritePaddingByte();

            _writer.WriteBlankLine();
        }
        public void Nand(Register in1, Register in2, Register output)
        {
            _writer.WriteComment($"nand {in1}, {in2} --> {output}");
            _writer.WriteOp(Opcode.Nand);
            _writer.WriteReg(in1);
            _writer.WriteReg(in2);
            _writer.WriteReg(output);

            _writer.WriteBlankLine();
        }       
        public void Xor(Register in1, Register in2, Register output)
        {
            _writer.WriteComment($"xor {in1}, {in2} --> {output}");
            _writer.WriteOp(Opcode.Xor);
            _writer.WriteReg(in1);
            _writer.WriteReg(in2);
            _writer.WriteReg(output);

            _writer.WriteBlankLine();
        }
    }
}
