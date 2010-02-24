using System.Collections.Generic;
using System.Xml;

namespace Arana.Web.Server.Html
{
   /// <summary>
   /// Represents an Index document for <see cref="WebServer"/>.
   /// </summary>
   internal class IndexDocument : Document
   {
      private readonly IEnumerable<string> files;


      /// <summary>
      /// Initializes a new instance of the <see cref="IndexDocument"/> class.
      /// </summary>
      /// <param name="files">The files.</param>
      public IndexDocument(IEnumerable<string> files)
         : base("Index")
      {
         this.files = files;
      }


      /// <summary>
      /// When overridden in a derived class, writes the content of the HTML body.
      /// </summary>
      /// <param name="xmlWriter">The XML writer to write the HTML body content to.</param>
      protected override void WriteBodyContent(XmlWriter xmlWriter)
      {
         base.WriteBodyContent(xmlWriter);

         // <ul>
         xmlWriter.WriteStartElement("ul", Xhtml.Namespace);

         foreach (string file in this.files)
         {
            // <li><a id="relativeName" href="#relativeName">relativeName</a></li>
            WriteListItem(xmlWriter, file);
         }

         // </ul>
         xmlWriter.WriteEndElement();
      }


      /// <summary>
      /// Writes the list item.
      /// </summary>
      /// <param name="xmlWriter">The XML writer.</param>
      /// <param name="file">The file.</param>
      private static void WriteListItem(XmlWriter xmlWriter, string file)
      {
         // <li>
         xmlWriter.WriteStartElement("li", Xhtml.Namespace);

         // <a href="filename.html">filename.html</a>
         xmlWriter.WriteStartElement("a", Xhtml.Namespace);
         xmlWriter.WriteAttributeString("href", file);
         xmlWriter.WriteString(file);
         xmlWriter.WriteEndElement();

         // </li>
         xmlWriter.WriteEndElement();
      }
   }
}