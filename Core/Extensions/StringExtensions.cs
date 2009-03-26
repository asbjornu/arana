using System;
using System.Web;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// A static class that contains extension methods for the <see cref="T:System.String" /> object.
   /// </summary>
   internal static class StringExtensions
   {
      /// <summary>
      /// Determines whether the given <see cref="T:System.String"/> is equal
      /// to any of the values in the specified <paramref name="values"/> array.
      /// </summary>
      /// <param name="s">The <see cref="T:System.String"/> to compare.</param>
      /// <param name="ignoreCase">if set to <c>true</c>, a case insensitive comparison is performed.</param>
      /// <param name="values">The values to compare with.</param>
      /// <returns>
      /// 	<c>true</c> if any of the specified <paramref name="values"/> are
      /// equal to the given <see cref="T:System.String"/>; otherwise, <c>false</c>.
      /// </returns>
      public static bool IsEqualTo(this string @s, bool ignoreCase, params string[] values)
      {
         if (String.IsNullOrEmpty(s) || (values == null) || (values.Length == 0))
            return false;

         foreach (string value in values)
            if (String.Compare(s, value, ignoreCase) == 0)
               return true;

         return false;
      }


      /// <summary>
      /// Trims the <see cref="T:System.String"/>. If it is null or empty,
      /// just returns it.
      /// </summary>
      /// <param name="s">The <see cref="T:System.String"/> to trim.</param>
      /// <returns>
      /// The trimmed <see cref="T:System.String"/>.
      /// </returns>
      public static string SafeTrim(this string @s)
      {
         return String.IsNullOrEmpty(s) ? s : s.Trim();
      }


      /// <summary>
      /// Converst the <see cref="T:System.String"/> to a <see cref="T:System.Uri"/>.
      /// If the URI string is relative, makes it relative to the specified <paramref name="baseUri"/>.
      /// </summary>
      /// <param name="uri">The <see cref="T:System.String"/> to convert.</param>
      /// <param name="baseUri">The base URI.</param>
      /// <returns>
      /// The converted <see cref="T:System.Uri"/>.
      /// </returns>
      public static Uri ToUri(this string uri, Uri baseUri)
      {
         try
         {
            // If the URI is absolute and for the HTTP(S) protocol, return it
            if (uri.StartsWith("http://", "https://"))
               return new Uri(uri);

            // If the URI is absolute and for any of the following protocols,
            // throw an exception
            if (uri.StartsWith("ftp://", "file://", "news://", "mailto:", "javascript:"))
               throw new InvalidUriException(uri, "Unsupported protocol");

            // If we've got here it means the URI must be relative, so throw an exception
            // if there's no base URI to resolve the relative URI to.
            if (baseUri == null)
               throw new InvalidUriException(uri, "The base URI can't be null when the URI is relative.");

            return new Uri(baseUri, uri);
         }
         catch (UriFormatException ex)
         {
            throw new InvalidUriException(uri, ex);
         }
      }


      /// <summary>
      /// URI encodes the given <see cref="T:System.String" />.
      /// </summary>
      /// <param name="s">The <see cref="T:System.String" /> to URI encode.</param>
      /// <returns>The URI encoded <see cref="T:System.String" />.</returns>
      public static string UriEncode(this string @s)
      {
         return String.IsNullOrEmpty(s) ? s : HttpUtility.UrlEncode(s);
      }


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
      private static bool StartsWith(this string @s, params string[] values)
      {
         if (String.IsNullOrEmpty(s) || (values == null) || (values.Length == 0))
            return false;

         foreach (string value in values)
            if (s.StartsWith(value))
               return true;

         return false;
      }
   }
}