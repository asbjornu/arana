using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Arana.Core.Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace Arana.Core
{
   /// <summary>
   /// The base testing engine of Araña.
   /// </summary>
   public class AranaEngine
   {
      private TextWriter output;

      /// <summary>
      /// A local field used to preserve the last request made for reference
      /// to future requests (to preserve the base URI as a way to resolve
      /// relative URI's, etc).
      /// </summary>
      private Request request;


      /// <summary>
      /// Initializes a new instance of the <see cref="Arana"/> class.
      /// </summary>
      /// <param name="uri">The application URI.</param>
      public AranaEngine(string uri)
         : this(uri, null, null)
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="AranaEngine"/> class.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="credentials">The credentials.</param>
      public AranaEngine(string uri, ICredentials credentials)
      {
         NavigateTo(uri, true, credentials, null);
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="AranaEngine"/> class.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="proxy">The proxy.</param>
      public AranaEngine(string uri, IWebProxy proxy)
      {
         NavigateTo(uri, true, null, proxy);
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="AranaEngine"/> class.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="credentials">The credentials.</param>
      /// <param name="proxy">The proxy.</param>
      public AranaEngine(string uri, ICredentials credentials, IWebProxy proxy)
      {
         NavigateTo(uri, true, credentials, proxy);
      }


      /// <summary>
      /// Sets the <see cref="TextWriter" /> to write debug information to.
      /// </summary>
      /// <value>The <see cref="TextWriter" /> to write debug information to.</value>
      public TextWriter Output
      {
         set { this.output = value; }
      }


      /// <summary>
      /// Gets the data for the last response.
      /// </summary>
      /// <value>The data for the last response.</value>
      public ResponseData Response { get; private set; }

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
         get { return (this.request == null) ? null : this.request.Uri; }
      }

      /// <summary>
      /// Gets or sets the document.
      /// </summary>
      /// <value>The document.</value>
      private HtmlDocument Document { get; set; }


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
      internal void NavigateTo(string uri, bool followRedirect)
      {
         SetCurrentDocument(uri, followRedirect, null, null, null, null);
      }


      /// <summary>
      /// Navigates to the specified <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="requestValues">The request values.</param>
      internal void NavigateTo(string uri,
                               bool followRedirect,
                               string httpMethod,
                               RequestDictionary requestValues)
      {
         SetCurrentDocument(uri, followRedirect, httpMethod, null, null, requestValues);
      }


      /// <summary>
      /// Queries the current <see cref="Document"/> with the given <paramref name="cssSelector"/>
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

         if (Document == null)
         {
            throw new InvalidOperationException("The Document is null.");
         }

         if (Document.DocumentNode == null)
         {
            throw new InvalidOperationException("The Document's DocumentNode is null.");
         }

         return Document.DocumentNode.QuerySelectorAll(cssSelector);
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
         this.request = new Request(this.request,
                                    uri,
                                    httpMethod,
                                    credentials,
                                    proxy,
                                    requestValues);

         Write(this.request, "Request");

         using (Response response = this.request.GetResponse())
         {
            if (response == null)
            {
               throw new InvalidUriException(uri);
            }

            Response = response.Data;

            Write(Response, "Response");

            // Set the cookie from the response
            this.request.SetCookie(Response);

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

            return !String.IsNullOrEmpty(Response.Body)
                      ? GetDocument(Response.Body)
                      : null;
         }
      }


      /// <summary>
      /// Navigates to the specified <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <param name="credentials">The credentials.</param>
      /// <param name="proxy">The proxy.</param>
      private void NavigateTo(string uri,
                              bool followRedirect,
                              ICredentials credentials,
                              IWebProxy proxy)
      {
         SetCurrentDocument(uri, followRedirect, null, credentials, proxy, null);
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
      private void SetCurrentDocument(string uri,
                                      bool followRedirect,
                                      string httpMethod,
                                      ICredentials credentials,
                                      IWebProxy proxy,
                                      RequestDictionary requestValues)
      {
         Document = GetDocument(uri,
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
      private void Write(object value, string section)
      {
         if (this.output == null)
         {
            return;
         }

         string divider = String.Format("{0,80}",
                                        String.Format("_{0}_#{1}_{2}",
                                                      section,
                                                      this.request.Number,
                                                      new String('-', 40)))
            .Replace(' ', '-')
            .Replace('_', ' ');

         this.output.WriteLine(divider);
         this.output.WriteLine(value);
         this.output.WriteLine();
      }


      /// <summary>
      /// Gets an <see cref="HtmlDocument"/> object from the provided <paramref name="html"/>.
      /// </summary>
      /// <param name="html">The HTML.</param>
      /// <returns>
      /// An <see cref="HtmlDocument"/> object from the provided <paramref name="html"/>.
      /// </returns>
      private static HtmlDocument GetDocument(string html)
      {
         HtmlDocument document = new HtmlDocument();
         document.LoadHtml(html);
         return document;
      }
   }
}