using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arana.Core.Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// Contains extension methods for the <see cref="HtmlNode"/> 
   /// and <see cref="HtmlNodeCollection"/> objects.
   /// </summary>
   internal static class HtmlNodeExtensions
   {
      /// <summary>
      /// Gets the CSS selector for the given <paramref name="node"/>.
      /// </summary>
      /// <param name="node">The node.</param>
      /// <returns>
      /// The CSS selector for the given <paramref name="node"/>.
      /// </returns>
      public static string GetCssSelector(this HtmlNode @node)
      {
         if (node == null)
            throw new ArgumentNullException("node");

         StringBuilder selectorBuilder = new StringBuilder();

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
            className = String.Join(".", classNames);
            selectorBuilder.Append(className);
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


      /// <summary>
      /// Gets the form submittable value from the <see cref="HtmlNode" />. If the
      /// node is an HTML 'select' element, returns the value of the currently
      /// selected 'option' element. If it is a 'textarea' element, returns the
      /// node's Inner HTML.
      /// </summary>
      /// <param name="node">The node.</param>
      /// <returns>The <see cref="HtmlNode" />'s value.</returns>
      public static string GetValue(this HtmlNode @node)
      {
         if (node == null)
            throw new ArgumentNullException("node");

         switch (node.Name.ToLowerInvariant())
         {
            case "select":
               return node.GetSelectedValue();

            case "textarea":
               return node.InnerHtml;
         }

         return node.Attributes.Get("value");
      }

      /// <summary>
      /// Determines whether the <paramref name="node"/>'s tag name is equal to
      /// the specified <paramref name="tagNames"/>, in a case insensitive match.
      /// </summary>
      /// <param name="node">The node.</param>
      /// <param name="tagNames">The names of the tag.</param>
      /// <returns>
      /// 	<c>true</c> if the <paramref name="node"/>'s tag name is equal to
      /// the specified <paramref name="tagNames"/>; otherwise, <c>false</c>.
      /// </returns>
      public static bool NameIsEqualTo(this HtmlNode @node, params string[] tagNames)
      {
         if (node == null)
            throw new ArgumentNullException("node");

         if (tagNames == null || tagNames.Length == 0)
            throw new ArgumentNullException("tagNames");

         foreach (string tagName in tagNames)
            if (String.Compare(node.Name, tagName, true) == 0)
               return true;

         return false;
      }

      /// <summary>
      /// Sets the 'option' element within <paramref name="node"/> that has an
      /// index equal to <paramref name="index"/> to 'selected'.
      /// </summary>
      /// <param name="node">The 'select' element.</param>
      /// <param name="index">The index of the 'option' to set as 'selected.</param>
      public static void SetSelectedIndex(this HtmlNode @node, int index)
      {
         if (node == null)
            throw new ArgumentNullException("node");

         if (!node.NameIsEqualTo("select"))
            throw new InvalidOperationException(
               String.Format(
                  "Can't set the selected value to '{0}' since it's not a 'select' element.",
                  node.Name));

         if (!node.HasChildNodes)
            throw new InvalidOperationException(
               "The 'select' element does not have any child nodes.");

         IEnumerable<HtmlNode> children = node.Elements().SkipWhile(
            child => !child.NameIsEqualTo("option"));

         int childCount = children.Count();
         int optionIndex = 0;

         if (index >= childCount)
         {
            string message = String.Format(
               "The index can't be higher than the number of 'option' elements ({0} items).",
               childCount);

            throw new ArgumentOutOfRangeException("index", index, message);
         }

         foreach (HtmlNode childNode in children)
         {
            // If the node's name isn't 'option', skip it
            if (!childNode.NameIsEqualTo("option"))
               continue;

            HtmlAttribute selectedAttribute = childNode.Attributes["selected"];

            // Remove any existing 'selected' attributes.
            if (selectedAttribute != null)
               childNode.Attributes.Remove(selectedAttribute);

            // Set the 'selected' attribute if the indices match
            if (optionIndex == index)
               childNode.Attributes.Append("selected", "selected");

            optionIndex++;
         }
      }

      /// <summary>
      /// Gets the value of the 'value' attribute from the selected (or first)
      /// 'option' element whithin a 'select' element.
      /// </summary>
      /// <param name="node">The 'select' node.</param>
      /// <returns>The value of the 'value' attribute.</returns>
      private static string GetSelectedValue(this HtmlNode @node)
      {
         if (node == null)
            throw new ArgumentNullException("node");

         if (!node.NameIsEqualTo("select"))
            throw new InvalidOperationException(
               String.Format(
                  "Can't get the selected value from '{0}' since it's not a 'select' element.",
                  node.Name));

         string selectedValue = null;

         if (node.HasChildNodes)
         {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
               HtmlNode childNode = node.ChildNodes[i];

               // If the node's name isn't 'option', skip it
               if (!childNode.NameIsEqualTo("option"))
                  continue;

               string value = childNode.Attributes.Get("value");

               // If we're not on the first 'option' element, or it isn't 'selected', skip
               if ((i != 0) && !childNode.Attributes.Contains("selected"))
                  continue;

               // Set the selected value to that of the first or 'selected' 'option' element.
               selectedValue = value;
               break;
            }
         }

         return selectedValue;
      }
   }
}