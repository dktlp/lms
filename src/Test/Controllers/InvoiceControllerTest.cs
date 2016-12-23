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
    public class InvoiceControllerTest
    {
        private UnitTestContext<Invoice> Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new UnitTestContext<Invoice>();
            Context.Template = new Invoice()
            {
                InvoiceNumber = null,
                Status = InvoiceStatus.Received,
                PaypalAddress = "mail@lplabelgroup.com"
            };
        }

        [TestCleanup]
        public void TestCleanup()
        {
            foreach (DomainResource data in Context.TestData)
            {
                if (data is Invoice)
                    Context.InvoiceRepository.Remove(data.Id);
            }
        }

        [TestMethod]
        public void InvoiceCreateTest()
        {
            InvoiceController controller = new InvoiceController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Invoice invoice = Context.Template;
            invoice.PaypalAddress = null;

            IHttpActionResult resultERR = controller.Create(invoice);
            Assert.IsTrue(resultERR is BadRequestErrorMessageResult);

            invoice.PaypalAddress = "mail@lplabelgroup.com";

            OkNegotiatedContentResult<Invoice> resultOK = controller.Create(invoice) as OkNegotiatedContentResult<Invoice>;

            Context.TestData.Add(resultOK.Content);

            Assert.IsNotNull(resultOK.Content);
            Assert.IsTrue(resultOK.Content.Id > 0);
            Assert.IsNotNull(resultOK.Content.InvoiceNumber);
            Assert.IsTrue(resultOK.Content.InvoiceNumber.Length == 16);
            Assert.IsTrue(resultOK.Content.EffectiveTime <= DateTime.Now);
            Assert.AreEqual(resultOK.Content.PaypalAddress, invoice.PaypalAddress);
            Assert.AreEqual(resultOK.Content.Status, invoice.Status);
        }

        [TestMethod]
        public void InvoiceReadTest()
        {
            InvoiceController controller = new InvoiceController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Invoice invoice = Context.Template;

            OkNegotiatedContentResult<Invoice> resultOK = controller.Create(invoice) as OkNegotiatedContentResult<Invoice>;

            invoice = resultOK.Content;
            Context.TestData.Add(resultOK.Content);

            resultOK = controller.Read(invoice.Id) as OkNegotiatedContentResult<Invoice>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.Id, invoice.Id);
            Assert.AreEqual(resultOK.Content.InvoiceNumber, invoice.InvoiceNumber);
            Assert.IsTrue(resultOK.Content.EffectiveTime <= DateTime.Now);
            Assert.AreEqual(resultOK.Content.PaypalAddress, invoice.PaypalAddress);
            Assert.AreEqual(resultOK.Content.Status, invoice.Status);
        }

        [TestMethod]
        public void InvoiceUpdateTest()
        {
            InvoiceController controller = new InvoiceController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            Invoice invoice = Context.Template;

            OkNegotiatedContentResult<Invoice> resultOK = controller.Create(invoice) as OkNegotiatedContentResult<Invoice>;
            Context.TestData.Add(resultOK.Content);

            invoice = resultOK.Content;
            invoice.PaypalAddress = "mail@lplabelgroup.com_changed";
            invoice.Status = InvoiceStatus.Paid;

            resultOK = controller.Update(invoice) as OkNegotiatedContentResult<Invoice>;

            Assert.IsNotNull(resultOK.Content);
            Assert.AreEqual(resultOK.Content.Id, invoice.Id);
            Assert.AreEqual(resultOK.Content.PaypalAddress, invoice.PaypalAddress);
            Assert.AreEqual(resultOK.Content.Status, invoice.Status);
        }
    }
}