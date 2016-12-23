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
    public class StatementControllerTest
    {
        private UnitTestContext<Statement> Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new UnitTestContext<Statement>();

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

            Context.TestData.Add(Context.AccountRepository.Add(new Account()
            {
                Name = "XXR001",
                Status = AccountStatus.Open,
                Transactions = null,
                Artist = new Reference(Reference.ArtistUri, Context.TestData.Find(m => m is Artist).Id),
                Label = new Reference(Reference.LabelUri, Context.TestData.Find(m => m is Label).Id)
            }));

            Context.TestData.Add(Context.TransactionRepository.Add(new Transaction()
            {
                Account = new Reference(Reference.AccountUri, Context.TestData.Find(m => m is Account).Id),
                Amount = -10,
                Description = "Advance payment.",
                Quarter = "Q4-2016",
                Status = TransactionStatus.Committed,
                Type = TransactionType.Advance
            }));

            Context.TestData.Add(Context.TransactionRepository.Add(new Transaction()
            {
                Account = new Reference(Reference.AccountUri, Context.TestData.Find(m => m is Account).Id),
                Amount = 12.57,
                Description = "Revenue from sales.",
                Quarter = "Q4-2016",
                Status = TransactionStatus.Committed,
                Type = TransactionType.Sales
            }));

            Context.TestData.Add(Context.TransactionRepository.Add(new Transaction()
            {
                Account = new Reference(Reference.AccountUri, Context.TestData.Find(m => m is Account).Id),
                Amount = 17.89,
                Description = "Revenue from streaming.",
                Quarter = "Q4-2016",
                Status = TransactionStatus.Committed,
                Type = TransactionType.Sales
            }));

            Context.TestData.Add(Context.TransactionRepository.Add(new Transaction()
            {
                Account = new Reference(Reference.AccountUri, Context.TestData.Find(m => m is Account).Id),
                Amount = 17.89,
                Description = "Revenue from streaming.",
                Quarter = "Q4-2016",
                Status = TransactionStatus.Failed,
                Type = TransactionType.Sales
            }));

            Context.Template = new Statement()
            {
                Artist = new Reference(Reference.ArtistUri, Context.TestData.Find(m => m is Artist).Id),
                Label = new Reference(Reference.LabelUri, Context.TestData.Find(m => m is Label).Id),
                Quarter = "Q4-2016",
                Reference = "TOMTEK-Q42016",
                Status = StatementStatus.Pending,
                Invoice = null
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
                if (data is Statement)
                    Context.StatementRepository.Remove(data.Id);
            }
        }

        [TestMethod]
        public void StatementCreateTest()
        {
            StatementController controller = new StatementController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Statement statement = Context.Template;
            statement.Quarter = null;

            IHttpActionResult resultERR = controller.Create(statement);
            Assert.IsTrue(resultERR is BadRequestErrorMessageResult);

            statement.Quarter = "Q4-2016";

            OkNegotiatedContentResult<Statement> resultOK = controller.Create(statement) as OkNegotiatedContentResult<Statement>;
            Context.TestData.Add(resultOK.Content);

            Assert.IsNotNull(resultOK.Content);
            Assert.IsTrue(resultOK.Content.Id > 0);
            Assert.AreEqual(resultOK.Content.Artist.GetId(), statement.Artist.GetId());
            Assert.AreEqual(resultOK.Content.Label.GetId(), statement.Label.GetId());
            Assert.AreEqual(resultOK.Content.Quarter, statement.Quarter);
            Assert.AreEqual(resultOK.Content.Reference, statement.Reference);
            Assert.AreEqual(resultOK.Content.Status, statement.Status);
            Assert.IsTrue(resultOK.Content.EffectiveTime <= DateTime.Now);

            double amount = 0;
            foreach(Transaction transaction in Context.TestData.FindAll(m => m is Transaction))
            {
                if (transaction.Status == TransactionStatus.Committed)
                    amount += transaction.Amount;
            }

            Assert.AreEqual(resultOK.Content.Amount, amount);
            Assert.IsNotNull(resultOK.Content.Data);
            Assert.IsTrue(resultOK.Content.Data.Length == 1);
            Assert.IsNotNull(resultOK.Content.Data[0].Transactions);
            Assert.IsTrue(resultOK.Content.Data[0].Transactions.Length == 3);

            Assert.AreEqual(resultOK.Content.Data[0].Transactions[0].Id, ((Transaction)Context.TestData.FindAll(m => m is Transaction && ((Transaction)m).Status == TransactionStatus.Committed)[0]).Id);
            Assert.IsNotNull(resultOK.Content.Data[0].Transactions[0].Statement);
            Assert.AreEqual(resultOK.Content.Data[0].Transactions[0].Statement.GetId(), resultOK.Content.Id);

            Assert.AreEqual(resultOK.Content.Data[0].Transactions[1].Id, ((Transaction)Context.TestData.FindAll(m => m is Transaction && ((Transaction)m).Status == TransactionStatus.Committed)[1]).Id);
            Assert.IsNotNull(resultOK.Content.Data[0].Transactions[1].Statement);
            Assert.AreEqual(resultOK.Content.Data[0].Transactions[1].Statement.GetId(), resultOK.Content.Id);

            Assert.AreEqual(resultOK.Content.Data[0].Transactions[2].Id, ((Transaction)Context.TestData.FindAll(m => m is Transaction && ((Transaction)m).Status == TransactionStatus.Committed)[2]).Id);
            Assert.IsNotNull(resultOK.Content.Data[0].Transactions[2].Statement);
            Assert.AreEqual(resultOK.Content.Data[0].Transactions[2].Statement.GetId(), resultOK.Content.Id);
        }

        [TestMethod]
        public void StatementReadTest()
        {
            StatementController controller = new StatementController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Statement statement = Context.Template;

            OkNegotiatedContentResult<Statement> resultOK = controller.Create(statement) as OkNegotiatedContentResult<Statement>;
            Context.TestData.Add(resultOK.Content);

            resultOK = controller.Read(resultOK.Content.Id) as OkNegotiatedContentResult<Statement>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.Id, statement.Id);
            Assert.AreEqual(resultOK.Content.Artist.GetId(), statement.Artist.GetId());
            Assert.AreEqual(resultOK.Content.Label.GetId(), statement.Label.GetId());
            Assert.AreEqual(resultOK.Content.Amount, statement.Amount);
            Assert.AreEqual(resultOK.Content.Quarter, statement.Quarter);
            Assert.AreEqual(resultOK.Content.Reference, statement.Reference);

            Assert.IsNotNull(resultOK.Content.Data);
            Assert.IsTrue(resultOK.Content.Data.Length == 1);
            Assert.AreEqual(resultOK.Content.Data[0].Id, ((Account)Context.TestData.Find(m => m is Account)).Id);
            Assert.AreEqual(resultOK.Content.Data[0].Name, ((Account)Context.TestData.Find(m => m is Account)).Name);

            Assert.IsNotNull(resultOK.Content.Data[0].Transactions);
            Assert.IsTrue(resultOK.Content.Data[0].Transactions.Length == 3);
            Assert.AreEqual(resultOK.Content.Data[0].Transactions[0].Id, ((Transaction)Context.TestData.FindAll(m => m is Transaction && ((Transaction)m).Status == TransactionStatus.Committed)[0]).Id);
            Assert.IsNotNull(resultOK.Content.Data[0].Transactions[0].Statement);
            Assert.AreEqual(resultOK.Content.Data[0].Transactions[0].Statement.GetId(), resultOK.Content.Id);
        }

        [TestMethod]
        public void StatementUpdateTest()
        {
            StatementController controller = new StatementController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Statement statement = Context.Template;

            OkNegotiatedContentResult<Statement> resultOK = controller.Create(statement) as OkNegotiatedContentResult<Statement>;
            Context.TestData.Add(resultOK.Content);

            statement = resultOK.Content;
            statement.Status = StatementStatus.Sent;

            resultOK = controller.Update(statement) as OkNegotiatedContentResult<Statement>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.Id, statement.Id);
            Assert.AreEqual(resultOK.Content.Status, statement.Status);

            statement = resultOK.Content;
            statement.Status = StatementStatus.Cancelled;

            resultOK = controller.Update(statement) as OkNegotiatedContentResult<Statement>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.Id, statement.Id);
            Assert.AreEqual(resultOK.Content.Status, statement.Status);

            // Test if status is set cancelled, then link between transaction and statement is broken.
            foreach (Transaction t in Context.TestData.FindAll(m => m is Transaction))
            {
                Transaction transaction = Context.TransactionRepository.Get(t.Id);
                Assert.IsNull(transaction.Statement);
            }

            statement = resultOK.Content;
            statement.Status = StatementStatus.Pending;

            // Test if status is already cancelled it cannot be changed to any other state
            IHttpActionResult resultERR = controller.Update(statement);
            Assert.IsTrue(resultERR is BadRequestErrorMessageResult);
        }

        [TestMethod]
        public void StatementDeleteTest()
        {
            StatementController controller = new StatementController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Statement statement = Context.Template;
            statement.Status = StatementStatus.Cancelled;

            OkNegotiatedContentResult<Statement> resultOK = controller.Create(statement) as OkNegotiatedContentResult<Statement>;
            Context.TestData.Add(resultOK.Content);

            statement = resultOK.Content;
            controller.Delete(statement.Id);

            IHttpActionResult resultNotFound = controller.Read(statement.Id);
            Assert.IsTrue(resultNotFound is NotFoundResult);

            statement = Context.Template;
            statement.Status = StatementStatus.Pending;

            resultOK = controller.Create(statement) as OkNegotiatedContentResult<Statement>;
            Context.TestData.Add(resultOK.Content);

            // Test if status is not cancelled statement cannot be deleted.
            IHttpActionResult resultBadRequest = controller.Delete(statement.Id);
            Assert.IsTrue(resultBadRequest is BadRequestErrorMessageResult);
        }

        [TestMethod]
        public void StatementSearchTest()
        {
            StatementController controller = new StatementController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Statement statement = Context.Template;

            OkNegotiatedContentResult<Statement> resultOK = controller.Create(statement) as OkNegotiatedContentResult<Statement>;
            Context.TestData.Add(resultOK.Content);

            statement = resultOK.Content;

            OkNegotiatedContentResult<List<Statement>> resultList = controller.List() as OkNegotiatedContentResult<List<Statement>>;
            Assert.IsNotNull(resultList.Content);
            Assert.IsTrue(resultList.Content.Count == 1);

            OkNegotiatedContentResult<List<Statement>> resultSearch = controller.Search("reference|tom") as OkNegotiatedContentResult<List<Statement>>;
            Assert.IsNotNull(resultSearch.Content);
            Assert.IsTrue(resultSearch.Content.Count == 1);

            resultSearch = controller.Search("reference|x") as OkNegotiatedContentResult<List<Statement>>;
            Assert.IsNotNull(resultSearch.Content);
            Assert.IsTrue(resultSearch.Content.Count == 0);
        }
    }
}
