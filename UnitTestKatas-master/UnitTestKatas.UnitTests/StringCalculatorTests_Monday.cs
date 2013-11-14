using System;
using System.Linq;
using NUnit.Framework;

namespace StringCalc.UnitTests
{
    [TestFixture]
    public class StringCalculatorTestsMonday
    {
        [Test]
        public void Add_Empty_ThenReturnDefaultValue()
        {
            var calculator = GetStringCalculator();

            int results = calculator.Add("");

            Assert.AreEqual(0, results);
        }

        [Test]        
        public void Add_SingleNumber_ReturnsSingleNumber()
        {
            var calculator = GetStringCalculator();

            int results = calculator.Add("1");

            Assert.AreEqual(1, results);
        }

        [Test]
        public void Add_MultipleNumbers_ReturnsSumOfNumbers()
        {
            var calculator = GetStringCalculator();

            int results = calculator.Add("1,2");

            Assert.AreEqual(3, results);
        }

        [Test]
        public void Add_MutipleNumbersWithNewLineAsDelimeter_ReturnsSumOfNumbers()
        {
            var calculator = GetStringCalculator();

            int results = calculator.Add("1,2\n3");

            Assert.AreEqual(6, results);
        }

        [Test]
        public void Add_MultipleNumbersWithDelimiterSpecified_UsesDelimeterAndReturnsSumOfNumbers()
        {
            var calculator = GetStringCalculator();

            int results = calculator.Add("//$\n1$2\n4");

            Assert.AreEqual(7, results);
        }

        [Test]
        [TestCase("-1", "-1")]
        [TestCase("-2,-3,4", "-2,-3")]
        public void Add_NegativeNumbers_ExceptionIsThrown(string input, string expectedExceptionMessage)
        {
            var calculator = GetStringCalculator();

            var exception = Assert.Catch<ArgumentException>(delegate
            {
                calculator.Add(input);
            });

            StringAssert.Contains(expectedExceptionMessage, exception.Message);            
        }

        [Test]
        public void Add_NumbersLargerThenOneThousand_LargeNumberIsIgnored()
        {
            var calculator = GetStringCalculator();

            int results = calculator.Add("1001,2");

            Assert.AreEqual(2, results);
        }

        [Test]
        public void Add_AnylengthDelimiter_UsesDelimeterAndReturnsSumOfNumbers()
        {
            var calculator = GetStringCalculator();

            int results = calculator.Add("//[***]\n1***2***3");

            Assert.AreEqual(6, results);
        }

        [Test]
        public void Add_MultipleDelimiters_UsesMultipleDelimeterAndReturnsSumOfNumbers()
        {
            var calculator = GetStringCalculator();

            int results = calculator.Add("//[*][%]\n1*2%3");

            Assert.AreEqual(6, results);
        }

        [Test]
        public void Add_MultipleDelimitersOfAnyLength_UsesMultipleDelimeterAndReturnsSumOfNumbers()
        {
            var calculator = GetStringCalculator();

            int results = calculator.Add("//[***][%%]\n1***2%%3");

            Assert.AreEqual(6, results);
        }

        private StringCalculatorMonday GetStringCalculator()
        {
            return new StringCalculatorMonday();
        }

        public class StringCalculatorMonday
        {
            public const int LargeValue = 1000;

            public int Add(string numbers)
            {                
                numbers = ReplaceNewLinesWithDelimeter(numbers);
                numbers = ReplaceMultipleSpecifiedDelimiter(numbers);
                numbers = ReplaceSpecifiedDelimiterWithDefaultDelimeter(numbers);
                
                if (IsEmptyInput(numbers))
                {
                    return DefaultValue();
                }

                numbers = ReplaceNumbersLargerThanLargeValue(numbers);
                HandleNegativeNumbers(numbers);

                if (IsMultipleNumbers(numbers))
                {
                    return HandleMultipleNumbers(numbers);
                }

                return HandleSingleNumber(numbers);
            }

            protected virtual string ReplaceNumbersLargerThanLargeValue(string numbers)
            {
                var numbersList = numbers.Split(',');

                var numberListWithoutLargeNumbers = numbersList.Where(number => int.Parse(number) <= LargeValue).ToList();

                return string.Join(",", numberListWithoutLargeNumbers.Select(number => number.ToString()));
            }

            protected virtual int HandleSingleNumber(string numbers)
            {
                return int.Parse(numbers);
            }

            protected virtual int HandleMultipleNumbers(string numbers)
            {
                var numberList = numbers.Split(',');
                return numberList.Sum(number => int.Parse(number));
            }

            protected virtual void HandleNegativeNumbers(string numbers)
            {
                var numberList = numbers.Split(',');
                var message = string.Empty;

                foreach (var number in numberList.Where(number => int.Parse(number) < 0))
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        message = number;
                    }
                    else
                    {
                        message = message + "," + number;
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(string.Format("Negatives not allowed {0}", message));
                }
            }

            protected virtual string ReplaceNewLinesWithDelimeter(string numbers)
            {
                if (numbers.Contains("\n"))
                {
                    numbers = numbers.Replace("\n", ",");
                }

                return numbers;
            }

            protected virtual string ReplaceSpecifiedDelimiterWithDefaultDelimeter(string numbers)
            {                
                if (numbers.StartsWith("//"))
                {
                    var delimiter = GetDelimiter(numbers);

                    delimiter = RemoveDelimeterBrackets(delimiter);

                    numbers = GetNumberListWithSpecifiedDelimeter(numbers);

                    return numbers.Replace(delimiter, ",");
                }

                return numbers;
            }

            protected virtual string GetNumberListWithSpecifiedDelimeter(string numbers)
            {
                return numbers.Substring(numbers.IndexOf(",") + 1);
            }

            protected virtual string ReplaceMultipleSpecifiedDelimiter(string numbers)
            {
                if (numbers.StartsWith("//") && numbers.Contains("]["))
                {
                    var delimeters = GetDelimiter(numbers);

                    var delimeterList = RemoveDelimeterBrackets(delimeters.Replace("][", ",")).Split(',');

                    numbers = GetNumberListWithSpecifiedDelimeter(numbers);

                    foreach (var delimeter in delimeterList)
                    {
                        numbers = numbers.Replace(delimeter, ",");
                    }                    
                }

                return numbers;
            }

            protected virtual string GetDelimiter(string numbers)
            {
                var delimiter = numbers.Substring(2, numbers.IndexOf(",") - 2);
                
                return delimiter;
            }

            protected virtual string RemoveDelimeterBrackets(string delimiter)
            {
                delimiter = delimiter.Replace("[", "").Replace("]", "");

                return delimiter;
            }

            protected virtual int DefaultValue()
            {
               return 0;
            }

            protected virtual bool IsEmptyInput(string numbers)
            {
                return string.IsNullOrEmpty(numbers);
            }

            protected virtual bool IsMultipleNumbers(string numbers)
            {
                return numbers.Contains(",");
            }
        }
    }
}
