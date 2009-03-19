using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;

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
      /// <summary>
      /// Contains the Araña Engine's User Agent string as used when performing HTTP web requests.
      /// </summary>
      private static readonly string UserAgentString = GetUserAgentString();

      private Uri baseUri;
      private SelectorEngine engine;


      /// <summary>
      /// Initializes a new instance of the <see cref="Arana"/> class.
      /// </summary>
      /// <param name="uri">The application URI.</param>
      public AranaEngine(string uri)
      {
         NavigateTo(uri);
      }


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
      internal void NavigateTo(string uri)
      {
         this.engine = GetSelectorEngine(uri, null, null);
      }


      /// <summary>
      /// Navigates to the specified <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="httpMethod">The HTTP method.</param>
      internal void NavigateTo(string uri, string httpMethod)
      {
         this.engine = GetSelectorEngine(uri, httpMethod, null);
      }


      /// <summary>
      /// Navigates to the specified <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="requestValues">The request values.</param>
      internal void NavigateTo(string uri, string httpMethod, NameValueCollection requestValues)
      {
         this.engine = GetSelectorEngine(uri, httpMethod, requestValues);
      }


      /// <summary>
      /// Gets the user agent string.
      /// </summary>
      /// <returns>The user agent string.</returns>
      private static string GetUserAgentString()
      {
         Assembly assembly = Assembly.GetAssembly(typeof(AranaEngine));

         return String.Format("Arana/{0} ({1} {2}; N; CLR {3}; {4})",
                              assembly.GetName().Version,
                              Environment.OSVersion.Platform,
                              Environment.OSVersion.VersionString,
                              Environment.Version,
                              assembly.GetName().ProcessorArchitecture);
      }


      /// <summary>
      /// Gets a new <see cref="SelectorEngine"/> for the given <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="httpMethod">The HTTP method.</param>
      /// <param name="requestValues">The request values.</param>
      /// <returns>
      /// A new <see cref="SelectorEngine"/> for the given <paramref name="uri"/>.
      /// </returns>
      private SelectorEngine GetSelectorEngine(string uri, string httpMethod, NameValueCollection requestValues)
      {
         Uri u = uri.ToUri(this.baseUri);
         this.baseUri = new Uri(u.GetLeftPart(UriPartial.Authority));

         HttpWebRequest request = HttpWebRequest.Create(u) as HttpWebRequest;

         if (request == null)
            throw new ArgumentException("The URI did not make much sense, sorry.", "uri");

         request.UserAgent = UserAgentString;
         request.Method = httpMethod ?? "GET";

         if ((requestValues != null) && (requestValues.Count > 0))
         {
            // request.
         }

         using (HttpWebResponse response = request.GetHttpWebResponse())
         {
            if (response == null)
               throw new ArgumentException(
                  String.Format("The URI '{0}' did not make much sense, sorry.", uri), "uri");

            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
               string html = streamReader.ReadToEnd();
               return new SelectorEngine(html);
            }
         }
      }
   }
}