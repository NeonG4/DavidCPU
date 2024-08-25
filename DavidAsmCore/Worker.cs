using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    public class Worker
    {
        private readonly OpEmitter _emitter;

        public Worker()
        {
            _emitter = new OpEmitter();
        }

        public void WriteToFile(TextWriter textWriter)
        {
            _emitter.WriteToFile(textWriter);
        }

        public void Work(IEnumerable<string> lines)
        {
            foreach(var line in lines)
            {
                // ignore comments or blank lines 
                if (line.Trim().StartsWith("//"))
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

        // Get label from:
        //   name:  
        //   name:  // comment 
        private readonly Regex _regExLabelMatch = new Regex(@"^\s*([A-Za-z0-9]+?):\s*(//.*)?$");

        // Labels are an identifier followed by a colon "Label:" 
        // Could have a comment after the ':'
        private bool HandleLabel(string line)
        {         
            var m = _regExLabelMatch.Match(line);

            if (m.Success)
            {
                string labelName = m.Groups[1].Value;

                Label l = Label.New(labelName);
                this._emitter.MarkLabel(l);

                return true;
            }

            return false;
        }

        public void HandleLine(string line)
        {
            // Does this declare a label?
            if (HandleLabel(line))
            {
                return;
            }


            var lp = new LineParser(line);
            var op = lp.GetOp();

            switch(op) 
            {
                case Opcode.Val:
                    {
                        var i = lp.GetConstant();
                        lp.GetArrow();
                        var r = lp.GetRegister();

                        _emitter.LoadConstant((Int16)i, r);
                    }
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

                case Opcode.JumpIf:
                    {
                        Label l = lp.GetLabel();
                        Register r = lp.GetRegister();
                        _emitter.JumpIf(l, r);
                    }
                    break;


                case Opcode.Mov:
                    {
                        // Determine overload. 
                        // lp.GetAddressOrRegister(out var addr1, out var addrReg1, out var Reg1);
                        var arg1 = lp.GetAddressOrRegister();

                        lp.GetArrow();

                        // lp.GetAddressOrRegister(out var addr2, out var addrReg2, out var Reg2);
                        var arg2 = lp.GetAddressOrRegister();


                        if (arg1 is AddressSpec addr1)
                        {
                            if (arg2 is Register reg2)
                            {
                                _emitter.MoveMemToReg(addr1, reg2);
                            } else
                            {
                                throw new InvalidOperationException($"First argument is an address, so second arg must be a register.");
                            }
                        }
                        else if (arg1 is Register reg1)
                        {
                            if (arg2 is AddressSpec addr2)
                            {
                                _emitter.MoveRegToMem(reg1, addr2);
                            }
                            else
                            {
                                throw new InvalidOperationException($"First argument is an register, so second arg must be a address.");
                            }
                        }
                        else
                        {
                            // shouldn't happen. 
                            throw new InvalidOperationException($"Compiler error");
                        }
                    }
                    break;
            }

            // Ensure end of line 
            lp.IsEOL();
        }
    }
}
