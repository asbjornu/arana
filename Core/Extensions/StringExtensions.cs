using System;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// A static class that contains extension methods for the <see cref="T:System.String" /> object.
   /// </summary>
   public static class StringExtensions
   {
      /// <summary>
      /// Determines whether the beginning of <paramref name="s"/>
      /// matches any of the specified <see cref="T:System.String"/>s
      /// in <paramref name="values"/>.
      /// </summary>
      /// <param name="s">The <see cref="T:System.String"/> to match.</param>
      /// <param name="values">The values to match against.</param>
      /// <returns>
      /// 	<c>true</c> if the beginning of <paramref name="s"/>
      /// matches any of the specified <see cref="T:System.String"/>s
      /// in <paramref name="values"/>; otherwise <c>false</c>.
      /// </returns>
      public static bool StartsWith(this string @s, params string[] values)
      {
         if (String.IsNullOrEmpty(s) || values == null || values.Length == 0)
            return false;

         foreach (string value in values)
            if (s.StartsWith(value))
               return true;

         return false;
      }


      /// <summary>
      /// Converst the <see cref="T:System.String"/> to a <see cref="T:System.Uri"/>.
      /// If the URI string is relative, makes it relative to the specified <paramref name="baseUri"/>.
      /// </summary>
      /// <param name="s">The <see cref="T:System.String"/> to convert.</param>
      /// <param name="baseUri">The base URI.</param>
      /// <returns>
      /// The converted <see cref="T:System.Uri"/>.
      /// </returns>
      public static Uri ToUri(this string s, Uri baseUri)
      {
         try
         {
            if (s.StartsWith("http://") || s.StartsWith("https://"))
               return new Uri(s);

            return new Uri(baseUri, s);
         }
         catch (UriFormatException ex)
         {
            throw new InvalidOperationException(
               String.Format("Can't create an URI from '{0}'.", s), ex);
         }
      }
   }
}