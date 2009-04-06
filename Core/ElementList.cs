using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Arana.Core.Extensions;

using HtmlAgilityPack;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Arana.Core
{
   /// <summary>
   /// Provides an encapsulation of <see cref="List{HtmlNode}" />.
   /// </summary>
   public partial class ElementList : List<HtmlNode>
   {
      /// <summary>
      /// Contains the tag names of all HTML form elements that should be posted to
      /// the server when the form is submitted.
      /// </summary>
      private static readonly string[] FormElements = new[]
      {
         "input", "textarea", "button"
      };

      /// <summary>
      /// Contains a comma separated list of the tag names of all HTML form elements
      /// that should be posted to the server when the form is submitted.
      /// </summary>
      private static readonly string FormElementsSelector = String.Join(",", FormElements);

      /// <summary>
      /// The <see cref="AranaEngine" /> to use for requests.
      /// </summary>
      private readonly AranaEngine engine;


      /// <summary>
      /// Initializes a new instance of the <see cref="ElementList"/> class.
      /// </summary>
      /// <param name="htmlNodes">The HTML nodes.</param>
      /// <param name="engine">The engine.</param>
      internal ElementList(IEnumerable<HtmlNode> htmlNodes, AranaEngine engine)
         : base(htmlNodes)
      {
         this.engine = engine;
      }


      /// <summary>
      /// Gets the <see cref="ElementList"/> with the specified <param name="tagName"/>.
      /// </summary>
      /// <value>
      /// The <see cref="ElementList"/> with the specified <param name="tagName"/>.
      /// </value>
      /// <exception cref="ArgumentNullException">
      /// If <paramref name="tagName"/> is null or empty.
      /// </exception>
      public ElementList this[string tagName]
      {
         get
         {
            if (String.IsNullOrEmpty(tagName))
               throw new ArgumentNullException("tagName");

            var v = from htmlNode in this
                    where (String.Compare(htmlNode.Name, tagName, true) == 0)
                    select htmlNode;

            return new ElementList(new List<HtmlNode>(v), this.engine);
         }
      }


      /// <summary>
      /// Gets the inner HTML of the currently selected list of elements.
      /// </summary>
      /// <value>The inner HTML of the currently selected list of elements.</value>
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
      /// Gets the inner text of the currently selected list of elements.
      /// </summary>
      /// <value>The inner text of the currently selected list of elements.</value>
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
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain an 'a' element.
      /// 2. If a currently selected 'a' element doesn't have any attributes.
      /// 3. If a currently selected 'a' element has an empty or non-existent 'href' attribute.
      /// </exception>
      public AranaEngine Follow()
      {
         return Follow(true);
      }


      /// <summary>
      /// Follows the 'href' attribute on the selected HTML elements.
      /// </summary>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 	<list type="number">
      /// 		<item>If the currently selected elements doesn't contain an 'a' element.</item>
      /// 		<item>If a currently selected 'a' element doesn't have any attributes.</item>
      /// 		<item>If a currently selected 'a' element has an empty or non-existent 'href' attribute.</item>
      /// 	</list>
      /// </exception>
      public AranaEngine Follow(bool followRedirect)
      {
         ElementList anchors = this["a"];

         if (anchors.Count == 0)
            throw new InvalidOperationException("The selected element(s) does not contain an HTML 'a' element.");

         if ((anchors.Attributes() == null) || (anchors.Attributes().Count() == 0))
            throw new InvalidOperationException("The HTML anchor has no attributes.");

         string href = anchors.Attribute("href");

         if (String.IsNullOrEmpty(href))
            throw new InvalidOperationException("The HTML anchor has an empty 'href' attribute.");

         this.engine.NavigateTo(href, followRedirect);

         return this.engine;
      }


      /// <summary>
      /// Returns the very next sibling for each <see cref="T:HtmlAgilityPack.HtmlNode"/>
      /// in the list of currently selected elements.
      /// </summary>
      /// <returns>
      /// The very next sibling for each <see cref="T:HtmlAgilityPack.HtmlNode"/>
      /// in the list of currently selected elements.
      /// </returns>
      public ElementList Next()
      {
         var v = from htmlNode in this
                 let nextSibling = htmlNode.NextSibling
                 where (nextSibling != null)
                 select nextSibling;

         return new ElementList(new List<HtmlNode>(v), this.engine);
      }


      /// <summary>
      /// Returns the very previous sibling for each <see cref="T:HtmlAgilityPack.HtmlNode"/>
      /// in the list of currently selected elements.
      /// </summary>
      /// <returns>
      /// The very previous sibling for each <see cref="T:HtmlAgilityPack.HtmlNode"/>
      /// in the list of currently selected elements.
      /// </returns>
      public ElementList Previous()
      {
         var v = from htmlNode in this
                 let nextSibling = htmlNode.PreviousSibling
                 where (nextSibling != null)
                 select nextSibling;

         return new ElementList(new List<HtmlNode>(v), this.engine);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain a 'form' element.
      /// 2. If a currently selected 'form' element doesn't have any attributes.
      /// 3. If a currently selected 'form' element has an empty or non-existent 'action' attribute.
      /// </exception>
      public AranaEngine Submit()
      {
         return Submit(true, null);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="requestValues">The request values.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain a 'form' element.
      /// 2. If a currently selected 'form' element doesn't have any attributes.
      /// 3. If a currently selected 'form' element has an empty or non-existent 'action' attribute.
      /// </exception>
      public AranaEngine Submit(NameValueCollection requestValues)
      {
         return Submit(true, requestValues);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <param name="requestValues">The request values.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 	<list type="number">
      /// 		<item>If the currently selected elements doesn't contain a 'form' element.</item>
      /// 		<item>If a currently selected 'form' element has an empty or non-existent 'action' attribute and an action URI can't be deduced from a previous <see cref="AranaRequest"/>.</item>
      /// 	</list>
      /// </exception>
      public AranaEngine Submit(bool followRedirect, NameValueCollection requestValues)
      {
         ElementList form = this["form"];

         if (form.Count == 0)
            throw new InvalidOperationException(
               "The selected element(s) does not contain an HTML 'form' element.");

         string method = form.Attribute("method");
         string action = form.Attribute("action");
         RequestDictionary requestDictionary = null;

         // Set the HTTP method. Default to "GET".
         method = String.IsNullOrEmpty(method)
                     ? HttpMethod.Get
                     : method.ToUpperInvariant();

         // Set the URI to post the form to. Default to the current URI.
         action = String.IsNullOrEmpty(action)
                     ? this.engine.Uri.ToString()
                     : action;

         if (String.IsNullOrEmpty(action))
            throw new InvalidOperationException("No valid 'action' to perform.");

         ElementList formElements = this.engine.Select(FormElementsSelector, form);

         if ((formElements != null) && (formElements.Count > 0))
            requestDictionary = formElements.GetRequestCollection(requestValues);

         this.engine.NavigateTo(action, followRedirect, method, requestDictionary);

         return this.engine;
      }


      /// <summary>
      /// Determines whether any of the elements in the currently selected list
      /// are parents of the specified <paramref name="node"/>.
      /// </summary>
      /// <param name="node">The HTML node.</param>
      /// <returns>
      /// 	<c>true</c> if any of the elements in the currently selected list are
      /// parents of the specified node; otherwise, <c>false</c>.
      /// </returns>
      /// <exception cref="ArgumentNullException">
      /// If <paramref name="node"/> is null.
      /// </exception>
      internal bool IsParentOf(HtmlNode node)
      {
         if (node == null)
            throw new ArgumentNullException("node");

         HtmlNode parentNode;

         while ((parentNode = node.ParentNode) != null)
            foreach (HtmlNode htmlNode in this)
               if (parentNode == htmlNode)
                  return true;

         return false;
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
      private RequestDictionary GetRequestCollection(NameValueCollection requestValues)
      {
         RequestDictionary requestDictionary = new RequestDictionary(Count);

         foreach (HtmlNode node in this)
         {
            // Skip nodes that aren't form elements
            if (!node.Name.IsEqualTo(true, FormElements))
               continue;

            HtmlAttribute nameAttribute = node.Attributes["name"];
            HtmlAttribute valueAttribute = node.Attributes["value"];

            // Skip elements that donesn't have a valid 'name' attribute
            if (!nameAttribute.HasValue())
               continue;

            string name = nameAttribute.Value;

            // Retrieve the value for the given form element from the request values collection.
            string value = requestValues[name];
            value = String.IsNullOrEmpty(value) && valueAttribute.HasValue()
                       ? valueAttribute.Value
                       : value;

            requestDictionary.Set(name, value);
         }

         return requestDictionary;
      }
   }
}