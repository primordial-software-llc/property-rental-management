using System;
using Api.Reports;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Reports
{
    public class DailyIncome
    {
        private ITestOutputHelper Output { get; }

        public DailyIncome(ITestOutputHelper output)
        {
            Output = output;
        }

        /// <summary>
        /// Some income doesn't get entered into QB as it happens.
        /// These sales are in excel.
        /// </summary>
        [Fact]
        public void PrintDailyIncome()
        {
            var reportDate = DateTime.Now.ToString("yyyy-MM-dd");
            DailyIncomeReport.PrintReport(reportDate, new XUnitLogger(Output));
        }

    }
}
