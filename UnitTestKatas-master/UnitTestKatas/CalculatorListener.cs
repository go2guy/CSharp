using System;

namespace StringCalc
{
    public class CalculatorListener
    {
        private readonly StringCalculator _stringCalculator;

        public CalculatorListener(StringCalculator stringCalculator)
        {
            _stringCalculator = stringCalculator;
            _stringCalculator.AddCalled += AddCalled;
        }

        public event Action<string> GotEvent = delegate { };

        protected virtual void AddCalled(string input, int output)
        {
            GotEvent(string.Format("Got Event: Add called with [{0}] and returned [{1}]", input, output));
        }
    }
}
