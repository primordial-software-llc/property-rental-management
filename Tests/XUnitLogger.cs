using Api;
using Xunit.Abstractions;

namespace Tests
{
    class XUnitLogger : ILogger
    {
        private ITestOutputHelper Output { get; }

        public XUnitLogger(ITestOutputHelper output)
        {
            Output = output;
        }

        public void Log(string message)
        {
            Output.WriteLine(message);
        }
    }
}
