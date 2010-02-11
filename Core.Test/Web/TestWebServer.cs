using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Arana.Core.Test.Web
{
    class TestWebServer
    {
        protected Boolean Started = false;
        protected String ResourcePrefix;
        protected HttpListener Listener;

        public TestWebServer(String listenPrefix,  String resourcePrefix)
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add(listenPrefix);
            Listener.Start();
            ResourcePrefix = resourcePrefix;
            IAsyncResult result = this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);
        }

        protected void WebRequestCallback(IAsyncResult result)
        {

            if (this.Listener == null)
                return;
            try
            {
                HttpListenerContext context = this.Listener.EndGetContext(result);
                this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);
                this.ProcessRequest(context);
            } catch( HttpListenerException  e )
            {
                // ignore exception if we have been stopped
                if (Started)
                    throw;
            }
            
        }

        protected void ProcessRequest(HttpListenerContext context)
        {
            Uri uri = context.Request.Url;
            String path = uri.LocalPath;
            if( "/".Equals(path))
            {
                ReturnIndex(context);
            } else
            {
                ReturnResource(context);
                
            }
        }

        protected void ReturnFileNotFound(String path, HttpListenerContext context )
        {
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
            SendStringData("Error: file " + path + " was not found", context);                
        }

        protected void ReturnResource(HttpListenerContext context) 
        {
            
            Uri uri = context.Request.Url;
            String path = uri.LocalPath.Substring(1);
            String resourceName = ResourcePrefix + "." + path;
            Stream dataStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if( dataStream == null )
            {
                ReturnFileNotFound(path,context);
                return;
            }
            context.Response.ContentType = "text/html";
           
            context.Response.ContentLength64 = dataStream.Length;
            Stream outputStream = context.Response.OutputStream;          
            CopyStream(dataStream, outputStream);
            outputStream.Close();
            dataStream.Close();
        }

        protected void CopyStream(Stream from, Stream to)
        {
            byte[] buffer = new byte[32768];
            while (true)
            {
                int read = from.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                to.Write(buffer, 0, read);
            }

        }

        protected void ReturnIndex(HttpListenerContext context)
        {
            String[] list = Assembly.GetCallingAssembly().GetManifestResourceNames();
            StringBuilder sb = new StringBuilder();
            sb.Append("<HTML><HEAD><TITLE>INDEX</TITLE></HEAD>");
            sb.Append("<BODY>");
            foreach(String resource in list)
            {
                if( resource.StartsWith(ResourcePrefix))
                {
                    String relativeName = resource.Substring(ResourcePrefix.Length +1 );
                    sb.Append("<a href=\"" + relativeName + "\">" + relativeName +
                              "</a><br />");
                }
            }
            sb.Append("</BODY></HTML>");
            SendStringData(sb.ToString(),context);           
        }

        protected  void SendStringData(String result, HttpListenerContext context)
        {
            context.Response.ContentType = "text/html";            
            byte[] bOutput = System.Text.Encoding.UTF8.GetBytes(result);
            context.Response.ContentLength64 = bOutput.Length;
            Stream outputStream = context.Response.OutputStream;
            outputStream.Write(bOutput, 0, bOutput.Length);
            outputStream.Close();
        }


        public void Stop()
        {
            Started = false;
            if (Listener != null)
            {
                this.Listener.Close();
                ((IDisposable)this.Listener).Dispose();
                this.Listener = null;
            }

        }
    }    
}
