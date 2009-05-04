using System;
using HtmlAgilityPack;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// Contains extension methods for the <see cref="HtmlAttribute"/>
   /// and <see cref="HtmlAttributeCollection"/> objects.
   /// </summary>
   internal static class HtmlAttributeExtensions
   {
      /// <summary>
      /// Determines whether the given <see cref="HtmlAttributeCollection"/> contains
      /// an <see cref="HtmlNode"/> with the given <paramref name="tagName"/>.
      /// </summary>
      /// <param name="attributes">The attributes.</param>
      /// <param name="tagName">Name of the tag to find..</param>
      /// <returns>
      /// 	<c>true</c> if the given <see cref="HtmlAttributeCollection"/> contains
      /// an <see cref="HtmlNode"/> with the given <paramref name="tagName"/>; otherwise, <c>false</c>.
      /// </returns>
      public static bool Contains(this HtmlAttributeCollection @attributes, string tagName)
      {
         return Get(attributes, tagName) != null;
      }


      /// <summary>
      /// Gets the value of the attribute with the given <paramref name="name"/>
      /// within the <paramref name="attributes"/> collection.
      /// </summary>
      /// <param name="attributes">The attributes.</param>
      /// <param name="name">The name.</param>
      /// <returns>
      /// The value of the attribute with the given <paramref name="name"/>
      /// within the <paramref name="attributes"/> collection.
      /// </returns>
      public static string Get(this HtmlAttributeCollection @attributes, string name)
      {
         if ((attributes == null) || (attributes.Count == 0))
            throw new ArgumentNullException("attributes");

         HtmlAttribute attribute = attributes[name];

         return (attribute != null) ? attribute.Value : null;
      }


      /// <summary>
      /// Determines whether the <see cref="HtmlAttribute.Value"/> is is equal to
      /// the specified <paramref name="value"/>.
      /// </summary>
      /// <param name="attribute">The attribute.</param>
      /// <param name="value">The value.</param>
      /// <returns>
      /// 	<c>true</c> if the <see cref="HtmlAttribute.Value"/> is is equal to
      /// the specified <paramref name="value"/>; otherwise <c>false</c>.
      /// </returns>
      public static bool ValueIsEqualTo(this HtmlAttribute @attribute, string value)
      {
         return (attribute != null) && attribute.Value.IsEqualTo(value);
      }
   }
}