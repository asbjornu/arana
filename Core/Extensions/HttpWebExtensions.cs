using System.IO;
using System.Net;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// A class that contains static extensions methods for the
   /// <see cref="HttpWebRequest" /> and <see cref="HttpWebResponse" />
   /// objects.
   /// </summary>
   internal static class HttpWebExtensions
   {
      /// <summary>
      /// Gets the response.
      /// </summary>
      /// <param name="request">The request.</param>
      /// <returns></returns>
      public static HttpWebResponse GetHttpWebResponse(this HttpWebRequest request)
      {
         try
         {
            return request.GetResponse() as HttpWebResponse;
         }
         catch (WebException ex)
         {
            return ex.Response as HttpWebResponse;
         }
      }

      /// <summary>
      /// Gets the response string.
      /// </summary>
      /// <param name="response">The response.</param>
      /// <returns></returns>
      public static string GetResponseString(this HttpWebResponse response)
      {
         using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
         {
            return streamReader.ReadToEnd();
         }
      }
   }
}