using System;

using NUnit.Framework;

namespace Arana.Web.Server.Test
{
   public abstract class WebServerTestBase
   {
      protected const string Prefix = "badabada";
      protected const string UriString = "http://localhost:8080/";
      protected static readonly Uri Uri = new Uri(UriString);

      private WebServer webServer;
      protected AranaEngine Engine { get; private set; }


      [TestFixtureSetUp]
      public virtual void FixtureSetUp()
      {
         this.webServer = new WebServer(Uri, typeof(WebServerTest), "Data");
      }


      [TestFixtureTearDown]
      public virtual void FixtureTearDown()
      {
         Engine.Dispose();
         this.webServer.Dispose();
      }


      [SetUp]
      public virtual void SetUp()
      {
         Engine = new AranaEngine(Uri);
      }
   }
}