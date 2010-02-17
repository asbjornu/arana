using System;

namespace Arana.Release
{
   /// <summary>
   /// An application that compiles, merges and compresses Araña into a downloadable archive.
   /// </summary>
   internal static class ReleaseApplication
   {
      /// <summary>
      /// The application's entry point.
      /// </summary>
      /// <param name="args">The args.</param>
      /// <returns><c>0</c> on success, <c>-1</c> on failure.</returns>
      public static int Main(string[] args)
      {
         int result = 0;
         bool isDebugging = (args == null) || (args.Length == 0);

         try
         {
            string solutionDirectory = isDebugging
                                          ? null
                                          : args[0].Replace('"', ' ').Trim();

            new ReleaseHelper(solutionDirectory).Release();
            
            Console.WriteLine();
            Console.WriteLine("Successfully merged and compressed Araña.");
            Console.WriteLine();
         }
         catch (Exception ex)
         {
            Console.WriteLine();
            Console.WriteLine(ex);
            Console.WriteLine();

            if (isDebugging)
            {
               Console.WriteLine("Press any key to exit");
               Console.Read();
            }

            result = -1;
         }

         return result;
      }
   }
}