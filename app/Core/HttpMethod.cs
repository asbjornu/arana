namespace Arana
{
   /// <summary>
   /// Contains the different HTTP methods supported by Araña.
   /// </summary>
   internal static class HttpMethod
   {
      /// <summary>
      /// Contains the value for the HTTP "GET" method.
      /// </summary>
      internal const string Get = "GET";

      /// <summary>
      /// Contains the value for the HTTP "DELETE" method.
      /// </summary>
      private const string Delete = "DELETE";

      /// <summary>
      /// Contains the value for the HTTP "HEAD" method.
      /// </summary>
      private const string Head = "HEAD";

      /// <summary>
      /// Contains the value for the HTTP "POST" method.
      /// </summary>
      private const string Post = "POST";

      /// <summary>
      /// Contains the value for the HTTP "PUT" method.
      /// </summary>
      private const string Put = "PUT";

      /// <summary>
      /// Contains an array of the HTTP methods supported by Araña.
      /// </summary>
      internal static string[] All
      {
         get { return new[] { Get, Post, Put, Delete, Head }; }
      }
   }
}