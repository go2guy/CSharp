using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace StringCalc
{
    public class settingPlaceholder : ISettings
    {
        public bool IsEnabled()
        {
            return true;
        }
    }

    public class LogWriterPlaceHolder : ILogWriter
    {
        public void Write(string output)
        {
        }
    }

    public class WebServicePlaceHolder : IWebservice
    {
        public void Write(string message)
        {
        }
    }

    public class Program
    {      
        public static void Main(string[] args)
        {
            var stringCalc = new StringCalculator(new settingPlaceholder(), new LogWriterPlaceHolder(), new WebServicePlaceHolder());

            Console.WriteLine(stringCalc.Add(args[0]));                
        }
    }
}
