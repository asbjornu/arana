using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Arana.Web.Server
{
   /// <summary>
   /// Represents an Index document for <see cref="WebServer"/>.
   /// </summary>
   internal class IndexDocument
   {
      private readonly IEnumerable<string> files;
      private readonly XmlWriterSettings settings;


      /// <summary>
      /// Initializes a new instance of the <see cref="IndexDocument"/> class.
      /// </summary>
      /// <param name="files">The files.</param>
      public IndexDocument(IEnumerable<string> files)
      {
         this.files = files;

         this.settings = new XmlWriterSettings
         {
            Indent = true,
            OmitXmlDeclaration = true
         };
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

               // <title>Index</title>
               xmlWriter.WriteElementString("title", Xhtml.Namespace, "Index");

               // <meta http-equiv="Content-Type" content="application/xhtml+xml; charset=utf-8" />
               WriteMeta(xmlWriter);

               // </head>
               xmlWriter.WriteEndElement();

               // <body>
               xmlWriter.WriteStartElement("body", Xhtml.Namespace);

               // <h1>Index</h1>
               xmlWriter.WriteElementString("h1", Xhtml.Namespace, "Index");

               // <ul>
               xmlWriter.WriteStartElement("ul", Xhtml.Namespace);

               foreach (string file in this.files)
               {
                  // <li><a id="relativeName" href="#relativeName">relativeName</a></li>
                  WriteListItem(xmlWriter, file);
               }

               // </ul>
               xmlWriter.WriteEndElement();

               // </body>
               xmlWriter.WriteEndElement();

               // </html>
               xmlWriter.WriteEndElement();
            }

            return stringWriter.ToString().Trim();
         }
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