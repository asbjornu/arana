using System.Net;

using NUnit.Framework;

namespace Arana.Core.Test.Web
{
   [TestFixture]
   [Category("Web")]
   public class LoginTest : TestBase
   {
      [Test]
      public void Login()
      {
         Engine.Select("li#login-test a").Follow();

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo("/login_test/"),
                     "The URI is incorrect.");
      }
   }
}