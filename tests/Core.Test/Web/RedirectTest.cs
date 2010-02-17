using System;

using NUnit.Framework;

namespace Arana.Test.Web
{
   [TestFixture]
   [Category("Web")]
   public class RedirectTest : TestBase
   {
      [Test]
      public void Redirect()
      {
         Uri expected = new Uri("http://code.google.com/p/arana/");
         Engine.Select("li#redirect-test a").Follow().Select("form").Submit(
            "input[type=submit]");

         Assert.That(Engine.Uri,
                     Is.EqualTo(expected),
                     String.Format("The redirect to {0} failed.", expected));
      }
   }
}