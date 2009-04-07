using System;
using System.Text;

using HtmlAgilityPack;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// Contains extension methods for the <see cref="HtmlNode"/> object.
   /// </summary>
   internal static class HtmlNodeExtensions
   {
      /// <summary>
      /// Gets the CSS selector for the given <paramref name="node"/>.
      /// </summary>
      /// <param name="node">The node.</param>
      /// <returns>The CSS selector for the given <paramref name="node"/>.</returns>
      public static string GetCssSelector(this HtmlNode @node)
      {
         StringBuilder selectorBuilder = new StringBuilder();

         if (node == null)
            throw new ArgumentNullException("node");

         string className = node.Attributes.Get("class");
         string name = node.Attributes.Get("name");
         string type = node.Attributes.Get("type");
         bool carryOn = true;

         selectorBuilder.Append(" ");

         if (!String.IsNullOrEmpty(node.Id))
         {
            selectorBuilder.AppendFormat("#{0}", node.Id);
            carryOn = false;
         }
         else if (!String.IsNullOrEmpty(className))
         {
            string[] classNames = className.Split(' ');
            
            // TODO: Bring this logic back once Fizzler implements support for multi-class selectors
            // className = String.Join(".", classNames);

            className = classNames[classNames.Length - 1];
            selectorBuilder.AppendFormat("{0}.{1}", node.Name, className);
         }
         else if (!String.IsNullOrEmpty(name))
         {
            selectorBuilder.AppendFormat("{0}[name='{1}']", node.Name, name);
         }
         else if (!String.IsNullOrEmpty(type))
         {
            selectorBuilder.AppendFormat("{0}[type='{1}']", node.Name, type);
         }
         else
         {
            selectorBuilder.Append(node.Name);
         }

         // If we're to carry on, prepend the selector of the parent element
         if (carryOn)
            selectorBuilder.Insert(0, node.ParentNode.GetCssSelector());

         return selectorBuilder.ToString().Trim();
      }
   }
}