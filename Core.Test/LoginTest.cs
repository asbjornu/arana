using System.Net;

using NUnit.Framework;

namespace Arana.Core.Test
{
   [TestFixture]
   public class LoginTest
   {
      [Test]
      public void Login()
      {
         AranaEngine engine = new AranaEngine("http://test.aranalib.net/")
            .Select("li#login-test a")
            .Follow();

         Assert.AreEqual(HttpStatusCode.OK,
                         engine.Response.Status,
                         "The HTTP status code is invalid.");

         Assert.AreEqual("/login_test/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");
      }
   }
}