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
      /// <param name="steps">The number of steps steps to navigate. A positive
      /// number navigates forward; negative backward.</param>
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
      /// Validates the steps.
      /// </summary>
      /// <param name="steps">The steps.</param>
      /// <returns></returns>
      private int ValidateSteps(int steps)
      {
         string message;
         int resultingRequestIndex = Index + steps;

         if (resultingRequestIndex < 0)
         {
            message = String.Format(
               "Can't navigate back {0} step{1}, as there's only {2} \"historical\" requests to navigate to.",
               steps * -1,
               steps > 1 ? "s" : String.Empty,
               ((Count - 1) - Index));

            this.engine.WriteToOutput(message, "Back");

            throw new ArgumentOutOfRangeException("steps", steps, message);
         }

         if (resultingRequestIndex > (Count - 1))
         {
            message = String.Format(
               "Can't navigate forward {0} step{1}, as there's only {2} \"future\" requests to navigate to.",
               steps,
               steps > 1 ? "s" : String.Empty,
               ((Count - 1) - Index));

            this.engine.WriteToOutput(message, "Forward");

            throw new ArgumentOutOfRangeException("steps", steps, message);
         }

         return resultingRequestIndex;
      }
   }
}