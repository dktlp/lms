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

namespace LMS.Service.Controllers
{
    [RoutePrefix("api")]
    public class AccountController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger("Trace");

        [HttpGet()]
        [Route("account/list", Name = "Account.List")]
        public IHttpActionResult List()
        {
            Log.Info("HTTP GET api/account/list");
            
            try
            {
                IRepository<Account> repository = RepositoryFactory<Account>.Create();
                List<Account> accounts = repository.Find(null);
                if (accounts != null)
                    return Ok(accounts);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return InternalServerError(e);
            }
        }

        [HttpGet()]
        [Route("account/{id}", Name = "Account.Read")]
        public IHttpActionResult Read(int id)
        {
            Log.Info(String.Format("HTTP GET api/account/{0}", id));

            try
            {
                IRepository<Account> repository = RepositoryFactory<Account>.Create();
                Account account = repository.Get(id);
                if (account != null)
                    return Ok(account);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return InternalServerError(e);
            }
        }

        [HttpPost()]
        [Route("account", Name = "Account.Create")]
        public IHttpActionResult Create([FromBody] Account account)
        {
            Log.Info("HTTP POST api/account");

            try
            {
                IDataValidator<Account> validator = new AccountValidator();
                DataValidationResult validationResult = validator.Validate(account);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                IRepository<Account> repository = RepositoryFactory<Account>.Create();
                Account result = repository.Add(account);
                if (result != null && result.Id > 0)
                    return Ok(account);
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return InternalServerError(e);
            }
        }

        //[HttpDelete()]
        //[Route("label/{id}", Name = "Label.Delete")]
        //public IHttpActionResult Delete(int id)
        //{
        //    Log.Info(String.Format("HTTP DELETE api/label/{0}", id));

        //    try
        //    {
        //        IRepository<Label> repository = RepositoryFactory<Label>.Create();
        //        repository.Remove(id);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e);
        //        return InternalServerError(e);
        //    }

        //    return Ok();
        //}

        //[HttpPut()]
        //[Route("label", Name = "Label.Update")]
        //public IHttpActionResult Update([FromBody] Label label)
        //{
        //    Log.Info("HTTP PUT api/label");

        //    try
        //    {
        //        IDataValidator<Label> validator = new LabelValidator();
        //        DataValidationResult validationResult = validator.Validate(label);
        //        if (!validationResult.IsValid)
        //            return BadRequest(validationResult.Message);

        //        IRepository<Label> repository = RepositoryFactory<Label>.Create();
        //        Label result = repository.Update(label);
        //        if (result != null && result.Id > 0)
        //            return Ok(label);
        //        else
        //            return BadRequest();
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e);
        //        return InternalServerError(e);
        //    }
        //}

        //[HttpGet()]
        //[Route("label/search", Name = "Label.Search")]
        //public IHttpActionResult Search([FromUri] string q)
        //{
        //    Log.Info(String.Format("HTTP GET api/label/search/?q={0}", q));

        //    try
        //    {
        //        // Search q:
        //        // q=name|kill;paramName|value

        //        QueryByExampleBuilder<Label> queryBuilder = new QueryByExampleBuilder<Label>();
        //        IRepository<Label> repository = RepositoryFactory<Label>.Create();
        //        List<Label> labels = repository.Find(queryBuilder.GetQueryExample(q));
        //        if (labels != null)
        //            return Ok(labels);
        //        else
        //            return NotFound();
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e);
        //        return InternalServerError(e);
        //    }
        //}
    }
}