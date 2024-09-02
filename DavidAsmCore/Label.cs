using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DavidAsmCore
{
    /// <summary>
    /// Defines a label in the code. 
    /// Can be used in Jump instructions. 
    /// </summary>
    public struct Label
    {
        public string _name;

        public static Label New(string name)
        {
            // Validates
            if (!IsValid(name))
            {
                throw new InvalidOperationException($"Label name is not valid: {name}");
            }
            return new Label { _name = name };
        }

        public static bool IsValid(string name)
        {
            var r = new Regex(@"^[_A-Za-z0-9]+$");
            if (r.IsMatch(name))
            {
                return true;
            }
            
            return false;
        }

        public override string ToString()
        {
            return $"{_name}:";
        }

        public override bool Equals(object obj)
        {
            if (obj is Label l)
            {
                return this._name == l._name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }
}
