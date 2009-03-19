using System;

using Arana.Core;

namespace Arana.Test.ConsoleApplication
{
   internal class Program
   {
      private static void Main()
      {
         try
         {
            TestArana();
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
         }

         Console.WriteLine();
         Console.WriteLine();
         Console.WriteLine("Press any key to exit.");
         Console.ReadKey(true);
      }


      private static void TestArana()
      {
         AranaEngine engine = new AranaEngine("http://localhost/Ruter.MinRuter/trunk/");
         engine.Select("div.login a").Follow();
         Console.WriteLine(engine.Select("title").InnerText);
      }
   }
}