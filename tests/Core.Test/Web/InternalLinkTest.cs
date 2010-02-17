using System;

using Arana.Web.Server;

using NUnit.Framework;

namespace Arana.Test.Web
{
   [TestFixture]
   [Category("Web")]
   public class InternalLinkTest
   {
      private WebServer webServer;
      private AranaEngine Engine { get; set; }
      private readonly Uri Uri = new Uri("http://localhost:8080/");


      [TestFixtureSetUp]
      public void SetUp()
      {
         this.webServer = new WebServer(Uri, typeof(InternalLinkTest), "Data");
         Engine = new AranaEngine(Uri, Console.Out);
      }


      [TestFixtureTearDown]
      public void FixtureTearDown()
      {
         Console.WriteLine("Stopping HTTP listener on " + Uri);
         this.webServer.Dispose();
      }


      [Test]
      public void FollowInternalLinkTest()
      {
         const string fileName = "internal-link.html";
         Engine.Navigate(Uri + fileName);
         Selection selection = Engine.Select("div#content a");
         selection.Follow();
         string actual = Engine.Uri.PathAndQuery;
         StringAssert.EndsWith(fileName, actual);
      }
   }
}