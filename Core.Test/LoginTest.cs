using System.Net;

using NUnit.Framework;

namespace Arana.Core.Test
{
   [TestFixture]
   public class LoginTest : TestBase
   {
      [Test]
      public void Login()
      {
         Engine.Select("li#login-test a").Follow();

         Assert.AreEqual(HttpStatusCode.OK,
                         Engine.Response.Status,
                         "The HTTP status code is invalid.");

         Assert.AreEqual("/login_test/",
                         Engine.Uri.PathAndQuery,
                         "The URI is incorrect.");
      }
   }
}