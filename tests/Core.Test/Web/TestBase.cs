using System;

using NUnit.Framework;

namespace Arana.Test.Web
{
   public abstract class TestBase
   {
      protected TestBase()
         : this("http://test.aranalib.net/")
      {
      }


      protected TestBase(string uri)
      {
         Uri = uri;
      }


      protected AranaEngine Engine { get; set; }
      protected string Uri { get; private set; }


      [SetUp]
      public virtual void SetUp()
      {
         Engine = new AranaEngine(Uri, Console.Out);
      }
   }
}