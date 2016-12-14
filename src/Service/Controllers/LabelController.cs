using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using log4net;

using LMS.Model;
using LMS.Model.Resource;
using LMS.Data;

namespace LMS.Service.Controllers
{
    [RoutePrefix("api")]
    public class LabelController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet()]
        [Route("label/list", Name = "Label.List")]
        public IHttpActionResult List()
        {
            Log.Info("HTTP GET api/label/list");

            try
            {
                IRepository<Label> repository = RepositoryFactory<Label>.Create();
                List<Label> labels = repository.Find(null);
                if (labels != null)
                    return Ok(labels);
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
        [Route("label/{id}", Name = "Label.Read")]
        public IHttpActionResult Read(int id)
        {
            Log.Info(String.Format("HTTP GET api/label/{0}", id));

            try
            {
                IRepository<Label> repository = RepositoryFactory<Label>.Create();
                Label label = repository.Get(id);
                if (label != null)
                    return Ok(label);
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
        [Route("label", Name = "Label.Create")]
        public IHttpActionResult Create([FromBody] Label label)
        {
            Log.Info("HTTP POST api/label");

            try
            {
                IDataValidator<Label> validator = new LabelValidator();
                DataValidationResult validationResult = validator.Validate(label);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                IRepository<Label> repository = RepositoryFactory<Label>.Create();
                Label result = repository.Add(label);
                if (result != null && result.Id > 0)
                    return Ok(label);
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
        [Route("label/{id}", Name = "Label.Delete")]
        public IHttpActionResult Delete(int id)
        {
            Log.Info(String.Format("HTTP DELETE api/label/{0}", id));

            try
            {
                IRepository<Label> repository = RepositoryFactory<Label>.Create();
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
        [Route("label", Name = "Label.Update")]
        public IHttpActionResult Update([FromBody] Label label)
        {
            Log.Info("HTTP PUT api/label");

            try
            {
                IDataValidator<Label> validator = new LabelValidator();
                DataValidationResult validationResult = validator.Validate(label);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                IRepository<Label> repository = RepositoryFactory<Label>.Create();
                Label result = repository.Update(label);
                if (result != null && result.Id > 0)
                    return Ok(label);
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
        [Route("label/search", Name = "Label.Search")]
        public IHttpActionResult Search([FromUri] string q)
        {
            Log.Info(String.Format("HTTP GET api/label/search/?q={0}", q));

            try
            {
                // Search q:
                // q=name|kill;paramName|value

                QueryByExampleBuilder<Label> queryBuilder = new QueryByExampleBuilder<Label>();
                IRepository<Label> repository = RepositoryFactory<Label>.Create();
                List<Label> labels = repository.Find(queryBuilder.GetQueryExample(q));
                if (labels != null)
                    return Ok(labels);
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