﻿using System;

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
         AranaEngine engine = new AranaEngine("http://test.aranalib.net/");

         Selection e = engine.Select("div");

         Console.WriteLine(e);
         Console.WriteLine(e);
      }
   }
}