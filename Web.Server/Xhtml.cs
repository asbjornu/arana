namespace Arana.Web.Server
{
   /// <summary>
   /// Contains namespace and DocType strings used in XHTML 1.0.
   /// </summary>
   internal static class Xhtml
   {
      /// <summary>
      /// The XHTML 1.0 namespace URI.
      /// </summary>
      internal const string Namespace = "http://www.w3.org/1999/xhtml";

      #region Nested type: DocType

      internal static class DocType
      {
         /// <summary>
         /// The Public DocType identifier for XHTML 1.0 Strict.
         /// </summary>
         internal const string PublicIdentifier = "-//W3C//DTD XHTML 1.0 Strict//EN";

         /// <summary>
         /// The System DocType identifier for XHTML 1.0 Strict.
         /// </summary>
         internal const string SystemIdentifier =
            "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd";
      }

      #endregion
   }


   /// <summary>
   /// Contains namespace strings used in XML.
   /// </summary>
   internal static class Xml
   {
      /// <summary>
      /// The 'xml:' namespace URI.
      /// </summary>
      internal const string Namespace = "http://www.w3.org/XML/1998/namespace";
   }
}