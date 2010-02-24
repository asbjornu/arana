using System;

using Arana.Web.Server;

using NUnit.Framework;

namespace Arana.Test.Web
{
   [Category("Web")]
   public abstract class WebServingTestBase : TestBase
   {
      protected WebServingTestBase()
         : base("http://localhost:8080/")
      {
      }


      private WebServer webServer;


      [TestFixtureSetUp]
      public void FixtureSetUp()
      {
         this.webServer = new WebServer(Uri, typeof(WebServingTestBase), "Data");
      }


      [TestFixtureTearDown]
      public void FixtureTearDown()
      {
         Console.WriteLine("Stopping HTTP listener on " + Uri);
         this.webServer.Dispose();
      }
   }
}