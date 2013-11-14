using System;
using System.Linq;

namespace StringCalc
{
    public class StringCalculator
    {
        private readonly ISettings _settings;
        private readonly ILogWriter _logWriter;
        private readonly IWebservice _webService;

        public StringCalculator(ISettings settings, ILogWriter logWriter, IWebservice webService)
        {
            _settings = settings;
            _logWriter = logWriter;
            _webService = webService;
        }

        public event Action<string, int> AddCalled = delegate { };

        public int Add(string numbers)
        {            
            if (!_settings.IsEnabled())
            {                
                throw new Exception();
            }

            var origNumbers = numbers;

            numbers = ReplaceNewLineWithStandardDelimeter(numbers);
            numbers = ReplaceCustomDelimeterWithStandardDelimeter(numbers);

            HandleNegativeNumbers(numbers);

            if (IsEmptyInput(numbers))
            {
                AddCalled(origNumbers, GetDefaultValue());
                WriteToLog(GetDefaultValue());
                return GetDefaultValue();
            }

            numbers = RemoveLargeNumbers(numbers);

            if (HasMultipleNumbers(numbers))
            {
                AddCalled(origNumbers, HandleMultipleNumbers(numbers));
                WriteToLog(HandleMultipleNumbers(numbers));
                return HandleMultipleNumbers(numbers);
            }

            WriteToLog(HandleSingleValue(numbers));
            AddCalled(origNumbers, HandleSingleValue(numbers));
            return HandleSingleValue(numbers);
        }

        protected virtual void WriteToLog(int value)
        {
            try
            {
                _logWriter.Write(value.ToString());
            }
            catch (Exception ex)
            {
                _webService.Write(ex.Message);                
            }
        }

        protected virtual string RemoveLargeNumbers(string numbers)
        {
            var numberList = numbers.Split(',');
            if (numberList.Any(number => int.Parse(number) > 1000))
            {
                var smallNumberList = numberList.Where(number => int.Parse(number) <= 1000);

                numbers = string.Join(",", smallNumberList.Select(number => number));
            }
            return numbers;
        }

        protected virtual void HandleNegativeNumbers(string numbers)
        {
            if (numbers.Contains("-"))
            {
                var numberList = numbers.Split(',');
                var negativeNumbers = numberList.Where(number => int.Parse(number) < 0);

                var negativeNumberString = string.Join(",", negativeNumbers.Select(number => number));

                throw new ArgumentException(string.Format("Negatives not allowed {0}", negativeNumberString));
            }
        }

        protected virtual string ReplaceCustomDelimeterWithStandardDelimeter(string numbers)
        {
            if (numbers.StartsWith("//"))
            {
                var delimeter = numbers.Substring(2, numbers.IndexOf(",") - 2);

                delimeter = RemoveBracketsFromDelimeter(delimeter);

                numbers = numbers.Substring(numbers.IndexOf(",") + 1).Replace(delimeter, ",");
            }
            return numbers;
        }

        protected virtual string RemoveBracketsFromDelimeter(string delimeter)
        {
            return delimeter.Replace("[", "").Replace("]", "");
        }

        protected virtual string ReplaceNewLineWithStandardDelimeter(string numbers)
        {
            if (numbers.Contains("\n"))
            {
                numbers = numbers.Replace("\n", ",");
            }
            return numbers;
        }

        protected virtual int HandleMultipleNumbers(string numbers)
        {
            var numberList = numbers.Split(',');
            return numberList.Sum(number => int.Parse(number));
        }

        protected virtual bool HasMultipleNumbers(string numbers)
        {
            return numbers.Contains(",");
        }

        protected virtual int HandleSingleValue(string numbers)
        {
            return int.Parse(numbers);
        }

        protected virtual int GetDefaultValue()
        {
            return 0;
        }

        protected virtual bool IsEmptyInput(string numbers)
        {
            return string.IsNullOrEmpty(numbers);
        }
    }
}
