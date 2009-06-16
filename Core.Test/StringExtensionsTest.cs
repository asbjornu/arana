using System;

using NUnit.Framework;

using Arana.Core.Extensions;

namespace Arana.Core.Test
{
   [TestFixture]
   public class StringExtensionsTest
   {
      [Test]
      public void ToDateTime()
      {
         // TODO: Find more obscure dates to parse
         string[] dates = new[] { "Thu, 16-Jun-2011 08:58:15 GMT" };
         DateTime defaultDateTime = default(DateTime);

         foreach (string date in dates)
         {
            DateTime dateTime = date.ToDateTime(defaultDateTime);
            Assert.AreNotEqual(defaultDateTime, dateTime);
         }
      }
   }
}