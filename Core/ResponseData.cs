using System.IO;
using System.Net;

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
      public ResponseData(HttpWebResponse response)
      {
         Status = response.StatusCode;
         Body = GetResponseBody(response);
         Location = response.GetResponseHeader("Location")
                    ?? response.GetResponseHeader("Content-Location");
      }


      /// <summary>
      /// Gets the response string.
      /// </summary>
      /// <returns></returns>
      public string Body { get; private set; }

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
   }
}