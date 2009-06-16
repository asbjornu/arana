using System;

using NUnit.Framework;

namespace Arana.Core.Test
{
   public class TestBase
   {
      protected AranaEngine Engine { get; private set; }


      [SetUp]
      public virtual void SetUp()
      {
         Engine = new AranaEngine("http://test.aranalib.net/")
         {
            Output = Console.Out
         };
      }
   }
}