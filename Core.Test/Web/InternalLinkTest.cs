using System;

using NUnit.Framework;

namespace Arana.Core.Test.Web
{
    [TestFixture]
    [Category("Web")]
    internal class InternalLinkTest
    {
        protected TestWebServer WebServer;
        protected AranaEngine Engine { get; set; }
        protected const string Uri = "http://localhost:8080/";


        [TestFixtureSetUp]
        public void SetUp()
        {
            WebServer = new TestWebServer(Uri, "Arana.Core.Test.Web.TestData");

            Engine = new AranaEngine(Uri, Console.Out);
        }


        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Console.WriteLine("Stopping HTTP listener on " + Uri);
            this.WebServer.Stop();
        }


        [Test]
        public void FollowInternalLinkTest()
        {
            Engine.Navigate(Uri + "internalLink.html");
            Selection selection = Engine.Select("div.content a");
            selection.Follow();
            string actual = Engine.Uri.PathAndQuery;
            StringAssert.EndsWith("link.html#selflink", actual);
            // or should we expect it to end with just "link.html"?
        }
    }
}
