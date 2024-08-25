using System;
using System.IO;

namespace DavidAsmCore.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("sumloop.david")]
        [InlineData("add.david")]
        [InlineData("memory.david")]
        public void Test1(string filename)
        {
            var w = new Worker();

            string dir = "Tests";
            var lines = File.ReadLines(Path.Combine(dir, filename));


            w.Work(lines);

            var sw = new StringWriter();
            
            w.WriteToFile(sw);

            var output = sw.ToString();

            var expectedPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(filename) + "_compiled.txt");
            var expected = File.ReadAllText(expectedPath);

            Assert.Equal(expected, output);
        }
    }
}