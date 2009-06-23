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
      /// Gets or sets the cookies that are associated with this request.
      /// </summary>
      private CookieCollection cookies;

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

         Request previousRequest = this.engine.Requests.Current;

         // Throw an exception if the HTTP method used is invalid.
         if (!this.method.IsEqualTo(false, HttpMethod.All))
         {
            throw new NotSupportedException(
               String.Format("The method '{0}' is invalid.", this.method));
         }

         if ((previousRequest != null) &&
             (previousRequest.cookies != null) &&
             (previousRequest.cookies.Count > 0))
         {
            this.cookies = this.cookies ?? new CookieCollection();
            this.cookies.Add(previousRequest.cookies);
         }

         this.currentWebRequest = CreateRequest(uri, requestValues);
      }


      public ResponseData Response { get; private set; }


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
         StringBuilder stringBuilder = new StringBuilder();
         StringBuilder cookieBuilder = new StringBuilder();

         string relativePart = Uri.PathAndQuery + Uri.Fragment;

         stringBuilder.AppendLine(String.Format("{0} {1} HTTP/1.1",
                                                this.method,
                                                relativePart));

         foreach (string key in this.currentWebRequest.Headers.AllKeys)
         {
            string value = this.currentWebRequest.Headers[key];
            stringBuilder.AppendLine(String.Format("{0}: {1}", key, value));
         }

         if (this.currentWebRequest.ContentLength > 0)
         {
            stringBuilder.AppendLine("Content-Length: " +
                                     this.currentWebRequest.ContentLength);
         }

         int i = 0;

         if ((this.cookies != null) && (this.cookies.Count > 0))
         {
            foreach (Cookie cookie in this.cookies)
            {
               cookieBuilder.AppendFormat("{0}={1}",
                                          cookie.Name,
                                          cookie.Value);

               if (i++ < (this.cookies.Count - 1))
               {
                  cookieBuilder.Append('&');
               }
            }
         }

         if (cookieBuilder.Length > 0)
         {
            stringBuilder.AppendLine("Cookie: " + cookieBuilder);
         }

         stringBuilder.AppendLine();
         stringBuilder.Append(this.requestString);

         return stringBuilder.ToString();
      }


      /// <summary>
      /// Gets the <see cref="Response"/> for the current <see cref="Request"/>.
      /// </summary>
      /// <returns>
      /// The <see cref="Response"/> for the current <see cref="Request"/>.
      /// </returns>
      internal Response GetResponse()
      {
         Func<HttpWebResponse> getHttpWebResponse = () =>
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
         };

         Response response = new Response(this, getHttpWebResponse);
         Response = response.Data;

         return response;
      }


      /// <summary>
      /// Sets the cookie.
      /// </summary>
      /// <param name="response">The response.</param>
      internal void SetCookie(Response response)
      {
         if (response.Data.Cookie == null)
         {
            return;
         }

         this.cookies = this.cookies ?? new CookieCollection();
         this.cookies.Add(response.Data.Cookie);
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

            using (Stream stream = request.GetRequestStream())
            {
               using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
               {
                  writer.Write(this.requestString);
               }
            }
         }

         SetRequestProperties(request);

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
         Request previousRequest = this.engine.Requests.Current;

         request.UserAgent = UserAgentString;
         request.Accept = Accept.ToString();
         request.Credentials = this.requestCredentials;

         // TODO: Add support for different types of cache policies
         request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         // TODO: Set more accepted charsets and handle decoding of them
         request.Headers.Add("Accept-Charset", "utf-8");

         // TODO: Set Accept-Encoding and handle decoding

         if (request.Method != HttpMethod.Get)
         {
            request.ContentType = "application/x-www-form-urlencoded";
         }

         // Set the "Referer" header
         if (previousRequest != null)
         {
            request.Referer = previousRequest.Uri.ToString();
         }

         // If there's any cookies to add to the request, do it
         if ((this.cookies == null) || (this.cookies.Count <= 0))
         {
            return;
         }

         request.CookieContainer = new CookieContainer(this.cookies.Count);
         request.CookieContainer.Add(this.cookies);
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