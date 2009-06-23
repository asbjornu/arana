using System;

using NUnit.Framework;

namespace Arana.Core.Test.Web
{
   public class TestBase
   {
      protected AranaEngine Engine { get; set; }

      protected const string Uri = "http://test.aranalib.net/";


      [SetUp]
      public virtual void SetUp()
      {
         Engine = new AranaEngine(Uri, Console.Out);
      }
   }
}