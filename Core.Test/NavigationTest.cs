using System;
using System.Net;

using NUnit.Framework;

namespace Arana.Core.Test
{
   [TestFixture]
   public class NavigationTest
   {
      [Test]
      public void ToLoginAndBackAndForwardAgain()
      {
         AranaEngine engine = new AranaEngine("http://test.aranalib.net/")
         {
            Output = Console.Out
         };

         Assert.AreEqual("/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         engine.Select("li#login-test a").Follow();

         Assert.AreEqual(HttpStatusCode.OK,
                         engine.Response.Status,
                         "The HTTP status code is invalid.");

         Assert.AreEqual("/login_test/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         // Navigate back
         engine.Navigate(-1);

         Assert.AreEqual(HttpStatusCode.OK,
                         engine.Response.Status,
                         "The HTTP status code is invalid.");

         Assert.AreEqual("/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         // Navigate forward again
         engine.Navigate(1);

         Assert.AreEqual(HttpStatusCode.OK,
                   engine.Response.Status,
                   "The HTTP status code is invalid.");

         Assert.AreEqual("/login_test/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");
      }

      [Test]
      [ExpectedException(typeof(ArgumentOutOfRangeException),
         ExpectedMessage = "Can't navigate back 1 step, as there's only 0 \"historical\" requests to navigate to.",
         MatchType = MessageMatch.Contains)]
      public void NavigatingBackOnFreshEngineThrowsArgumentOutOfRangeException()
      {
         AranaEngine engine = new AranaEngine("http://test.aranalib.net/")
         {
            Output = Console.Out
         };

         engine.Navigate(-1);
      }

      [Test]
      [ExpectedException(typeof(ArgumentOutOfRangeException),
         ExpectedMessage = "Can't navigate forward 1 step, as there's only 0 \"future\" requests to navigate to.",
         MatchType = MessageMatch.Contains)]
      public void NavigatingForwardOnFreshEngineThrowsArgumentOutOfRangeException()
      {
         AranaEngine engine = new AranaEngine("http://test.aranalib.net/")
         {
            Output = Console.Out
         };

         engine.Navigate(1);
      }
   }
}