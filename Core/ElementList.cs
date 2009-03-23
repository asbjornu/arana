using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using Arana.Core.Extensions;

using HtmlAgilityPack;

namespace Arana.Core
{
   /// <summary>
   /// Provides an encapsulation of <see cref="HtmlNode" />.
   /// </summary>
   public class ElementList : List<HtmlNode>
   {
      private readonly AranaEngine engine;


      /// <summary>
      /// Initializes a new instance of the <see cref="ElementList"/> class.
      /// </summary>
      /// <param name="htmlNodes">The HTML nodes.</param>
      /// <param name="exerciser">The engine.</param>
      internal ElementList(IEnumerable<HtmlNode> htmlNodes, AranaEngine exerciser)
         : base(htmlNodes)
      {
         this.engine = exerciser;
      }


      /// <summary>
      /// Gets the <see cref="HtmlAgilityPack.HtmlNode"/> with the specified tag name.
      /// </summary>
      /// <value></value>
      public HtmlNode this[string tagName]
      {
         get
         {
            foreach (HtmlNode htmlNode in this)
               if (String.Compare(htmlNode.Name, tagName, true) == 0)
                  return htmlNode;

            return null;
         }
      }


      /// <summary>
      /// Gets the inner HTML.
      /// </summary>
      /// <value>The inner HTML.</value>
      public string InnerHtml
      {
         get
         {
            StringBuilder sb = new StringBuilder();

            foreach (HtmlNode htmlNode in this)
               sb.AppendLine(htmlNode.InnerHtml);

            return sb.ToString().Trim();
         }
      }

      /// <summary>
      /// Gets the inner text.
      /// </summary>
      /// <value>The inner text.</value>
      public string InnerText
      {
         get
         {
            StringBuilder sb = new StringBuilder();

            foreach (HtmlNode htmlNode in this)
               sb.AppendLine(htmlNode.InnerText);

            return sb.ToString().Trim();
         }
      }


      /// <summary>
      /// Follows the 'href' attribute on the selected HTML elements.
      /// </summary>
      /// <returns>An updated <see cref="AranaEngine" />.</returns>
      public AranaEngine Follow()
      {
         return Follow(true);
      }


      /// <summary>
      /// Follows the 'href' attribute on the selected HTML elements.
      /// </summary>
      /// <param name="followRedirect">
      /// <c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.
      /// </param>
      /// <returns>An updated <see cref="AranaEngine" />.</returns>
      public AranaEngine Follow(bool followRedirect)
      {
         HtmlNode anchor = this["a"];

         if (anchor == null)
            throw new InvalidOperationException("The selected elements does not contain an HTML 'a' element.");

         if ((anchor.Attributes == null) || (anchor.Attributes.Count == 0))
            throw new InvalidOperationException("The HTML anchor has no attributes.");

         HtmlAttribute hrefAttribute = anchor.Attributes["href"];

         if ((hrefAttribute == null) || String.IsNullOrEmpty(hrefAttribute.Value))
            throw new InvalidOperationException("The HTML anchor has an empty 'href' attribute.");

         this.engine.NavigateTo(hrefAttribute.Value, followRedirect);
         return this.engine;
      }


      /// <summary>
      /// Gets a <see cref="NameValueCollection"/> containing the names of all
      /// the form elements within the collection, with either the current values
      /// of the elements or with the value provided in the specified
      /// <paramref name="requestValues"/>, matched by the key.
      /// </summary>
      /// <param name="requestValues">The request values.
      /// A <see cref="NameValueCollection"/> containing the names of all the
      /// elements that should have the corresponding value set.</param>
      /// <returns>
      /// The merged <see cref="NameValueCollection"/> with the values from
      /// <paramref name="requestValues"/> applied.
      /// </returns>
      public NameValueCollection GetRequestCollection(NameValueCollection requestValues)
      {
         NameValueCollection requestCollection = new NameValueCollection(Count);

         foreach (HtmlNode node in this)
         {
            // Skip nodes that aren't form elements
            if (!node.Name.IsEqualTo(true, "input", "textarea", "button"))
               continue;

            HtmlAttribute nameAttribute = node.Attributes["name"];
            HtmlAttribute valueAttribute = node.Attributes["value"];

            // Skip elements that donesn't have a valid 'name' attribute
            if ((nameAttribute == null) || String.IsNullOrEmpty(nameAttribute.Value))
               continue;

            // Retrieve the value for the given form element from the request values collection.
            string value = requestValues[nameAttribute.Value];
            value = String.IsNullOrEmpty(value) && (valueAttribute != null)
                       ? valueAttribute.Value
                       : value;

            requestCollection.Add(nameAttribute.Value, value);
         }

         return requestCollection;
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      public AranaEngine Submit()
      {
         return Submit(true, null);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="requestValues">The request values.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      public AranaEngine Submit(NameValueCollection requestValues)
      {
         return Submit(true, requestValues);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="followRedirect">if set to <c>true</c> [follow redirect].</param>
      /// <param name="requestValues">The request values.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      public AranaEngine Submit(bool followRedirect, NameValueCollection requestValues)
      {
         HtmlNode form = this["form"];
         HtmlAttribute actionAttribute = form.Attributes["action"];
         HtmlAttribute methodAttribute = form.Attributes["method"];
         NameValueCollection requestCollection = null;
         string method = (methodAttribute != null) && !String.IsNullOrEmpty(methodAttribute.Value)
                            ? methodAttribute.Value.ToUpperInvariant()
                            : "GET";

         if (form == null)
            throw new InvalidOperationException("The selected elements does not contain an HTML 'form' element.");

         if ((form.Attributes == null) || (form.Attributes.Count == 0))
            throw new InvalidOperationException("The HTML form has no attributes.");

         if ((actionAttribute == null) || String.IsNullOrEmpty(actionAttribute.Value))
            throw new InvalidOperationException("The HTML form has an empty 'action' attribute.");

         // TODO: Make the CSS selector more precise, so it only select child nodes of the current form element.
         ElementList formElements = this.engine.Select("input, textarea, button");

         if ((formElements != null) && (formElements.Count > 0))
            requestCollection = formElements.GetRequestCollection(requestValues);

         this.engine.NavigateTo(actionAttribute.Value, followRedirect, method, requestCollection);

         return this.engine;
      }
   }
}