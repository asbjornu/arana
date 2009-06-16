using System;
using System.Net;

using NUnit.Framework;

namespace Arana.Core.Test
{
   [TestFixture]
   public class NavigationTest : TestBase
   {
      [Test]
      public void ToLoginAndBackAndForwardAgain()
      {
         Assert.AreEqual("/",
                         Engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         Engine.Select("li#login-test a").Follow();

         Assert.AreEqual(HttpStatusCode.OK,
                         Engine.Response.Status,
                         "The HTTP status code is invalid.");

         Assert.AreEqual("/login_test/",
                         Engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         // Navigate back
         Engine.Navigate(-1);

         Assert.AreEqual(HttpStatusCode.OK,
                         Engine.Response.Status,
                         "The HTTP status code is invalid.");

         Assert.AreEqual("/",
                         Engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         // Navigate forward again
         Engine.Navigate(1);

         Assert.AreEqual(HttpStatusCode.OK,
                   Engine.Response.Status,
                   "The HTTP status code is invalid.");

         Assert.AreEqual("/login_test/",
                         Engine.Uri.PathAndQuery,
                         "The URI is incorrect.");
      }

      [Test]
      [ExpectedException(typeof(ArgumentOutOfRangeException),
         ExpectedMessage = "Can't navigate back 1 step, as there's only 0 \"historical\" requests to navigate to.",
         MatchType = MessageMatch.Contains)]
      public void NavigatingBackOnFreshEngineThrowsArgumentOutOfRangeException()
      {
         Engine.Navigate(-1);
      }

      [Test]
      [ExpectedException(typeof(ArgumentOutOfRangeException),
         ExpectedMessage = "Can't navigate forward 1 step, as there's only 0 \"future\" requests to navigate to.",
         MatchType = MessageMatch.Contains)]
      public void NavigatingForwardOnFreshEngineThrowsArgumentOutOfRangeException()
      {
         Engine.Navigate(1);
      }
   }
}