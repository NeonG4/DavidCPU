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

        public List<Label> _paramNames = new List<Label>();

        public List<Label> _localNames = new List<Label>();

        // body. Used in 2nd pass for compilation.  
        public List<string> _body = new List<string>();

        public override string ToString()
        {
            return _name.ToString();
        }
    }
}
