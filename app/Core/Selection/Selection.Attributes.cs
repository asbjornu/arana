using System;
using System.Collections.Generic;
using System.Text;

using Arana.Extensions;

using HtmlAgilityPack;

namespace Arana
{
   public partial class Selection
   {
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
      /// <exception cref="InvalidOperationException">
      /// If called on a 'select' element. Choose() should be used instead.
      /// </exception>
      /// <exception cref="InvalidOperationException">
      /// If called on a radio button. Check() should be used instead.
      /// </exception>
      /// <exception cref="InvalidOperationException">
      /// If the element isn't valid for form submission.
      /// </exception>
      public Selection Value(string value)
      {
         foreach (HtmlNode node in this)
         {
            if (!node.Name.IsEqualTo(FormElements))
            {
               throw new InvalidOperationException(
                  String.Format("Can't set the 'value' of element '{0}'.",
                                node.Name));
            }

            if (node.Name.IsEqualTo("select"))
            {
               throw new InvalidOperationException(
                  "Can't set the 'value' on a 'select' element. Use Choose() instead.");
            }

            if (node.Name.IsEqualTo("input") &&
                node.Attributes.Get("type").IsEqualTo("radio"))
            {
               throw new InvalidOperationException(
                  "Can't set the 'value' on a radio button. Use Check() instead.");
            }
         }

         return Attribute("value", value);
      }


      /// <summary>
      /// Gets the value of the attribute(s) with the given <paramref name="name"/>.
      /// </summary>
      /// <param name="name">The name of the attribute.</param>
      /// <returns>
      /// The value of the attribute(s) with the given <paramref name="name"/>.
      /// </returns>
      public string Attribute(string name)
      {
         StringBuilder sb = new StringBuilder();

         foreach (HtmlNode node in this)
         {
            // If the attribute name we're looking for is 'value',
            string value = name.IsEqualTo("value")
                           // Get the value with the extension method
                              ? node.GetValue()
                           // Else, get the attribute's value, whichever attribute it is
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
         {
            // If we're trying to set the 'value' on 'textarea';
            if (name.IsEqualTo("value") && node.Name.IsEqualTo("textarea"))
            {
               // set the textarea's inner HTML instead.
               node.InnerHtml = value;
            }
            else
            {
               HtmlAttribute attribute = node.Attributes[name];

               // If the attribute exists and its value is to be set to null,
               // remove it and continue
               if ((attribute != null) && (value == null))
               {
                  node.Attributes.Remove(attribute);
                  continue;
               }

               node.SetAttributeValue(name, value);
            }
         }

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
         {
            foreach (HtmlAttribute attribute in htmlNode.Attributes)
            {
               yield return attribute;
            }
         }
      }
   }
}