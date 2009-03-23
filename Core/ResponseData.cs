using System;
using System.IO;
using System.Net;

using Arana.Core.Extensions;

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
      /// <param name="uri">The URI.</param>
      /// <returns></returns>
      private static Cookie ParseCookie(string cookieString, Uri uri)
      {
         if (String.IsNullOrEmpty(cookieString))
            return null;

         Cookie cookie = new Cookie();

         // Split the cookie into parts:
         string[] parts = cookieString.Split(';');

         if (parts.Length == 0)
         {
            if (cookieString.Contains("="))
            {
               string[] keyValue = cookieString.Split('=');
               cookie.Name = keyValue[0];
               cookie.Value = keyValue[0];
            }
         }
         else
         {
            foreach (string part in parts)
            {
               string[] keyValue = part.Split('=');

               if (keyValue.Length < 2)
                  continue;

               string key = keyValue[0].SafeTrim();
               string value = keyValue[1].SafeTrim();

               switch (key.ToLowerInvariant())
               {
                  case "expires":
                     // TODO: This is most probably going to fail in epic exceptions, so make it more sturdy
                     cookie.Expires = DateTime.ParseExact(value, "ddd, dd-MMM-yy HH:mm:ss zzz", null);
                     break;

                  case "domain":
                     cookie.Domain = value;
                     break;

                  case "path":
                     cookie.Path = value;
                     break;

                  case "secure":
                     cookie.Secure = true;
                     break;

                  case "httponly":
                     cookie.HttpOnly = true;
                     break;

                  default:
                     cookie.Name = key;
                     cookie.Value = value;
                     break;
               }
            }
         }

         if (String.IsNullOrEmpty(cookie.Domain))
         {
            if (uri == null)
               throw new ArgumentNullException(
                  "uri", "The URI can't be null when the cookie has no 'domain' parameter available.");

            cookie.Domain = uri.Authority;
         }

         return cookie;
      }
   }
}