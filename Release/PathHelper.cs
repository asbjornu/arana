using System;
using System.IO;
using System.Reflection;
using Arana.Core;

namespace Arana.Release
{
   /// <summary>
   /// Helper class for getting the different paths for the release build process.
   /// </summary>
   internal class PathHelper
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="PathHelper"/> class.
      /// </summary>
      /// <param name="solutionDirectory">The solution directory.</param>
      public PathHelper(string solutionDirectory)
      {
         string coreAssemblyName, releaseBin, version;
         GetAssemblyInfo(out coreAssemblyName, out releaseBin, out version);

         Solution = solutionDirectory;

         string thirdParty = Solution.CombineWith("ThirdParty");

         Core = Solution.CombineWith("Core");
         KeyFile = Core.CombineWith(coreAssemblyName.ConcatWith(".snk"));

         string coreReleaseBin = Core.CombineWith("bin").CombineWith("Release");
         ReleaseOutputBin = releaseBin.CombineWith("Output");

         Arana = coreReleaseBin.CombineWith(coreAssemblyName.ConcatWith(".dll"));
         Fizzler = thirdParty.CombineWith("Fizzler.Parser.dll");
         HtmlAgilityPack = thirdParty.CombineWith("HtmlAgilityPack.dll");
         InputDocumentation =
               coreReleaseBin.CombineWith(coreAssemblyName.ConcatWith(".xml"));
         OutputDocumentation = ReleaseOutputBin.CombineWith("Arana.xml");
         OutputFile = ReleaseOutputBin.CombineWith("Arana.dll");
         SevenZip = thirdParty.CombineWith("7z.dll");
         Archive = releaseBin.CombineWith("Archive");
         OutputArchiveName = Archive.CombineWith(String.Format("Arana-{0}", version));

         ValidateExistance();
      }


      /// <summary>
      /// Gets or sets the arana.
      /// </summary>
      /// <value>The arana.</value>
      public string Arana { get; private set; }

      /// <summary>
      /// Gets the merge assemblies.
      /// </summary>
      /// <value>The merge assemblies.</value>
      public string[] InputAssemblies
      {
         get { return new[] { Arana, Fizzler, HtmlAgilityPack }; }
      }

      /// <summary>
      /// Gets or sets the documentation.
      /// </summary>
      /// <value>The documentation.</value>
      public string InputDocumentation { get; private set; }

      /// <summary>
      /// Gets or sets the key file.
      /// </summary>
      /// <value>The key file.</value>
      public string KeyFile { get; private set; }

      /// <summary>
      /// Gets or sets the name of the output archive.
      /// </summary>
      /// <value>The name of the output archive.</value>
      public string OutputArchiveName { get; private set; }

      /// <summary>
      /// Gets or sets the output documentation.
      /// </summary>
      /// <value>The output documentation.</value>
      public string OutputDocumentation { get; private set; }

      /// <summary>
      /// Gets or sets the output.
      /// </summary>
      /// <value>The output.</value>
      public string OutputFile { get; private set; }

      /// <summary>
      /// Gets or sets the release output bin.
      /// </summary>
      /// <value>The release output bin.</value>
      public string ReleaseOutputBin { get; private set; }

      /// <summary>
      /// Gets or sets the seven zip.
      /// </summary>
      /// <value>The seven zip.</value>
      public string SevenZip { get; private set; }

      /// <summary>
      /// Gets or sets the archive.
      /// </summary>
      /// <value>The archive.</value>
      private string Archive { get; set; }

      /// <summary>
      /// Gets or sets the core.
      /// </summary>
      /// <value>The core.</value>
      private string Core { get; set; }

      /// <summary>
      /// Gets or sets the fizzler.
      /// </summary>
      /// <value>The fizzler.</value>
      private string Fizzler { get; set; }

      /// <summary>
      /// Gets or sets the HTML agility pack.
      /// </summary>
      /// <value>The HTML agility pack.</value>
      private string HtmlAgilityPack { get; set; }

      /// <summary>
      /// Gets or sets the solution.
      /// </summary>
      /// <value>The solution.</value>
      private string Solution { get; set; }


      /// <summary>
      /// Gets the assembly info.
      /// </summary>
      /// <param name="coreAssemblyName">Name of the core assembly.</param>
      /// <param name="releaseBin">The path to the 'Release' application's 'bin' folder..</param>
      /// <param name="version">The version.</param>
      private static void GetAssemblyInfo(out string coreAssemblyName,
                                          out string releaseBin,
                                          out string version)
      {
         Assembly coreAssembly = Assembly.GetAssembly(typeof (AranaEngine));
         Assembly releaseAssembly = Assembly.GetAssembly(typeof (PathHelper));

         if ((releaseAssembly == null) || String.IsNullOrEmpty(releaseAssembly.Location))
            throw new InvalidOperationException(
                  "The 'Release' Assembly null or has no location.");

         FileInfo releaseAssemblyInfo = new FileInfo(releaseAssembly.Location);

         if ((releaseAssemblyInfo.Directory == null) ||
             (releaseAssemblyInfo.Directory.Parent == null))
            throw new InvalidOperationException(
                  "The 'Release' Assembly has an invalid directory structure.");

         AssemblyName coreName = coreAssembly.GetName();

         coreAssemblyName = coreName.Name;
         releaseBin = releaseAssemblyInfo.Directory.Parent.FullName;
         version = coreName.Version.ToString(2);
      }


      /// <summary>
      /// Validates the existance of all the paths.
      /// </summary>
      private void ValidateExistance()
      {
         if (!Directory.Exists(Solution))
            throw new FileNotFoundException("Solution directory not found.", Solution);

         if (!Directory.Exists(Core))
            throw new FileNotFoundException("Core directory not found.", Core);

         if (!File.Exists(Arana))
            throw new FileNotFoundException("Arana assembly not found.", Arana);

         if (!File.Exists(KeyFile))
            throw new FileNotFoundException("Key file not found.", KeyFile);

         if (!File.Exists(InputDocumentation))
            throw new FileNotFoundException("Input documentation file not found.",
                                            InputDocumentation);

         if (!File.Exists(SevenZip))
            throw new FileNotFoundException("7-Zip assembly not found.", SevenZip);

         if (!File.Exists(Fizzler))
            throw new FileNotFoundException("Fizzler assembly not found.", Fizzler);

         if (!File.Exists(HtmlAgilityPack))
            throw new FileNotFoundException("HtmlAgilityPack assembly not found.",
                                            HtmlAgilityPack);

         // Create the "Output" directory if it doesn't exist
         if (!Directory.Exists(ReleaseOutputBin))
            Directory.CreateDirectory(ReleaseOutputBin);

         // Create the "Archive" directory if it doesn't exist
         if (!Directory.Exists(Archive))
            Directory.CreateDirectory(Archive);
      }
   }
}