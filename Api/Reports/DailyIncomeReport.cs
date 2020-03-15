using System.Collections.Generic;
using System.Linq;
using Amazon;
using Amazon.DynamoDBv2;
using Api.DatabaseModel;
using Api.QuickBooksOnline;
using Api.QuickBooksOnline.Models;

namespace Api.Reports
{
    public class DailyIncomeReport
    {
        public const int CUSTOMER_PARKING_A = 1859;
        public const int CUSTOMER_PARKING_B = 1861;
        public const int CUSTOMER_BAR_A = 1862;
        public const int CUSTOMER_BAR_B = 1863;
        public const int CUSTOMER_RESTAURANT = 1864;

        public static void PrintReport(string reportDate, ILogger logger)
        {
            var awsDbClient = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            var databaseClient = new DatabaseClient<QuickBooksOnlineConnection>(awsDbClient);
            var qboConnection = databaseClient.Get(new QuickBooksOnlineConnection { RealmId = Configuration.RealmId });
            var qboClient = new QuickBooksOnlineClient(qboConnection, logger);
            var rentalCustomerIds = new List<int> { CUSTOMER_PARKING_A, CUSTOMER_PARKING_B, CUSTOMER_BAR_A, CUSTOMER_BAR_B, CUSTOMER_RESTAURANT };

            var salesReceipts = qboClient.QueryAll<SalesReceipt>($"select * from SalesReceipt Where TxnDate = '{reportDate}'")
                .Where(x => !rentalCustomerIds.Contains(int.Parse(x.CustomerRef.Value)));

            var payments = qboClient.QueryAll<Payment>($"select * from Payment Where TxnDate = '{reportDate}'")
                .Where(x => !rentalCustomerIds.Contains(int.Parse(x.CustomerRef.Value)));

            var rentInQuickBooks = (salesReceipts.Sum(x => x.TotalAmount) + payments.Sum(x => x.TotalAmount));
            logger.Log($"Income for {reportDate}");
            logger.Log($"Parking A: {GetTotalIncomeFromCustomer(qboClient, reportDate, CUSTOMER_PARKING_A, logger):C}");
            logger.Log($"Parking B: {GetTotalIncomeFromCustomer(qboClient, reportDate, CUSTOMER_PARKING_B, logger):C}");
            logger.Log($"Bar A: {GetTotalIncomeFromCustomer(qboClient, reportDate, CUSTOMER_BAR_A, logger):C}");
            logger.Log($"Bar B: {GetTotalIncomeFromCustomer(qboClient, reportDate, CUSTOMER_BAR_B, logger):C}");
            logger.Log($"Restaurant (cash): {GetTotalIncomeFromCustomer(qboClient, reportDate, CUSTOMER_RESTAURANT, logger):C}");
        }

        public static decimal GetTotalIncomeFromCustomer(QuickBooksOnlineClient client, string date, int customerId, ILogger logger)
        {
            var salesReceipts = client.QueryAll<SalesReceipt>(
                $"select * from SalesReceipt Where TxnDate = '{date}' and CustomerRef = '{customerId}'");
            var payments = client.QueryAll<Payment>(
                $"select * from Payment Where TxnDate = '{date}' and CustomerRef = '{customerId}'");
            return salesReceipts.Sum(x => x.TotalAmount) + payments.Sum(x => x.TotalAmount);
        }
    }
}
