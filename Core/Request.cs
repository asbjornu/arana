using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text;

using Arana.Core.Extensions;

namespace Arana.Core
{
   /// <summary>
   /// Provides a specialized and simplified wrapper around <see cref="HttpWebRequest" />.
   /// </summary>
   internal class Request
   {
      /// <summary>
      /// "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
      /// </summary>
      private static readonly AcceptDictionary Accept = new AcceptDictionary
      {
         "text/html",
         "application/xhtml+xml",
         { "application/xml", 0.9 },
         { "*/*", 0.8 },
      };

      /// <summary>
      /// The underlying <see cref="HttpWebRequest" /> object used to perform the HTTP requests.
      /// </summary>
      private readonly HttpWebRequest currentWebRequest;

      /// <summary>
      /// The engine used to handle the request.
      /// </summary>
      private readonly AranaEngine engine;

      /// <summary>
      /// Gets the HTTP method for the request.
      /// </summary>
      /// <value>The HTTP method for the request.</value>
      private readonly string method;

      /// <summary>
      /// The credentials to use on requests.
      /// </summary>
      private readonly ICredentials requestCredentials;

      /// <summary>
      /// The proxy server to use on requests.
      /// </summary>
      private readonly IWebProxy requestProxy;

      /// <summary>
      /// Contains the Araña Engine's User Agent string as used when performing HTTP web requests.
      /// </summary>
      private static readonly string UserAgentString = GetUserAgentString();

      /// <summary>
      /// The base URI of the web application we're testing.
      /// </summary>
      private Uri baseUri;

      /// <summary>
      /// The string to put into the request
      /// </summary>
      private string requestString;


      /// <summary>
      /// Initializes a new instance of the <see cref="Request"/> class.
      /// </summary>
      /// <param name="engine">The engine.</param>
      /// <param name="uri">The URI.</param>
      /// <param name="method">The HTTP method to use for the request.</param>
      /// <param name="credentials">The credentials.</param>
      /// <param name="proxy">The proxy.</param>
      /// <param name="requestValues">The request values.</param>
      public Request(AranaEngine engine,
                     string uri,
                     string method,
                     ICredentials credentials,
                     IWebProxy proxy,
                     RequestDictionary requestValues)
      {
         if (engine == null)
         {
            throw new ArgumentNullException("engine");
         }

         this.engine = engine;
         this.requestCredentials = GetCredentials(credentials);
         this.requestProxy = GetProxy(proxy);
         this.method = (method ?? HttpMethod.Get).ToUpperInvariant();

         // Throw an exception if the HTTP method used is invalid.
         if (!this.method.IsEqualTo(false, HttpMethod.All))
         {
            throw new NotSupportedException(
               String.Format("The method '{0}' is invalid.", this.method));
         }

         this.currentWebRequest = CreateRequest(uri, requestValues);
      }


      /// <summary>
      /// Gets the "If-Modified-Since" HTTP header from the request.
      /// </summary>
      /// <value>The "If-Modified-Since" HTTP header from the request.</value>
      public DateTime IfModifiedSince
      {
         get { return (Response != null) ? Response.LastModified : default(DateTime); }
      }

      /// <summary>
      /// Gets the "If-None-Match" HTTP header from the request.
      /// </summary>
      /// <value>The "If-None-Match" HTTP header from the request.</value>
      public string IfNoneMatch
      {
         get { return (Response != null) ? Response.ETag : null; }
      }

      /// <summary>
      /// Gets or sets the <see cref="Response"/> for the given <see cref="Request"/>.
      /// </summary>
      /// <value>
      /// The <see cref="Response"/> for the given <see cref="Request"/>.
      /// </value>
      public Response Response { get; private set; }


      /// <summary>
      /// Gets the Uniform Resource Identifier (<see cref="T:System.Uri" />) of the request.
      /// </summary>
      /// <value>The Uniform Resource Identifier (<see cref="T:System.Uri" />) of the request.</value>
      public Uri Uri
      {
         get { return this.currentWebRequest.RequestUri; }
      }


      /// <summary>
      /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </returns>
      public override string ToString()
      {
         StringBuilder responseBuilder = new StringBuilder();

         string relativePart = Uri.PathAndQuery + Uri.Fragment;

         responseBuilder.AppendLine(
            String.Format("{0} {1} HTTP/{2}",
                          this.method,
                          relativePart,
                          this.currentWebRequest.ProtocolVersion.ToString(2)));

         // Add the 'Host' header.
         responseBuilder.AppendLine(String.Format("Host: {0}", Uri.Host));

         if (IfModifiedSince != default(DateTime))
         {
            responseBuilder.AppendLine(String.Format("If-Modified-Since: {0}",
                                                     IfModifiedSince));
         }

         if (!String.IsNullOrEmpty(IfNoneMatch))
         {
            responseBuilder.AppendLine(String.Format("If-None-Match: {0}", IfNoneMatch));
         }

         foreach (string key in this.currentWebRequest.Headers.AllKeys)
         {
            // The 'Host' header is already added, so skip it
            if (key.IsEqualTo("host"))
            {
               continue;
            }

            string value = this.currentWebRequest.Headers[key];
            responseBuilder.AppendLine(String.Format("{0}: {1}", key, value));
         }

         if (this.currentWebRequest.ContentLength > 0)
         {
            responseBuilder.AppendLine("Content-Length: " +
                                       this.currentWebRequest.ContentLength);
         }

         CookieContainer cookies = this.engine.CookieContainer;
         string cookieHeader;

         if ((cookies != null) && (cookies.Count > 0) &&
             !String.IsNullOrEmpty(cookieHeader = cookies.GetCookieHeader(Uri)))
         {
            responseBuilder.AppendLine("Cookie: " + cookieHeader);
         }

         responseBuilder.AppendLine();
         responseBuilder.Append(this.requestString);

         return responseBuilder.ToString();
      }


      /// <summary>
      /// Gets the <see cref="Response"/> for the current <see cref="Request"/>.
      /// </summary>
      /// <returns>
      /// The <see cref="Response"/> for the current <see cref="Request"/>.
      /// </returns>
      internal Response GetResponse()
      {
         return (Response = new Response(() =>
         {
            HttpWebResponse webResponse = null;
            Exception exception = null;

            try
            {
               this.currentWebRequest.AllowAutoRedirect = false;
               webResponse = this.currentWebRequest.GetResponse() as
                             HttpWebResponse;
            }
            catch (WebException webException)
            {
               webResponse = webException.Response as HttpWebResponse;
               exception = webException;
            }
            catch (Exception ex)
            {
               exception = ex;
            }

            if (webResponse == null)
            {
               if (exception != null)
               {
                  throw new InvalidUriException(Uri, exception);
               }

               throw new InvalidUriException(Uri);
            }

            return webResponse;
         }));
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
                                           RequestDictionary requestValues)
      {
         bool methodIsGet = (this.method == HttpMethod.Get);
         Uri createdUri = uri.ToUri(GetBaseUri());

         this.baseUri = new Uri(createdUri.GetLeftPart(UriPartial.Authority));
         HttpWebRequest request = WebRequest.Create(createdUri) as HttpWebRequest;		 

         if (request == null)
         {
            throw new InvalidUriException(uri);
         }

         // Set the content length to '0' when the body is empty on non-GET requests
         if (!methodIsGet && ((requestValues == null) || (requestValues.Count == 0)))
         {
            request.ContentLength = 0;
         }

         // Set the HTTP method
         request.Method = this.method;

         // Set the Proxy
         request.Proxy = this.requestProxy;

         // Add the request values if we have any
         if ((requestValues != null) && (requestValues.Count > 0))
         {
            this.requestString = requestValues.ToString();

            // If the HTTP method is GET, recreate the request with
            // the values in the query string
            if (methodIsGet)
            {
               return CreateRequest(String.Concat(uri, '?', this.requestString), null);
            }
            SetRequestProperties(request);	
            using (Stream stream = request.GetRequestStream())
            {
               using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
               {
                  writer.Write(this.requestString);
               }
            }
         }
         

         return request;
      }


      /// <summary>
      /// Gets the base URI of the request, if it's not null.
      /// </summary>
      /// <returns>
      /// The base URI of the request, if it's not null.
      /// </returns>
      private Uri GetBaseUri()
      {
         Request request = this.engine.Requests.Current;
         return (request != null) ? request.baseUri : null;
      }


      /// <summary>
      /// Gets the <see cref="ICredentials"/> to use for the request.
      /// </summary>
      /// <param name="credentials">The credentials.</param>
      /// <returns>
      /// The <see cref="ICredentials"/> to use for the request.
      /// </returns>
      private ICredentials GetCredentials(ICredentials credentials)
      {
         Request r = this.engine.Requests.Current;
         return credentials ?? ((r != null) ? r.requestCredentials : null);
      }


      /// <summary>
      /// Gets the <see cref="IWebProxy"/> to use for the request.
      /// </summary>
      /// <param name="proxy">The proxy.</param>
      /// <returns>
      /// The <see cref="IWebProxy"/> to use for the request.
      /// </returns>
      private IWebProxy GetProxy(IWebProxy proxy)
      {
         Request r = this.engine.Requests.Current;
         return proxy ?? ((r != null) ? r.requestProxy : null);
      }


      /// <summary>
      /// Sets the various properties on the given <paramref name="request"/>.
      /// </summary>
      /// <param name="request">The request on which to set the properties.</param>
      private void SetRequestProperties(HttpWebRequest request)
      {
         request.KeepAlive = true;
         request.UserAgent = UserAgentString;
         request.Accept = Accept.ToString();
         request.Credentials = this.requestCredentials;

         // TODO: Add support for different types of cache policies
         request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         if (!String.IsNullOrEmpty(IfNoneMatch))
         {
            request.Headers.Add(HttpRequestHeader.IfNoneMatch, IfNoneMatch);
         }

         if (IfModifiedSince != default(DateTime))
         {
            request.IfModifiedSince = IfModifiedSince;
         }

         request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
         request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us,en;q=0.5");
         request.Headers.Add(HttpRequestHeader.AcceptCharset,
                             "utf-8,iso-8859-1;q=0.7,*;q=0.6");

         if (request.Method != HttpMethod.Get)
         {
            request.ContentType = "application/x-www-form-urlencoded";
         }

         // Set the "Referer" header
         if (this.engine.Requests.Current != null)
         {
            request.Referer = this.engine.Requests.Current.Uri.ToString();
         }

         request.CookieContainer = this.engine.CookieContainer;
      }


      /// <summary>
      /// Gets the user agent string.
      /// </summary>
      /// <returns>The user agent string.</returns>
      private static string GetUserAgentString()
      {
         Assembly assembly = Assembly.GetAssembly(typeof(Request));

         return String.Format("Arana/{0} ({1} {2}; N; .NET CLR {3}; {4})",
                              assembly.GetName().Version,
                              Environment.OSVersion.Platform,
                              Environment.OSVersion.VersionString,
                              Environment.Version,
                              assembly.GetName().ProcessorArchitecture);
      }
   }
}