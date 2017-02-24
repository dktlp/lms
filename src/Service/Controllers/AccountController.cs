using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

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

        [HttpDelete()]
        [Route("account/{id}", Name = "Account.Delete")]
        public IHttpActionResult Delete(int id)
        {
            Log.Info(String.Format("HTTP DELETE api/account/{0}", id));

            try
            {
                // If account has any related resources, then artist cannot be deleted.
                // Related resource; Transaction
                IRepository<Transaction> transactionRepository = RepositoryFactory<Transaction>.Create();
                List<Transaction> transactions = transactionRepository.Find(new Transaction() { Account = new Reference(Reference.AccountUri, id) });
                if (transactions != null && transactions.Count > 0)
                    return Conflict();

                // Delete account resource.
                IRepository<Account> repository = RepositoryFactory<Account>.Create();
                repository.Remove(id);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return InternalServerError(e);
            }

            return Ok();
        }

        [HttpPut()]
        [Route("account", Name = "Account.Update")]
        public IHttpActionResult Update([FromBody] Account account)
        {
            Log.Info("HTTP PUT api/account");

            try
            {
                IDataValidator<Account> validator = new AccountValidator();
                DataValidationResult validationResult = validator.Validate(account);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                IRepository<Account> repository = RepositoryFactory<Account>.Create();
                Account result = repository.Update(account);
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

        [HttpGet()]
        [Route("account/search", Name = "Account.Search")]
        public IHttpActionResult Search([FromUri] string q)
        {
            Log.Info(String.Format("HTTP GET api/account/search/?q={0}", q));

            try
            {
                // Search q:
                // q=name|ctx001;status|1;paramName|value

                QueryByExampleBuilder<Account> queryBuilder = new QueryByExampleBuilder<Account>();
                IRepository<Account> repository = RepositoryFactory<Account>.Create();
                List<Account> accounts = repository.Find(queryBuilder.GetQueryExample(q));
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

        [HttpPost()]
        [Route("account/transaction", Name = "Transaction.Create")]
        public IHttpActionResult Create([FromBody] Transaction transaction)
        {
            Log.Info("HTTP POST api/account/transaction");

            try
            {
                IDataValidator<Transaction> validator = new TransactionValidator();
                DataValidationResult validationResult = validator.Validate(transaction);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                IRepository<Transaction> repository = RepositoryFactory<Transaction>.Create();
                Transaction result = repository.Add(transaction);
                if (result != null && result.Id > 0)
                    return Ok(transaction);
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