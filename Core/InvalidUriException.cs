using System;

namespace Arana.Core
{
   /// <summary>
   /// The exception that is thrown when an URI is constructed from an invalid string.
   /// </summary>
   public class InvalidUriException : ApplicationException
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="InvalidUriException"/> class.
      /// </summary>
      /// <param name="uri">The invalid URI.</param>
      public InvalidUriException(Uri uri)
         : this(uri.ToString())
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="InvalidUriException"/> class.
      /// </summary>
      /// <param name="uri">The URI.</param>
      public InvalidUriException(string uri)
         : this(uri, String.Empty)
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="InvalidUriException"/> class.
      /// </summary>
      /// <param name="uri">The invalid URI.</param>
      /// <param name="innerException">The inner exception.</param>
      public InvalidUriException(Uri uri, Exception innerException)
         : this(uri.ToString(), innerException)
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="InvalidUriException"/> class.
      /// </summary>
      /// <param name="uri">The invalid URI.</param>
      /// <param name="message">The message.</param>
      public InvalidUriException(string uri, string message)
         : base(CreateMessage(uri, message))
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="InvalidUriException"/> class.
      /// </summary>
      /// <param name="uri">The invalid URI.</param>
      /// <param name="innerException">The inner exception.</param>
      public InvalidUriException(string uri, Exception innerException)
         : base(CreateMessage(uri, null), innerException)
      {
      }


      /// <summary>
      /// Creates the message to present in the exception.
      /// </summary>
      /// <param name="uri">The invalid URI.</param>
      /// <param name="message">The message.</param>
      /// <returns>The message</returns>
      private static string CreateMessage(string uri, string message)
      {
         return String.Format("The URI '{0}' is invalid. {1}", uri, message).Trim();
      }
   }
}