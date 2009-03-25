using System;
using System.Configuration;

namespace Arana.Test.ConsoleApplication
{
   /// <summary>
   /// Contains the values defined in the application configuration
   /// file's file &lt;appSettings /&gt; section.
   /// </summary>
   internal static class Settings
   {
      private static string email;
      private static string password;
      private static string webSiteUri;

      /// <summary>
      /// Gets the email.
      /// </summary>
      /// <value>The email.</value>
      public static string Email
      {
         get
         {
            email = email ?? ConfigurationManager.AppSettings["Email"];

            if (String.IsNullOrEmpty(email))
               throw new ConfigurationErrorsException(
                  "'Email' is not properly defined in the application's configuration appSettings.");

            return email;
         }
      }

      /// <summary>
      /// Gets the password.
      /// </summary>
      /// <value>The password.</value>
      public static string Password
      {
         get
         {
            password = password ?? ConfigurationManager.AppSettings["Password"];

            if (String.IsNullOrEmpty(password))
               throw new ConfigurationErrorsException(
                  "'Password' is not properly defined in the application's configuration appSettings.");

            return password;
         }
      }

      /// <summary>
      /// Gets the web site URI.
      /// </summary>
      /// <value>The web site URI.</value>
      public static string WebSiteUri
      {
         get
         {
            webSiteUri = webSiteUri ?? ConfigurationManager.AppSettings["WebSiteUri"];

            if (String.IsNullOrEmpty(webSiteUri))
               throw new ConfigurationErrorsException(
                  "'WebSiteUri' is not properly defined in the application's configuration appSettings.");

            return webSiteUri;
         }
      }
   }
}