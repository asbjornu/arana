using System;

using ILMerging;

using SevenZip;

namespace Arana.Release
{
   /// <summary>
   /// An application that compiles, merges and compresses Araña into a downloadable archive.
   /// </summary>
   internal static class ReleaseApplication
   {
#if DEBUG
      private const bool Debug = true;
#else
      private const bool Debug = false;
#endif


      /// <summary>
      /// The application's entry point.
      /// </summary>
      /// <param name="args">The args.</param>
      /// <returns><c>0</c> on success, <c>-1</c> on failure.</returns>
      public static int Main(string[] args)
      {
         int result = 0;

         if (Debug)
         {
            Console.WriteLine();
            Console.WriteLine("No merging or archiving done in debug mode.");
            Console.WriteLine();
            return result;
         }

         try
         {
            string solutionDirectory = args[0].Replace('"', ' ').Trim();
            PathHelper path = MergeOutput(solutionDirectory);
            CompressOutput(path);

            Console.WriteLine();
            Console.WriteLine("Successfully merged and compressed Araña.");
            Console.WriteLine();
         }
         catch (Exception ex)
         {
            Console.WriteLine();
            Console.WriteLine(ex);
            Console.WriteLine();
            result = -1;
         }

         return result;
      }


      /// <summary>
      /// Merges the output.
      /// </summary>
      /// <param name="solutionDirectory">The solution directory.</param>
      /// <returns></returns>
      private static PathHelper MergeOutput(string solutionDirectory)
      {
         PathHelper path = new PathHelper(solutionDirectory);

         ILMerge merge = new ILMerge
         {
            Log = true,
            TargetKind = ILMerge.Kind.Dll,
            KeyFile = path.KeyFile,
            AttributeFile = path.Arana,
            OutputFile = path.OutputFile,
         };

         merge.SetInputAssemblies(path.InputAssemblies);
         merge.Merge();

         path.InputDocumentation.CopyTo(path.OutputDocumentation, true);

         return path;
      }


      /// <summary>
      /// Compresses the output.
      /// </summary>
      /// <param name="path">The path.</param>
      private static void CompressOutput(PathHelper path)
      {
         SevenZipCompressor.SetLibraryPath(path.SevenZip);
         SevenZipCompressor compressor = new SevenZipCompressor(true);

         string sevenZipArchiveName = path.OutputArchiveName.ConcatWith(".7z");
         string zipArchiveName = path.OutputArchiveName.ConcatWith(".zip");

         // Compress a 7z file
         compressor.CompressDirectory(path.ReleaseOutputBin,
                                      sevenZipArchiveName,
                                      OutArchiveFormat.SevenZip);

         // Compress a Zip file.
         compressor.CompressDirectory(path.ReleaseOutputBin,
                                      zipArchiveName,
                                      OutArchiveFormat.Zip);
      }
   }
}