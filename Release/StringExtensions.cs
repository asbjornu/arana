using System;
using System.IO;

namespace Arana.Release
{
   /// <summary>
   /// Defines extension methods on <see cref="T:System.String" />.
   /// </summary>
   internal static class StringExtensions
   {
      /// <summary>
      /// Combines the file system path of <paramref name="path1"/> with <paramref name="path2"/>.
      /// </summary>
      /// <param name="path1">The first path.</param>
      /// <param name="path2">The second path.</param>
      /// <returns>The combined path</returns>
      public static string CombineWith(this string @path1, string path2)
      {
         if (String.IsNullOrEmpty(path1))
            throw new ArgumentNullException("path1");

         return Path.Combine(path1, path2);
      }


      /// <summary>
      /// Concatenates <paramref name="string1"/> with <paramref name="string2"/>.
      /// </summary>
      /// <param name="string1">The first <see cref="T:System.String" />.</param>
      /// <param name="string2">The second <see cref="T:System.String" />.</param>
      /// <returns>The concatenated <see cref="T:System.String" />.</returns>
      public static string ConcatWith(this string @string1, string string2)
      {
         if (String.IsNullOrEmpty(string1))
            throw new ArgumentNullException("string1");

         return String.Concat(string1, string2);
      }


      /// <summary>
      /// Copies the file placed at <paramref name="sourcePath"/> to <paramref name="destinationPath"/>.
      /// Overwrites <paramref name="destinationPath"/> if <paramref name="overwrite"/> is set to <c>true</c>.
      /// </summary>
      /// <param name="sourcePath">The source path.</param>
      /// <param name="destinationPath">The destination path.</param>
      /// <param name="overwrite">if set to <c>true</c>, overwrites any existing file at <paramref name="destinationPath"/>.</param>
      public static void CopyTo(this string @sourcePath, string destinationPath, bool overwrite)
      {
         if (String.IsNullOrEmpty(sourcePath))
            throw new ArgumentNullException("sourcePath");

         File.Copy(sourcePath, destinationPath, overwrite);
      }
   }
}