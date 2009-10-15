using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Arana.Core
{
   /// <summary>
   /// Provides a specialized and simplified wrapper around <see cref="HttpWebResponse" />.
   /// </summary>
   public class Response : IDisposable
   {
      private readonly HttpWebResponse webResponse;


      /// <summary>
      /// Initializes a new instance of the <see cref="Response"/> class.
      /// </summary>
      /// <param name="getHttpWebResponse">A function reference used to get the
      /// <see cref="HttpWebResponse"/>.</param>
      internal Response(Func<HttpWebResponse> getHttpWebResponse)
      {
         if (getHttpWebResponse == null)
         {
            throw new ArgumentNullException("getHttpWebResponse");
         }

         this.webResponse = getHttpWebResponse.Invoke();

         Status = this.webResponse.StatusCode;
         Location = this.webResponse.GetResponseHeader("Location")
                    ?? this.webResponse.GetResponseHeader("Content-Location");

         LastModified = this.webResponse.LastModified;
         ETag = this.webResponse.GetResponseHeader("ETag");

         if (!String.IsNullOrEmpty(this.webResponse.CharacterSet))
         {
            Encoding = Encoding.GetEncoding(this.webResponse.CharacterSet);
         }

         Headers = new Dictionary<string, string>(this.webResponse.Headers.Count);

         foreach (string key in this.webResponse.Headers.AllKeys)
         {
            string value = this.webResponse.Headers[key];
            Headers.Add(key, value);
         }

         Body = GetResponseBody();
      }


      /// <summary>
      /// Gets the response string.
      /// </summary>
      /// <returns></returns>
      public string Body { get; private set; }


      /// <summary>
      /// Gets the encoding of the response.
      /// </summary>
      /// <value>The encoding of the response.</value>
      public Encoding Encoding { get; private set; }

      /// <summary>
      /// Gets the value of the "ETag" HTTP header from the response.
      /// </summary>
      /// <value>The value of the "ETag" HTTP header from the response.</value>
      public string ETag { get; private set; }

      /// <summary>
      /// Gets the <see cref="Dictionary{TKey,TValue}"/> of headers from the response.
      /// </summary>
      /// <value>
      /// The <see cref="Dictionary{String,String}"/> of headers from the response..
      /// </value>
      public Dictionary<string, string> Headers { get; private set; }

      /// <summary>
      /// Gets the value of the "Location" or "Content-Location" HTTP headers from the response.
      /// </summary>
      /// <remarks>
      /// Returns null if neither of these headers are set. To know the URI of the current
      /// response, use <see cref="AranaEngine.Uri" />.
      /// </remarks>
      /// <value>The value of the "Location" or "Content-Location" HTTP header from the response.</value>
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
      /// Gets or the "Last-Modified" HTTP header from the response.
      /// </summary>
      /// <value>The "Last-Modified" HTTP header from the response.</value>
      internal DateTime LastModified { get; private set; }


      /// <summary>
      /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </returns>
      public override string ToString()
      {
         StringBuilder responseBuilder = new StringBuilder();

         responseBuilder.AppendLine(
            String.Format("HTTP/{0} {1} {2}",
                          this.webResponse.ProtocolVersion.
                             ToString(2),
                          (int) Status,
                          this.webResponse.StatusDescription));

         foreach (string key in Headers.Keys)
         {
            string value = Headers[key];
            responseBuilder.AppendLine(String.Format("{0}: {1}", key, value));
         }

         responseBuilder.AppendLine();
         responseBuilder.Append(Body);

         return responseBuilder.ToString();
      }


      /// <summary>
      /// Gets the content of the HTTP body as a <see cref="T:System.String"/>.
      /// </summary>
      /// <returns>The content of the HTTP body as a <see cref="T:System.String"/>.</returns>
      private string GetResponseBody()
      {
         using (Stream responseStream = this.webResponse.GetResponseStream())
         {
            if (!responseStream.CanRead)
            {
               return null;
            }

            string contentEncodig = this.webResponse.ContentEncoding;
            Stream stream = responseStream;

            if (contentEncodig.Contains("gzip"))
            {
               stream = new GZipStream(responseStream, CompressionMode.Decompress, true);
            }
            else if (contentEncodig.Contains("deflate"))
            {
               stream = new DeflateStream(responseStream, CompressionMode.Decompress, true);
            }

            using (stream)
            {
               StreamReader streamReader = Encoding != null
                                              ? new StreamReader(stream, Encoding)
                                              : new StreamReader(stream);
               using (streamReader)
               {
                  return streamReader.ReadToEnd();
               }
            }
         }
      }


      /// <summary>
      /// Disposes the underlying <see cref="HttpWebResponse" />.
      /// </summary>
      public void Dispose()
      {
         ((IDisposable) this.webResponse).Dispose();
      }
   }
}