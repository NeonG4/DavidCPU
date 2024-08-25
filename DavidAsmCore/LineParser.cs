using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{


    // Parse a specific line 
    public class LineParser
    {
        int _idx = 0;

        private readonly string[] _parts;

        public LineParser(string line)
        {
            _parts = line.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        }

        // Get the next token        
        // return null if at end
        private string GetToken()
        {
            if (_idx >= _parts.Length)
            {
                return null;
            }

            return _parts[_idx++];
        }

        public int GetConstant()
        {
            var t = GetToken();
            if (!int.TryParse(t, out var val))
            {
                throw new InvalidOperationException($"Expected number, got: {t}");
            }

            return val;
        }

        public void IsEOL()
        {
            var t = GetToken();
            if (t != null)
            {
                throw new InvalidOperationException($"Expected end of line.");
            }
        }

        public void GetArrow()
        {
            var t = GetToken();
            if (t != "-->")
            {
                throw new InvalidOperationException($"Expected '-->' operator");
            }
        }

        public Label GetLabel()
        {
            var t = GetToken();
            return Label.New(t); // will validate
        }

        public Register GetRegister()
        {
            var t = GetToken().ToLower();
            return GetRegister(t);
        }

        public Register GetRegister(string t)
        {
            if (t.Length < 2 || t[0] != 'r')
            {
                throw new InvalidOperationException($"Expected register, like `r#`.");
            }

            var t2 = t.Substring(1);
            if (!int.TryParse(t2, out var regId))
            {
                throw new InvalidOperationException($"Expected regsiter, got '{t}'");
            }            

            if (regId < 0 || regId > 4)
            {
                throw new InvalidOperationException($"Invalid register index. Only supports r1...r4");
            }

            return new Register { Value = regId };
        }

        // Will set one of hte overloads...
        // [1234]  // literal address
        // [r1] // address specified by register 
        // r1 // direct register 
       
        // Returns a AddressSpec or Register
        public object GetAddressOrRegister()
        {
            var t = GetToken().Trim();

            if (t[0] == '[')
            {
                if (t[t.Length - 1] != ']')
                {
                    throw new InvalidOperationException($"Memory address must be enclosed in [ ... ] ");
                }

                var val = t.Substring(1, t.Length - 2);

                if (int.TryParse(val, out var number))
                {
                    return new ConstantAddressSpec { Address = number };
                }
                var reg = GetRegister(val);
                return new RegisterAddressSpec {  Register = reg };
            }

            if (t[0] == 'r')
            {
                var reg = GetRegister(t);
                return reg;
            }

            throw new InvalidOperationException($"Must be a register or an address (enclosed in [ ... ]).");
        }


        private readonly Dictionary<string, Opcode> _opcOdes = new Dictionary<string, Opcode>(StringComparer.OrdinalIgnoreCase)
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
            { "jmp.if", Opcode.JumpIf },
            { "mov", Opcode.Mov }
        };

        public Opcode GetOp()
        {
            var token = GetToken();

            // // if (!Enum.TryParse<Opcode>(token, ignoreCase: true, out var op))
            if (!_opcOdes.TryGetValue(token, out var op))            
            {
                throw new InvalidOperationException($"Expected opcode, got: {token}");
            }

            return op;
        }
    }
}
