using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

using Arana.Web.Server.Html;

namespace Arana.Web.Server
{
   /// <summary>
   /// Contains extension methods useful to <see cref="WebServer"/>.
   /// </summary>
   internal static class Extensions
   {
      internal static DateTime? GetLastModified(this Assembly @assembly)
      {
         if (assembly == null)
         {
            throw new ArgumentNullException("assembly");
         }

         return (assembly.Location != null)
                   ? new FileInfo(assembly.Location).LastWriteTime
                   : null as DateTime?;
      }


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
      internal static void WriteIndex(this HttpListenerContext @context,
                                      Assembly assembly,
                                      string resourcePrefix)
      {
         if (context == null)
         {
            throw new ArgumentNullException("context");
         }

         IEnumerable<string> relativeFileNames =
            assembly.GetManifestResourceNames().Select(
               n => n.Substring(resourcePrefix.Length + 1));

         IndexDocument indexDocument = new IndexDocument(relativeFileNames);
         context.WriteHtml(indexDocument.ToString());
      }


      /// <summary>
      /// Returns the resource.
      /// </summary>
      /// <param name="context">The context.</param>
      /// <param name="assembly">The assembly.</param>
      /// <param name="resourcePrefix">The resource prefix.</param>
      internal static void WriteResource(this HttpListenerContext @context,
                                         Assembly assembly,
                                         string resourcePrefix)
      {
         if (context == null)
         {
            throw new ArgumentNullException("context");
         }

         Uri uri = context.Request.Url;
         string path = uri.LocalPath.Substring(1);
         string resourceName = String.Concat(resourcePrefix, ".", path);

         using (Stream dataStream = assembly.GetManifestResourceStream(resourceName))
         {
            using (Stream outputStream = context.Response.OutputStream)
            {
               if (dataStream == null)
               {
                  context.Response.StatusCode = (int) HttpStatusCode.NotFound;

                  ErrorDocument errorDocument = new ErrorDocument(
                     "Not found",
                     String.Format("'{0}' was not found.", uri.LocalPath),
                     outputStream);

                  errorDocument.Flush();
                  return;
               }

               // TODO: Add support for more media types
               context.Response.ContentType = "text/html; charset=utf-8";
               context.Response.ContentEncoding = Encoding.UTF8;
               context.Response.ContentLength64 = dataStream.Length;

               dataStream.CopyTo(outputStream);
            }
         }
      }


      /// <summary>
      /// Copies the stream.
      /// </summary>
      /// <param name="from">The <see cref="Stream"/> to copy from.</param>
      /// <param name="to">The <see cref="Stream"/> to copy to.</param>
      private static void CopyTo(this Stream @from, Stream to)
      {
         if (from == null)
         {
            throw new ArgumentNullException("from");
         }

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
      private static void Write(this HttpListenerContext @context,
                                string message,
                                HttpStatusCode statusCode)
      {
         if (context == null)
         {
            throw new ArgumentNullException("context");
         }

         context.Response.StatusCode = (int) statusCode;
         context.Response.ContentEncoding = Encoding.UTF8;
         context.WriteHtml(message);
      }


      /// <summary>
      /// Sends the string data.
      /// </summary>
      /// <param name="result">The result.</param>
      /// <param name="context">The context.</param>
      private static void WriteHtml(this HttpListenerContext @context, string result)
      {
         if (context == null)
         {
            throw new ArgumentNullException("context");
         }

         context.Response.ContentType = "text/html; charset=utf-8";
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