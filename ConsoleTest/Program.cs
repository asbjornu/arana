using System;
using System.Collections.Specialized;

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

         PressAnyKey("Press any key to exit.");
      }


      private static void PressAnyKey(string message)
      {
         Console.WriteLine();
         Console.WriteLine();
         Console.WriteLine(message);
         Console.WriteLine();
         Console.ReadKey(true);
      }


      private static void TestArana()
      {
         AranaEngine engine = new AranaEngine(Settings.WebSiteUri);

         ElementList anchor1 = engine.Select("div.login a");

         Console.WriteLine(anchor1.InnerText);

         string selector = anchor1.GetSelector();

         Console.WriteLine(selector);

         ElementList anchor2 = engine.Select(selector);

         Console.WriteLine(anchor2.InnerText);

         /*Console.WriteLine(engine.Select("title").InnerText);

         engine.Select(".card-overview .section.settings a").Follow();

         Console.WriteLine(engine.Select("title").InnerText);*/
      }
   }
}