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

namespace LMS.Test.Controllers
{
    [TestClass]
    public class LabelControllerTest
    {
        private UnitTestContext<Label> Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new UnitTestContext<Label>();
            Context.Template = new Label()
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
            };
        }

        [TestCleanup]
        public void TestCleanup()
        {
            foreach (Label data in Context.TestData)
            {
                Context.LabelRepository.Remove(data.Id);
            }
        }

        [TestMethod]
        public void LabelCreateTest()
        {
            LabelController controller = new LabelController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Label label = Context.Template;
            label.Name = null;

            IHttpActionResult resultERR = controller.Create(label);
            Assert.IsTrue(resultERR is BadRequestErrorMessageResult);

            label.Name = "lplabelgroup";

            OkNegotiatedContentResult<Label> resultOK = controller.Create(label) as OkNegotiatedContentResult<Label>;

            Context.TestData.Add(resultOK.Content);

            Assert.IsNotNull(resultOK.Content);
            Assert.IsTrue(resultOK.Content.Id > 0);
            Assert.AreEqual(resultOK.Content.Name, label.Name);
            Assert.AreEqual(resultOK.Content.Email, label.Email);
            Assert.AreEqual(resultOK.Content.Telecom, label.Telecom);
            Assert.IsTrue(resultOK.Content.Address.Line.Length == 2);
            Assert.AreEqual(resultOK.Content.Address.Line[0], label.Address.Line[0]);
            Assert.AreEqual(resultOK.Content.Address.Line[1], label.Address.Line[1]);
            Assert.AreEqual(resultOK.Content.Address.City, label.Address.City);
            Assert.AreEqual(resultOK.Content.Address.PostalCode, label.Address.PostalCode);
            Assert.AreEqual(resultOK.Content.Address.Country, label.Address.Country);
            Assert.AreEqual(resultOK.Content.Address.District, label.Address.District);
            Assert.AreEqual(resultOK.Content.Address.State, label.Address.State);
        }

        [TestMethod]
        public void LabelReadTest()
        {
            LabelController controller = new LabelController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Label label = Context.Template;

            OkNegotiatedContentResult<Label> resultOK = controller.Create(label) as OkNegotiatedContentResult<Label>;
            Context.TestData.Add(resultOK.Content);
            
            resultOK = controller.Read(resultOK.Content.Id) as OkNegotiatedContentResult<Label>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.Name, label.Name);
            Assert.AreEqual(resultOK.Content.Email, label.Email);
            Assert.AreEqual(resultOK.Content.Telecom, label.Telecom);
            Assert.IsTrue(resultOK.Content.Address.Line.Length == 2);
            Assert.AreEqual(resultOK.Content.Address.Line[0], label.Address.Line[0]);
            Assert.AreEqual(resultOK.Content.Address.Line[1], label.Address.Line[1]);
            Assert.AreEqual(resultOK.Content.Address.City, label.Address.City);
            Assert.AreEqual(resultOK.Content.Address.PostalCode, label.Address.PostalCode);
            Assert.AreEqual(resultOK.Content.Address.Country, label.Address.Country);
            Assert.AreEqual(resultOK.Content.Address.District, label.Address.District);
            Assert.AreEqual(resultOK.Content.Address.State, label.Address.State);
        }

        [TestMethod]
        public void LabelUpdateTest()
        {
            LabelController controller = new LabelController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Label label = Context.Template;

            OkNegotiatedContentResult<Label> resultOK = controller.Create(label) as OkNegotiatedContentResult<Label>;
            Context.TestData.Add(resultOK.Content);

            label = resultOK.Content;
            label.Name = "lplabelgroup_changed";
            label.Address.Line[1] = "Vorup_changed";

            resultOK = controller.Update(label) as OkNegotiatedContentResult<Label>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.Id, label.Id);
            Assert.AreEqual(resultOK.Content.Name, label.Name);
            Assert.IsTrue(resultOK.Content.Address.Line.Length == 2);
            Assert.AreEqual(resultOK.Content.Address.Line[1], label.Address.Line[1]);
        }

        [TestMethod]
        public void LabelDeleteTest()
        {
            LabelController controller = new LabelController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Label label = Context.Template;

            OkNegotiatedContentResult<Label> resultOK = controller.Create(label) as OkNegotiatedContentResult<Label>;
            Context.TestData.Add(resultOK.Content);

            label = resultOK.Content;
            controller.Delete(label.Id);

            IHttpActionResult resultNotFound = controller.Read(label.Id);
            Assert.IsTrue(resultNotFound is NotFoundResult);
        }

        [TestMethod]
        public void LabelSearchTest()
        {
            LabelController controller = new LabelController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Label label = Context.Template;

            OkNegotiatedContentResult<Label> resultOK = controller.Create(label) as OkNegotiatedContentResult<Label>;
            Context.TestData.Add(resultOK.Content);

            label = resultOK.Content;

            OkNegotiatedContentResult<List<Label>> resultList = controller.List() as OkNegotiatedContentResult<List<Label>>;
            Assert.IsNotNull(resultList.Content);
            Assert.IsTrue(resultList.Content.Count == 1);

            OkNegotiatedContentResult<List<Label>> resultSearch = controller.Search("name|lplabelg") as OkNegotiatedContentResult<List<Label>>;
            Assert.IsNotNull(resultSearch.Content);
            Assert.IsTrue(resultSearch.Content.Count == 1);

            resultSearch = controller.Search("name|x") as OkNegotiatedContentResult<List<Label>>;
            Assert.IsNotNull(resultSearch.Content);
            Assert.IsTrue(resultSearch.Content.Count == 0);
        }        
    }
}