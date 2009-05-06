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
         AranaEngine engine = new AranaEngine("http://localhost:9999/");

         engine.Select("li#simple-post-test a").Follow();

         engine.Select("form").Submit(new Preselection
         {
            { "p.textbox input", input => input.Value("Textbox1") },
            { "p.radio input", radio => radio.Check() },
            { "p.checkbox input", checkbox => checkbox.Check() },
            { "p.textarea textarea", textarea => textarea.Value("Textarea1") },
            { "p.select select", select => select.Choose(3) },
         });

         Console.WriteLine(engine.Select("p.textbox span.value").InnerText);
         Console.WriteLine(engine.Select("p.radio span.value").InnerText);
         Console.WriteLine(engine.Select("p.checkbox span.value").InnerText);
         Console.WriteLine(engine.Select("p.textarea span.value").InnerText);
         Console.WriteLine(engine.Select("p.select span.value").InnerText);
         Console.WriteLine(engine.Select("p.submit span.value").InnerText);
      }
   }
}