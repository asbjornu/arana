using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HtmlAgilityPack;

namespace Arana.Core.Fizzler.Systems.HtmlAgilityPack
{
   /// <summary>
   /// HtmlNode extension methods.
   /// </summary>
   internal static class HtmlNodeExtensions
   {
      /// <summary>
      /// Determines whether this node is an element or not.
      /// </summary>
      private static bool IsElement(this HtmlNode node)
      {
         if (node == null)
            throw new ArgumentNullException("node");
         return node.NodeType == HtmlNodeType.Element;
      }

      /// <summary>
      /// Returns a collection of elements from this collection.
      /// </summary>
      public static IEnumerable<HtmlNode> Elements(this IEnumerable<HtmlNode> nodes)
      {
         if (nodes == null)
            throw new ArgumentNullException("nodes");
         return nodes.Where(n => n.IsElement());
      }

      /// <summary>
      /// Returns a collection of child nodes of this node.
      /// </summary>
      private static IEnumerable<HtmlNode> Children(this HtmlNode node)
      {
         if (node == null)
            throw new ArgumentNullException("node");
         return node.ChildNodes.Cast<HtmlNode>();
      }

      /// <summary>
      /// Returns a collection of child elements of this node.
      /// </summary>
      public static IEnumerable<HtmlNode> Elements(this HtmlNode node)
      {
         if (node == null)
            throw new ArgumentNullException("node");
         return node.Children().Elements();
      }

      /// <summary>
      /// Returns a collection of the sibling elements after this node.
      /// </summary>
      public static IEnumerable<HtmlNode> ElementsAfterSelf(this HtmlNode node)
      {
         if (node == null)
            throw new ArgumentNullException("node");
         return node.NodesAfterSelf().Elements();
      }

      /// <summary>
      /// Returns a collection of the sibling nodes after this node.
      /// </summary>
      private static IEnumerable<HtmlNode> NodesAfterSelf(this HtmlNode node)
      {
         if (node == null)
            throw new ArgumentNullException("node");
         return NodesAfterSelfImpl(node);
      }

      private static IEnumerable<HtmlNode> NodesAfterSelfImpl(HtmlNode node)
      {
         while ((node = node.NextSibling) != null)
            yield return node;
      }

      /// <summary>
      /// Returns a collection of the sibling elements before this node.
      /// </summary>
      public static IEnumerable<HtmlNode> ElementsBeforeSelf(this HtmlNode node)
      {
         if (node == null)
            throw new ArgumentNullException("node");
         return node.NodesBeforeSelf().Elements();
      }

      /// <summary>
      /// Returns a collection of the sibling nodes before this node.
      /// </summary>
      private static IEnumerable<HtmlNode> NodesBeforeSelf(this HtmlNode node)
      {
         if (node == null)
            throw new ArgumentNullException("node");
         return NodesBeforeSelfImpl(node);
      }

      private static IEnumerable<HtmlNode> NodesBeforeSelfImpl(HtmlNode node)
      {
         while ((node = node.PreviousSibling) != null)
            yield return node;
      }

      /// <summary>
      /// Returns a collection of all descendant nodes of this element.
      /// </summary>
      public static IEnumerable<HtmlNode> Descendants(this HtmlNode node)
      {
         if (node == null)
            throw new ArgumentNullException("node");
         return DescendantsImpl(node);
      }

      private static IEnumerable<HtmlNode> DescendantsImpl(HtmlNode node)
      {
         Debug.Assert(node != null);
         foreach (HtmlNode child in node.ChildNodes)
         {
            yield return child;
            foreach (HtmlNode descendant in child.Descendants())
               yield return descendant;
         }
      }
   }
}