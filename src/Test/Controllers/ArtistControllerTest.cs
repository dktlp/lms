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
    public class ArtistControllerTest
    {
        private UnitTestContext<Artist> Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new UnitTestContext<Artist>();
            Context.Template = new Artist()
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
            };

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
        public void ArtistCreateTest()
        {
            ArtistController controller = new ArtistController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Artist artist = Context.Template;
            artist.StageName = null;

            IHttpActionResult resultERR = controller.Create(artist);
            Assert.IsTrue(resultERR is BadRequestErrorMessageResult);

            artist.StageName = "Tomtek";

            OkNegotiatedContentResult<Artist> resultOK = controller.Create(artist) as OkNegotiatedContentResult<Artist>;

            Context.TestData.Add(resultOK.Content);

            Assert.IsNotNull(resultOK.Content);
            Assert.IsTrue(resultOK.Content.Id > 0);
            Assert.AreEqual(resultOK.Content.StageName, artist.StageName);
            Assert.IsTrue(resultOK.Content.Name.Given.Length == 1);
            Assert.AreEqual(resultOK.Content.Name.Given[0], artist.Name.Given[0]);
            Assert.IsTrue(resultOK.Content.Name.Family.Length == 2);
            Assert.AreEqual(resultOK.Content.Name.Family[0], artist.Name.Family[0]);
            Assert.AreEqual(resultOK.Content.Name.Family[1], artist.Name.Family[1]);
            Assert.AreEqual(resultOK.Content.Email, artist.Email);
            Assert.AreEqual(resultOK.Content.Telecom, artist.Telecom);
            Assert.IsTrue(resultOK.Content.Address.Line.Length == 2);
            Assert.AreEqual(resultOK.Content.Address.Line[0], artist.Address.Line[0]);
            Assert.AreEqual(resultOK.Content.Address.Line[1], artist.Address.Line[1]);
            Assert.AreEqual(resultOK.Content.Address.City, artist.Address.City);
            Assert.AreEqual(resultOK.Content.Address.PostalCode, artist.Address.PostalCode);
            Assert.AreEqual(resultOK.Content.Address.Country, artist.Address.Country);
            Assert.AreEqual(resultOK.Content.Address.District, artist.Address.District);
            Assert.AreEqual(resultOK.Content.Address.State, artist.Address.State);
        }

        [TestMethod]
        public void ArtistReadTest()
        {
            ArtistController controller = new ArtistController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Artist artist = Context.Template;

            OkNegotiatedContentResult<Artist> resultOK = controller.Create(artist) as OkNegotiatedContentResult<Artist>;
            Context.TestData.Add(resultOK.Content);

            resultOK = controller.Read(resultOK.Content.Id) as OkNegotiatedContentResult<Artist>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.StageName, artist.StageName);
            Assert.IsTrue(resultOK.Content.Name.Given.Length == 1);
            Assert.AreEqual(resultOK.Content.Name.Given[0], artist.Name.Given[0]);
            Assert.IsTrue(resultOK.Content.Name.Family.Length == 2);
            Assert.AreEqual(resultOK.Content.Name.Family[0], artist.Name.Family[0]);
            Assert.AreEqual(resultOK.Content.Name.Family[1], artist.Name.Family[1]);
            Assert.AreEqual(resultOK.Content.Email, artist.Email);
            Assert.AreEqual(resultOK.Content.Telecom, artist.Telecom);
            Assert.IsTrue(resultOK.Content.Address.Line.Length == 2);
            Assert.AreEqual(resultOK.Content.Address.Line[0], artist.Address.Line[0]);
            Assert.AreEqual(resultOK.Content.Address.Line[1], artist.Address.Line[1]);
            Assert.AreEqual(resultOK.Content.Address.City, artist.Address.City);
            Assert.AreEqual(resultOK.Content.Address.PostalCode, artist.Address.PostalCode);
            Assert.AreEqual(resultOK.Content.Address.Country, artist.Address.Country);
            Assert.AreEqual(resultOK.Content.Address.District, artist.Address.District);
            Assert.AreEqual(resultOK.Content.Address.State, artist.Address.State);
        }

        [TestMethod]
        public void ArtistUpdateTest()
        {
            ArtistController controller = new ArtistController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Artist artist = Context.Template;

            OkNegotiatedContentResult<Artist> resultOK = controller.Create(artist) as OkNegotiatedContentResult<Artist>;
            Context.TestData.Add(resultOK.Content);

            artist = resultOK.Content;
            artist.StageName = "lplabelgroup_changed";
            artist.Address.Line[1] = "Vorup_changed";

            resultOK = controller.Update(artist) as OkNegotiatedContentResult<Artist>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.Id, artist.Id);
            Assert.AreEqual(resultOK.Content.StageName, artist.StageName);
            Assert.IsTrue(resultOK.Content.Address.Line.Length == 2);
            Assert.AreEqual(resultOK.Content.Address.Line[1], artist.Address.Line[1]);
        }

        [TestMethod]
        public void ArtistDeleteTest()
        {
            ArtistController controller = new ArtistController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Artist artist = Context.Template;

            OkNegotiatedContentResult<Artist> resultOK = controller.Create(artist) as OkNegotiatedContentResult<Artist>;
            Context.TestData.Add(resultOK.Content);

            artist = resultOK.Content;
            controller.Delete(artist.Id);

            IHttpActionResult resultNotFound = controller.Read(artist.Id);
            Assert.IsTrue(resultNotFound is NotFoundResult);

            // Test that artist cannot be delated if any related resources are found.

            artist = Context.Template;

            resultOK = controller.Create(artist) as OkNegotiatedContentResult<Artist>;
            Context.TestData.Add(resultOK.Content);

            artist = resultOK.Content;

            Account account = new Account()
            {
                Name = "XXR001",
                Status = AccountStatus.Open,
                Transactions = null,
                Artist = new Reference(Reference.ArtistUri, artist.Id),
                Label = new Reference(Reference.LabelUri, Context.TestData.Find(m => m is Label).Id)
            };

            account = Context.AccountRepository.Add(account);
            Context.TestData.Add(account);

            IHttpActionResult resultBadRequest = controller.Delete(artist.Id);
            Assert.IsTrue(resultBadRequest is BadRequestErrorMessageResult);
        }

        [TestMethod]
        public void ArtistSearchTest()
        {
            ArtistController controller = new ArtistController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Artist artist = Context.Template;

            OkNegotiatedContentResult<Artist> resultOK = controller.Create(artist) as OkNegotiatedContentResult<Artist>;
            Context.TestData.Add(resultOK.Content);

            artist = resultOK.Content;

            OkNegotiatedContentResult<List<Artist>> resultList = controller.List() as OkNegotiatedContentResult<List<Artist>>;
            Assert.IsNotNull(resultList.Content);
            Assert.IsTrue(resultList.Content.Count == 1);

            OkNegotiatedContentResult<List<Artist>> resultSearch = controller.Search("stageName|tom") as OkNegotiatedContentResult<List<Artist>>;
            Assert.IsNotNull(resultSearch.Content);
            Assert.IsTrue(resultSearch.Content.Count == 1);

            resultSearch = controller.Search("stageName|x") as OkNegotiatedContentResult<List<Artist>>;
            Assert.IsNotNull(resultSearch.Content);
            Assert.IsTrue(resultSearch.Content.Count == 0);
        }
    }
}