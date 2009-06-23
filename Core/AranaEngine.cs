using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using Arana.Core.Extensions;
using Arana.Core.Fizzler.Systems.HtmlAgilityPack;

using HtmlAgilityPack;

namespace Arana.Core
{
   /// <summary>
   /// The base testing engine of Araña.
   /// </summary>
   public class AranaEngine
   {
      /// <summary>
      /// Gets or sets the document.
      /// </summary>
      /// <value>The document.</value>
      private HtmlDocument document;


      /// <summary>
      /// Initializes a new instance of the <see cref="Arana"/> class.
      /// </summary>
      /// <param name="uri">The application URI.</param>
      public AranaEngine(string uri)
         : this(uri, null, null, null)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="Arana"/> class.
      /// </summary>
      /// <param name="uri">The application URI.</param>
      /// <param name="output">The <see cref="TextWriter" /> to write debug information to.</param>
      public AranaEngine(string uri, TextWriter output)
         : this(uri, null, null, output)
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="AranaEngine"/> class.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="credentials">The credentials.</param>
      public AranaEngine(string uri, ICredentials credentials)
         : this(uri, credentials, null, null)
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="AranaEngine"/> class.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="proxy">The proxy.</param>
      public AranaEngine(string uri, IWebProxy proxy)
         : this(uri, null, proxy, null)
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="AranaEngine"/> class.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="credentials">The credentials.</param>
      /// <param name="proxy">The proxy.</param>
      /// <param name="output">The <see cref="TextWriter" /> to write debug information to.</param>
      public AranaEngine(string uri, ICredentials credentials, IWebProxy proxy, TextWriter output)
      {
         Requests = new RequestList(this);
         Navigate(uri, true, null, credentials, proxy, null);
         Output = output;
      }


      /// <summary>
      /// Sets the <see cref="TextWriter" /> to write debug information to.
      /// </summary>
      /// <value>The <see cref="TextWriter" /> to write debug information to.</value>
      public TextWriter Output { get; set; }


      /// <summary>
      /// Gets the data for the last response.
      /// </summary>
      /// <value>The data for the last response.</value>
      public ResponseData Response
      { 
         get { return Requests.Current.Response; }
      }

      /// <summary>
      /// Gets the URI that is currently being manipulated by this
      /// <see cref="AranaEngine"/> instance.
      /// </summary>
      /// <value>
      /// The URI that is currently being manipulated by this
      /// <see cref="AranaEngine"/> instance.
      /// </value>
      public Uri Uri
      {
         get { return (Requests.Count > 0) ? Requests[Requests.Index].Uri : null; }
      }


      /// <summary>
      /// Gets the list of requests.
      /// </summary>
      /// <value>The list of requests.</value>
      internal RequestList Requests { get; private set; }


      /// <summary>
      /// Navigates the specified number of <paramref name="steps"/> within the
      /// request history.
      /// </summary>
      /// <param name="steps">The number of steps steps to navigate. A positive
      /// number navigates forward; negative backward.</param>
      public void Navigate(int steps)
      {
         Requests.Navigate(steps);
      }


      /// <summary>
      /// Navigates to the specified uri. If 
      /// </summary>
      /// <param name="uri">The URI.</param>
      public void Navigate(string uri)
      {
         Navigate(uri, true, null, null, null, null);
      }


      /// <summary>
      /// Selects a list of elements matching the given CSS selector.
      /// </summary>
      /// <param name="cssSelector">The CSS selector.</param>
      /// <returns>
      /// A list of elements matching the given CSS selector.
      /// </returns>
      public Selection Select(string cssSelector)
      {
         return new Selection(this, cssSelector);
      }


      /// <summary>
      /// Navigates to the specified <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      internal void Navigate(string uri, bool followRedirect)
      {
         Navigate(uri, followRedirect, null, null, null, null);
      }


      /// <summary>
      /// Navigates to the specified <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="requestValues">The request values.</param>
      internal void Navigate(string uri,
                             bool followRedirect,
                             string httpMethod,
                             RequestDictionary requestValues)
      {
         Navigate(uri, followRedirect, httpMethod, null, null, requestValues);
      }


      /// <summary>
      /// Queries the current <see cref="document"/> with the given <paramref name="cssSelector"/>
      /// and returns the matching <see cref="HtmlNode"/>s.
      /// </summary>
      /// <param name="cssSelector">The CSS selector.</param>
      /// <returns>
      /// The <see cref="HtmlNode"/>s matching the <paramref name="cssSelector"/>.
      /// </returns>
      internal IEnumerable<HtmlNode> QuerySelectorAll(string cssSelector)
      {
         if (String.IsNullOrEmpty(cssSelector))
         {
            throw new ArgumentNullException("cssSelector");
         }

         if (this.document == null)
         {
            throw new InvalidOperationException("The document is null.");
         }

         if (this.document.DocumentNode == null)
         {
            throw new InvalidOperationException("The document's DocumentNode is null.");
         }

         return this.document.DocumentNode.QuerySelectorAll(cssSelector);
      }


      /// <summary>
      /// Gets a new <see cref="HtmlDocument"/> for the given <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="credentials">The credentials.</param>
      /// <param name="proxy">The proxy.</param>
      /// <param name="requestValues">The request values.</param>
      /// <returns>
      /// A new <see cref="HtmlDocument"/> for the given <paramref name="uri"/>.
      /// </returns>
      /// <exception cref="InvalidUriException">
      /// If <paramref name="uri"/> doesn't yield a valid <see cref="Core.Response"/>.
      /// </exception>
      private HtmlDocument GetDocument(string uri,
                                       bool followRedirect,
                                       string httpMethod,
                                       ICredentials credentials,
                                       IWebProxy proxy,
                                       RequestDictionary requestValues)
      {
         Request request = new Request(this,
                                       uri,
                                       httpMethod,
                                       credentials,
                                       proxy,
                                       requestValues);

         WriteToOutput(request, "Request");

         using (Response response = request.GetResponse())
         {
            if (response == null)
            {
               throw new InvalidUriException(uri);
            }

            WriteToOutput(response, "Response");

            // Set the cookie from the response
            request.SetCookie(response);

            // If we're to follow redirects and the status indicates a redirect;
            if (followRedirect && (response.Data.StatusBase == 300))
            {
               // Get a new selector engine for the location we're being redirected to
               return GetDocument(response.Data.Location,
                                  true,
                                  HttpMethod.Get,
                                  credentials,
                                  proxy,
                                  null);
            }

            Requests.Add(request);

            return !String.IsNullOrEmpty(Response.Body)
                      ? Response.Body.ToHtmlDocument()
                      : null;
         }
      }


      /// <summary>
      /// Sets a new <see cref="HtmlDocument"/> for the given <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="credentials">The credentials.</param>
      /// <param name="proxy">The proxy.</param>
      /// <param name="requestValues">The request values.</param>
      /// <exception cref="InvalidUriException">
      /// If <paramref name="uri"/> doesn't yield a valid <see cref="Core.Response"/>.
      /// </exception>
      private void Navigate(string uri,
                            bool followRedirect,
                            string httpMethod,
                            ICredentials credentials,
                            IWebProxy proxy,
                            RequestDictionary requestValues)
      {
         this.document = GetDocument(uri,
                                     followRedirect,
                                     httpMethod,
                                     credentials,
                                     proxy,
                                     requestValues);
      }




      /// <summary>
      /// Writes the <paramref name="value"/> to the <see cref="Output" />.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <param name="section">The section.</param>
      internal void WriteToOutput(object value, string section)
      {
         if (Output == null)
         {
            return;
         }

         int index = Requests.Count - 1;

         string divider = String.Format("{0,80}",
                                        String.Format("_{0}_#{1}_{2}",
                                                      section,
                                                      index,
                                                      new String('-', 40)))
            .Replace(' ', '-')
            .Replace('_', ' ');

         Output.WriteLine(divider);
         Output.WriteLine(value);
         Output.WriteLine();
      }
   }
}