using System;
using System.Collections.Generic;
using System.Web.Http;

using log4net;

using LMS.Model.Resource;
using LMS.Data;
using LMS.Configuration;
using LMS.Service.Logging;
using LMS.Service.Security.JWT;
using LMS.Model.Resource.Logging;
using LMS.Model.Composite;

namespace LMS.Service.Controllers
{
    [RoutePrefix("api")]
    public class AuthenticationController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger("Trace");
        
        [HttpPost()]
        [Route("auth/login", Name = "AuthenticationController.Login")]
        public IHttpActionResult Login([FromBody] User user)
        {
            Log.Info("HTTP POST api/auth/login");

            try
            {
                IRepository<User> repository = RepositoryFactory<User>.Create();
                List<User> result = repository.Find(user);
                if (result != null && result.Count == 1)
                {
                    string token = JsonWebTokenSerializer.Encode(new JsonWebToken()
                    {
                        Header = new JsonWebTokenHeader()
                        {
                            Algorithm = "RS256",
                            Type = "JWT"
                        },
                        Data = new JsonWebTokenData()
                        {
                            Issuer = "http://labeleaze.com/api/auth/login",
                            Audience = "http://labeleaze.com/api",
                            Subject = new Reference(Reference.UserUri, result[0].Id).Uri,
                            Username = result[0].Username,
                            ExpirationTime = DateTime.Now.AddMinutes(60).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds.ToString()
                        }
                    });

                    return Ok(token);
                }                    
                else
                    return Unauthorized();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return InternalServerError(e);
            }
        }

        [HttpPost()]
        [Route("auth/logout", Name = "AuthenticationController.Logout")]
        public IHttpActionResult Logout([FromBody] User user)
        {
            Log.Info("HTTP POST api/auth/logout");

            throw new NotImplementedException();

            //try
            //{
            //    IRepository<Transaction> repository = RepositoryFactory<Transaction>.Create();
            //    Transaction result = repository.Add(transaction);
            //    if (result != null && result.Id > 0)
            //        return Ok(transaction);
            //    else
            //        return BadRequest();
            //}
            //catch (Exception e)
            //{
            //    Log.Error(e);
            //    return InternalServerError(e);
            //}
        }
    }
}