using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidAsmCore.Tests
{
    public class WriterTests
    {
        [Fact]
        public void WriteI8()
        {
            var w = new Writer();

            var sw = new StringWriter();

            w.WriteI8(3);
            w.WriteI8(-3);

            w.WriteToFile(sw, compact: true);

            var output = sw.ToString().Trim();
            output = output.Replace("\n", ",").Replace("\r", "");

            Assert.Equal("00000011,11111101", output);
        }
    }
}
