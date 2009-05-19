using System.Collections.Generic;
using System.Linq;
using Fizzler;
using HtmlAgilityPack;

namespace Arana.Core.Fizzler.Systems.HtmlAgilityPack
{
   /// <summary>
   /// Selector API for <see cref="HtmlNode"/>.
   /// </summary>
   /// <remarks>
   /// For more information, see <a href="http://www.w3.org/TR/selectors-api/">Selectors API</a>.
   /// </remarks>
   internal static class HtmlNodeSelection
   {
      /// <summary>
      /// Retrieves all element nodes from descendants of the starting
      /// element node that match any selector within the supplied
      /// selector strings.
      /// </summary>
      /// <param name="node">The node.</param>
      /// <param name="selector">The selector.</param>
      /// <returns></returns>
      public static IEnumerable<HtmlNode> QuerySelectorAll(this HtmlNode node, string selector)
      {
         var generator = new SelectorGenerator<HtmlNode>(new HtmlNodeOps());
         Parser.Parse(selector, generator);
         return generator.Selector(Enumerable.Repeat(node, 1));
      }
   }
}