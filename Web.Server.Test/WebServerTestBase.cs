using System;

namespace Arana.Web.Server.Test
{
   public abstract class WebServerTestBase
   {
      protected const string Prefix = "badabada";
      protected const string UriString = "http://localhost:8080/";
      protected static readonly Uri Uri = new Uri(UriString);
   }
}