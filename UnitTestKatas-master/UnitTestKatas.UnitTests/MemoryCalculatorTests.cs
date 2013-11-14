using NUnit.Framework;

namespace StringCalc.UnitTests
{
    [TestFixture]
    public class MemoryCalculatorTests
    {
        [Test]
        public void GetSum_InvokeWithoutAddInvoked_ReturnsDefaultValue()
        {
            var mc = GetMemoryCalulator();

            int results = mc.GetSum();

            Assert.AreEqual(0, results);
        }
 

        [Test]
        public void Add_WhenInvoked_AddsNumberToMemory()
        {
            var mc = GetMemoryCalulator();

            mc.Add(1);

            int results = mc.GetSum();

            Assert.AreEqual(1, results);
        }

        [Test]
        public void Add_WhenInvokedMultipleTimes_AddsNumbersToMemory()
        {
            var mc = GetMemoryCalulator();

            mc.Add(1);
            mc.Add(2);
            mc.Add(3);

            int results = mc.GetSum();

            Assert.AreEqual(6, results);
        }

        [Test]
        public void GetSum_WhenInvokedAfterAdd_ResetToZero()
        {
            var mc = GetMemoryCalulator();

            mc.Add(3);

            mc.GetSum();

            int results = mc.GetSum();

            Assert.AreEqual(0, results);
        }

        protected virtual MemoryCalulator GetMemoryCalulator()
        {
            return new MemoryCalulator();            
        }
    }
    
    public class MemoryCalulator
    {        
        private int _memoryValue;

        public int GetSum()
        {
            var currentValue = _memoryValue;

            _memoryValue = 0;

            return currentValue;
        }

        public void Add(int value)
        {
            _memoryValue += value;
        }
    }
}
