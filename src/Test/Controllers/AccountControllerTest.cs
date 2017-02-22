using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using System.Net.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using LMS.Service;
using LMS.Service.Controllers;
using LMS.Model;
using LMS.Model.Resource;
using LMS.Model.Composite;
using LMS.Data;
using LMS.Model.Resource.Enums;

namespace LMS.Test.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        private UnitTestContext<Account> Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new UnitTestContext<Account>();

            Context.TestData.Add(Context.LabelRepository.Add(new Label()
            {
                Name = "lplabelgroup",
                Address = new Address()
                {
                    Line = new string[] { "Skansegade 1D", "Vorup" },
                    PostalCode = "8940",
                    City = "Randers SV",
                    Country = "Denmark",
                    District = "-",
                    State = "-"
                },
                Email = "mail@lplabelgroup.com",
                Telecom = "+4541990756"
            }));

            Context.TestData.Add(Context.ArtistRepository.Add(new Artist()
            {
                StageName = "Tomtek",
                Name = new HumanName()
                {
                    Family = new string[] { "Lykke", "Petersen" },
                    Given = new string[] { "Thomas" }
                },
                Address = new Address()
                {
                    Line = new string[] { "Skansegade 1D", "Vorup" },
                    PostalCode = "8940",
                    City = "Randers SV",
                    Country = "Denmark",
                    District = "-",
                    State = "-"
                },
                Email = "thomas@ktrecordings.com",
                Telecom = "+45 41990756"
            }));

            Context.Template = new Account()
            {
                Name = "XXR001",
                Status = AccountStatus.Open,
                Transactions = null,
                Artist = new Reference(Reference.ArtistUri, Context.TestData.Find(m => m is Artist).Id),
                Label = new Reference(Reference.LabelUri, Context.TestData.Find(m => m is Label).Id)
            };
        }

        [TestCleanup]
        public void TestCleanup()
        {
            foreach (DomainResource data in Context.TestData)
            {
                if (data is Label)
                    Context.LabelRepository.Remove(data.Id);
                if (data is Artist)
                    Context.ArtistRepository.Remove(data.Id);
                if (data is Account)
                    Context.AccountRepository.Remove(data.Id);
                if (data is Transaction)
                    Context.TransactionRepository.Remove(data.Id);
            }
        }

        [TestMethod]
        public void AccountCreateTest()
        {
            AccountController controller = new AccountController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Account account = Context.Template;
            account.Name = null;

            IHttpActionResult resultERR = controller.Create(account);
            Assert.IsTrue(resultERR is BadRequestErrorMessageResult);

            account.Name = "XXR001";

            OkNegotiatedContentResult<Account> resultOK = controller.Create(account) as OkNegotiatedContentResult<Account>;

            Context.TestData.Add(resultOK.Content);

            Assert.IsNotNull(resultOK.Content);
            Assert.IsTrue(resultOK.Content.Id > 0);
            Assert.AreEqual(resultOK.Content.Name, account.Name);
            Assert.AreEqual(resultOK.Content.Status, account.Status);
            Assert.IsNull(resultOK.Content.Transactions);
            Assert.IsNotNull(resultOK.Content.Artist);
            Assert.AreEqual(resultOK.Content.Artist.GetId(), account.Artist.GetId());
            Assert.IsNotNull(resultOK.Content.Label);
            Assert.AreEqual(resultOK.Content.Label.GetId(), account.Label.GetId());
        }

        [TestMethod]
        public void AccountReadTest()
        {
            AccountController controller = new AccountController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Account account = Context.Template;

            OkNegotiatedContentResult<Account> resultOK = controller.Create(account) as OkNegotiatedContentResult<Account>;
            Context.TestData.Add(resultOK.Content);

            resultOK = controller.Read(resultOK.Content.Id) as OkNegotiatedContentResult<Account>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.Name, account.Name);
            Assert.AreEqual(resultOK.Content.Status, account.Status);
            Assert.IsNull(resultOK.Content.Transactions);
            Assert.IsNotNull(resultOK.Content.Artist);
            Assert.AreEqual(resultOK.Content.Artist.GetId(), account.Artist.GetId());
            Assert.IsNotNull(resultOK.Content.Label);
            Assert.AreEqual(resultOK.Content.Label.GetId(), account.Label.GetId());
        }

        [TestMethod]
        public void AccountUpdateTest()
        {
            AccountController controller = new AccountController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Account account = Context.Template;

            OkNegotiatedContentResult<Account> resultOK = controller.Create(account) as OkNegotiatedContentResult<Account>;
            Context.TestData.Add(resultOK.Content);

            account = resultOK.Content;
            account.Name = "XXR001_changed";
            account.Status = AccountStatus.Closed;

            resultOK = controller.Update(account) as OkNegotiatedContentResult<Account>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.Id, account.Id);
            Assert.AreEqual(resultOK.Content.Name, account.Name);
            Assert.AreEqual(resultOK.Content.Status, account.Status);
        }

        [TestMethod]
        public void AccountDeleteTest()
        {
            AccountController controller = new AccountController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Account account = Context.Template;

            OkNegotiatedContentResult<Account> resultOK = controller.Create(account) as OkNegotiatedContentResult<Account>;
            Context.TestData.Add(resultOK.Content);

            account = resultOK.Content;
            controller.Delete(account.Id);

            IHttpActionResult resultNotFound = controller.Read(account.Id);
            Assert.IsTrue(resultNotFound is NotFoundResult);

            // Test that account cannot be deleted, if any related resources are found.

            account = Context.Template;

            resultOK = controller.Create(account) as OkNegotiatedContentResult<Account>;
            Context.TestData.Add(resultOK.Content);

            account = resultOK.Content;

            Transaction transaction = new Transaction()
            {
                Account = new Reference(Reference.AccountUri, Context.TestData.Find(m => m is Account).Id),
                Amount = -10,
                Description = "Advance payment.",
                Quarter = "Q4-2016",
                Status = TransactionStatus.Committed,
                Type = TransactionType.Advance
            };

            transaction = Context.TransactionRepository.Add(transaction);
            Context.TestData.Add(transaction);

            IHttpActionResult resultBadRequest = controller.Delete(account.Id);
            Assert.IsTrue(resultBadRequest is BadRequestErrorMessageResult);
        }

        [TestMethod]
        public void AccountSearchTest()
        {
            AccountController controller = new AccountController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Account account = Context.Template;

            OkNegotiatedContentResult<Account> resultOK = controller.Create(account) as OkNegotiatedContentResult<Account>;
            Context.TestData.Add(resultOK.Content);

            account = resultOK.Content;

            OkNegotiatedContentResult<List<Account>> resultList = controller.List() as OkNegotiatedContentResult<List<Account>>;
            Assert.IsNotNull(resultList.Content);
            Assert.IsTrue(resultList.Content.Count == 1);

            OkNegotiatedContentResult<List<Account>> resultSearch = controller.Search("name|xxr") as OkNegotiatedContentResult<List<Account>>;
            Assert.IsNotNull(resultSearch.Content);
            Assert.IsTrue(resultSearch.Content.Count == 1);

            resultSearch = controller.Search("name|vvr") as OkNegotiatedContentResult<List<Account>>;
            Assert.IsNotNull(resultSearch.Content);
            Assert.IsTrue(resultSearch.Content.Count == 0);
        }

        [TestMethod]
        public void TransactionCreateTest()
        {
            AccountController controller = new AccountController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Account account = Context.Template;

            OkNegotiatedContentResult<Account> resultOK = controller.Create(account) as OkNegotiatedContentResult<Account>;
            Context.TestData.Add(resultOK.Content);

            account = resultOK.Content;

            Transaction transaction = new Transaction()
            {
                Account = new Reference(Reference.AccountUri, account.Id),
                Amount = 12.57,
                Description = "Sales added.",
                Quarter = "Q4-2016",
                Status = TransactionStatus.Committed,
                Type = TransactionType.Sales
            };

            OkNegotiatedContentResult<Transaction> resultTransactionOK = controller.Create(transaction) as OkNegotiatedContentResult<Transaction>;
            Context.TestData.Add(resultTransactionOK.Content);

            Assert.IsNotNull(resultTransactionOK.Content);
            Assert.IsTrue(resultTransactionOK.Content.Id > 0);
            Assert.IsNotNull(resultTransactionOK.Content.Account);
            Assert.IsNull(resultTransactionOK.Content.Statement);
            Assert.AreEqual(resultTransactionOK.Content.Account.GetId(), transaction.Account.GetId());
            Assert.AreEqual(resultTransactionOK.Content.Status, transaction.Status);
            Assert.AreEqual(resultTransactionOK.Content.Amount, transaction.Amount);
            Assert.AreEqual(resultTransactionOK.Content.Description, transaction.Description);
            Assert.AreEqual(resultTransactionOK.Content.Quarter, transaction.Quarter);
            Assert.AreEqual(resultTransactionOK.Content.Type, transaction.Type);
        }

        [TestMethod]
        public void AccountBalanceTest()
        {
            AccountController controller = new AccountController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Account account = Context.Template;

            OkNegotiatedContentResult<Account> resulAccounttOK = controller.Create(account) as OkNegotiatedContentResult<Account>;
            Context.TestData.Add(resulAccounttOK.Content);

            account = resulAccounttOK.Content;

            Transaction transactionA = new Transaction()
            {
                Account = new Reference(Reference.AccountUri, account.Id),
                Amount = 12.57,
                Description = "Revenue from streaming.",
                Quarter = "Q4-2016",
                Status = TransactionStatus.Committed,
                Type = TransactionType.Sales
            };

            OkNegotiatedContentResult<Transaction> resultTransactionOK = controller.Create(transactionA) as OkNegotiatedContentResult<Transaction>;
            Context.TestData.Add(resultTransactionOK.Content);

            Transaction transactionB = new Transaction()
            {
                Account = new Reference(Reference.AccountUri, account.Id),
                Amount = 17.89,
                Description = "Revenue from sales.",
                Quarter = "Q4-2016",
                Status = TransactionStatus.Committed,
                Type = TransactionType.Sales
            };

            resultTransactionOK = controller.Create(transactionB) as OkNegotiatedContentResult<Transaction>;
            Context.TestData.Add(resultTransactionOK.Content);

            Transaction transactionC = new Transaction()
            {
                Account = new Reference(Reference.AccountUri, account.Id),
                Amount = -10,
                Description = "Advance payment.",
                Quarter = "Q4-2016",
                Status = TransactionStatus.Failed,
                Type = TransactionType.Advance
            };

            resultTransactionOK = controller.Create(transactionC) as OkNegotiatedContentResult<Transaction>;
            Context.TestData.Add(resultTransactionOK.Content);

            resulAccounttOK = controller.Read(account.Id) as OkNegotiatedContentResult<Account>;

            Assert.IsNotNull(resulAccounttOK.Content);
            Assert.AreEqual(resulAccounttOK.Content.Name, account.Name);
            Assert.AreEqual(resulAccounttOK.Content.Status, account.Status);
            Assert.IsNotNull(resulAccounttOK.Content.Transactions);
            Assert.IsTrue(resulAccounttOK.Content.Transactions.Length == 3);
            Assert.IsTrue(resulAccounttOK.Content.Balance == (transactionA.Amount + transactionB.Amount));
        }
    }
}
