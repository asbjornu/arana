using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using HtmlAgilityPack;

namespace Arana.Core
{
   /// <summary>
   /// Provides an encapsulation of <see cref="HtmlNode" />.
   /// </summary>
   public class ElementList : List<HtmlNode>
   {
      private readonly AranaEngine exerciser;


      /// <summary>
      /// Initializes a new instance of the <see cref="ElementList"/> class.
      /// </summary>
      /// <param name="htmlNodes">The HTML nodes.</param>
      /// <param name="exerciser">The exerciser.</param>
      internal ElementList(IEnumerable<HtmlNode> htmlNodes, AranaEngine exerciser)
         : base(htmlNodes)
      {
         this.exerciser = exerciser;
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
      /// <returns></returns>
      public AranaEngine Follow()
      {
         HtmlNode anchor = this["a"];

         if (anchor == null)
            throw new InvalidOperationException("The selected elements does not contain an HTML 'a' element.");

         if ((anchor.Attributes == null) || (anchor.Attributes.Count == 0))
            throw new InvalidOperationException("The HTML anchor has no attributes.");

         HtmlAttribute hrefAttribute = anchor.Attributes["href"];

         if ((hrefAttribute == null) || String.IsNullOrEmpty(hrefAttribute.Value))
            throw new InvalidOperationException("The HTML anchor has an empty 'href' attribute.");

         this.exerciser.NavigateTo(hrefAttribute.Value);
         return this.exerciser;
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <returns></returns>
      public AranaEngine Submit()
      {
         return Submit(null);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <returns></returns>
      public AranaEngine Submit(NameValueCollection requestValues)
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
         ElementList formElements = this.exerciser.Select("input, textarea, button");

         if ((formElements != null) && (formElements.Count > 0))
         {
            requestCollection = new NameValueCollection(formElements.Count);

            foreach (HtmlNode formElement in formElements)
            {
               HtmlAttribute nameAttribute = formElement.Attributes["name"];
               HtmlAttribute valueAttribute = formElement.Attributes["value"];

               if ((nameAttribute == null) || String.IsNullOrEmpty(nameAttribute.Value))
                  continue;

               // Retrieve the value for the given form element from the request values collection.
               string value = requestValues[nameAttribute.Value];
               value = String.IsNullOrEmpty(value) && (valueAttribute != null)
                          ? valueAttribute.Value
                          : value;

               requestCollection.Add(nameAttribute.Value, value);
            }
         }

         this.exerciser.NavigateTo(actionAttribute.Value, method, requestCollection);

         return this.exerciser;
      }
   }
}