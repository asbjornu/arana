﻿using System;
using System.IO;
using System.Net;

namespace Arana.Core
{
   /// <summary>
   /// Provides a specialized and simplified wrapper around <see cref="HttpWebResponse" />.
   /// </summary>
   internal class AranaResponse : IDisposable
   {
      private readonly HttpWebResponse response;


      /// <summary>
      /// Initializes a new instance of the <see cref="AranaResponse"/> class.
      /// </summary>
      /// <param name="request">The request.</param>
      /// <param name="getHttpWebResponse">The get HTTP web response.</param>
      internal AranaResponse(AranaRequest request, Func<HttpWebResponse> getHttpWebResponse)
      {
         if (getHttpWebResponse == null)
            throw new ArgumentNullException("getHttpWebResponse");

         this.response = getHttpWebResponse();

         if (this.response == null)
            throw new InvalidOperationException(
               String.Format("The URI '{0}' did not make much sense, sorry.",
                             request.RequestUri));

         request.Cookies = this.response.Cookies;
      }


      /// <summary>
      /// Gets the status of the response.
      /// </summary>
      /// <value>One of the <see cref="T:System.Net.HttpStatusCode"/> values.</value>
      public HttpStatusCode StatusCode
      {
         get { return this.response.StatusCode; }
      }

      /// <summary>
      /// Gets the response string.
      /// </summary>
      /// <returns></returns>
      internal string ResponseString
      {
         get
         {
            using (Stream stream = this.response.GetResponseStream())
            {
               using (StreamReader streamReader = new StreamReader(stream))
               {
                  return streamReader.ReadToEnd();
               }
            }
         }
      }


      /// <summary>
      /// Disposes the underlying <see cref="HttpWebResponse" />.
      /// </summary>
      public void Dispose()
      {
         ((IDisposable)this.response).Dispose();
      }
   }
}