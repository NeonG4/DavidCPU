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
        AddImmediate = 0x1C,
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

        // Calls
        Call = 1100,
        Function = 1101,
        Variable = 1102,
        
        // Overloads. 
        Mov_Overload = 1000,
        Mov_RCA = 0x41, // 0100 0001
        Mov_CRA = 0x42, // 0100 0010
        Mov_RCR = 0x43, // 0100 0011
        Mov_CRR = 0x44, // 0100 0100

        
    }

    public static class OpcodeHelper
    {
        private static readonly Dictionary<string, Opcode> _opcOdes = new Dictionary<string, Opcode>(StringComparer.OrdinalIgnoreCase)
        {
            { "val", Opcode.Val },
            { "add", Opcode.Add },
            { "sub", Opcode.Sub },
            { "mul", Opcode.Mul },
            { "div", Opcode.Div },
            { "and", Opcode.And },
            { "or", Opcode.Or },
            { "not", Opcode.Not },
            { "nand", Opcode.Nand },
            { "xor", Opcode.Xor },
            { "exit", Opcode.Exit },

            { "call", Opcode.Call },
            { "function", Opcode.Function },
            { "variable", Opcode.Variable },

            { "jmp.if", Opcode.JumpIf },
            { "jmp", Opcode.Jump_Overload},

            { "mov", Opcode.Mov_Overload }
        };

        public static Opcode GetOp(string token)
        {
            // // if (!Enum.TryParse<Opcode>(token, ignoreCase: true, out var op))
            if (!_opcOdes.TryGetValue(token, out var op))
            {
                throw new InvalidOperationException($"Expected opcode, got: {token}");
            }

            return op;
        }
    }
}
