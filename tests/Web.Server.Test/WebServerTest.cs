using NUnit.Framework;

namespace Arana.Web.Server.Test
{
   public class WebServerTest : WebServerTestBase
   {
      private AranaEngine engine;
      private WebServer webServer;


      [TestFixtureSetUp]
      public void FixtureSetUp()
      {
         this.webServer = new WebServer(Uri, typeof(WebServerTest), "Data");
         this.engine = new AranaEngine(Uri);
      }


      [TestFixtureTearDown]
      public void FixtureTearDown()
      {
         this.engine.Dispose();
         this.webServer.Dispose();
      }


      [Test]
      public void Test01_IndexPage_ContainsCorrectData()
      {
         string title = this.engine.Select("title").InnerText;
         Selection listItems = this.engine.Select("ul li");

         Assert.That(title, Is.EqualTo("Index"));
         Assert.That(listItems, Has.Count.EqualTo(1));
      }


      [Test]
      public void Test02_CanNavigateToTest()
      {
         this.engine.Select("ul li a[href='test.html']").Follow();

         string title = this.engine.Select("title").InnerText;
         Assert.That(title, Is.EqualTo("Test"));
      }
   }
}