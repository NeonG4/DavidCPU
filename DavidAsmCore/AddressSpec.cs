using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore
{

    // Represent an address specifier 
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
}
