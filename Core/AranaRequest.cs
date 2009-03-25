using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

using Arana.Core.Extensions;

namespace Arana.Core
{
   /// <summary>
   /// Provides a specialized and simplified wrapper around <see cref="HttpWebRequest" />.
   /// </summary>
   internal class AranaRequest
   {
      /// <summary>
      /// Contains the value for the HTTP "GET" method.
      /// </summary>
      internal const string HttpGet = "GET";

      /// <summary>
      /// Contains the Araña Engine's User Agent string as used when performing HTTP web requests.
      /// </summary>
      private static readonly string UserAgentString = GetUserAgentString();

      /// <summary>
      /// The underlying <see cref="HttpWebRequest" /> object used to perform the HTTP requests.
      /// </summary>
      private readonly HttpWebRequest baseRequest;

      /// <summary>
      /// Gets the HTTP method for the request.
      /// </summary>
      /// <value>The HTTP method for the request.</value>
      private readonly string method;

      /// <summary>
      /// Contains a reference to the previous request made. Null if this is the first request.
      /// </summary>
      private readonly AranaRequest previousRequest;

      /// <summary>
      /// The base URI of the web application we're testing.
      /// </summary>
      private Uri baseUri;

      /// <summary>
      /// Gets or sets the cookies that are associated with this request.
      /// </summary>
      private CookieCollection cookies;


      /// <summary>
      /// Initializes a new instance of the <see cref="AranaRequest"/> class.
      /// </summary>
      /// <param name="previousRequest">The previous request, used to preserve the base
      /// URI from different requests to the same domain, to be able to resolve relative
      /// URI's.</param>
      /// <param name="uri">The URI.</param>
      /// <param name="method">The HTTP method to use for the request.</param>
      /// <param name="requestValues">The request values.</param>
      internal AranaRequest(AranaRequest previousRequest,
                            string uri,
                            string method,
                            NameValueCollection requestValues)
      {
         this.method = (method ?? HttpGet).ToUpperInvariant();

         // Throw an exception if the HTTP method used is invalid.
         if (!this.method.IsEqualTo(false, HttpGet, "PUT", "POST", "DELETE", "HEAD"))
            throw new InvalidOperationException(
               String.Format("The method '{0}' is invalid.", this.method));

         if ((previousRequest != null) && (previousRequest.cookies != null) && (previousRequest.cookies.Count > 0))
         {
            this.cookies = this.cookies ?? new CookieCollection();
            this.cookies.Add(previousRequest.cookies);
         }

         this.previousRequest = previousRequest;
         this.baseRequest = CreateRequest(uri, requestValues);
      }


      /// <summary>
      /// Gets the Uniform Resource Identifier (<see cref="T:System.Uri" />) of the request.
      /// </summary>
      /// <value>The Uniform Resource Identifier (<see cref="T:System.Uri" />) of the request.</value>
      internal Uri Uri
      {
         get { return this.baseRequest.RequestUri; }
      }


      /// <summary>
      /// Sets the cookie.
      /// </summary>
      /// <param name="responseData">The response data.</param>
      public void SetCookie(ResponseData responseData)
      {
         if (responseData.Cookie == null)
            return;

         this.cookies = this.cookies ?? new CookieCollection();
         this.cookies.Add(responseData.Cookie);
      }


      /// <summary>
      /// Gets the <see cref="AranaResponse"/> for the current <see cref="AranaRequest"/>.
      /// </summary>
      /// <returns>
      /// The <see cref="AranaResponse"/> for the current <see cref="AranaRequest"/>.
      /// </returns>
      internal AranaResponse GetResponse()
      {
         return new AranaResponse(this, () =>
         {
            HttpWebResponse response;

            try
            {
               this.baseRequest.AllowAutoRedirect = false;
               response = this.baseRequest.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
               response = ex.Response as HttpWebResponse;
            }

            if (response == null)
               throw new InvalidOperationException(
                  String.Format("The URI '{0}' did not make much sense, sorry.",
                                Uri));

            return response;
         });
      }


      /// <summary>
      /// Gets the user agent string.
      /// </summary>
      /// <returns>The user agent string.</returns>
      private static string GetUserAgentString()
      {
         Assembly assembly = Assembly.GetAssembly(typeof(AranaRequest));

         return String.Format("Arana/{0} ({1} {2}; N; .NET CLR {3}; {4})",
                              assembly.GetName().Version,
                              Environment.OSVersion.Platform,
                              Environment.OSVersion.VersionString,
                              Environment.Version,
                              assembly.GetName().ProcessorArchitecture);
      }


      /// <summary>
      /// Creates the request.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="requestValues">The request values.</param>
      /// <returns>
      /// The created <see cref="HttpWebRequest"/>.
      /// </returns>
      private HttpWebRequest CreateRequest(string uri,
                                           NameValueCollection requestValues)
      {
         Uri createdUri = uri.ToUri((this.previousRequest == null) ? null : this.previousRequest.baseUri);
         this.baseUri = new Uri(createdUri.GetLeftPart(UriPartial.Authority));
         HttpWebRequest request = HttpWebRequest.Create(createdUri) as HttpWebRequest;

         if (request == null)
            throw new InvalidUriException(uri, "Couldn't create an HTTP request for the given URI.");

         // Set and default the HTTP method
         request.Method = this.method;

         // Add the given values to the request if they are 
         if ((requestValues != null) && (requestValues.Count > 0))
         {
            string requestString = requestValues.GetRequestString((this.method == HttpGet));

            // If the HTTP method is GET, create a new request with the values in the query string
            if (this.method == HttpGet)
            {
               uri = String.Concat(uri + requestString);
               return CreateRequest(uri, null);
            }

            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream(), Encoding.UTF8))
            {
               streamWriter.Write(requestString);
            }
         }

         SetRequestProperties(request);

         return request;
      }


      /// <summary>
      /// Sets the various properties on the given <paramref name="request"/>.
      /// </summary>
      /// <param name="request">The request on which to set the properties.</param>
      private void SetRequestProperties(HttpWebRequest request)
      {
         request.UserAgent = UserAgentString;
         request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

         // TODO: Set more accepted charsets and handle decoding of them
         request.Headers.Add("Accept-Charset", "utf-8");

         // TODO: Set Accept-Encoding and handle decoding

         if (request.Method != HttpGet)
            request.ContentType = "application/x-www-form-urlencoded";

         // If there's any cookies to add to the request, do it
         if ((this.cookies == null) || (this.cookies.Count <= 0))
            return;

         request.CookieContainer = new CookieContainer(this.cookies.Count);
         request.CookieContainer.Add(this.cookies);
      }
   }
}