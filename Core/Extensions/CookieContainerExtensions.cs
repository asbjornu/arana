using System;
using System.Collections;
using System.Net;
using System.Reflection;

namespace Arana.Extensions
{
   /// <summary>
   /// Contains extension methods for the <see cref="CookieContainer" /> class.
   /// </summary>
   internal static class CookieContainerExtensions
   {
      /// <summary>
      /// Fixes the cookie domains.
      /// </summary>
      /// <param name="cookieContainer">The cookie container.</param>
      // See http://channel9.msdn.com/forums/TechOff/260235-Bug-in-CookieContainer-where-do-I-report/
      public static CookieContainer FixDomains(this CookieContainer cookieContainer)
      {
			
         // TODO: fix does not run on mono, find out if it is actually needed there
         int p = (int) Environment.OSVersion.Platform;
         if ((p == 4) || (p == 6) || (p == 128)) 
            return cookieContainer;

         if (cookieContainer != null)
         {
            Type type = typeof(CookieContainer);
            const BindingFlags bindings = BindingFlags.NonPublic |
                                          BindingFlags.GetField |
                                          BindingFlags.Instance;

            Hashtable table = type.InvokeMember("m_domainTable",
                                                bindings,
                                                null,
                                                cookieContainer,
                                                new object[] { }) as Hashtable;
            if (table != null)
            {
               ArrayList keys = new ArrayList(table.Keys);

               // For all domains that starts with '.', add the cookie again
               // without the '.' prefix.
               foreach (string key in keys)
               {
                  if (!key.StartsWith("."))
                  {
                     continue;
                  }

                  string newKey = key.Remove(0, 1);
                  table[newKey] = table[key];
               }
            }
         }

         return cookieContainer;
      }
   }
}