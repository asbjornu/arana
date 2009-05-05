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
         AranaEngine engine = new AranaEngine("http://test.aranalib.net/");

         Selection e = engine.Select("div");

         Console.WriteLine(e);
      }
   }
}