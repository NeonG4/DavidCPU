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
            
            }

            // Ensure end of line 
            lp.IsEOL();
        }
    }
}
