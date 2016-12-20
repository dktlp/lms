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
    public class ArtistController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger("Trace");

        [HttpGet()]
        [Route("artist/list", Name = "Artist.List")]
        public IHttpActionResult List()
        {
            Log.Info("HTTP GET api/artist/list");

            try
            {
                IRepository<Artist> repository = RepositoryFactory<Artist>.Create();
                List<Artist> artists = repository.Find(null);
                if (artists != null)
                    return Ok(artists);
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
        [Route("artist/{id}", Name = "Artist.Read")]
        public IHttpActionResult Read(int id)
        {
            Log.Info(String.Format("HTTP GET api/artist/{0}", id));

            try
            {
                IRepository<Artist> repository = RepositoryFactory<Artist>.Create();
                Artist artist = repository.Get(id);
                if (artist != null)
                    return Ok(artist);
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
        [Route("artist", Name = "Artist.Create")]
        public IHttpActionResult Create([FromBody] Artist artist)
        {
            Log.Info("HTTP POST api/artist");

            try
            {
                IDataValidator<Artist> validator = new ArtistValidator();
                DataValidationResult validationResult = validator.Validate(artist);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                IRepository<Artist> repository = RepositoryFactory<Artist>.Create();
                Artist result = repository.Add(artist);
                if (result != null && result.Id > 0)
                    return Ok(artist);
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
        [Route("artist/{id}", Name = "Artist.Delete")]
        public IHttpActionResult Delete(int id)
        {
            Log.Info(String.Format("HTTP DELETE api/artist/{0}", id));

            try
            {
                IRepository<Artist> repository = RepositoryFactory<Artist>.Create();
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
        [Route("artist", Name = "Artist.Update")]
        public IHttpActionResult Update([FromBody] Artist artist)
        {
            Log.Info("HTTP PUT api/artist");

            try
            {
                IDataValidator<Artist> validator = new ArtistValidator();
                DataValidationResult validationResult = validator.Validate(artist);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Message);

                IRepository<Artist> repository = RepositoryFactory<Artist>.Create();
                Artist result = repository.Update(artist);
                if (result != null && result.Id > 0)
                    return Ok(artist);
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
        [Route("artist/search", Name = "Artist.Search")]
        public IHttpActionResult Search([FromUri] string q)
        {
            Log.Info(String.Format("HTTP GET api/artist/search/?q={0}", q));

            try
            {
                // Search q:
                // q=stageName|tomtek;paramName|value
                
                QueryByExampleBuilder<Artist> queryBuilder = new QueryByExampleBuilder<Artist>();
                IRepository<Artist> repository = RepositoryFactory<Artist>.Create();
                List<Artist> artists = repository.Find(queryBuilder.GetQueryExample(q));
                if (artists != null)
                    return Ok(artists);
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