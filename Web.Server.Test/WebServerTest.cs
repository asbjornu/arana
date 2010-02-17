using System;

using NUnit.Framework;

namespace Arana.Web.Server.Test
{
   [TestFixture]
   public class WebServerTest
   {
      private const string Prefix = "badabada";
      private const string UriString = "http://localhost:8080/";
      private static readonly Uri Uri = new Uri(UriString);


      [Test]
      public void UriStringTypeConstructor()
      {
         using (new WebServer(UriString, typeof(WebServerTest)))
         {
         }
      }


      [Test]
      public void UriStringTypePrefixConstructor()
      {
         using (new WebServer(UriString, typeof(WebServerTest), Prefix))
         {
         }
      }


      [Test]
      public void UriTypeConstructor()
      {
         using (new WebServer(Uri, typeof(WebServerTest)))
         {
         }
      }


      [Test]
      public void UriTypePrefixConstructor()
      {
         using (new WebServer(Uri, typeof(WebServerTest), Prefix))
         {
         }
      }
   }
}