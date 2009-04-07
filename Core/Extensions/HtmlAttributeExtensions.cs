using System;

using HtmlAgilityPack;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// Contains extension methods for the <see cref="HtmlAttribute" /> object.
   /// </summary>
   internal static class HtmlAttributeExtensions
   {
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
            return null;

         HtmlAttribute attribute = attributes[name];

         return attribute.HasValue() ? attribute.Value : null;
      }


      /// <summary>
      /// Determines whether the specified attribute has value.
      /// </summary>
      /// <param name="attribute">The attribute.</param>
      /// <returns>
      /// 	<c>true</c> if the specified attribute has value; otherwise, <c>false</c>.
      /// </returns>
      public static bool HasValue(this HtmlAttribute @attribute)
      {
         return (attribute != null) && !String.IsNullOrEmpty(attribute.Value);
      }
   }
}