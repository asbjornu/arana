using System;
using System.Globalization;
using System.Web;

using HtmlAgilityPack;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// A static class that contains extension methods for the <see cref="T:System.String" /> object.
   /// </summary>
   public static class StringExtensions
   {
      /// <summary>
      /// Converts the <see cref="T:System.String"/> to a <see cref="T:System.DateTime"/>.
      /// If it can't be successfully converted, returns <paramref name="defaultValue"/>.
      /// </summary>
      /// <param name="instance">The instance.</param>
      /// <param name="defaultValue">The default value to return if parsing fails.</param>
      /// <returns>
      /// The converted <see cref="T:System.DateTime"/> value or <paramref name="defaultValue"/>
      /// if the conversion failed.
      /// </returns>
      public static DateTime ToDateTime(this string @instance, DateTime defaultValue)
      {
         DateTimeFormatInfo info = new DateTimeFormatInfo();

         string[] formats = new[]
         {
            "ddd, dd-MMM-yy HH:mm:ss zzz",
            "ddd, dd-MMM-yyyy HH:mm:ss zzz",
            "ddd, dd-MMM-yyyy HH':'mm':'ss 'GMT'",
            info.RFC1123Pattern
         };

         DateTime parsedDate;
         bool parsed = DateTime.TryParseExact(instance,
                                              formats,
                                              info,
                                              DateTimeStyles.AdjustToUniversal,
                                              out parsedDate);

         if (!parsed)
         {
            Console.WriteLine("Couldn't parse '{0}' into a valid date.", instance);
         }

         return parsed ? parsedDate : defaultValue;
      }


      /// <summary>
      /// Determines whether the given <see cref="T:System.String"/> is equal
      /// to any of the values in the specified <paramref name="values"/> array,
      /// in a case insensitive way.
      /// </summary>
      /// <param name="instance">The <see cref="T:System.String"/> to compare.</param>
      /// <param name="values">The values to compare with.</param>
      /// <returns>
      /// 	<c>true</c> if any of the specified <paramref name="values"/> are
      /// equal to the given <see cref="T:System.String"/>; otherwise, <c>false</c>.
      /// </returns>
      internal static bool IsEqualTo(this string @instance, params string[] values)
      {
         return IsEqualTo(instance, true, values);
      }


      /// <summary>
      /// Determines whether the given <see cref="T:System.String"/> is equal
      /// to any of the values in the specified <paramref name="values"/> array.
      /// </summary>
      /// <param name="instance">The <see cref="T:System.String"/> to compare.</param>
      /// <param name="ignoreCase">if set to <c>true</c>, a case insensitive comparison is performed.</param>
      /// <param name="values">The values to compare with.</param>
      /// <returns>
      /// 	<c>true</c> if any of the specified <paramref name="values"/> are
      /// equal to the given <see cref="T:System.String"/>; otherwise, <c>false</c>.
      /// </returns>
      internal static bool IsEqualTo(this string @instance,
                                     bool ignoreCase,
                                     params string[] values)
      {
         if (String.IsNullOrEmpty(instance) || (values == null) || (values.Length == 0))
         {
            return false;
         }

         foreach (string value in values)
         {
            if (String.Compare(instance, value, ignoreCase) == 0)
            {
               return true;
            }
         }

         return false;
      }


      /// <summary>
      /// Returns null if the <see cref="T:System.String" /> is null or equals
      /// to <see cref="string.Empty" />. Useful with the ?? operator. Example:
      /// <example>
      /// <code>
      /// string value = value.NullWhenEmpty() ?? "Some default";
      /// </code>
      /// </example>
      /// </summary>
      /// <param name="instance">The <see cref="T:System.String" />.</param>
      /// <returns>Null if <paramref name="instance"/> is null or equals to <see cref="string.Empty" />.</returns>
      internal static string NullWhenEmpty(this string @instance)
      {
         return String.IsNullOrEmpty(instance) ? null : instance;
      }


      /// <summary>
      /// Trims the <see cref="T:System.String"/>. If it is null or empty,
      /// just returns it.
      /// </summary>
      /// <param name="instance">The <see cref="T:System.String"/> to trim.</param>
      /// <returns>
      /// The trimmed <see cref="T:System.String"/>.
      /// </returns>
      internal static string SafeTrim(this string @instance)
      {
         return String.IsNullOrEmpty(instance) ? instance : instance.Trim();
      }


      /// <summary>
      /// Gets an <see cref="HtmlDocument"/> object from the provided <paramref name="html"/>.
      /// </summary>
      /// <param name="html">The HTML.</param>
      /// <returns>
      /// An <see cref="HtmlDocument"/> object from the provided <paramref name="html"/>.
      /// </returns>
      internal static HtmlDocument ToHtmlDocument(this string @html)
      {
         if (String.IsNullOrEmpty(html))
         {
            throw new ArgumentNullException("html");
         }

         HtmlDocument document = new HtmlDocument();
         document.LoadHtml(html);
         return document;
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
      internal static Uri ToUri(this string uri, Uri baseUri)
      {
         try
         {
            // If the URI is absolute and for the HTTP(S) protocol, return it
            if (uri.StartsWith("http://", "https://"))
            {
               return new Uri(uri);
            }

            // If the URI is absolute and for any of the following protocols,
            // throw an exception
            if (uri.StartsWith("ftp://", "file://", "news://", "mailto:", "javascript:"))
            {
               throw new InvalidUriException(uri, "Unsupported protocol");
            }

            // If we've got here it means the URI must be relative, so throw an exception
            // if there's no base URI to resolve the relative URI to.
            if (baseUri == null)
            {
               throw new InvalidUriException(uri,
                                             "The base URI can't be null when the URI is relative.");
            }

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
      /// <param name="instance">The <see cref="T:System.String" /> to URI encode.</param>
      /// <returns>The URI encoded <see cref="T:System.String" />.</returns>
      internal static string UriEncode(this string @instance)
      {
         return String.IsNullOrEmpty(instance)
                   ? instance
                   : HttpUtility.UrlEncode(instance);
      }


      /// <summary>
      /// Determines whether the beginning of <paramref name="instance"/>
      /// matches any of the specified <see cref="T:System.String"/>s
      /// in <paramref name="values"/>.
      /// </summary>
      /// <param name="instance">The <see cref="T:System.String"/> to match.</param>
      /// <param name="values">The values to match against.</param>
      /// <returns>
      /// 	<c>true</c> if the beginning of <paramref name="instance"/>
      /// matches any of the specified <see cref="T:System.String"/>s
      /// in <paramref name="values"/>; otherwise <c>false</c>.
      /// </returns>
      private static bool StartsWith(this string @instance, params string[] values)
      {
         if (String.IsNullOrEmpty(instance) || (values == null) || (values.Length == 0))
         {
            return false;
         }

         foreach (string value in values)
         {
            if (instance.StartsWith(value))
            {
               return true;
            }
         }

         return false;
      }
   }
}