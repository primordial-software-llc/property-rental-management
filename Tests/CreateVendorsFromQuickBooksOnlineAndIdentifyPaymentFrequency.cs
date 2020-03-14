using System;
using System.Globalization;
using System.Linq;
using Api;
using Api.DatabaseModel;
using Api.QuickBooksOnline;
using Api.QuickBooksOnline.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class CreateVendorsFromQuickBooksOnlineAndIdentifyPaymentFrequency
    {
        private ITestOutputHelper Output { get; }

        public CreateVendorsFromQuickBooksOnlineAndIdentifyPaymentFrequency(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Run()
        {
            var client = new QuickBooksOnlineClient(new XUnitLogger(Output));
            var activeCustomers = client.QueryAll<Customer>("select * from Customer Where Active = true");
            var start = new DateTime(2020, 2, 1).ToString("yyyy-MM-dd");

            foreach (var customer in activeCustomers)
            {
                var isWeekly = IsWeekly(customer.Id, start);
                string paymentFrequency = isWeekly.HasValue && isWeekly.Value ? "weekly" : string.Empty;
                Output.WriteLine($"{customer.Id}: {paymentFrequency}");
                new VendorService().Create(int.Parse(customer.Id), true, paymentFrequency);
            }
        }

        public bool? IsWeekly(string customerId, string date)
        {
            var client = new QuickBooksOnlineClient(new XUnitLogger(Output));
            var salesReceipts = client.QueryAll<SalesReceipt>(
                $"select * from SalesReceipt Where TxnDate >= '{date}' and CustomerRef = '{customerId}'");
            var invoices = client.QueryAll<Invoice>(
                $"select * from Invoice Where TxnDate >= '{date}' and CustomerRef = '{customerId}'");

            if (salesReceipts.Count() + invoices.Count() > 2)
            {
                return true;
            }

            return null;
        }

    }
}
