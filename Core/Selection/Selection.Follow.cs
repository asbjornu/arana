using System;
using System.Linq;

namespace Arana
{
   public partial class Selection
   {
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
         Selection anchors = Get("a");

         if ((anchors.Attributes() == null) || (anchors.Attributes().Count() == 0))
         {
            throw new InvalidOperationException(
               String.Format("The HTML anchor selected with '{0}' has no attributes.",
                             CssSelector));
         }

         string href = anchors.Attribute("href");
         if( href.StartsWith("#"))
         {             
            // its a fragment URL, and it's referring to the current document              
            href = this.engine.Requests.Current.Uri.AbsolutePath;
         }


         if (String.IsNullOrEmpty(href))
         {
            throw new InvalidOperationException(
               "The HTML anchor has an empty 'href' attribute.");
         }

         this.engine.Navigate(href, followRedirect);

         return this.engine;
      }
   }
}