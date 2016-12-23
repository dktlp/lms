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
    public class StatementController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger("Trace");

        [HttpGet()]
        [Route("statement/list", Name = "Statement.List")]
        public IHttpActionResult List()
        {
            Log.Info("HTTP GET api/statement/list");

            try
            {
                IRepository<Statement> repository = RepositoryFactory<Statement>.Create();
                List<Statement> statements = repository.Find(null);
                if (statements != null)
                    return Ok(statements);
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
        [Route("statement/{id}", Name = "Statement.Read")]
        public IHttpActionResult Read(int id)
        {
            Log.Info(String.Format("HTTP GET api/statement/{0}", id));

            try
            {
                IRepository<Statement> repository = RepositoryFactory<Statement>.Create();
                Statement statement = repository.Get(id);
                if (statement != null)
                    return Ok(statement);
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
        [Route("statement", Name = "Statement.Create")]
        public IHttpActionResult Create([FromBody] Statement statement)
        {
            Log.Info("HTTP POST api/statement");

            try
            {
                IDataValidator<Statement> validator = new StatementValidator();
                DataValidationResult validationResult = validator.Validate(statement);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                // Find all accounts relevant for statement (artist/label).
                IRepository<Transaction> transactionRepository = RepositoryFactory<Transaction>.Create();
                IRepository<Account> accountRepository = RepositoryFactory<Account>.Create();
                List<Account> accounts = accountRepository.Find(new Account()
                {
                    Artist = statement.Artist,
                    Label = statement.Label
                });

                // Find all relevant transactions for each account (statement_id=null and status=committed).
                foreach (Account account in accounts)
                {
                    account.Transactions = transactionRepository.Find(new Transaction()
                    {
                        Account = new Reference(Reference.AccountUri, account.Id)
                    }, m => m.Statement == null && m.Status == TransactionStatus.Committed).ToArray();

                    // Calculate amount of statement.
                    foreach (Transaction transaction in account.Transactions)
                    {
                        statement.Amount += transaction.Amount;
                    }
                }

                statement.Data = accounts.ToArray();

                IRepository<Statement> repository = RepositoryFactory<Statement>.Create();
                Statement result = repository.Add(statement);
                if (result != null && result.Id > 0)
                    return Ok(statement);
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
        [Route("statement/{id}", Name = "Statement.Delete")]
        public IHttpActionResult Delete(int id)
        {
            Log.Info(String.Format("HTTP DELETE api/statement/{0}", id));

            try
            {
                // If status of existing Statement resource is not 'cancelled' throw an exception.
                IRepository<Statement> repository = RepositoryFactory<Statement>.Create();
                Statement existingStatement = repository.Get(id);
                if (existingStatement != null && existingStatement.Status != StatementStatus.Cancelled)
                    return BadRequest("Statement can only be deleted if it has been cancelled.");

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
        [Route("statement", Name = "Statement.Update")]
        public IHttpActionResult Update([FromBody] Statement statement)
        {
            Log.Info("HTTP PUT api/statement");

            try
            {
                IDataValidator<Statement> validator = new StatementValidator();
                DataValidationResult validationResult = validator.Validate(statement);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                // If status of existing Statement resource is already 'cancelled' throw an exception.
                IRepository<Statement> repository = RepositoryFactory<Statement>.Create();
                Statement existingStatement = repository.Get(statement.Id);
                if (existingStatement != null && existingStatement.Status == StatementStatus.Cancelled)
                    return BadRequest("Statement cannot change status when it has been cancelled.");

                Statement result = repository.Update(statement);
                if (result != null && result.Id > 0)
                    return Ok(statement);
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
        [Route("statement/search", Name = "Statement.Search")]
        public IHttpActionResult Search([FromUri] string q)
        {
            Log.Info(String.Format("HTTP GET api/statement/search/?q={0}", q));

            try
            {
                // Search q:
                // q=quarter|ctx001;status|1;paramName|value

                QueryByExampleBuilder<Statement> queryBuilder = new QueryByExampleBuilder<Statement>();
                IRepository<Statement> repository = RepositoryFactory<Statement>.Create();
                List<Statement> statements = repository.Find(queryBuilder.GetQueryExample(q));
                if (statements != null)
                    return Ok(statements);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return InternalServerError(e);
            }
        }
    }
}