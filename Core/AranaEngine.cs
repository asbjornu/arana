using System;
using System.Net;

using Fizzler.Parser;

namespace Arana.Core
{
   /// <summary>
   /// The base testing engine of Araña.
   /// </summary>
   public class AranaEngine
   {
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
         : this(uri, null)
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="AranaEngine"/> class.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="credentials">The credentials.</param>
      public AranaEngine(string uri, ICredentials credentials)
      {
         NavigateTo(uri, true, credentials);
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
      /// The <see cref="Fizzler.Parser.SelectorEngine" /> used to parse and execute CSS selectors.
      /// </summary>
      internal SelectorEngine SelectorEngine { get; private set; }


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
         SelectorEngine = GetSelectorEngine(uri, followRedirect, null, null, null);
      }


      /// <summary>
      /// Navigates to the specified <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <param name="credentials">The credentials.</param>
      internal void NavigateTo(string uri, bool followRedirect, ICredentials credentials)
      {
         SelectorEngine = GetSelectorEngine(uri, followRedirect, null, credentials, null);
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
         SelectorEngine = GetSelectorEngine(uri, followRedirect, httpMethod, null, requestValues);
      }


      /// <summary>
      /// Gets a new <see cref="Fizzler.Parser.SelectorEngine"/> for the given <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="credentials">The credentials.</param>
      /// <param name="requestValues">The request values.</param>
      /// <returns>
      /// A new <see cref="Fizzler.Parser.SelectorEngine"/> for the given <paramref name="uri"/>.
      /// </returns>
      /// <exception cref="InvalidUriException">
      /// If <paramref name="uri"/> doesn't yield a valid <see cref="AranaResponse"/>.
      /// </exception>
      private SelectorEngine GetSelectorEngine(string uri,
                                               bool followRedirect,
                                               string httpMethod,
                                               ICredentials credentials,
                                               RequestDictionary requestValues)
      {
         this.request = new AranaRequest(this.request,
                                         uri,
                                         httpMethod,
                                         credentials,
                                         requestValues);

         using (AranaResponse response = this.request.GetResponse())
         {
            if (response == null)
               throw new InvalidUriException(uri);

            Response = response.Data;

            // Set the cookie from the response
            this.request.SetCookie(Response);

            // If we're to follow redirects and the status indicates a redirect;
            if (followRedirect && (response.Data.StatusBase == 300))
            {
               // Get a new selector engine for the location we're being redirected to
               return GetSelectorEngine(response.Data.Location,
                                        true,
                                        HttpMethod.Get,
                                        credentials,
                                        null);
            }

            return !String.IsNullOrEmpty(Response.Body)
                      ? new SelectorEngine(Response.Body)
                      : null;
         }
      }
   }
}