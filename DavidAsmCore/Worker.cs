using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    public class Worker
    {
        private readonly OpEmitter _emitter;

        private readonly Dictionary<Label, FunctionDefinition> _functionDefinitions = new Dictionary<Label, FunctionDefinition>();


        public Worker()
        {
            _emitter = new OpEmitter();
        }

        public void WriteToFile(TextWriter textWriter, bool compact = false)
        {
            _emitter.WriteToFile(textWriter, compact);
        }

        private FunctionDefinition ParseFunctionSignature(string line)
        {
            // function Name(param1,param2,param3)

            var l2 = line.Substring("function ".Length).Trim();

            int iLeft = l2.IndexOf('(');
            int iRight = l2.IndexOf(')');
            if (iRight != l2.Length-1)
            {
                throw new InvalidOperationException($"Function declaration should end in ')'");
            }

            string name = l2.Substring(0, iLeft).Trim();
            
            string args = l2.Substring(iLeft+1, iRight-iLeft-1);

            var funcDef = new FunctionDefinition
            {
                 _name = Label.New(name)
            };

            if (!string.IsNullOrWhiteSpace(args))
            {
                var parts = args.Split(',');
                foreach(var part in parts)
                {
                    var paramName = Label.New(part.Trim());
                    funcDef._paramNames.Add(paramName);
                }
            }

            return funcDef;
        }

        // Populate definitions. 
        private void ScanDefinitions(IEnumerable<string> lines)
        {
            FunctionDefinition currentDef = null;

            foreach(var line2 in lines)
            {
                var line = line2.Trim();

                var idxComment = line.IndexOf("//");
                if (idxComment >= 0)
                {
                    line = line.Substring(0, idxComment).Trim();
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                
                // function Name(param1, param2) 
                if (line.StartsWith("function "))
                {
                    if (currentDef != null)
                    {
                        throw new InvalidOperationException($"Can't define nested functions");
                    }

                    currentDef = ParseFunctionSignature(line);
                    _functionDefinitions.Add(currentDef._name, currentDef);
                }
                else if (currentDef != null)
                {
                    // In middle of existing function 
                    if (line[0] == '{')
                    {
                        // $$$ ensure this is next token. 
                    }
                    else if (line[0] == '}')
                    {
                        currentDef = null; 
                    } else
                    {
                        if (line.StartsWith("variable "))
                        {
                            var parts = line.Split(" ");
                            var varName = parts[1];
                            currentDef._localNames.Add(Label.New(varName));
                        }
                        else
                        {
                            // Body of function
                            currentDef._body.Add(line);
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Illegal top-level statement: {line}");
                }
            }

            // ensure terminated
            if (currentDef != null)
            {
                throw new InvalidOperationException($"Unterminated function. Missing closing '}}'");
            }
        }

        public void Work(IEnumerable<string> lines)
        {
            ScanDefinitions(lines);

            // Console is: 1000... 1594   // 27*11, 2 bytes per char

            // Preemable. Allocate the stack.  $$$ - this should be a touchup. 
            _emitter.LoadConstant(1600, Register.R5);


            foreach(var funcDef in  _functionDefinitions.Values) 
            {
                foreach (var line in funcDef._body)
                {
                    HandleLine(line);
                }

            }

            // End of Program. 
            _emitter.Exit();
        }



        // Get label from:
        //   name:  
        //   name:  // comment 
        private static readonly Regex _regExLabelMatch = new Regex(@"^\s*([A-Za-z0-9]+?):\s*(//.*)?$");

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

                // Add r1 r2 --> R3
                // Add r1 number --> R3
                case Opcode.Add:
                    {
                        Register in1 = lp.GetRegister();

                        object arg2 = lp.GetRegisterOrNumber();                       
                        
                        lp.GetArrow();
                        Register regOutput = lp.GetRegister();

                        if (arg2 is Register r2)
                        {
                            _emitter.Add(in1, r2, regOutput);
                        } else if (arg2 is int num2)
                        {
                            _emitter.Add(in1, num2, regOutput);
                        } else
                        {
                            // compiler should have blocked
                            throw new NotImplementedException($"should happen");
                        }
                    }
                    break;
                case Opcode.Sub:
                    {
                        Register in1 = lp.GetRegister();
                        Register in2 = lp.GetRegister();
                        lp.GetArrow();
                        Register regOutput = lp.GetRegister();

                        _emitter.Sub(in1, in2, regOutput);
                    }
                    break;
                case Opcode.Mul:
                    {
                        Register in1 = lp.GetRegister();
                        Register in2 = lp.GetRegister();
                        lp.GetArrow();
                        Register regOutput = lp.GetRegister();

                        _emitter.Mul(in1, in2, regOutput);
                    }
                    break;
                case Opcode.Div:
                    {
                        Register in1 = lp.GetRegister();
                        Register in2 = lp.GetRegister();
                        lp.GetArrow();
                        Register regOutput = lp.GetRegister();

                        _emitter.Div(in1, in2, regOutput);
                    }
                    break;
                case Opcode.And:
                    {
                        Register in1 = lp.GetRegister();
                        Register in2 = lp.GetRegister();
                        lp.GetArrow();
                        Register regOutput = lp.GetRegister();

                        _emitter.And(in1, in2, regOutput);
                    }
                    break;
                case Opcode.Or:
                    {
                        Register in1 = lp.GetRegister();
                        Register in2 = lp.GetRegister();
                        lp.GetArrow();
                        Register regOutput = lp.GetRegister();

                        _emitter.Or(in1, in2, regOutput);
                    }
                    break;
                case Opcode.Not:
                    {
                        Register in1 = lp.GetRegister();
                        lp.GetArrow();
                        Register regOutput = lp.GetRegister();

                        _emitter.Not(in1, regOutput);
                    }
                    break;
                case Opcode.Nand:
                    {
                        Register in1 = lp.GetRegister();
                        Register in2 = lp.GetRegister();
                        lp.GetArrow();
                        Register regOutput = lp.GetRegister();

                        _emitter.Nand(in1, in2, regOutput);
                    }
                    break;                
                case Opcode.Xor:
                    {
                        Register in1 = lp.GetRegister();
                        Register in2 = lp.GetRegister();
                        lp.GetArrow();
                        Register regOutput = lp.GetRegister();

                        _emitter.Xor(in1, in2, regOutput);
                    }
                    break;
                case Opcode.JumpIf:
                    {
                        Label l = lp.GetLabel();
                        Register r = lp.GetRegister();
                        _emitter.JumpIf(l, r);
                    }
                    break;


                // jmp register 
                // jmp label 
                case Opcode.Jump_Overload:
                    {
                        var arg1 = lp.GetRegisterOrLabel();
                        if (arg1 is Register r1)
                        {
                            _emitter.JumpReg(r1);
                        }
                        else if (arg1 is Label l1)
                        {
                            _emitter.JumpLabel(l1);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Expected Register or Label");
                        }
                    }
                    break;


                case Opcode.Mov_Overload:
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
                                throw new InvalidOperationException($"First argument is an register, so second arg must be an address.");
                            }
                        }
                        else
                        {
                            // shouldn't happen. 
                            throw new InvalidOperationException($"Compiler error");
                        }
                    }
                    break;

                case Opcode.Exit:
                    _emitter.Exit();
                    break;

                // Call func(p1,p2,p3) --> r1
                case Opcode.Call:
                    {
                        var label = lp.GetLabel();

                        // add rIP +0 --> r5 
                        // jmp Func1

                        // _emitter.Add(Register.RIP, 0, Register.R5);
                        _emitter.WriteComment($"Call {label}.");
                        EmitPush(Register.RIP);

                        _emitter.JumpLabel(label);
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unrecognized opcode: {op} is not supported.");
            }

            // Ensure end of line 
            lp.IsEOL();
        }

        // Using R5 as the stack pointer.
        // Preemable must set R5....  $$$

        private readonly RegisterAddressSpec _stackAddr = new RegisterAddressSpec { Register = Register.R5 };

        // Emit pushing a register onto the stack
        private void EmitPush(Register r)
        {
            if (r.Value == Register.R5.Value)
            {
                throw new InvalidOperationException($"Can't push R5 since it's the stack register.");
            }

            _emitter.WriteComment($"Push {r}");
            _emitter.MoveRegToMem(r, _stackAddr);
            _emitter.Add(Register.R5, 2, Register.R5); // each stack item is two bytes
        }

        // Pop a value and save to the register. 
        private void EmitPop(Register r)
        {
            if (r.Value == Register.R5.Value)
            {
                throw new InvalidOperationException($"Can't pop to R5 since it's the stack register.");
            }

            _emitter.WriteComment($"Pop {r}");
            _emitter.Add(Register.R5, -2, Register.R5); // each stack item is two bytes
            _emitter.MoveMemToReg(_stackAddr, r);
        }
    }
}
