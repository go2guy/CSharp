using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace StringCalc.UnitTests
{
    [TestFixture]
    public class ThStringCalculatorTests
    {
        [Test]
        public void Add_ParamIsEmpty_ReturnDefaultValue()
        {
            var stringCalc = BuildStringCalculator();

            var results = stringCalc.Add("");

            Assert.AreEqual(0, results);
        }

        [Test]
        public void Add_ParamIsSingleNumber_ReturnSingleNumber()
        {
            var stringCalc = BuildStringCalculator();

            var results = stringCalc.Add("1");

            Assert.AreEqual(1, results);
        }

        [Test]
        public void Add_ParamIsTwoNumbers_ReturnsSumOfTwoNumbers()
        {
            var stringCalc = BuildStringCalculator();

            var results = stringCalc.Add("1,2");

            Assert.AreEqual(3, results);
        }

        [Test]
        public void Add_ParamIsThreeNumbers_ReturnsSumOfThreeNumbers()
        {
            var stringCalc = BuildStringCalculator();

            var results = stringCalc.Add("1,2,3");

            Assert.AreEqual(6, results);
        }

        [Test]
        public void Add_ParamContainsNewLine_NewLineUsedAsDelimeterAndNumbersAreSummed()
        {
            var stringCalc = BuildStringCalculator();

            var results = stringCalc.Add("1,2\n3");

            Assert.AreEqual(6, results);
        }

        [Test]
        public void Add_ParamContainsSpecifiedDelimeter_UseDelimeterAndSumNumbers()
        {
            var stringCalc = BuildStringCalculator();

            var results = stringCalc.Add("//%\n1%2");

            Assert.AreEqual(3, results);
        }

        [Test]
        public void Add_ParameterContainsNegativeNumbers_ThrowException()
        {
            var stringCalc = BuildStringCalculator();

            var results = Assert.Catch<Exception>(delegate
            {
                stringCalc.Add("-1001,2");
            });

            StringAssert.Contains("-1001", results.Message);
            StringAssert.DoesNotContain("2", results.Message);
        }

        [Test]
        public void Add_ParameterContainsMultipleNegativeNumbers_ThrowException()
        {
            var stringCalc = BuildStringCalculator();

            var results = Assert.Catch<Exception>(delegate
            {
                stringCalc.Add("-1001,2,-400");
            });

            StringAssert.Contains("-1001,-400", results.Message);
            StringAssert.DoesNotContain("2", results.Message);
        }

        [Test]
        public void Add_ParameterContainsNumbersLargerThanAcceptedLargeCalues_IgnoreThoseNumbersAndSum()
        {
            var stringCalc = BuildStringCalculator();

            var results = stringCalc.Add("1001,2");

            Assert.AreEqual(2, results);
        }

        [Test]
        public void Add_ParameterContainsSpecifiedDelimeterWithLengthLargerThanOne_UseDelimeterAndSumNumbers()
        {
            var stringCalc = BuildStringCalculator();

            var results = stringCalc.Add("//[***]\n1***2");

            Assert.AreEqual(3, results);
        }

        [Test]
        public void Add_ParameterContainsMultipleSpecifiedDelimeters_ThenUseAllOfThemAndSumNumbers()
        {
            var stringCal = BuildStringCalculator();

            var results = stringCal.Add("//[*][%]\n1*2%3");

            Assert.AreEqual(6, results);
        }

        private static ThStringCalculator BuildStringCalculator()
        {
            var stringCalc = new ThStringCalculator();
            return stringCalc;
        }
    }

    public class ThStringCalculator
    {
        public int Add(string numbers)
        {
            numbers = ReplaceNewLineWithDefaultDelimeter(numbers);
            numbers = ReplaceSpecifiedDelimeterWithDefaultDelimeter(numbers);

            if (ParameterIsEmpty(numbers))
            {
                return GetDefaultValue();    
            }
            
            CheckForNegativeNumbers(numbers);

            numbers = RemoveLargeNumbers(numbers);

            if (ParameterHasMultipleNumbers(numbers))
            {
                return GetSumOfMultipleNumbers(numbers);
            }

            return HandleSingleNumber(numbers);
        }

        protected virtual string RemoveLargeNumbers(string numbers)
        {
            var numberList = numbers.Split(',');
            var positiveNumbers = numberList.Where(_ => int.Parse(_) <= 1000);

            numbers = string.Join(",", positiveNumbers.Select(_ => _.ToString()));
            return numbers;
        }

        protected virtual void CheckForNegativeNumbers(string numbers)
        {
            if (numbers.Contains("-"))
            {
                var numberList = numbers.Split(',');
                var negativeNumbers = numberList.Where(_ => int.Parse(_) < 0);

                var errorMessage = string.Join(",", negativeNumbers.Select(_ => _.ToString()));
                throw new Exception(string.Format("negatives not allowed {0}", errorMessage));
            }
        }

        protected virtual string ReplaceSpecifiedDelimeterWithDefaultDelimeter(string numbers)
        {
            if (numbers.StartsWith("//"))
            {
                var delimeter = numbers.Substring(2, numbers.IndexOf(",") - 2);
                var delimeterList = delimeter.Replace("][", ",").Split(',');

                numbers = numbers.Substring(numbers.IndexOf(",") + 1);

                foreach (var customDelim in delimeterList)
                {
                    var delimeterToUse = RemoveBracketsInDelimeter(customDelim);
                    numbers = numbers.Replace(delimeterToUse, ",");
                }                
            }

            return numbers;
        }

        protected virtual string RemoveBracketsInDelimeter(string delimeter)
        {
            delimeter = delimeter.Replace("]", "").Replace("[", "");
            return delimeter;
        }

        protected virtual int HandleSingleNumber(string numbers)
        {
            return int.Parse(numbers);
        }

        protected virtual int GetSumOfMultipleNumbers(string numbers)
        {
            var numberList = numbers.Split(',');

            return numberList.Sum(_ => int.Parse(_));
        }

        protected virtual bool ParameterHasMultipleNumbers(string numbers)
        {
            return numbers.Contains(",");
        }

        private static int GetDefaultValue()
        {
            return 0;
        }

        protected virtual bool ParameterIsEmpty(string numbers)
        {
            return numbers.Length == 0;
        }

        protected virtual string ReplaceNewLineWithDefaultDelimeter(string numbers)
        {
            if (numbers.Contains("\n"))
            {
                numbers = numbers.Replace("\n", ",");
            }
            return numbers;
        }
    }
}
