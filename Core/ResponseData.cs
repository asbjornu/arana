using System;
using System.IO;
using System.Net;
using Arana.Core.Extensions;

// ReSharper disable MemberCanBePrivate.Global

namespace Arana.Core
{
   /// <summary>
   /// Contains data from a given <see cref="AranaResponse" />.
   /// </summary>
   public class ResponseData
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="ResponseData"/> class.
      /// </summary>
      /// <param name="response">The response.</param>
      /// <param name="requestUri">The request URI.</param>
      public ResponseData(HttpWebResponse response, Uri requestUri)
      {
         Status = response.StatusCode;
         Body = GetResponseBody(response);
         Location = response.GetResponseHeader("Location")
                    ?? response.GetResponseHeader("Content-Location");

         string cookie = response.GetResponseHeader("Set-Cookie");
         Cookie = ParseCookie(cookie, requestUri);
      }


      /// <summary>
      /// Gets the response string.
      /// </summary>
      /// <returns></returns>
      public string Body { get; private set; }

      /// <summary>
      /// Gets the "Set-Cookie" HTTP header from the resposne.
      /// </summary>
      /// <value>The "Set-Cookie" HTTP header from the resposne..</value>
      public Cookie Cookie { get; private set; }

      /// <summary>
      /// Gets the "Location" HTTP header from the response.
      /// </summary>
      /// <value>The "Location" HTTP header from the response..</value>
      public string Location { get; private set; }

      /// <summary>
      /// Gets the status of the response.
      /// </summary>
      /// <value>One of the <see cref="T:System.Net.HttpStatusCode"/> values.</value>
      public HttpStatusCode Status { get; private set; }

      /// <summary>
      /// Gets the base of the HTTP <see cref="Status" /> Code.
      /// 100, 200, 300, 400 or 500 for respective HTTP status codes.
      /// </summary>
      /// <value>The base status code of the given specific status code.</value>
      /// <example>
      ///   For a given <see cref="HttpStatusCode" /> of 304 (Not Modified),
      ///   the base is found like this:
      /// 
      ///   <code>
      ///          statusCode = 304
      ///           304 / 100 = 3.04
      ///    Math.Floor(3.04) = 3.00
      ///          3.00 * 100 = 300
      ///   </code>
      /// </example>
      public int StatusBase
      {
         get { return (int) Math.Floor((double) Status / 100) * 100; }
      }


      /// <summary>
      /// Gets the response string.
      /// </summary>
      /// <param name="response">The response.</param>
      /// <returns></returns>
      private static string GetResponseBody(WebResponse response)
      {
         using (Stream stream = response.GetResponseStream())
         {
            if (!stream.CanRead)
               return null;

            using (StreamReader streamReader = new StreamReader(stream))
            {
               return streamReader.ReadToEnd();
            }
         }
      }


      /// <summary>
      /// Parses the cookie.
      /// </summary>
      /// <param name="cookieString">The cookie string.</param>
      /// <param name="uri">The URI to use for the domain part of the cookie
      /// if the cookie itself doesn't include the information.</param>
      /// <returns>
      /// The parsed and constructed <see cref="Cookie"/>.
      /// </returns>
      private static Cookie ParseCookie(string cookieString, Uri uri)
      {
         if (String.IsNullOrEmpty(cookieString))
            return null;

         Cookie cookie = new Cookie();

         // Split the cookie into parts:
         string[] cookieParts = cookieString.Split(';');

         if (cookieParts.Length == 0)
            SplitAndSet(cookie, cookieString);
         else
            foreach (string cookiePart in cookieParts)
               SplitAndSet(cookie, cookiePart);

         if (String.IsNullOrEmpty(cookie.Domain))
         {
            // If both the domain part on the cookie and the 'uri' argument is null, throw an
            // exception as the Domain property of the cookie is required.
            if (uri == null)
               throw new ArgumentNullException(
                  "uri",
                  "The URI can't be null when the cookie has no 'domain' parameter available.");

            cookie.Domain = uri.Authority;
         }

         return cookie;
      }


      /// <summary>
      /// Splits the and sets the values on the <paramref name="cookie"/>.
      /// </summary>
      /// <param name="cookie">The cookie.</param>
      /// <param name="cookiePart">The cookie part.</param>
      private static void SplitAndSet(Cookie cookie, string cookiePart)
      {
         // Do nothing if the part doesn't contain an equals sign.
         if (!cookiePart.Contains("="))
            return;

         string[] keyValue = cookiePart.Split('=');

         // Do nothing the key/value if its length is less than 2.
         if (keyValue.Length < 2)
            return;

         // Set the key and value on the cookie
         cookie.Set(keyValue[0], keyValue[1]);
      }
   }
}