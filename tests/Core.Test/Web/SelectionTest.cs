using System;

using NUnit.Framework;

namespace Arana.Test.Web
{
   [TestFixture]
   public class SelectionTest : WebServingTestBase
   {
      [Test]
      public void TitleSelector()
      {
         string titleSelector = Engine.Select("title").ToString();
         Assert.That(titleSelector, Is.StringMatching("^html head title$"));
      }
   }
}