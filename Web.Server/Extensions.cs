using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Arana.Web.Server
{
   /// <summary>
   /// Contains extension methods useful to <see cref="WebServer"/>.
   /// </summary>
   internal static class Extensions
   {
      /// <summary>
      /// Returns a <see cref="System.String"/> that represents the <paramref name="uri"/>.
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <returns>
      /// A <see cref="System.String"/> that represents the <paramref name="uri"/>.
      /// </returns>
      internal static string GetStringValue(this Uri @uri)
      {
         if (uri == null)
         {
            throw new ArgumentNullException("uri");
         }

         return uri.ToString();
      }


      /// <summary>
      /// Returns the index.
      /// </summary>
      /// <param name="context">The context.</param>
      /// <param name="assembly">The assembly.</param>
      /// <param name="resourcePrefix">The resource prefix.</param>
      internal static void WriteIndex(this HttpListenerContext context,
                                      Assembly assembly,
                                      string resourcePrefix)
      {
         XmlWriterSettings settings = new XmlWriterSettings
         {
            Indent = true,
            OmitXmlDeclaration = true
         };

         IEnumerable<string> relativeNames =
            assembly.GetManifestResourceNames().Select(
               n => n.Substring(resourcePrefix.Length + 1));

         using (StringWriter stringWriter = new StringWriter())
         {
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
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
               xmlWriter.WriteStartElement("meta");
               xmlWriter.WriteAttributeString("http-equiv", "Content-Type");
               xmlWriter.WriteAttributeString("content",
                                              "application/xhtml+xml; charset=utf-8");
               xmlWriter.WriteEndElement();

               // </head>
               xmlWriter.WriteEndElement();

               // <body>
               xmlWriter.WriteStartElement("body", Xhtml.Namespace);

               // <ul>
               xmlWriter.WriteStartElement("ul", Xhtml.Namespace);

               foreach (string relativeName in relativeNames)
               {
                  // <li>
                  xmlWriter.WriteStartElement("li", Xhtml.Namespace);

                  // <a id="relativeName" href="#relativeName">relativeName</a>
                  xmlWriter.WriteStartElement("a", Xhtml.Namespace);
                  xmlWriter.WriteAttributeString("id", relativeName);
                  xmlWriter.WriteAttributeString("href", String.Concat('#', relativeName));
                  xmlWriter.WriteString(relativeName);
                  xmlWriter.WriteEndElement();

                  // </li>
                  xmlWriter.WriteEndElement();
               }

               // </ul>
               xmlWriter.WriteEndElement();

               // </body>
               xmlWriter.WriteEndElement();

               // </html>
               xmlWriter.WriteEndElement();
            }

            context.WriteHtml(stringWriter.ToString());
         }
      }


      /// <summary>
      /// Returns the resource.
      /// </summary>
      /// <param name="context">The context.</param>
      /// <param name="assembly">The assembly.</param>
      /// <param name="resourcePrefix">The resource prefix.</param>
      internal static void WriteResource(this HttpListenerContext context,
                                         Assembly assembly,
                                         string resourcePrefix)
      {
         Uri uri = context.Request.Url;
         string path = uri.LocalPath.Substring(1);
         string resourceName = String.Concat(resourcePrefix, ".", path);

         using (Stream dataStream = assembly.GetManifestResourceStream(resourceName))
         {
            if (dataStream == null)
            {
               context.Write("Error: file " + path + " was not found",
                             HttpStatusCode.NotFound);
               return;
            }

            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = dataStream.Length;

            using (Stream outputStream = context.Response.OutputStream)
            {
               dataStream.CopyTo(outputStream);
            }
         }
      }


      /// <summary>
      /// Copies the stream.
      /// </summary>
      /// <param name="from">The <see cref="Stream"/> to copy from.</param>
      /// <param name="to">The <see cref="Stream"/> to copy to.</param>
      private static void CopyTo(this Stream from, Stream to)
      {
         byte[] buffer = new byte[32768];

         while (true)
         {
            int read = from.Read(buffer, 0, buffer.Length);

            if (read <= 0)
            {
               return;
            }

            to.Write(buffer, 0, read);
         }
      }


      /// <summary>
      /// Returns the file not found.
      /// </summary>
      /// <param name="context">The context.</param>
      /// <param name="message">The message.</param>
      /// <param name="statusCode">The status code.</param>
      private static void Write(this HttpListenerContext context,
                                string message,
                                HttpStatusCode statusCode)
      {
         context.Response.StatusCode = (int) statusCode;
         context.Response.ContentEncoding = Encoding.UTF8;
         context.WriteHtml(message);
      }


      /// <summary>
      /// Sends the string data.
      /// </summary>
      /// <param name="result">The result.</param>
      /// <param name="context">The context.</param>
      private static void WriteHtml(this HttpListenerContext context, string result)
      {
         context.Response.ContentType = "text/html";
         context.Response.ContentEncoding = Encoding.UTF8;

         using (context.Response.OutputStream)
         {
            using (StreamWriter writer =
               new StreamWriter(context.Response.OutputStream, Encoding.UTF8))
            {
               writer.Write(result);
            }
         }
      }
   }
}