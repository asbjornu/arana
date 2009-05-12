using System;
using System.Collections.Generic;
using System.Text;
using Arana.Core.Extensions;

namespace Arana.Core
{
   /// <summary>
   /// A convenient implementation of <see cref="Dictionary{String,String}" />.
   /// </summary>
   internal class RequestDictionary : Dictionary<string, string>
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="RequestDictionary"/> class.
      /// </summary>
      /// <param name="count">The count.</param>
      public RequestDictionary(int count) : base(count)
      {
      }


      /// <summary>
      /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </returns>
      public override string ToString()
      {
         StringBuilder stringBuilder = new StringBuilder();

         int i = 0;

         foreach (string key in Keys)
         {
            string encodedKey = key.UriEncode();
            string encodedValue = this[key].UriEncode();

            stringBuilder.AppendFormat("{0}={1}",
                                       encodedKey,
                                       encodedValue);
            
            // Don't append '&' at the end of the request string
            if (i++ < (Count - 1))
            {
               stringBuilder.Append('&');
            }
         }

         return stringBuilder.ToString();
      }


      /// <summary>
      /// Sets the specified <paramref name="key"/> and <paramref name="value"/>
      /// in the dictionary. Overwrites the existing value if the <paramref name="key"/>
      /// exists, otherwise it's added.
      /// </summary>
      /// <param name="key">The key.</param>
      /// <param name="value">The value.</param>
      public void Set(string key, string value)
      {
         if (ContainsKey(key))
         {
            this[key] = value;
         }
         else
         {
            Add(key, value);
         }
      }
   }
}