using System;
using System.Collections.Generic;
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

         this.exerciser.GoTo(hrefAttribute.Value);
         return this.exerciser;
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <returns></returns>
      public AranaEngine Submit()
      {
         HtmlNode form = this["form"];

         if (form == null)
            throw new InvalidOperationException("The selected elements does not contain an HTML 'form' element.");

         if ((form.Attributes == null) || (form.Attributes.Count == 0))
            throw new InvalidOperationException("The HTML form has no attributes.");

         HtmlAttribute actionAttribute = form.Attributes["action"];

         if ((actionAttribute == null) || String.IsNullOrEmpty(actionAttribute.Value))
            throw new InvalidOperationException("The HTML form has an empty 'action' attribute.");

         // TODO: Add submit action

         return this.exerciser;
      }
   }
}