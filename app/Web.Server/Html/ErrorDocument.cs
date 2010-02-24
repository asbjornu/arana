using System.IO;
using System.Xml;

namespace Arana.Web.Server.Html
{
   /// <summary>
   /// Represents an Error document for <see cref="WebServer"/>.
   /// </summary>
   internal class ErrorDocument : Document
   {
      private readonly string errorMessage;

      /// <summary>
      /// Initializes a new instance of the <see cref="IndexDocument"/> class.
      /// </summary>
      /// <param name="title">The title.</param>
      /// <param name="errorMessage">The error message.</param>
      /// <param name="stream">The stream.</param>
      public ErrorDocument(string title, string errorMessage, Stream stream)
         : base(title, stream)
      {
         this.errorMessage = errorMessage;
      }


      /// <summary>
      /// When overridden in a derived class, writes the content of the HTML body.
      /// </summary>
      /// <param name="xmlWriter">The XML writer to write the HTML body content to.</param>
      protected override void WriteBodyContent(XmlWriter xmlWriter)
      {
         base.WriteBodyContent(xmlWriter);

         // <p>errorMessage</p>
         xmlWriter.WriteStartElement("p");
         xmlWriter.WriteString(this.errorMessage);
         xmlWriter.WriteEndElement();
      }
   }
}