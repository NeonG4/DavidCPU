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
        public static Register R5 = new Register { Value = 5 };

        // Register 6 is the instruction pointer. 
        public static Register RIP = new Register { Value = 6 };

        public static Register Parse(string t)
        {
            t = t.ToLower();

            if (t.Length < 2 || t[0] != 'r')
            {
                throw new InvalidOperationException($"Expected register, like `r#`.");
            }

            if (t == "rip")
            {
                return Register.RIP;
            }

            var t2 = t.Substring(1);
            if (!int.TryParse(t2, out var regId))
            {
                throw new InvalidOperationException($"Expected regsiter, got '{t}'");
            }

            // 6 is IP. 
            // R5 is reserver for stack ... don't allow referencing it. 
            int maxReg = 4;
            if (regId < 0 || regId > maxReg)
            {
                throw new InvalidOperationException($"Invalid register index. Only supports r1...r{maxReg}");
            }

            return new Register { Value = regId };
        }
    }
}
