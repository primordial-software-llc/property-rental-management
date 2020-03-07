using System;
using AwsTools;

namespace Api
{
    class Logger : ILogging, ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
