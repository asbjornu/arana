using System;
using System.Net;

using NUnit.Framework;

namespace Arana.Test.Web
{
   [TestFixture]
   [Ignore("Run this explicitly after the GAE app is running.")]
   public class SessionTest : TestBase
   {
      private const string Administrator = "Administrator";
      private const string LoginFormPath = "/login_test/";
      private const string LoggedInPath = "/logged_in_test/";


      private void AssertUserNameFromSpan(string expectedUsername)
      {
         Selection usernameSelection = Engine.Select("span.username");

         Assert.That(usernameSelection.Count,
                     Is.EqualTo(1),
                     "No <span class='username'> element found.");

         string username = usernameSelection.InnerText;

         Assert.That(username,
                     Is.EqualTo(expectedUsername),
                     "The username was not submitted or returned correctly from the session.");
      }


      private void Initialize()
      {
         // TODO: Figure out why this test only works on a locally hosted Google App Engine and not on the public one
         Engine = new AranaEngine("http://localhost:9999/", Console.Out);

         Engine.Select("li#login-test a").Follow();

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo(LoginFormPath),
                     "The URI is incorrect.");
      }


      private void LogIn()
      {
         Engine.Select("form").Submit(
            "input[type=submit]",
            new Preselection
            {
               { "input#username", input => input.Value(Administrator) },
               { "input#password", input => input.Value("2fmckX32a") },
            });

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo(LoggedInPath),
                     "The login failed.");

         AssertUserNameFromSpan(Administrator);
      }


      private void NavigateBackAndForth()
      {
         Engine.Navigate(-1);

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo(LoginFormPath),
                     "Navigating back from the logged in page resulted in a wrong path.");

         AssertUserNameFromSpan(Administrator);

         Engine.Navigate(1);

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo(LoggedInPath),
                     "Navigating forward from the login form resulted in a wrong path.");

         AssertUserNameFromSpan(Administrator);
      }


      private void LogOut()
      {
         Engine.Select("form").Submit("input[type=submit]");

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo(LoginFormPath),
                     "Logging out failed.");

         Selection loggedInSelection = Engine.Select("div#logged-in");

         Assert.That(loggedInSelection.Count,
                     Is.EqualTo(0),
                     "The session was not terminated successfully.");
      }


      [Test]
      [Category("Web")]
      public void LogInNavigateBackAndForthThenLogOut()
      {
         Initialize();
         LogIn();
         NavigateBackAndForth();
         LogOut();
      }
   }
}