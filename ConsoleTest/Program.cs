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


      private static void TestArana()
      {
         AranaEngine engine = new AranaEngine(Settings.WebSiteUri);
         
         engine.Select("div.login a").Follow();
         Console.WriteLine(engine.Select("title").InnerText);

         engine.Select("form#aspnetForm").Submit(false, new NameValueCollection
         {
            {"email", Settings.Email},
            {"password", Settings.Password},
         });

         Console.WriteLine(engine.Response.Location);

         /*Console.WriteLine(engine.Select("title").InnerText);

         engine.Select(".card-overview .section.settings a").Follow();

         Console.WriteLine(engine.Select("title").InnerText);*/
      }

      private static void PressAnyKey(string message)
      {
         Console.WriteLine();
         Console.WriteLine();
         Console.WriteLine(message);
         Console.WriteLine();
         Console.ReadKey(true);
      }
   }
}