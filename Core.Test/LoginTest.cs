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

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");
      }
   }
}