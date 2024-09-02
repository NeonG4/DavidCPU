using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    /// <summary>
    /// Represent an address specifier.
    /// </summary>
    public abstract class AddressSpec
    {
    }

    // Constant memory address, like [1234]
    public class ConstantAddressSpec : AddressSpec
    {
        public int Address { get; set; }

        public override string ToString()
        {
            return $"[{Address}]";
        }
    }

    // address specified by register, like [r1]
    public class RegisterAddressSpec : AddressSpec
    {
        public Register Register { get; set; }

        public override string ToString()
        {
            return $"[{Register}]";
        }
    }
    
    // a symbol, could be a parameter/local (from the stack) or a global 

    public class StackAddressSpec : AddressSpec
    {
        public Label _name;

        // Must be resolved. 
        // Offset relative to stack. 
        private int _offset = int.MaxValue;

        public void SetOffset(int offset)
        {
            _offset = offset;
        }

        public int GetOffset()
        {
            if (_offset == int.MaxValue)
            {
                throw new InvalidOperationException($"Offset isn't resolved yet!");
            }
            return _offset;
        }

        public override string ToString()
        {
            return $"[{_name} : R5 + {_offset}]";
        }
    }
}
