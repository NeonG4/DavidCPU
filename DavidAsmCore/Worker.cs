using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    public class Worker
    {
        private readonly OpEmitter _emitter;

        public Worker(OpEmitter emitter)
        {
            _emitter = emitter;
        }

        public void Work(IEnumerable<string> lines)
        {
            foreach(var line in lines)
            {
                // ignore comments or blank lines 
                if (line.StartsWith("//"))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                HandleLine(line);
            }

            // End of Program. 
            _emitter.Exit();
        }

        public void HandleLine(string line)
        {
            var lp = new LineParser(line);
            var op = lp.GetOp();

            switch(op) 
            {
                case Opcode.Val:
                    var i = lp.GetConstant();
                    lp.GetArrow();
                    var r = lp.GetRegister();

                    _emitter.LoadConstant((Int16) i, r);
                    break;

                case Opcode.Add:
                    {
                        Register in1 = lp.GetRegister();
                        Register in2 = lp.GetRegister();
                        lp.GetArrow();
                        Register regOutput = lp.GetRegister();

                        _emitter.Add(in1, in2, regOutput);
                    }
                    break;
            
            }

            // Ensure end of line 
            lp.IsEOL();
        }
    }


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
            if (int.TryParse(t, out var val))
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

        public Register GetRegister()
        {
            var t = GetToken();

            if (t.Length < 2 || t[0] != 'r')
            {
                throw new InvalidOperationException($"Expected register, like `r#`.");
            }

            var t2 = t.Substring(1);
            var regId = int.Parse(t2);

            if (regId < 0 || regId > 4) 
            {
                throw new InvalidOperationException($"Invalid register index. Only supports r1...r4");
            }

            return new Register { Value = regId };
        }

        public Opcode GetOp()
        {
            var token = GetToken();


            if (!Enum.TryParse<Opcode>(token, ignoreCase:true, out var op))
            {
                throw new InvalidOperationException($"Expected opcode");
            }

            return op;
        }
    }
}
