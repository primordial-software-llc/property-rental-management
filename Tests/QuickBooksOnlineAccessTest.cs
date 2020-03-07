using System;
using System.Linq;
using System.Net.Http;
using Api.QuickBooksOnline;
using Api.QuickBooksOnline.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;
using Customer = Api.QuickBooksOnline.Models.Customer;
using Invoice = Api.QuickBooksOnline.Models.Invoice;

namespace Tests
{
    public class QuickBooksOnlineAccessTest
    {
        private ITestOutputHelper Output { get; }

        public QuickBooksOnlineAccessTest(ITestOutputHelper output)
        {
            Output = output;
        }

        //[Fact]
        public void Inactivate_Customers_By_Transaction_Report()
        {
            var client = new QuickBooksOnlineClient(new XUnitLogger(Output));
            var activeCustomers = client.QueryAll<Customer>("select * from Customer Where Active = true");
            var start = DateTime.Now.AddDays(-120).ToString("yyyy-MM-dd");
            var end = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");

            foreach (var customer in activeCustomers)
            {
                var rawTransactionReport = client.Request($"reports/TransactionList?start_date={start}&end_date={end}&customer={customer.Id}", HttpMethod.Get);
                var transactionReport = JsonConvert.DeserializeObject<TransactionListReport>(rawTransactionReport);
                if (transactionReport.Rows?["Row"] == null || !transactionReport.Rows["Row"].Any())
                {

                    var jsonUpdate = new JObject
                    {
                        { "SyncToken", "0" },
                        { "Id", customer.Id },
                        { "Active", false },
                        { "sparse", true }
                    };

                    try
                    {
                        client.Request("customer", HttpMethod.Post, jsonUpdate.ToString());
                        Output.WriteLine($"No transactions for customer {customer.DisplayName} with customer id {customer.Id}."
                                         + $" The customer will be set to inactive.");
                    }
                    catch (Exception)
                    {
                        Output.WriteLine($"No transactions for customer {customer.DisplayName} with customer id {customer.Id}."
                                         + $" The customer cannot be set to inactive, because it has a balance.");
                    }
                }
            }
        }

        //[Fact]
        public void Test_Request()
        {
            //Output.WriteLine(Get($"companyinfo/{Configuration.RealmId}"));
            //Output.WriteLine(Get($"query?query={HttpUtility.UrlEncode()}"));

            var oldestDate = DateTime.Now.AddDays(-120).ToString("yyyy-MM-dd");
            var client = new QuickBooksOnlineClient(new XUnitLogger(Output));
            var activeCustomers = client.QueryAll<Customer>("select * from Customer Where Active = true");
            foreach (var customer in activeCustomers)
            {
                var payments = client.Query<Payment>($"select * from Payment Where CustomerRef = '{customer.Id}' and TxnDate > '{oldestDate}'");
                if (payments.Count > 0)
                {
                    continue;
                }

                var salesReceipts = client.Query<SalesReceipt>($"select * from SalesReceipt Where CustomerRef = '{customer.Id}' and TxnDate > '{oldestDate}'");
                if (salesReceipts.Count > 0)
                {
                    continue;
                }

                var invoices = client.Query<Invoice>($"select * from Invoice Where CustomerRef = '{customer.Id}' and TxnDate > '{oldestDate}'");
                if (invoices.Count > 0)
                {
                    continue;
                }
                Output.WriteLine("Customer may not be active: " + customer.Id);
                throw new Exception("stop");
            }
        }
    }
}
