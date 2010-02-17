using System.Web;

using NUnit.Framework;

namespace Arana.Test.Web
{
   [TestFixture]
   [Category("Web")]
   public class QueryStringTest : TestBase
   {
      [Test]
      public void Test()
      {
         const string expectedPathAndQuery = "/querystring_test/?foo=bar&baz=zux";

         Engine.Navigate(expectedPathAndQuery);

         string actualPathAndQuery =
            HttpUtility.HtmlDecode(Engine.Select("p#querystring").InnerText);

         Assert.That(actualPathAndQuery,
                     Is.EqualTo(expectedPathAndQuery),
                     "The query string was not transferred correctly.");
      }
   }
}