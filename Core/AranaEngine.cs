using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using Fizzler.Parser;

using HtmlAgilityPack;

using Arana.Core.Extensions;

namespace Arana.Core
{
   /// <summary>
   /// The base testing engine of Araña.
   /// </summary>
   public class AranaEngine
   {
      private Uri baseUri;
      private SelectorEngine engine;


      /// <summary>
      /// Initializes a new instance of the <see cref="Arana"/> class.
      /// </summary>
      /// <param name="uri">The application URI.</param>
      public AranaEngine(string uri)
      {
         GoTo(uri);
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
      /// Goes to the URI.
      /// </summary>
      /// <param name="uri">The URI.</param>
      internal void GoTo(string uri)
      {
         this.engine = GetSelectorEngine(uri);
      }

      // internal void GoTo(string uri)


      /// <summary>
      /// Gets the document.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <returns></returns>
      private SelectorEngine GetSelectorEngine(string uri)
      {
         Uri u = uri.ToUri(this.baseUri);
         baseUri = new Uri(u.GetLeftPart(UriPartial.Authority));

         HttpWebRequest request = HttpWebRequest.Create(u) as HttpWebRequest;

         if (request == null)
            throw new ArgumentException("The URI did not make much sense, sorry.", "uri");

         HttpWebResponse response = request.GetHttpWebResponse();

         if (response == null)
            throw new ArgumentException("The URI did not make much sense, sorry.", "uri");

         using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
         {
            string html = streamReader.ReadToEnd();
            return new SelectorEngine(html);
         }
      }
   }
}