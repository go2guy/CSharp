using System;
using System.IO;
using NUnit.Framework;

namespace StringCalc.UnitTests
{
    [TestFixture]
    public class ProgramTests
    {
        [Test]
        public void Main_SingleNumberReceived_PrintSingleNumber()
        {            
            var writer = new StringWriter();

            Console.SetOut(writer);
            Program.Main(new[] { "1" });
            
            StringAssert.Contains("1", writer.ToString());
        }
    }
}
