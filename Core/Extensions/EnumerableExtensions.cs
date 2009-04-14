using System;
using System.Collections.Generic;

namespace Arana.Core.Extensions
{
   /// <summary>
   /// Contains extension methods for <see cref="IEnumerable{T}" />.
   /// </summary>
   internal static class EnumerableExtensions
   {
      /// <summary>
      /// Invokes the specified <paramref name="action"/> on each of the items
      /// in the <paramref name="enumerable"/>.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to iterate over.</param>
      /// <param name="action">The action to perform on each item in the <paramref name="enumerable"/>.</param>
      public static void Each<T>(this IEnumerable<T> @enumerable, Action<T> action)
      {
         if (enumerable == null)
            return;

         if (action == null)
            throw new ArgumentNullException("action");

         foreach (T item in enumerable)
            action.Invoke(item);
      }
   }
}