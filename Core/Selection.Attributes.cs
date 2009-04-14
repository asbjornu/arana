using System;
using System.Collections.Generic;
using System.Text;

using Arana.Core.Extensions;

using HtmlAgilityPack;

namespace Arana.Core
{
   public partial class Selection
   {
      // ReSharper disable UnusedMember.Global
      // ReSharper disable MemberCanBePrivate.Global
      // ReSharper disable UnusedMethodReturnValue.Global

      /// <summary>
      /// Gets the value of the 'class' attribute(s) of the currently selected list of elements.
      /// </summary>
      /// <returns>
      /// The value of the 'class' attribute(s) of the currently selected list of elements.
      /// </returns>
      public string Class()
      {
         return Attribute("class");
      }


      /// <summary>
      /// Sets the value of the 'class' attribute(s) of the currently selected list of elements
      /// to the specified <paramref name="className"/>.
      /// </summary>
      /// <param name="className">The id.</param>
      /// <returns>The list of currently selected elements.</returns>
      public Selection Class(string className)
      {
         return Attribute("class", className);
      }


      /// <summary>
      /// Gets the value of the 'id' attribute(s) of the currently selected list of elements.
      /// </summary>
      /// <returns>
      /// The value of the 'id' attribute(s) of the currently selected list of elements.
      /// </returns>
      public string ID()
      {
         return Attribute("id");
      }


      /// <summary>
      /// Sets the value of the 'id' attribute(s) of the currently selected list of elements
      /// to the specified <paramref name="id"/>.
      /// </summary>
      /// <param name="id">The id.</param>
      /// <returns>The list of currently selected elements.</returns>
      public Selection ID(string id)
      {
         return Attribute("id", id);
      }


      /// <summary>
      /// Gets the value of the 'name' attribute(s) of the currently selected list of elements.
      /// </summary>
      /// <returns></returns>
      /// <value>The value of the 'name' attribute(s) of the currently selected list of elements.</value>
      public string Name()
      {
         return Attribute("name");
      }


      /// <summary>
      /// Sets the value of the 'name' attribute(s) of the currently selected list of elements
      /// and returns the list.
      /// </summary>
      /// <param name="name">The name.</param>
      /// <returns>The list of currently selected elements.</returns>
      public Selection Name(string name)
      {
         return Attribute("name", name);
      }


      /// <summary>
      /// Gets the value of the 'value' attribute(s) of the currently selected list of elements.
      /// </summary>
      /// <returns></returns>
      /// <value>The value of the 'value' attribute(s) of the currently selected list of elements.</value>
      public string Value()
      {
         return Attribute("value");
      }


      /// <summary>
      /// Sets the value of the 'value' attribute(s) of the currently selected list of elements
      /// and returns the list.
      /// </summary>
      /// <param name="value">The value.</param>
      /// <returns>The list of currently selected elements.</returns>
      public Selection Value(string value)
      {
         // If the currently selected list of elements contains a non-form element, throw.
         foreach (HtmlNode node in this)
            if (Array.IndexOf(FormElements, node.Name) == -1)
               throw new InvalidOperationException(
                  String.Format("Can't add 'value' attribute to element '{0}'.",
                                node.Name));

         return Attribute("value", value);
      }


      // ReSharper restore UnusedMember.Global
      // ReSharper restore MemberCanBePrivate.Global
      // ReSharper restore UnusedMethodReturnValue.Global

      /// <summary>
      /// Gets the value of the attribute(s) with the given <paramref name="name"/>.
      /// </summary>
      /// <param name="name">The name of the attribute.</param>
      /// <returns>
      /// The value of the attribute(s) with the given <paramref name="name"/>.
      /// </returns>
      private string Attribute(string name)
      {
         StringBuilder sb = new StringBuilder();

         foreach (HtmlNode node in this)
         {
            string value =
               // If the node's name is 'select' and the attribute we're after is 'value',
               node.NameIsEqualTo("select") && name.IsEqualTo("value")
               // fetch the 'value' from a special sub routine
                  ? node.GetSelectedValue()
               // Else, just get the attribute's value.
                  : node.Attributes.Get(name);

            sb.AppendLine(value);
         }

         return sb.ToString().Trim();
      }


      /// <summary>
      /// Sets the value of the attribute(s) with the given <paramref name="name"/>
      /// to the specified <paramref name="value"/>.
      /// </summary>
      /// <param name="name">The name.</param>
      /// <param name="value">The value.</param>
      /// <returns>
      /// The list of currently selected elements, updated with the new <paramref name="value"/>.
      /// </returns>
      private Selection Attribute(string name, string value)
      {
         foreach (HtmlNode node in this)
            if (node.NameIsEqualTo("select") && name.IsEqualTo("value"))
               node.SetSelectedValue(value);
            else
               (node.Attributes[name] ?? node.Attributes.Append(name)).Value = value;

         return this;
      }


      /// <summary>
      /// Gets the attribute(s) of the currently selected list of elements.
      /// </summary>
      /// <returns>
      /// The attribute(s) of the currently selected list of elements.
      /// </returns>
      private IEnumerable<HtmlAttribute> Attributes()
      {
         foreach (HtmlNode htmlNode in this)
            foreach (HtmlAttribute attribute in htmlNode.Attributes)
               yield return attribute;
      }
   }
}