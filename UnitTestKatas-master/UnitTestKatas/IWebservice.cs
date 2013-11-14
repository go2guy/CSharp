using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace StringCalc
{
    public interface IWebservice
    {
        void Write(string message);
    }
}
