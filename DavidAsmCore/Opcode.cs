using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    /// <summary>
    /// Opcodes for CPU. 
    /// Values over 1000 are "virtual" opcodes and can't be emitted. (such as for overloads). 
    /// </summary>
    public enum Opcode
    {
        Val = 0x40,
        Add = 0x10,
        Sub = 0x11,
        Mul = 0x12,
        Div = 0x13,
        And = 0x14,
        Or = 0x15,
        Not = 0x16,
        Nand = 0x17,
        Xor = 0x18,

        JumpIf = 0x22, // condition jump, relative constant adddress

        Jump_Overload = 1001,

        Jump = 0x21, // unconditional jump, relative constant adddress
        JumpReg = 0x24, // unconditional jump to an absolute address specified by reg

        Exit = 0x20,

        // Overloads. 
        Mov_Overload = 1000,
        Mov_RCA = 0x41, // 0100 0001
        Mov_CRA = 0x42, // 0100 0010
        Mov_RCR = 0x43, // 0100 0011
        Mov_CRR = 0x44, // 0100 0100
    }
}
