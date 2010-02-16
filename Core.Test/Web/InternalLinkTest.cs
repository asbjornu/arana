using System;

using NUnit.Framework;

namespace Arana.Core.Test.Web
{
    [TestFixture]
    [Category("Web")]
    public class InternalLinkTest
    {
        private TestWebServer WebServer;
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
            const string fileName = "internalLink.html";
            Engine.Navigate(Uri + fileName);
            Selection selection = Engine.Select("div.content a");
            selection.Follow();
            string actual = Engine.Uri.PathAndQuery;
            StringAssert.EndsWith(fileName, actual);           
        }
    }
}
