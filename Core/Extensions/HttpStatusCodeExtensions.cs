using System;
using System.Net;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// Contains extension methods for the <see cref="HttpStatusCode" /> enumeration.
   /// </summary>
   internal static class HttpStatusCodeExtensions
   {
      /// <summary>
      /// Returns the base of the HTTP Status Code.
      /// 100, 200, 300, 400 or 500 for respective HTTP status codes.
      /// </summary>
      /// <param name="statusCode">The HTTP Status Code on which to return the base.</param>
      /// <returns>
      /// The base status code of the given specific status code.
      /// </returns>
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
      public static int GetBase(this HttpStatusCode statusCode)
      {
         return (int)Math.Floor((double)statusCode / 100) * 100;
      }
   }
}