using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Arana.Core.Extensions;

using Fizzler.Parser;

using HtmlAgilityPack;

namespace Arana.Core
{
   /// <summary>
   /// The base testing engine of Araña.
   /// </summary>
   public class AranaEngine
   {
      private SelectorEngine engine;

      /// <summary>
      /// A local field used to preserve the last request made for reference
      /// to future requests (to preserve the base URI as a way to resolve
      /// relative URI's, etc).
      /// </summary>
      private AranaRequest request;


      /// <summary>
      /// Initializes a new instance of the <see cref="Arana"/> class.
      /// </summary>
      /// <param name="uri">The application URI.</param>
      public AranaEngine(string uri)
      {
         NavigateTo(uri, FollowRedirect = true);
      }


      /// <summary>
      /// Gets or sets a value that indicates whether the request should follow redirection responses.
      /// </summary>
      /// <value>
      /// <c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.
      /// </value>
      public bool FollowRedirect { get; set; }

      /// <summary>
      /// Gets the data for the last response.
      /// </summary>
      /// <value>The data for the last response.</value>
      public ResponseData Response { get; private set; }


      /// <summary>
      /// Selects the specified xpath expression.
      /// </summary>
      /// <param name="cssSelector">The CSS selector.</param>
      /// <returns></returns>
      public ElementList Select(string cssSelector)
      {
         IList<HtmlNode> nodes = this.engine.Parse(cssSelector);

         if (nodes == null || nodes.Count == 0)
            throw new ArgumentException("The CSS selector returned an empty node set.",
                                        "cssSelector");

         return new ElementList(nodes, this);
      }


      /// <summary>
      /// Navigates to the specified <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect">
      /// <c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.
      /// </param>
      internal void NavigateTo(string uri, bool followRedirect)
      {
         this.engine = GetSelectorEngine(uri, followRedirect, null, null);
      }


      /// <summary>
      /// Navigates to the specified <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect">
      /// <c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.
      /// </param>
      /// <param name="httpMethod">The HTTP method.</param>
      internal void NavigateTo(string uri, bool followRedirect, string httpMethod)
      {
         this.engine = GetSelectorEngine(uri, followRedirect, httpMethod, null);
      }


      /// <summary>
      /// Navigates to the specified <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect">
      /// <c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.
      /// </param>
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="requestValues">The request values.</param>
      internal void NavigateTo(string uri,
                               bool followRedirect,
                               string httpMethod,
                               NameValueCollection requestValues)
      {
         this.engine = GetSelectorEngine(uri, followRedirect, httpMethod, requestValues);
      }


      /// <summary>
      /// Gets a new <see cref="SelectorEngine"/> for the given <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect">
      /// <c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.
      /// </param>
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="requestValues">The request values.</param>
      /// <returns>
      /// A new <see cref="SelectorEngine"/> for the given <paramref name="uri"/>.
      /// </returns>
      private SelectorEngine GetSelectorEngine(string uri,
                                               bool followRedirect,
                                               string httpMethod,
                                               NameValueCollection requestValues)
      {
         this.request = new AranaRequest(this.request, uri, httpMethod, requestValues);

         using (AranaResponse response = this.request.GetResponse())
         {
            if (response == null)
               throw new ArgumentException(
                  String.Format("The URI '{0}' did not make much sense, sorry.", uri), "uri");

            Response = response.Data;

            // Set the cookie from the response
            request.SetCookie(Response);

            // If we're to follow redirects and the status indicates a redirect;
            if (followRedirect && (response.Data.Status.GetBase() == 300))
            {
               // Get a new selector engine for the location we're being redirected to
               return GetSelectorEngine(response.Data.Location,
                                        true,
                                        AranaRequest.HttpGet,
                                        null);
            }


            return !String.IsNullOrEmpty(Response.Body)
                      ? new SelectorEngine(Response.Body)
                      : null;
         }
      }
   }
}