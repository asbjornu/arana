using System.Collections.Specialized;
using System.Text;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// A static class that contains extension methods for various collection types.
   /// </summary>
   internal static class CollectionExtensions
   {
      /// <summary>
      /// Gets the request string.
      /// </summary>
      /// <param name="collection">The collection.</param>
      /// <param name="getRequest">if set to <c>true</c> [get request].</param>
      /// <returns>
      /// The request string.
      /// </returns>
      public static string GetRequestString(this NameValueCollection collection, bool getRequest)
      {
         StringBuilder stringBuilder = new StringBuilder();

         if (getRequest)
            stringBuilder.Append('?');

         for (int i = 0; i < collection.Count; i++)
         {
            string key = collection.Keys[i];
            string value = collection[i].UriEncode();

            stringBuilder.AppendFormat("{0}={1}", key, value);

            if (i < (collection.Count - 1))
               stringBuilder.Append('&');
         }

         return stringBuilder.ToString();
      }
   }
}