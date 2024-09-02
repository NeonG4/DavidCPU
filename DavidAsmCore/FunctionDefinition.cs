using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    /// <summary>
    /// Definition of a function 
    /// </summary>
    public class FunctionDefinition
    {
        public Label _name;

        // $$$ Need to track storage for each... Stack slot....
        // positional args, used to push... 

        // $$$ detect collisions. 

        public List<Label> _paramNames = new List<Label>();

        public List<Label> _localNames = new List<Label>();

        // body. Used in 2nd pass for compilation.  
        public List<string> _body = new List<string>();

        public override string ToString()
        {
            return _name.ToString();
        }

        internal void Resolve(StackAddressSpec s1)
        {
#if false
Stack frame looks like:
    rip
    p0
    p1
    p2
    l0
    l1
    l2 
        <-- R5
#endif
            // Is it a local?
            if (TryGetLocalIdx(s1._name, out var idx))
            {
                s1.SetOffset((0 - _localNames.Count + idx) * 2);

                return;
            }

            if (TryGetParamIdx(s1._name, out idx))
            {
                s1.SetOffset((0 - _localNames.Count - _paramNames.Count + idx) * 2);
                return;
            }

            throw new InvalidOperationException($"Unrecognized symbol: {s1._name}");            
        }

        private bool TryGetLocalIdx(Label l, out int idx)
        {
            idx = _localNames.IndexOf(l);
                        
            if (idx >= 0)
            {
                return true;
            }
            return false;
        }

        private bool TryGetParamIdx(Label l, out int idx)
        {
            idx = _paramNames.IndexOf(l);

            if (idx >= 0)
            {
                return true;
            }
            return false;
        }
    }
}
