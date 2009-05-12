using System;
using Arana.Core;

namespace Arana.Test.ConsoleApplication
{
   internal static class Program
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

         PressAnyKey();
      }


      private static void PressAnyKey()
      {
         Console.WriteLine();
         Console.WriteLine();
         Console.WriteLine("Press any key to exit.");
         Console.WriteLine();
         Console.ReadKey(true);
      }


      private static void TestArana()
      {
         AranaEngine engine = new AranaEngine("http://localhost:9999/")
         {
            Output = Console.Out
         };

         engine.Select("li#simple-post-test a").Follow();

         engine.Select("form").Submit(new Preselection
         {
            { "p.radio1 input", radio => radio.Check() },
         });

         Console.WriteLine(engine.Select("p.radio1 span.value").InnerText);
         Console.WriteLine(engine.Select("p.radio2 span.value").InnerText);
      }
   }
}