﻿using System;
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
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //[HttpGet()]
        //[Route("category/list", Name = "GetCategoryList")]
        //public IHttpActionResult GetCategoryList()
        //{
        //    Log.Info("HTTP GET api/inventory/category/list");

        //    try
        //    {
        //        IRepository<Category> repository = RepositoryFactory<Category>.Create();
        //        List<Category> categories = repository.Find(null);
        //        if (categories != null)
        //            return Ok(categories);
        //        else
        //            return NotFound();
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e);
        //        return InternalServerError(e);
        //    }
        //}

        //[HttpGet()]
        //[Route("category/{id}", Name = "GetCategory")]
        //public IHttpActionResult GetCategory(int id)
        //{
        //    Log.Info(String.Format("HTTP GET api/inventory/category/{0}", id));

        //    try
        //    {
        //        IRepository<Category> repository = RepositoryFactory<Category>.Create();
        //        Category category = repository.Get(id);
        //        if (category != null)
        //            return Ok(category);
        //        else
        //            return NotFound();
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e);
        //        return InternalServerError(e);
        //    }
        //}

        [HttpPost()]
        [Route("artist", Name = "AddArtist")]
        public IHttpActionResult AddArtist([FromBody] Artist artist)
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

        //[HttpDelete()]
        //[Route("category/{id}", Name = "DeleteCategory")]
        //public IHttpActionResult DeleteCategory(int id)
        //{
        //    Log.Info(String.Format("HTTP DELETE api/inventory/category/{0}", id));

        //    try
        //    {
        //        IRepository<Category> repository = RepositoryFactory<Category>.Create();
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
        //[Route("category", Name = "UpdateCategory")]
        //public IHttpActionResult UpdateCategory([FromBody] Category category)
        //{
        //    Log.Info("HTTP PUT api/inventory/category");

        //    try
        //    {
        //        IDataValidator<Category> validator = new CategoryValidator();
        //        DataValidationResult validationResult = validator.Validate(category);
        //        if (!validationResult.IsValid)
        //            return BadRequest(validationResult.Message);

        //        IRepository<Category> repository = RepositoryFactory<Category>.Create();
        //        Category result = repository.Update(category);
        //        if (result != null && result.Id > 0)
        //            return Ok(category);
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
        //[Route("category/search", Name = "SearchCategory")]
        //public IHttpActionResult SearchCategory([FromUri] string q)
        //{
        //    Log.Info(String.Format("HTTP GET api/inventory/category/search/?q={0}", q));

        //    try
        //    {
        //        // Search q:
        //        // q=name|music;active|true

        //        // TODO: Support query parameters in Repository.Find.

        //        QueryByExampleBuilder<Category> queryBuilder = new QueryByExampleBuilder<Category>();
        //        IRepository<Category> repository = RepositoryFactory<Category>.Create();
        //        List<Category> categories = repository.Find(queryBuilder.GetQueryExample(q));
        //        if (categories != null)
        //            return Ok(categories);
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