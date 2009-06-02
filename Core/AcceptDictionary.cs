using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using System.Text;

namespace Arana.Core
{
   /// <summary>
   /// 
   /// </summary>
   internal class AcceptDictionary : Dictionary<ContentType, double?>
   {
      /// <summary>
      /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </returns>
      public override string ToString()
      {
         NumberFormatInfo format = new NumberFormatInfo
         {
            NumberDecimalSeparator = "."
         };

         StringBuilder sb = new StringBuilder();

         foreach (ContentType contentType in Keys)
         {
            if (sb.Length > 0)
            {
               sb.Append(',');
            }

            double? weight = this[contentType];
            sb.Append(contentType);
            sb.AppendFormat(format, "{0:\\;0.0}", weight);
         }

         return sb.ToString();
      }


      /// <summary>
      /// Adds the specified content type.
      /// </summary>
      /// <param name="contentType">Type of the content.</param>
      /// <param name="weight">The weight.</param>
      public void Add(string contentType, double? weight)
      {
         Add(new ContentType(contentType), weight);
      }


      /// <summary>
      /// Adds the specified content type.
      /// </summary>
      /// <param name="contentType">Type of the content.</param>
      public void Add(string contentType)
      {
         Add(new ContentType(contentType), null);
      }
   }
}