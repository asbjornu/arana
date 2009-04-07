using System.Collections.Generic;
using System.Linq;
using System.Text;

using HtmlAgilityPack;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Arana.Core
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
      public Selection Value(string value)
      {
         return Attribute("value", value);
      }


      /// <summary>
      /// Gets the attribute(s) with the given <paramref name="name"/>
      /// of the currently selected list of elements.
      /// </summary>
      /// <param name="name">The name of the attributes to retrieve.</param>
      /// <returns>
      /// The attribute(s) with the given <paramref name="name"/> of
      /// the currently selected list of elements.
      /// </returns>
      internal IEnumerable<HtmlAttribute> Attributes(string name)
      {
         return from htmlNode in this
                let attribute = htmlNode.Attributes[name]
                where (attribute != null)
                select attribute;
      }


      /// <summary>
      /// Gets the attribute(s) of the currently selected list of elements.
      /// </summary>
      /// <returns>
      /// The attribute(s) of the currently selected list of elements.
      /// </returns>
      internal IEnumerable<HtmlAttribute> Attributes()
      {
         foreach (HtmlNode htmlNode in this)
            foreach (HtmlAttribute attribute in htmlNode.Attributes)
               yield return attribute;
      }


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

         foreach (HtmlAttribute attribute in Attributes(name))
            sb.AppendLine(attribute.Value);

         return sb.ToString().Trim();
      }


      /// <summary>
      /// Sets the value of the attribute(s) with the given <paramref name="name"/>
      /// to the specified <paramref name="value"/>.
      /// </summary>
      /// <param name="name">The name.</param>
      /// <param name="value">The value.</param>
      /// <returns>The list of currently selected elements.</returns>
      private Selection Attribute(string name, string value)
      {
         foreach (HtmlAttribute attribute in Attributes(name))
            attribute.Value = value;

         return this;
      }
   }
}