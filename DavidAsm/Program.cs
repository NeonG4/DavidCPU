using DavidAsmCore;
using System;
using System.IO;

namespace DavidAsm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var writer = new Writer(Console.Out);
            var emit = new OpEmitter(writer);

#if false
            emit.LoadConstant(123, Register.R1);
            emit.Add(Register.R1, Register.R2, Register.R3);
#endif


            var w = new Worker(emit);

            var lines = File.ReadLines(@"C:\dev\DavidCPU\samples\add.david");
            w.Work(lines);

            // w.HandleLine("add r1 r2 --> r3");
        }
    }
}
