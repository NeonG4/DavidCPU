using DavidAsmCore;
using System;
using System.IO;

namespace DavidAsm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This has been changed... Just a test commit...");

            var emit = new OpEmitter();

#if false
            emit.LoadConstant(123, Register.R1);
            emit.Add(Register.R1, Register.R2, Register.R3);
#endif


            var w = new Worker();

            //var path = args[0];

            var lines = File.ReadLines(@"C:\Users\david\dev\DavidCPU\samples\ConsoleLib.david");

            w.Work(lines);
            w.WriteToFile(Console.Out, compact: true);

            // w.HandleLine("add r1 r2 --> r3");
        }
    }
}
