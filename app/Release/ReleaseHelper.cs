using ILMerging;

using SevenZip;

namespace Arana.Release
{
   /// <summary>
   /// An application that compiles, merges and compresses Araña into a downloadable archive.
   /// </summary>
   public class ReleaseHelper
   {
      private readonly PathHelper pathHelper;


      /// <summary>
      /// Initializes a new instance of the <see cref="ReleaseApplication"/> class.
      /// </summary>
      /// <param name="solutionDirectory">The solution directory.</param>
      public ReleaseHelper(string solutionDirectory)
      {
         this.pathHelper = new PathHelper(solutionDirectory);
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ReleaseApplication"/> class.
      /// </summary>
      public ReleaseHelper()
      {
         this.pathHelper = new PathHelper();
      }

      /// <summary>
      /// Merges the output into one assembly and compresses it into one 7-Zip and one regular Zip archive.
      /// </summary>
      public void Release()
      {
         MergeOutput(this.pathHelper);
         CompressOutput(this.pathHelper);
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
   }
}