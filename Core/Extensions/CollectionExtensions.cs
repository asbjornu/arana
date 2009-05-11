using System.Collections.Specialized;

namespace Arana.Core.Extensions
{
   internal static class CollectionExtensions
   {
      /// <summary>
      /// Gets the value for the specified <paramref name="name"/> from the
      /// <see cref="NameValueCollection"/>. If the <paramref name="collection"/>
      /// is null or empty, returns the default value.
      /// </summary>
      /// <param name="collection">The collection.</param>
      /// <param name="name">The name.</param>
      /// <param name="defaultValue">The default value.</param>
      /// <returns>
      /// The value for the specified <paramref name="name"/>.
      /// </returns>
      public static string Get(this NameValueCollection @collection,
                               string name,
                               string defaultValue)
      {
         if ((collection == null) || (collection.Count == 0))
            return defaultValue;

         return collection[name] ?? defaultValue;
      }
   }
}