using NUnit.Framework;

namespace Arana.Test.Web
{
   [TestFixture]
   public class EncodingTest
   {
      [Test]
      public void PerformGoogleSearchWithNonAsciiCharacters()
      {
         const string search = "æøå";
         AranaEngine engine = new AranaEngine("http://www.google.com/search?q=" + search);
         Assert.That(engine.Response.Body,
                     Is.Not.Null.And.ContainsSubstring(search),
                     "The Google search was not successful.");
      }
   }
}