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
using LMS.Service.Security.JWT;

namespace LMS.Test.Controllers
{
    [TestClass]
    public class AuthenticationControllerTest
    {
        private UnitTestContext<User> Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new UnitTestContext<User>();
            Context.TestData.Add(Context.UserRepository.Add(new User()
            {
                Username = "lms_testuser",
                Password = "lms_testpsw",
            }));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            foreach (User data in Context.TestData)
            {
                Context.UserRepository.Remove(data.Id);
            }
        }

        [TestMethod]
        public void LoginTest()
        {
            AuthenticationController controller = new AuthenticationController();
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();

            User user = Context.TestData.Find(m => m is User) as User;

            OkNegotiatedContentResult<string> resultOK = controller.Login(user) as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(resultOK.Content);
            
            JsonWebToken token = JsonWebTokenSerializer.Decode(resultOK.Content);
            Assert.AreEqual(token.Data.Username, user.Username);
        }        
    }
}
