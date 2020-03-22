using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Api.QuickBooksOnline;
using Api.QuickBooksOnline.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tests.PdfBankStatementProcessing;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class ProcessBankStatements
    {
        private ITestOutputHelper Output { get; }
        private QuickBooksOnlineClient QboClient { get; set; }

        public ProcessBankStatements(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void RunProcessBankStatementsData()
        {
            QboClient = Factory.CreateQuickBooksOnlineClient(new XUnitLogger(Output));

            //Output.WriteLine(JsonConvert.SerializeObject();



            /*
            Output.WriteLine(JsonConvert.SerializeObject(
                GetWithdrawalTransactions(ProcessBankStatementsData.lakelandmerchantdecember2019),
                Formatting.Indented));
                */

            CreateExpenses("134", ProcessBankStatementsData.lakeland2august2019);
        }

        public void CreateExpenses(string qboBankAccountId, string pdfStatement)
        {
            var transactions = GetWithdrawalTransactions(pdfStatement);
            foreach (var transaction in transactions)
            {
                var accountId = GetAccountId(transaction.Description);
                if (accountId < 1)
                {
                    continue;
                }
                Output.WriteLine(transaction.Description);

                var qboDate = transaction.Date.ToString("yyyy-MM-dd");
                var existing = QboClient.Query<Purchase>($"select * from purchase where TxnDate = '{qboDate}' and TotalAmt = '{transaction.Amount * -1}'");
                if (existing.Any())
                {
                    Output.WriteLine("Found duplicate date and amount. Skipping: " + JsonConvert.SerializeObject(transaction));
                }
                var qboExpense = new Purchase
                {
                    AccountRef = new Reference { Value = qboBankAccountId },
                    TxnDate = qboDate,
                    PaymentType = "Check",
                    Line = new List<PurchaseLine>
                    {
                        new PurchaseLine
                        {
                            DetailType = "AccountBasedExpenseLineDetail",
                            Amount = transaction.Amount * -1,
                            AccountBasedExpenseLineDetail = new AccountBasedExpenseLineDetail
                            {
                                TaxCodeRef = new Reference { Value = "NON" },
                                AccountRef = new Reference { Value = accountId.ToString() },
                                BillableStatus = "NotBillable"
                            },
                            Description = transaction.Description
                        }
                    },
                    PrivateNote = "Created using QuickBooks Online API's from PDF Account Statement by Timothy Gonzalez"
                };
                var updateJson = JsonConvert.SerializeObject(qboExpense);
                QboClient.Request("purchase", HttpMethod.Post, updateJson);
            }
        }

        public int GetAccountId(string transactionDescription)
        {
            if (transactionDescription.StartsWith("Online Banking transfer to CHK 6222", StringComparison.OrdinalIgnoreCase))
            {
                return 133; // Owners Draw
            }

            return 0;
        }

        public List<BankTransaction> GetWithdrawalTransactions(string pdfStatement)
        {
            var transactions = new List<BankTransaction>();
            var withdrawalLines = GetWithdrawals(pdfStatement);
            foreach (var line in withdrawalLines)
            {
                
                var dateIndex = line.IndexOf(" ");
                var date = line.Substring(0, dateIndex);

                var amountIndex = line.LastIndexOf(" ");
                var amount = line.Substring(amountIndex + 1);

                var descriptionStart = line.Substring(dateIndex + 1);
                var description = descriptionStart.Substring(0, descriptionStart.LastIndexOf(" "));
                var transaction = new BankTransaction
                {
                    Date = DateTime.ParseExact(date, "MM/dd/yy", CultureInfo.InvariantCulture),
                    Description = description,
                    Amount = decimal.Parse(amount)
                };
                transactions.Add(transaction);
            }

            return transactions;
        }

        public List<string> GetWithdrawals(string pdfStatement)
        {
            var debitsMarkerStart = @"Withdrawals and other debits
Date Description Amount";
            var withdrawalsRawTextStartIndex = pdfStatement.IndexOf(debitsMarkerStart, StringComparison.OrdinalIgnoreCase)
                                               + debitsMarkerStart.Length;
            var withdrawalsRawText = pdfStatement.Substring(withdrawalsRawTextStartIndex);
            var debitsMarkerEnd = "Total withdrawals and other";
            var debitsMarkerEndIndex = withdrawalsRawText.IndexOf(debitsMarkerEnd, StringComparison.OrdinalIgnoreCase);
            withdrawalsRawText = withdrawalsRawText.Substring(0, debitsMarkerEndIndex);

            var withdrawalsLines = withdrawalsRawText.Split(Environment.NewLine).ToList();
            for (var ct = withdrawalsLines.Count - 1; ct > 0; ct -= 1)
            {
                var line = withdrawalsLines[ct];
                if (line.StartsWith("continued on the next page", StringComparison.Ordinal) ||
                    line.StartsWith("Page", StringComparison.Ordinal) ||
                    line.StartsWith("Withdrawals and other debits - continued", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("LAKELAND MI PUEBLO FLEA MARKET INC", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("Your checking account", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("Subtotal for card account #", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("Card account # XXXX", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("Date Description Amount", StringComparison.OrdinalIgnoreCase))
                {
                    withdrawalsLines.RemoveAt(ct);
                }
            }

            for (var ct = withdrawalsLines.Count - 1; ct > 0; ct -= 1)
            {
                var line = withdrawalsLines[ct];
                if (line.StartsWith("ID:", StringComparison.Ordinal) ||
                    line.StartsWith("CO ID:", StringComparison.Ordinal) ||
                    line.EndsWith("XXXX 3414", StringComparison.Ordinal) ||
                    line.EndsWith("XXXX 0201", StringComparison.Ordinal))
                {
                    withdrawalsLines[ct - 1] += line;
                    withdrawalsLines.RemoveAt(ct);
                    withdrawalsLines[ct - 1] += " " + withdrawalsLines[ct];
                    withdrawalsLines.RemoveAt(ct);
                }
            }

            return withdrawalsLines
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}
