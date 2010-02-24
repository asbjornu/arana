using NUnit.Framework;

namespace Arana.Test.Web
{
   [TestFixture]
   public class LinkTest : WebServingTestBase
   {
      [Test]
      public void FollowInternalLink()
      {
         const string fileName = "internal-link.html";

         Engine.Navigate(Uri + fileName)
            .Select("div#content a")
            .Follow();

         Assert.That(Engine.Uri.PathAndQuery, Is.StringEnding(fileName));
         Assert.That(Engine.Select("title").InnerText,
                     Is.StringMatching("^Internal link test$"));
      }


      [Test]
      public void FollowNewLineLink()
      {
         const string fileName = "newline-link.html";

         Selection selection = Engine.Navigate(Uri + fileName)
            .Select("div#content a");

         Assert.That(selection, Has.Count.EqualTo(1));

         Assert.That(selection.Attribute("href"), Is.StringMatching("^/$"));

         selection.Follow();

         Assert.That(Engine.Uri.PathAndQuery, Is.StringEnding("/"));
         Assert.That(Engine.Select("title").InnerText, Is.StringContaining("Index"));
      }
   }
}