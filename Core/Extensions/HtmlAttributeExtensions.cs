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