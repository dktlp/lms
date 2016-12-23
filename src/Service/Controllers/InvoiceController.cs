using System;
using System.Collections.Generic;
using System.Web.Http;

using log4net;

using LMS.Model.Resource;
using LMS.Data;
using LMS.Configuration;
using LMS.Service.Logging;
using LMS.Model.Resource.Logging;
using LMS.Model.Composite;
using LMS.Model.Resource.Enums;

namespace LMS.Service.Controllers
{
    [RoutePrefix("api")]
    public class InvoiceController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger("Trace");
        
        [HttpPost()]
        [Route("invoice", Name = "Invoice.Create")]
        public IHttpActionResult Create([FromBody] Invoice invoice)
        {
            Log.Info("HTTP POST api/invoice");

            try
            {
                // Generate a 16 digit invoice number.
                if (invoice.InvoiceNumber == null)
                {
                    DateTime dt = DateTime.Now;
                    invoice.InvoiceNumber = String.Format("{0}{1}", dt.ToString("ddd").Substring(0, 1).ToUpper(), dt.ToString("yyyyMMdd-HHmmss"));
                }

                IDataValidator<Invoice> validator = new InvoiceValidator();
                DataValidationResult validationResult = validator.Validate(invoice);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                IRepository<Invoice> repository = RepositoryFactory<Invoice>.Create();
                Invoice result = repository.Add(invoice);
                if (result != null && result.Id > 0)
                    return Ok(invoice);
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return InternalServerError(e);
            }
        }

        [HttpGet()]
        [Route("invoice/{id}", Name = "Invoice.Read")]
        public IHttpActionResult Read(int id)
        {
            Log.Info(String.Format("HTTP GET api/invoice/{0}", id));

            try
            {
                IRepository<Invoice> repository = RepositoryFactory<Invoice>.Create();
                Invoice invoice = repository.Get(id);
                if (invoice != null)
                    return Ok(invoice);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return InternalServerError(e);
            }
        }

        [HttpPut()]
        [Route("invoice", Name = "Invoice.Update")]
        public IHttpActionResult Update([FromBody] Invoice invoice)
        {
            Log.Info("HTTP PUT api/invoice");

            try
            {
                IDataValidator<Invoice> validator = new InvoiceValidator();
                DataValidationResult validationResult = validator.Validate(invoice);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                IRepository<Invoice> repository = RepositoryFactory<Invoice>.Create();
                Invoice result = repository.Update(invoice);
                if (result != null && result.Id > 0)
                    return Ok(invoice);
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return InternalServerError(e);
            }
        }
    }
}