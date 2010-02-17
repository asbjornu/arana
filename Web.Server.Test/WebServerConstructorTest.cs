using System;

using NUnit.Framework;

namespace Arana.Web.Server.Test
{
   [TestFixture]
   public class WebServerConstructorTest : WebServerTestBase
   {
      [Test]
      public void NullUriStringType_ThrowsArgumentNullException()
      {
         TestDelegate throwing = () =>
         {
            using (new WebServer(null as string, typeof(WebServerTest)))
            {
            }
         };

         Assert.That(throwing, Throws.TypeOf<ArgumentNullException>());
      }


      [Test]
      public void NullUriType_ThrowsArgumentNullException()
      {
         TestDelegate throwing = () =>
         {
            using (new WebServer(null as Uri, typeof(WebServerTest)))
            {
            }
         };

         Assert.That(throwing, Throws.TypeOf<ArgumentNullException>());
      }


      [Test]
      public void UriNullType_ThrowsArgumentNullException()
      {
         TestDelegate throwing = () =>
         {
            using (new WebServer(Uri, null))
            {
            }
         };

         Assert.That(throwing, Throws.TypeOf<ArgumentNullException>());
      }


      [Test]
      public void UriStringNullType_ThrowsArgumentNullException()
      {
         TestDelegate throwing = () =>
         {
            using (new WebServer(UriString, null))
            {
            }
         };

         Assert.That(throwing, Throws.TypeOf<ArgumentNullException>());
      }


      [Test]
      public void UriStringType()
      {
         using (new WebServer(UriString, typeof(WebServerTest)))
         {
         }
      }


      [Test]
      public void UriStringTypePrefix()
      {
         using (new WebServer(UriString, typeof(WebServerTest), Prefix))
         {
         }
      }


      [Test]
      public void UriType()
      {
         using (new WebServer(Uri, typeof(WebServerTest)))
         {
         }
      }


      [Test]
      public void UriTypePrefix()
      {
         using (new WebServer(Uri, typeof(WebServerTest), Prefix))
         {
         }
      }
   }
}