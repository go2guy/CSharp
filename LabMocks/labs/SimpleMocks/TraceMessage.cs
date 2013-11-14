using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Step1Mocks
{
    public class TraceMessage
    {        
        public TraceMessage(int severity, string message)
        {
            Severity = severity;
            Message = message;
        }

        public int Severity { get; set; }

        public string Message { get; set; }
    }
}
