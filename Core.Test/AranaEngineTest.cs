using System;
using System.Net;

using NUnit.Framework;

namespace Arana.Test
{
   [TestFixture]
   public class AranaEngineTest
   {
      [TestFixtureTearDown]
      public void FixtureTearDown()
      {
         Console.WriteLine("Stopping HTTP listener on localhost:8080");
         this.listener.Stop();
         ((IDisposable) this.listener).Dispose();
      }


      [TestFixtureSetUp]
      public void FixtureSetUp()
      {
         this.credential = new NetworkCredential("admin", "arana", "aranalib.net");
         this.proxy = new WebProxy("localhost", 8080);

         Console.WriteLine("Starting HTTP listener on localhost:8080");

         this.listener = new HttpListener();
         this.listener.Prefixes.Add("http://localhost:8080/");
         this.listener.Start();
      }


      private const string UriString = "http://test.aranalib.net/";
      private static readonly Uri Uri = new Uri(UriString);
      private NetworkCredential credential;
      private WebProxy proxy;
      private HttpListener listener;


      [Test]
      public void UriConstructor()
      {
         AranaEngine engine = new AranaEngine(Uri);
         Assert.That(engine.Uri.ToString(),
                     Is.EqualTo(UriString),
                     "The URI isn't correct.");
      }


      [Test]
      public void UriCredentialConstructor()
      {
         AranaEngine engine = new AranaEngine(Uri, this.credential);
         Assert.That(engine.Response.Body,
                     Is.Not.Null,
                     "The response body is null.");
         Assert.That(engine.Uri.ToString(),
                     Is.EqualTo(UriString),
                     "The URI isn't correct.");
      }


      [Test]
      public void UriCredentialProxyTextWriterConstructor()
      {
         AranaEngine engine = new AranaEngine(Uri,
                                              this.credential,
                                              this.proxy,
                                              Console.Out);

         Assert.That(engine.Output,
                     Is.EqualTo(Console.Out),
                     "The engine's output isn't equal to the one set in the constructor.");

         Assert.That(engine.Uri.ToString(),
                     Is.EqualTo(UriString),
                     "The URI isn't correct.");
      }


      [Test]
      public void UriProxyConstructor()
      {
         AranaEngine engine = new AranaEngine(Uri, this.proxy);

         Assert.That(engine.Response.Body,
                     Is.Not.Null,
                     "The response body is null.");

         Assert.That(engine.Uri.ToString(),
                     Is.EqualTo(UriString),
                     "The URI isn't correct.");
      }


      [Test]
      public void UriStringConstructor()
      {
         AranaEngine engine = new AranaEngine(UriString);

         Assert.That(engine.Uri.ToString(),
                     Is.EqualTo(UriString),
                     "The URI isn't correct.");
      }


      [Test]
      public void UriStringCredentialConstructor()
      {
         AranaEngine engine = new AranaEngine(UriString, this.credential);

         Assert.That(engine.Response.Body,
                     Is.Not.Null,
                     "The response body is null.");

         Assert.That(engine.Uri.ToString(),
                     Is.EqualTo(UriString),
                     "The URI isn't correct.");
      }


      [Test]
      public void UriStringCredentialProxyTextWriterConstructor()
      {
         AranaEngine engine = new AranaEngine(UriString,
                                              this.credential,
                                              this.proxy,
                                              Console.Out);

         Assert.That(engine.Output,
                     Is.EqualTo(Console.Out),
                     "The engine's output isn't equal to the one set in the constructor.");

         Assert.That(engine.Uri.ToString(),
                     Is.EqualTo(UriString),
                     "The URI isn't correct.");
      }


      [Test]
      public void UriStringProxyConstructor()
      {
         AranaEngine engine = new AranaEngine(UriString, this.proxy);

         Assert.That(engine.Response.Body,
                     Is.Not.Null,
                     "The response body is null.");

         Assert.That(engine.Uri.ToString(),
                     Is.EqualTo(UriString),
                     "The URI isn't correct.");
      }


      [Test]
      public void UriStringTextWriterConstructor()
      {
         AranaEngine engine = new AranaEngine(UriString, Console.Out);

         Assert.That(engine.Output,
                     Is.EqualTo(Console.Out),
                     "The engine's output isn't equal to the one set in the constructor.");

         Assert.That(engine.Uri.ToString(),
                     Is.EqualTo(UriString),
                     "The URI isn't correct.");
      }


      [Test]
      public void UriTextWriterConstructor()
      {
         AranaEngine engine = new AranaEngine(Uri, Console.Out);

         Assert.That(engine.Output,
                     Is.EqualTo(Console.Out),
                     "The engine's output isn't equal to the one set in the constructor.");

         Assert.That(engine.Uri.ToString(),
                     Is.EqualTo(UriString),
                     "The URI isn't correct.");
      }
   }
}