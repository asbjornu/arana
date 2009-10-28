using System;
using System.Collections.Generic;

namespace Arana.Core
{
   /// <summary>
   /// An implementation of <see cref="List{Request}" />.
   /// </summary>
   internal class RequestList : List<Request>
   {
      private readonly AranaEngine engine;


      /// <summary>
      /// Initializes a new instance of the <see cref="RequestList"/> class.
      /// </summary>
      public RequestList(AranaEngine engine)
      {
         Index = -1;
         this.engine = engine;
      }


      /// <summary>
      /// Gets the <see cref="Request" /> in the list at the index of <see cref="Index" />.
      /// </summary>
      /// <value>The <see cref="Request" /> in the list at the index of <see cref="Index" />.</value>
      public Request Current
      {
         get { return ((Count == 0) || (Count < Index)) ? null : this[Index]; }
      }


      /// <summary>
      /// Gets or sets the index of the current <see cref="Request" />.
      /// </summary>
      /// <value>The index of the current <see cref="Request" />.</value>
      public int Index { get; private set; }


      /// <summary>
      /// Adds the specified request.
      /// </summary>
      /// <param name="request">The request.</param>
      public new void Add(Request request)
      {
         Index++;
         base.Add(request);
      }


      /// <summary>
      /// Navigates the specified number of <paramref name="steps"/> within the
      /// request history by increasing <see cref="Index" /> with <paramref name="steps"/>.
      /// </summary>
      /// <param name="steps">The relative number of steps to navigate from the
      /// current <see cref="Index"/>. A positive number navigates forward,
      /// a negative number navigates backward.</param>
      public void Navigate(int steps)
      {
         int resultingRequestIndex = ValidateSteps(steps);

         string message = String.Format("Navigating {0} step{1} {2} from {3} to {4}.",
                                        steps < 0 ? steps * -1 : steps,
                                        steps > 1 ? "s" : String.Empty,
                                        steps > 0 ? "forward" : "back",
                                        this[Index].Uri,
                                        this[resultingRequestIndex].Uri);

         this.engine.WriteToOutput(message, "Navigate");

         Index = resultingRequestIndex;
      }


      /// <summary>
      /// Gets the index of a non-redirecting request.
      /// </summary>
      /// <param name="steps">The relative number of steps to navigate from the
      /// current <see cref="Index"/>. A positive number navigates forward,
      /// a negative number navigates backward.</param>
      /// <param name="currentIndex">Index of the current.</param>
      /// <returns>The index of a non-redirecting request.</returns>
      private int GetIndexOfNonRedirectingRequest(int steps, int currentIndex)
      {
         while ((this[currentIndex].Response.StatusBase == 300)
                // Or until we've iterated us down to the first request
                && (currentIndex >= 0)
                // Or we've iterated us up to the last request.
                && (currentIndex < Count))
         {
            // If the given number of steps to navigate was negative, decrease the index.
            if (steps < 0)
            {
               currentIndex--;
            }
               // If the given number of steps to navigate was positive, increase the index.
            else
            {
               currentIndex++;
            }
         }

         return currentIndex;
      }


      /// <summary>
      /// Returns the index of the <see cref="Request"/> that corresponds to
      /// the relative number of <paramref name="steps"/> to navigate from the
      /// current <see cref="Index"/>.
      /// </summary>
      /// <param name="steps">The relative number of steps to navigate from the
      /// current <see cref="Index"/>. A positive number navigates forward,
      /// a negative number navigates backward.</param>
      /// <returns>
      /// The index of the <see cref="Request"/> that corresponds to the relative
      /// number of <paramref name="steps"/> to navigate from the current
      /// <see cref="Index"/>.
      /// </returns>
      private int ValidateSteps(int steps)
      {
         string message;
         int resultingRequestIndex = Index + steps;

         if (resultingRequestIndex < 0)
         {
            message = String.Format(
               "Can't navigate back {0} step{1}, as there's only {2} 'historical' requests to navigate to.",
               steps * -1,
               steps > 1 ? "s" : String.Empty,
               ((Count - 1) - Index));

            this.engine.WriteToOutput(message, "Back");

            throw new ArgumentOutOfRangeException("steps", steps, message);
         }

         if (resultingRequestIndex > (Count - 1))
         {
            message = String.Format(
               "Can't navigate forward {0} step{1}, as there's only {2} 'future' requests to navigate to.",
               steps,
               steps > 1 ? "s" : String.Empty,
               ((Count - 1) - Index));

            this.engine.WriteToOutput(message, "Forward");

            throw new ArgumentOutOfRangeException("steps", steps, message);
         }

         // Move back or forward in the history until a request that didn't
         // result in a redirect (HTTP Status Code 30X) is found.
         resultingRequestIndex = GetIndexOfNonRedirectingRequest(steps,
                                                                 resultingRequestIndex);

         return resultingRequestIndex;
      }
   }
}