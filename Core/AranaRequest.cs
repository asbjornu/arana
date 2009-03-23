﻿using System;
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
      private const string HttpGet = "GET";

      /// <summary>
      /// Contains the Araña Engine's User Agent string as used when performing HTTP web requests.
      /// </summary>
      private static readonly string UserAgentString = GetUserAgentString();

      private readonly HttpWebRequest baseRequest;
      private readonly AranaRequest previousRequest;
      private Uri baseUri;


      /// <summary>
      /// Initializes a new instance of the <see cref="AranaRequest"/> class.
      /// </summary>
      /// <param name="previousRequest">The previous request.</param>
      /// <param name="uri">The URI.</param>
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="requestValues">The request values.</param>
      internal AranaRequest(AranaRequest previousRequest,
                            string uri,
                            string httpMethod,
                            NameValueCollection requestValues)
      {
         this.previousRequest = previousRequest;
         this.baseRequest = CreateRequest(uri, httpMethod, requestValues);
      }


      /// <summary>
      /// Gets or sets the cookies that are associated with this request.
      /// </summary>
      /// <value>
      /// A <see cref="CookieCollection"/> that contains the cookies that
      /// are associated with this request.
      /// </value>
      internal CookieCollection Cookies { get; set; }

      /// <summary>
      /// Gets the Uniform Resource Identifier (<see cref="T:System.Uri" />) of the request.
      /// </summary>
      /// <value>The Uniform Resource Identifier (<see cref="T:System.Uri" />) of the request..</value>
      internal Uri RequestUri
      {
         get { return this.baseRequest.RequestUri; }
      }


      /// <summary>
      /// Gets the response.
      /// </summary>
      /// <param name="followRedirect">
      /// <c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.
      /// </param>
      /// <returns>The response.</returns>
      internal AranaResponse GetResponse(bool followRedirect)
      {
         return new AranaResponse(this, () =>
         {
            HttpWebResponse response;

            try
            {
               this.baseRequest.AllowAutoRedirect = followRedirect;
               response = this.baseRequest.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
               response = ex.Response as HttpWebResponse;
            }

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
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="requestValues">The request values.</param>
      /// <returns>
      /// The created <see cref="HttpWebRequest"/>.
      /// </returns>
      private HttpWebRequest CreateRequest(string uri,
                                           string httpMethod,
                                           NameValueCollection requestValues)
      {
         Uri createdUri = uri.ToUri((this.previousRequest == null) ? null : this.previousRequest.baseUri);
         this.baseUri = new Uri(createdUri.GetLeftPart(UriPartial.Authority));
         HttpWebRequest request = HttpWebRequest.Create(createdUri) as HttpWebRequest;

         if (request == null)
            throw new InvalidUriException(uri, "Couldn't create an HTTP request for the given URI.");

         // Set and default the HTTP method
         request.Method = httpMethod = (httpMethod ?? HttpGet).ToUpperInvariant();

         // Add the given values to the request if they are 
         if ((requestValues != null) && (requestValues.Count > 0))
         {
            string requestString = requestValues.GetRequestString((httpMethod == HttpGet));

            // If the HTTP method is GET, create a new request with the values in the query string
            if (httpMethod == HttpGet)
            {
               uri = String.Concat(uri + requestString);
               return CreateRequest(uri, HttpGet, null);
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
         if ((Cookies == null) || (Cookies.Count <= 0))
            return;

         request.CookieContainer = new CookieContainer(Cookies.Count);
         request.CookieContainer.Add(Cookies);
      }
   }
}