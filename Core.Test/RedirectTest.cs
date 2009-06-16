using System;

using NUnit.Framework;

namespace Arana.Core.Test
{
   [TestFixture]
   public class RedirectTest : TestBase
   {
      [Test]
      public void Redirect()
      {
         Uri expected = new Uri("http://code.google.com/p/arana/");
         Engine.Select("li#redirect-test a").Follow().Select("form").Submit(
            "input[type=submit]");

         Assert.AreEqual(expected,
                         Engine.Uri,
                         String.Format("The redirect to {0} failed.", expected));
      }
   }
}