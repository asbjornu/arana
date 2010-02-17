using System;
using System.Net;
using System.Reflection;

namespace Arana.Web.Server
{
   /// <summary>
   /// A simple web server, serving up embedded resource files.
   /// </summary>
   public class WebServer : IDisposable
   {
      private readonly Assembly assembly;
      private readonly HttpListener listener;
      private readonly string resourcePrefix;
      private bool disposed;


      /// <summary>
      /// Initializes a new instance of the <see cref="WebServer"/> class.
      /// </summary>
      /// <param name="listeningAuthority">The Domain Name System (DNS) host name or IP
      /// address and the port number the <see cref="WebServer"/> should
      /// be listening on.</param>
      /// <param name="type">The <see cref="T:System.Type"/> whose namespace
      /// is used to scope the manifest resource name.</param>
      public WebServer(string listeningAuthority, Type type)
         : this(listeningAuthority, type, null)
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="WebServer"/> class.
      /// </summary>
      /// <param name="listeningAuthority">The Domain Name System (DNS) host name or IP
      /// address and the port number the <see cref="WebServer"/> should
      /// be listening on.</param>
      /// <param name="type">The <see cref="T:System.Type"/> whose namespace
      /// is used to scope the manifest resource name.</param>
      /// <param name="resourcePrefix">The prefix (typically represented by the folder(s)
      /// the resource files are placed within) of the resource files, excluding the
      /// namespace of <paramref name="type"/>.</param>
      public WebServer(string listeningAuthority, Type type, string resourcePrefix)
      {
         if (listeningAuthority == null)
         {
            throw new ArgumentNullException("listeningAuthority");
         }

         if (type == null)
         {
            throw new ArgumentNullException("type");
         }

         this.resourcePrefix = String.Join(".", new[] { type.Namespace, resourcePrefix });
         this.assembly = type.Assembly;

         this.listener = new HttpListener();
         this.listener.Prefixes.Add(listeningAuthority);
         this.listener.Start();
         this.listener.BeginGetContext(WebRequestCallback, this.listener);
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="WebServer"/> class.
      /// </summary>
      /// <param name="listeningAuthority">The Domain Name System (DNS) host name or IP
      /// address and the port number the <see cref="WebServer"/> should
      /// be listening on.</param>
      /// <param name="type">The <see cref="T:System.Type"/> whose namespace
      /// is used to scope the manifest resource name.</param>
      public WebServer(Uri listeningAuthority, Type type)
         : this(listeningAuthority, type, null)
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="WebServer"/> class.
      /// </summary>
      /// <param name="listeningAuthority">The Domain Name System (DNS) host name or IP
      /// address and the port number the <see cref="WebServer"/> should
      /// be listening on.</param>
      /// <param name="type">The <see cref="T:System.Type"/> whose namespace
      /// is used to scope the manifest resource name.</param>
      /// <param name="resourcePrefix">The prefix (typically represented by the folder(s)
      /// the resource files are placed within) of the resource files, excluding the
      /// namespace of <paramref name="type"/>.</param>
      public WebServer(Uri listeningAuthority, Type type, string resourcePrefix)
         : this(listeningAuthority.GetStringValue(), type, resourcePrefix)
      {
      }


      /// <summary>
      /// Processes the request.
      /// </summary>
      /// <param name="context">The context.</param>
      private void ProcessRequest(HttpListenerContext context)
      {
         Uri uri = context.Request.Url;
         string path = uri.LocalPath;

         if (path == "/")
         {
            context.WriteIndex(this.assembly, this.resourcePrefix);
         }
         else
         {
            context.WriteResource(this.assembly, this.resourcePrefix);
         }
      }


      /// <summary>
      /// Webs the request callback.
      /// </summary>
      /// <param name="result">The result.</param>
      private void WebRequestCallback(IAsyncResult result)
      {
         // Lock to prevent race conditions between Dispose() and the callback
         lock (this)
         {
            if ((this.listener == null) || this.disposed)
            {
               return;
            }
         }

         try
         {
            HttpListenerContext context = this.listener.EndGetContext(result);
            this.listener.BeginGetContext(WebRequestCallback, this.listener);
            ProcessRequest(context);
         }
         catch (HttpListenerException)
         {
            // ignore exception if we have been disposed
            if (!this.disposed)
            {
               throw;
            }
         }
      }


      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
         // Lock to prevent race conditions between Dispose() and the callback
         lock (this)
         {
            if (this.listener != null)
            {
               ((IDisposable) this.listener).Dispose();
            }

            this.disposed = true;
         }
      }
   }
}