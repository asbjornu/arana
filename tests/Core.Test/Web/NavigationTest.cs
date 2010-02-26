using System;
using System.Net;

using NUnit.Framework;

namespace Arana.Test.Web
{
   [TestFixture]
   [Category("Web")]
   public class NavigationTest : WebServingTestBase
   {
      [Test]
      public void IndexThenAbsoluteUri()
      {
         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo("/"),
                     "The URI is incorrect.");

         const string uri = "http://code.google.com/p/arana/";

         Engine.Navigate(uri);

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The response status code was not OK.");

         Assert.That(Engine.Uri.ToString(),
                     Is.EqualTo(uri),
                     "The URI was not correct.");
      }


      [Test]
      public void IndexThenRelativeUri()
      {
         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo("/"),
                     "The URI is incorrect.");

         const string path = "/internal-link.html";

         Engine.Navigate(path);

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The response status code was not OK.");

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo(path),
                     "The URI is incorrect.");
      }


      [Test]
      public void IndexThenRelativeUriWithQueryString()
      {
         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo("/"),
                     "The URI is incorrect.");

         const string pathAndQuery = "/internal-link.html?foo=bar&baz=zux";

         Engine.Navigate(pathAndQuery);

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The response status code was not OK.");

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo(pathAndQuery),
                     "The URI is incorrect.");
      }


      [Test]
      public void NavigatingBackOnFreshEngineThrowsArgumentOutOfRangeException()
      {
         Assert.That(() => Engine.Navigate(-1),
                     Throws.TypeOf<ArgumentOutOfRangeException>()
                        .With.Message.ContainsSubstring(
                           "Can't navigate back 1 step"));
      }


      [Test]
      public void NavigatingForwardOnFreshEngineThrowsArgumentOutOfRangeException()
      {
         Assert.That(() => Engine.Navigate(1),
                     Throws.TypeOf<ArgumentOutOfRangeException>()
                        .With.Message.ContainsSubstring(
                           "Can't navigate forward 1 step"));
      }


      [Test]
      public void ForwardAndBackAndForwardAgain()
      {
         const string file = "/newline-link.html";

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo("/"),
                     "The URI is incorrect.");

         Engine.Select("li:first-child a").Follow();

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");
          
         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo(file),
                     "The URI is incorrect.");

         // Navigate back
         Engine.Navigate(-1);

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo("/"),
                     "The URI is incorrect.");

         // Navigate forward again
         Engine.Navigate(1);

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo(file),
                     "The URI is incorrect.");
      }
   }
}