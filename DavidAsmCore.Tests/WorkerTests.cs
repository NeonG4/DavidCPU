using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore.Tests
{
    public class WorkerTests
    {
        [Theory]
        [InlineData("function fib(f1, f2)", 2)]
        [InlineData("function fib(f1)", 1)]
        [InlineData("function fib()", 0)]
        public void ParseFunc2(string expr, int expectedParamCount)
        {
            Worker worker = new Worker();

            var sig = worker.ParseFunctionSignature(expr);

            Assert.NotNull(sig);

            Assert.Equal("fib:", sig.ToString());

            Assert.Equal(expectedParamCount, sig.ParamCount);
            Assert.Equal(0, sig.LocalCount);
        }
    }
}