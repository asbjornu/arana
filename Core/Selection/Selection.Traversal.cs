using System.Collections.Generic;
using System.Linq;

using Fizzler.Systems.HtmlAgilityPack;

using HtmlAgilityPack;

namespace Arana.Core
{
   public partial class Selection
   {
      /// <summary>
      /// Returns the very next sibling for each <see cref="HtmlNode"/>
      /// in the list of currently selected elements.
      /// </summary>
      /// <returns>
      /// The very next sibling for each <see cref="HtmlNode"/>
      /// in the list of currently selected elements.
      /// </returns>
      public Selection Next()
      {
         IEnumerable<HtmlNode> nextSiblings =
            from htmlNode in this.Elements()
            let nextSibling = htmlNode.NextSibling
            where (nextSibling != null)
            select nextSibling;

         return new Selection(nextSiblings, this.engine);
      }


      /// <summary>
      /// Returns the very previous sibling for each <see cref="HtmlNode"/>
      /// in the list of currently selected elements.
      /// </summary>
      /// <returns>
      /// The very previous sibling for each <see cref="HtmlNode"/>
      /// in the list of currently selected elements.
      /// </returns>
      public Selection Previous()
      {
         IEnumerable<HtmlNode> previousSiblings =
            from htmlNode in this.Elements()
            let nextSibling = htmlNode.PreviousSibling
            where (nextSibling != null)
            select nextSibling;

         return new Selection(previousSiblings, this.engine);
      }
   }
}