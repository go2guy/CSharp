using System;
using System.Runtime;
using NUnit.Framework;

namespace StringCalc.UnitTests
{
    [TestFixture]
    public class StringCalculatorTests
    {
        [Test]
        public void Add_ParameterIsEmpty_ReturnZero()
        {
            var calculator = GetStringCalculator();

            var results = calculator.Add("");

            Assert.AreEqual(0, results);
        }

        [Test]
        public void Add_ParameterHasSingleValue_ReturnsSingleValue()
        {
            var calculator = GetStringCalculator();

            var results = calculator.Add("1");

            Assert.AreEqual(1, results);
        }

        [Test]
        public void Add_ParameterHasTwoValues_ReturnsSumOfTwoValues()
        {
            var calculator = GetStringCalculator();

            var results = calculator.Add("1,2");

            Assert.AreEqual(3, results);
        }

        [Test]
        public void Add_ParameterHasThreeValues_ReturnsSumOfThreeValues()
        {
            var calculator = GetStringCalculator();

            var results = calculator.Add("1,2,3");

            Assert.AreEqual(6, results);
        }

        [Test]
        public void Add_ParameterHasNewLineAsDelimeter_UseNewLineAsDelimeterAndSumValues()
        {
            var calculator = GetStringCalculator();

            var results = calculator.Add("1,2\n3");

            Assert.AreEqual(6, results);
        }

        [Test]
        public void Add_ParameterSpecificesDelimeter_UseCustomerDelimeterAndSumValues()
        {
            var calculator = GetStringCalculator();

            var results = calculator.Add("//$\n1$2");

            Assert.AreEqual(3, results);
        }

        [Test]        
        public void Add_ParameterContainsNegativeValues_ExceptionIsThrown()
        {
            var calculator = GetStringCalculator();

            var results = Assert.Catch<ArgumentException>(delegate
            {
                calculator.Add("-1");
            });

            StringAssert.Contains("-1", results.Message);
        }

        [Test]
        public void Add_ParameterContainsNegativeAndPostiveValuesExceptionIsThrownAndOnlyNegativeNumbersListed()
        {
            var calculator = GetStringCalculator();

            var results = Assert.Catch<ArgumentException>(delegate
            {
                calculator.Add("-1,-2,3");
            });
            
            StringAssert.DoesNotContain("3", results.Message);
        }        

        [Test]
        public void Add_ParameterHasNumbersLargerThanLargeValue_LargeNumberIsIgnored()
        {
            var calculator = GetStringCalculator();

            var results = calculator.Add("1001,2");

            Assert.AreEqual(2, results);
        }

        [Test]
        public void Add_ParameterHasCustomDelimeterWithLengthGreaterThanOne_UseThatDelimeterAndSumValues()
        {
            var calculator = GetStringCalculator();

            var results = calculator.Add("//[***]\n1***2");

            Assert.AreEqual(3, results);
        }

        [Test]
        [TestCase("1",1)]
        [TestCase("", 0)]
        [TestCase("1,2", 3)]
        [TestCase("//;\n1;2", 3)]
        public void Add_Invoked_AddCalledEventFired(string input, int expectedResult)
        {
            var stringCalculator = GetStringCalculator();

            string eventinput = null;
            int result = 0;

            stringCalculator.AddCalled += delegate(string s, int i)
            {
                eventinput = s;
                result = i;
            };

            stringCalculator.Add(input);

            Assert.AreEqual(input, eventinput);
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase("1", 1)]
        [TestCase("", 0)]
        [TestCase("1,2", 3)]
        [TestCase("//;\n1;2", 3)]
        public void CalculatorListener_AddCalledOnCalculator_HandlesAddCalledEvent(string input, int expectedResult)
        {
            var stringCalculator = GetStringCalculator();
            var cl = new CalculatorListener(stringCalculator);

            string message = null;
            cl.GotEvent += delegate(string s)
            {
                message = s;
            };

            stringCalculator.Add(input);

            Assert.AreEqual(string.Format("Got Event: Add called with [{0}] and returned [{1}]", input, expectedResult), message);
        }

        [Test]
        public void Add_SettingsDisabled_ThrowsException()
        {
            var stubSettings = new FakeSettings { WillStringCalculatorBeEnabled = false };

            var sc = GetStringCalculator(stubSettings);

            Assert.Throws<Exception>(() => sc.Add(""));
        }

        [Test]
        [TestCase("", "0")]
        [TestCase("1", "1")]
        [TestCase("1,2", "3")]
        public void Add_Always_CallsLoggerWithSum(string input, string expectedOutput)
        {
            var mockWriter = new FakeLogger();

            var sc = GetStringCalculator(mockWriter);

            sc.Add(input);

            StringAssert.Contains(expectedOutput, mockWriter.WriteMethodValue);
        }

        [Test]
        [TestCase("")]
        [TestCase("1")]
        [TestCase("1,2")]
        public void Add_WhenLoggerThrowsException_NotifyWebservice(string input)
        {
            var mockWebService = new FakeWebService();

            var stubWriter = new FakeLogger
            {
                WillThrow = new Exception("SomeMessage")
            };

            var sc = GetStringCalculator(stubWriter, mockWebService);
            
            sc.Add(input);            

            StringAssert.Contains("SomeMessage", mockWebService.WriteMessage);
        }

        private static StringCalculator GetStringCalculator(FakeLogger stubWriter, FakeWebService mockWebService)
        {
            var sc = new StringCalculator(new FakeSettings {WillStringCalculatorBeEnabled = true}, stubWriter, mockWebService);
            return sc;
        }

        private static StringCalculator GetStringCalculator(FakeLogger mockWriter)
        {
            return new StringCalculator(new FakeSettings {WillStringCalculatorBeEnabled = true}, mockWriter, new FakeWebService());            
        }

        private static StringCalculator GetStringCalculator()
        {
            return new StringCalculator(new FakeSettings { WillStringCalculatorBeEnabled = true }, new FakeLogger(), new FakeWebService());            
        }

        private static StringCalculator GetStringCalculator(FakeSettings stubSettings)
        {
            return new StringCalculator(stubSettings, new FakeLogger(), new FakeWebService());            
        }
    }

    public class FakeSettings: ISettings
    {
        public bool WillStringCalculatorBeEnabled;

        public bool IsEnabled()
        {
            return WillStringCalculatorBeEnabled;
        }        
    }

    public class FakeLogger : ILogWriter
    {
        public string WriteMethodValue;
        public Exception WillThrow;

        public void Write(string output)
        {            
            WriteMethodValue = output;
            if (WillThrow != null)
            {
                throw WillThrow;
            }
        }
    }   
 
    public class FakeWebService : IWebservice
    {
        public string WriteMessage;

        public void Write(string message)
        {
            WriteMessage = message;
        }
    }
}
