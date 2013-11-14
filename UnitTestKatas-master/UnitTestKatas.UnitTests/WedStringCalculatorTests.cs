using System;
using System.Linq;
using NUnit.Framework;

namespace StringCalc.UnitTests
{
    [TestFixture]
    public class WedStringCalculatorTests
    {
        [Test]
        public void Add_ParameterIsEmpty_ReturnDefaultValue()
        {
            var calc = BuildStringCalc();

            var results = calc.Add("");

            Assert.AreEqual(0, results);
        }

        [Test]
        public void Add_ParameterIsSingleNumber_ReturnSingleNumber()
        {
            var calc = BuildStringCalc();

            var results = calc.Add("1");

            Assert.AreEqual(1, results);
        }

        [Test]
        public void Add_ParameterIsMultipleNumbers_ReturnSumOfNumbers()
        {
            var calc = BuildStringCalc();

            var results = calc.Add("1,2");

            Assert.AreEqual(3, results);
        }

        [Test]
        public void Add_ParameterIsMoreThanTwoNumbers_ReturnSumOfNumbers()
        {
            var calc = BuildStringCalc();

            var results = calc.Add("1,2,3");

            Assert.AreEqual(6, results);
        }

        [Test]
        public void Add_ParameterContainsNewLine_UseNewLineAsParameterAndSumValues()
        {
            var calc = BuildStringCalc();

            var results = calc.Add("1,2\n3");

            Assert.AreEqual(6, results);
        }

        [Test]
        public void Add_DelimieterIsSpecifiedInFirstLine_UseSpecifiedDelimeterAndSumValues()
        {
            var calc = BuildStringCalc();

            var results = calc.Add("//$\n1$2");

            Assert.AreEqual(3, results);
        }

        [Test]
        public void Add_ParameterContainsNegativeNumbers_ThrowsException()
        {
            var calc = BuildStringCalc();

            var results = Assert.Catch<Exception>(delegate
            {
                calc.Add("-1");
            });

            StringAssert.Contains("-1", results.Message);
        }

        [Test]
        public void Add_ParameterContainsMultipleNegativeNumbers_ThrowsException()
        {
            var calc = BuildStringCalc();

            var results = Assert.Catch<Exception>(delegate
            {
                calc.Add("-1,-2,3");
            });

            StringAssert.Contains("-1,-2", results.Message);
        }

        [Test]
        public void Add_ParameterContainsNumbersLargerThanMaxValue_IgnoreThoseNumbersAndSumTheRest()
        {
            var calc = BuildStringCalc();

            var results = calc.Add("1001,1,2");

            Assert.AreEqual(3, results);
        }

        [Test]
        public void Add_ParameterContainsDefaultDelimeterWithLengthLargerThanOne_UseThatDelimeterAndSumUpNumbers()
        {
            var calc = BuildStringCalc();

            var results = calc.Add("//[***]\n1***2");

            Assert.AreEqual(3, results);
        }

        [Test]
        public void Add_ParameterContainsMultipleDefaultDelimeterWithLengthLargerThanOne_UseTheDelimeterAndSumUpNumbers()
        {
            var calc = BuildStringCalc();

            var results = calc.Add("//[***][$$]\n1***2$$3");

            Assert.AreEqual(6, results);
        }

        private static WedStringCalculator BuildStringCalc()
        {
            return new WedStringCalculator();            
        }
    }

    public class WedStringCalculator
    {
        public int Add(string numbers)
        {
            HandleNegativeValuesInNumbers(numbers);

            numbers = ReplaceNewLineCharacters(numbers);
            numbers = ReplaceSpecifiedDelimeterWithDefaultDelimeter(numbers);
            
            if (IsEmptyValue(numbers))
            {
                return GetDefaultValue();    
            }
            
            numbers = RemoveLargeNumbers(numbers);

            if (ContainsMultipleNumbers(numbers))
            {
                return GetSumOfNumbers(numbers);
            }

            return GetSingleNumberValue(numbers);
        }

        protected virtual string RemoveLargeNumbers(string numbers)
        {
            var numberList = numbers.Split(',');
            var smallNumberList = numberList.Where(_ => int.Parse(_) <= 1000);

            numbers = string.Join(",", smallNumberList.Select(_ => _.ToString()));
            return numbers;
        }

        private static void HandleNegativeValuesInNumbers(string numbers)
        {
            if (numbers.Contains("-"))
            {
                var numberList = numbers.Split(',');

                var negativeNumberList = numberList.Where(_ => int.Parse(_) < 0);

                var message = string.Join(",", negativeNumberList.Select(_ => _.ToString()));

                throw new Exception(message);
            }
        }

        private static string ReplaceSpecifiedDelimeterWithDefaultDelimeter(string numbers)
        {
            if (numbers.StartsWith("//"))
            {
                var delimeterSection = numbers.Substring(2, numbers.IndexOf(",") - 2);
                var delimeterList = delimeterSection.Replace("][", ",").Split(',');

                numbers = numbers.Substring(numbers.IndexOf(",") + 1);
                
                foreach (var delimeter in delimeterList)
                {
                    var delimeterToUse = RemoveDelimeterBrackets(delimeter);
                    numbers = numbers.Replace(delimeterToUse, ",");
                }
            }
            return numbers;
        }

        private static string RemoveDelimeterBrackets(string delimeterSection)
        {
            return delimeterSection.Replace("]", "").Replace("[", "");
        }

        protected virtual string ReplaceNewLineCharacters(string numbers)
        {
            if (numbers.Contains("\n"))
            {
                numbers = numbers.Replace("\n", ",");
            }
            return numbers;
        }

        protected virtual int GetSingleNumberValue(string numbers)
        {
            return int.Parse(numbers);
        }

        protected virtual int GetSumOfNumbers(string numbers)
        {
            var numberList = numbers.Split(',');

            return numberList.Sum(_ => int.Parse(_));
        }

        protected virtual bool ContainsMultipleNumbers(string numbers)
        {
            return numbers.Contains(",");
        }

        protected virtual int GetDefaultValue()
        {
            return 0;
        }

        protected virtual bool IsEmptyValue(string numbers)
        {
            return numbers.Length == 0;
        }
    }
}
