using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fizzler.Systems.HtmlAgilityPack;

using HtmlAgilityPack;

namespace Arana.Extensions
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
         {
            throw new ArgumentNullException("node");
         }

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

         // If we're to carry on, prepend the selector of the parent non-null element
         if (carryOn &&
            (node.ParentNode != null) &&
            (node.ParentNode.NodeType == HtmlNodeType.Element))
         {
            selectorBuilder.Insert(0, node.ParentNode.GetCssSelector());
         }

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
         {
            throw new ArgumentNullException("node");
         }

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
      /// Determines whether the node is set for submission.
      /// </summary>
      /// <param name="node">The node.</param>
      /// <returns>
      /// 	<c>true</c> if the node is set for submission; otherwise, <c>false</c>.
      /// </returns>
      public static bool IsSetForSubmission(this HtmlNode @node)
      {
         if (node == null)
         {
            throw new ArgumentNullException("node");
         }

         return node.Attributes.Get(Selection.SetForSubmissionAttribute)
                == Selection.SetForSubmissionAttribute;
      }


      /// <summary>
      /// Determines whether the specified node is a button.
      /// </summary>
      /// <param name="node">The node.</param>
      /// <returns>
      /// 	<c>true</c> if the specified node is a button; otherwise, <c>false</c>.
      /// </returns>
      public static bool IsButton(this HtmlNode @node)
      {
         if (node == null)
         {
            throw new ArgumentNullException("node");
         }

         return node.Attributes.Get("type").IsEqualTo("button", "submit")
                || node.Name.IsEqualTo("button");
      }


      /// <summary>
      /// Determines whether the node is a radio button or a checkbox.
      /// </summary>
      /// <param name="node">The node.</param>
      /// <returns>
      /// 	<c>true</c> if the node is a radio button or a checkbox; otherwise, <c>false</c>.
      /// </returns>
      public static bool IsRadioOrCheckbox(this HtmlNode @node)
      {
         if (node == null)
         {
            throw new ArgumentNullException("node");
         }

         return node.Attributes.Get("type").IsEqualTo("checkbox", "radio");
      }

      /// <summary>
      /// Determines whether the node is checked or not.
      /// </summary>
      /// <param name="node">The node.</param>
      /// <returns>
      /// 	<c>true</c> if the node is checked; otherwise, <c>false</c>.
      /// </returns>
      public static bool IsChecked(this HtmlNode @node)
      {
         if (node == null)
         {
            throw new ArgumentNullException("node");
         }

         return node.Attributes.Get("checked") != null;
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
         {
            throw new ArgumentNullException("node");
         }

         if (!node.Name.IsEqualTo("select"))
         {
            throw new InvalidOperationException(
               String.Format(
                  "Can't set the selected value to '{0}' since it's not a 'select' element.",
                  node.Name));
         }

         if (!node.HasChildNodes)
         {
            throw new InvalidOperationException(
               "The 'select' element does not have any child nodes.");
         }

         IEnumerable<HtmlNode> children = node.Elements().SkipWhile(
            child => !child.Name.IsEqualTo("option"));

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
            if (!childNode.Name.IsEqualTo("option"))
            {
               continue;
            }

            HtmlAttribute selectedAttribute = childNode.Attributes["selected"];

            // Remove any existing 'selected' attributes.
            if (selectedAttribute != null)
            {
               childNode.Attributes.Remove(selectedAttribute);
            }

            // Set the 'selected' attribute if the indices match
            if (optionIndex == index)
            {
               childNode.Attributes.Append("selected", "selected");
            }

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
         {
            throw new ArgumentNullException("node");
         }

         if (!node.Name.IsEqualTo("select"))
         {
            throw new InvalidOperationException(
               String.Format(
                  "Can't get the selected value from '{0}' since it's not a 'select' element.",
                  node.Name));
         }

         string selectedValue = null;

         if (node.HasChildNodes)
         {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
               HtmlNode childNode = node.ChildNodes[i];

               // If the node's name isn't 'option', skip it
               if (!childNode.Name.IsEqualTo("option"))
               {
                  continue;
               }

               string value = childNode.Attributes.Get("value");

               // If we're not on the first 'option' element, or it isn't 'selected', skip
               if ((i != 0) && !childNode.Attributes.Contains("selected"))
               {
                  continue;
               }

               // Set the selected value to that of the first or 'selected' 'option' element.
               selectedValue = value;
               break;
            }
         }

         return selectedValue;
      }
   }
}