using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    /// <summary>
    /// A register index in the David CPU
    /// </summary>
    public struct Register
    {
        public int Value;

        public override string ToString()
        {
            return $"r{Value}";
        }

        public Register() { }

        public static Register R1 = new Register { Value = 1 };
        public static Register R2 = new Register { Value = 2 };
        public static Register R3 = new Register { Value = 3 };
        public static Register R4 = new Register { Value = 4 };
    }
}
