using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    /// <summary>
    ///  Define a global variable 
    /// </summary>
    public class VariableDefinition
    {
        public Label _name;
        internal ConstantAddressSpec address;

        // Address we store at...
    }

    /// <summary>
    /// Definition of a function 
    /// </summary>
    public class FunctionDefinition
    {
        /// <summary>
        /// Name of the main() function. This is the entry point 
        /// and all programs should have this. 
        /// </summary>
        public static string Main = "main";

        public Label _name;

        // $$$ Need to track storage for each... Stack slot....
        // positional args, used to push... 

        // $$$ detect collisions. 

        private List<Label> _paramNames = new List<Label>();

        private List<Label> _localNames = new List<Label>();

        // collision detection
        private HashSet<Label> _names = new HashSet<Label>();

        public void AddParam(Label name)
        {
            if (!_names.Add(name))
            {
                throw new InvalidOperationException($"{name} is already defined.");
            }

            _paramNames.Add(name);
        }

        public void AddLocal(Label name)
        {
            if (!_names.Add(name))
            {
                throw new InvalidOperationException($"{name} is already defined.");
            }

            _localNames.Add(name);
        }

        public int ParamCount => _paramNames.Count();

        public int LocalCount => _localNames.Count();

        // body. Used in 2nd pass for compilation.  
        public List<string> _body = new List<string>();

        public override string ToString()
        {
            return _name.ToString();
        }
        
        // return true if the label is a local/param in this function. 
        // else, return false.
        internal bool TryResolve(Label label, out StackAddressSpec s1)
        {
#if false
Stack frame looks like:
    p0
    p1
    p2
    l0
    l1
    l2 
    rip
        <-- R5
#endif
            // Is it a local?
            if (TryGetLocalIdx(label, out var idx))
            {
                s1 = new StackAddressSpec {  _name = label };
                s1.SetOffset((0 - _localNames.Count + idx - 1) * 2);

                return true;
            }

            if (TryGetParamIdx(label, out idx))
            {
                s1 = new StackAddressSpec { _name = label };
                s1.SetOffset((0 - _localNames.Count - _paramNames.Count + idx - 1) * 2);
                return true;
            }

            s1 = default;
            return false;
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
