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

            PathHelper pathHelper = new PathHelper(solutionDirectory);
            MergeOutput(pathHelper);
            CompressOutput(pathHelper);

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
      /// <param name="pathHelper">The path helper.</param>
      /// <returns></returns>
      private static void MergeOutput(PathHelper pathHelper)
      {
         ILMerge merge = new ILMerge
         {
            Log = true,
            TargetKind = ILMerge.Kind.Dll,
            KeyFile = pathHelper.KeyFile,
            AttributeFile = pathHelper.Arana,
            OutputFile = pathHelper.OutputFile,
         };

         merge.SetInputAssemblies(pathHelper.InputAssemblies);
         merge.Merge();

         pathHelper.InputDocumentation.CopyTo(pathHelper.OutputDocumentation, true);
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