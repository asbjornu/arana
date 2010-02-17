using System;

using NUnit.Framework;

namespace Arana.Release.Test
{
   [TestFixture]
   public class ReleaseTest
   {
      [Test]
      [Ignore("Find a way to test this without going through temporary directories.")]
      public void Release()
      {
         try
         {
            ReleaseHelper releaseHelper = new ReleaseHelper();
            releaseHelper.Release();
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
            throw;
         }
      }
   }
}