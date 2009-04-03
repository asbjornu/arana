using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using Arana.Core.Extensions;

using HtmlAgilityPack;

namespace Arana.Core
{
   /// <summary>
   /// Provides an encapsulation of <see cref="List&lt;HtmlNode&gt;" />.
   /// </summary>
   public class ElementList : List<HtmlNode>
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
      // ReSharper disable MemberCanBePrivate.Global
      public HtmlNode this[string tagName]
         // ReSharper restore MemberCanBePrivate.Global
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
      /// Gets the inner HTML of the currently selected list of elements.
      /// </summary>
      /// <value>The inner HTML of the currently selected list of elements.</value>
      // ReSharper disable UnusedMember.Global
      public string InnerHtml
         // ReSharper restore UnusedMember.Global
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
      /// Gets the value of the 'value' attribute of the currently selected list of elements.
      /// </summary>
      /// <value>The value of the 'value' attribute of the currently selected list of elements.</value>
      public string Value
      {
         get { return Attribute("value"); }
      }


      /// <summary>
      /// Gets the value of the attribute with the given <paramref name="name"/>
      /// of the currently selected list of elements.
      /// </summary>
      /// <param name="name">The name of the attribute whose value to retrieve.</param>
      /// <returns>
      /// The value of the attribute with the given <paramref name="name"/> of
      /// the currently selected list of elements.
      /// </returns>
      // ReSharper disable MemberCanBePrivate.Global
      public string Attribute(string name)
         // ReSharper restore MemberCanBePrivate.Global
      {
         StringBuilder sb = new StringBuilder();

         foreach (HtmlNode htmlNode in this)
         {
            HtmlAttribute attribute = htmlNode.Attributes[name];

            if (attribute == null)
               continue;

            sb.AppendLine(attribute.Value);
         }

         return sb.ToString().Trim();
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
      /// 1. If the currently selected elements doesn't contain an 'a' element.
      /// 2. If a currently selected 'a' element doesn't have any attributes.
      /// 3. If a currently selected 'a' element has an empty or non-existent 'href' attribute.
      /// </exception>
      // ReSharper disable MemberCanBePrivate.Global
      public AranaEngine Follow(bool followRedirect)
         // ReSharper restore MemberCanBePrivate.Global
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
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain a 'form' element.
      /// 2. If a currently selected 'form' element doesn't have any attributes.
      /// 3. If a currently selected 'form' element has an empty or non-existent 'action' attribute.
      /// </exception>
      // ReSharper disable UnusedMember.Global
      public AranaEngine Submit()
         // ReSharper restore UnusedMember.Global
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
      // ReSharper disable UnusedMethodReturnValue.Global
      public AranaEngine Submit(NameValueCollection requestValues)
         // ReSharper restore UnusedMethodReturnValue.Global
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
      /// 1. If the currently selected elements doesn't contain a 'form' element.
      /// 2. If a currently selected 'form' element doesn't have any attributes.
      /// 3. If a currently selected 'form' element has an empty or non-existent 'action' attribute.
      /// </exception>
      public AranaEngine Submit(bool followRedirect, NameValueCollection requestValues)
      {
         HtmlNode form = this["form"];

         if (form == null)
            throw new InvalidOperationException("The selected elements does not contain an HTML 'form' element.");

         HtmlAttribute actionAttribute = form.Attributes["action"];
         HtmlAttribute methodAttribute = form.Attributes["method"];
         RequestDictionary requestDictionary = null;

         string method = (methodAttribute != null) && !String.IsNullOrEmpty(methodAttribute.Value)
                            ? methodAttribute.Value.ToUpperInvariant()
                            : HttpMethod.Get;

         if ((form.Attributes == null) || (form.Attributes.Count == 0))
            throw new InvalidOperationException("The HTML form has no attributes.");

         if ((actionAttribute == null) || String.IsNullOrEmpty(actionAttribute.Value))
            throw new InvalidOperationException("The HTML form has an empty 'action' attribute.");

         // TODO: Make the CSS selector more precise, so it only select child nodes of the current form element.
         ElementList formElements = this.engine.Select(FormElementsSelector);

         if ((formElements != null) && (formElements.Count > 0))
            requestDictionary = formElements.GetRequestCollection(requestValues);

         this.engine.NavigateTo(actionAttribute.Value, followRedirect, method, requestDictionary);

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
            if ((nameAttribute == null) || String.IsNullOrEmpty(nameAttribute.Value))
               continue;

            string name = nameAttribute.Value;

            // Retrieve the value for the given form element from the request values collection.
            string value = requestValues[name];
            value = String.IsNullOrEmpty(value) && (valueAttribute != null)
                       ? valueAttribute.Value
                       : value;

            requestDictionary.Set(name, value);
         }

         return requestDictionary;
      }
   }
}