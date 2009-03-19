using System;

namespace Arana.Core.Extensions
{
   public static class StringExtensions
   {
      public static bool StartsWith(this string @s, params string[] values)
      {
         if (String.IsNullOrEmpty(s) || values == null || values.Length == 0)
            return false;

         foreach (string value in values)
         {
            if (s.StartsWith(value))
               return true;
         }

         return false;
      }
   }
}