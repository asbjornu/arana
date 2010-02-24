using System;
using System.IO;
using System.Xml;

namespace Arana.Web.Server.Html
{
   /// <summary>
   /// Represents a generic, abstract HTML document for <see cref="WebServer"/>.
   /// </summary>
   internal abstract class Document : IDisposable
   {
      private readonly XmlWriterSettings settings;
      private readonly Stream stream;
      private readonly string title;


      /// <summary>
      /// Initializes a new instance of the <see cref="IndexDocument"/> class.
      /// </summary>
      /// <param name="title">The title.</param>
      protected Document(string title)
      {
         if (title == null)
         {
            throw new ArgumentNullException("title");
         }

         this.title = title;
         this.settings = new XmlWriterSettings
         {
            Indent = true,
            OmitXmlDeclaration = true
         };
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="IndexDocument"/> class.
      /// </summary>
      /// <param name="title">The title.</param>
      /// <param name="stream">The stream.</param>
      protected Document(string title, Stream stream)
         : this(title)
      {
         if (stream == null)
         {
            throw new ArgumentNullException("stream");
         }

         this.stream = stream;
      }


      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      public override string ToString()
      {
         using (StringWriter stringWriter = new StringWriter())
         {
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, this.settings))
            {
               if (xmlWriter == null)
               {
                  throw new InvalidOperationException("Couldn't create XML writer.");
               }

               WriteTo(xmlWriter);
            }

            return stringWriter.ToString().Trim();
         }
      }


      /// <summary>
      /// When overridden in a derived class, writes the content of the HTML body.
      /// </summary>
      /// <param name="xmlWriter">The XML writer to write the HTML body content to.</param>
      protected virtual void WriteBodyContent(XmlWriter xmlWriter)
      {
         // <h1>Index</h1>
         xmlWriter.WriteElementString("h1", Xhtml.Namespace, this.title);
      }


      /// <summary>
      /// When overridden in a derived class, writes the content of the HTML head.
      /// </summary>
      /// <param name="xmlWriter">The XML writer to write the HTML head content to.</param>
      protected virtual void WriteHeadContent(XmlWriter xmlWriter)
      {
         // <title>Index</title>
         xmlWriter.WriteElementString("title", Xhtml.Namespace, this.title);

         // <meta http-equiv="Content-Type" content="application/xhtml+xml; charset=utf-8" />
         WriteMeta(xmlWriter);
      }


      /// <summary>
      /// Flushes the <see cref="Document"/> to the underlying stream.
      /// </summary>
      public void Flush()
      {
         if (this.stream == null)
         {
            throw new InvalidOperationException(
               "Can't flush to the stream because it is null.");
         }

         using (XmlWriter xmlWriter = XmlWriter.Create(this.stream, this.settings))
         {
            if (xmlWriter == null)
            {
               throw new InvalidOperationException("Couldn't create XML writer.");
            }

            WriteTo(xmlWriter);
         }
      }


      /// <summary>
      /// Writes out the XML to the <paramref name="xmlWriter"/>.
      /// </summary>
      /// <param name="xmlWriter">The XML writer.</param>
      private void WriteTo(XmlWriter xmlWriter)
      {
         if (xmlWriter == null)
         {
            throw new ArgumentNullException("xmlWriter");
         }

         // <!DOCTYPE>
         xmlWriter.WriteDocType("html",
                                Xhtml.DocType.PublicIdentifier,
                                Xhtml.DocType.SystemIdentifier,
                                null);

         // <html xml:lang="en">
         xmlWriter.WriteStartElement("html", Xhtml.Namespace);
         xmlWriter.WriteAttributeString("lang", Xml.Namespace, "en");

         // <head>
         xmlWriter.WriteStartElement("head", Xhtml.Namespace);

         WriteHeadContent(xmlWriter);

         // </head>
         xmlWriter.WriteEndElement();

         // <body>
         xmlWriter.WriteStartElement("body", Xhtml.Namespace);

         WriteBodyContent(xmlWriter);

         // </body>
         xmlWriter.WriteEndElement();

         // </html>
         xmlWriter.WriteEndElement();
      }


      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
         Flush();
      }


      /// <summary>
      /// Writes the meta.
      /// </summary>
      /// <param name="xmlWriter">The XML writer.</param>
      private static void WriteMeta(XmlWriter xmlWriter)
      {
         // <meta http-equiv="Content-Type" content="application/xhtml+xml; charset=utf-8" />
         xmlWriter.WriteStartElement("meta");
         xmlWriter.WriteAttributeString("http-equiv", "Content-Type");
         xmlWriter.WriteAttributeString("content",
                                        "application/xhtml+xml; charset=utf-8");
         xmlWriter.WriteEndElement();
      }
   }
}