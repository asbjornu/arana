using NUnit.Framework;

namespace Arana.Web.Server.Test
{
   public class WebServerTest : WebServerTestBase
   {
      [Test]
      public void CanNavigateToTest()
      {
         Engine.Select("ul li a[href='test.html']").Follow();

         string title = Engine.Select("title").InnerText;
         Assert.That(title, Is.EqualTo("Test"));
      }


      [Test]
      public void HeadersAreCorrect()
      {
         string server = Engine.Response.Headers["Server"];
         Assert.That(server, Is.StringStarting("Arana-Web-Server"));
      }


      [Test]
      public void IndexPageContainsCorrectData()
      {
         string title = Engine.Select("title").InnerText;
         Selection listItems = Engine.Select("ul li");

         Assert.That(title, Is.EqualTo("Index"));
         Assert.That(listItems, Has.Count.EqualTo(1));
      }
   }
}