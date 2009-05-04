using System;
using System.Net;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// Contains extension methods for the <see cref="Cookie" /> class.
   /// </summary>
   internal static class CookieExtensions
   {
      /// <summary>
      /// Sets the specified <paramref name="value"/> for the given <paramref name="key"/>
      /// on the <paramref name="cookie"/>.
      /// </summary>
      /// <param name="cookie">The cookie to set the <paramref name="value"/> on.</param>
      /// <param name="key">The key on which to set the <paramref name="value"/>.</param>
      /// <param name="value">The value to set.</param>
      public static void Set(this Cookie cookie, string key, string value)
      {
         key = key.SafeTrim();
         value = value.SafeTrim();

         switch (key.ToLowerInvariant())
         {
            case "expires":
               // TODO: This is most probably going to fail in epic exceptions, so make it more sturdy
               cookie.Expires = DateTime.ParseExact(value,
                                                    "ddd, dd-MMM-yy HH:mm:ss zzz",
                                                    null);
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
}