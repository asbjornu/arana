using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Arana.Core
{
   /// <summary>
   /// Dictionary used to connect CSS selectors and actions to perform on
   /// the <see cref="Selection" />s returned from them, typically in the
   /// <see cref="Selection.Submit(Arana.Core.Preselection)" /> method.
   /// </summary>
   public class Preselection : Dictionary<string, Func<Selection, Selection>>
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="Preselection"/> class.
      /// </summary>
      public Preselection()
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="Preselection"/> class.
      /// </summary>
      /// <param name="dictionary">The dictionary.</param>
      public Preselection(IDictionary<string, Func<Selection, Selection>> dictionary)
         : base(dictionary)
      {
         ValidateDictionary();
      }


      /// <summary>
      /// Adds the specified CSS selector and selection function to the dictionary.
      /// </summary>
      /// <param name="cssSelector">The CSS selector.</param>
      /// <param name="selectionFunc">The selection function.</param>
      public new void Add(string cssSelector, Func<Selection, Selection> selectionFunc)
      {
         Validate(cssSelector, selectionFunc);
         base.Add(cssSelector, selectionFunc);
      }


      /// <summary>
      /// Iterates through the dictionary and executes each CSS selector added
      /// in <see cref="Preselection.Keys"/> on the specified <paramref name="engine"/>
      /// and then invokes the function corresponding to the CSS selector with
      /// the <see cref="Selection"/> retrieved from the <paramref name="engine"/>.
      /// </summary>
      /// <param name="engine">The engine.</param>
      /// <returns></returns>
      internal NameValueCollection Invoke(AranaEngine engine)
      {
         NameValueCollection result = new NameValueCollection(Count);

         foreach (string cssSelector in Keys)
         {
            // Execute the CSS selector.
            Selection selection = engine.Select(cssSelector);

            if ((selection == null) || (selection.Count == 0))
               throw new InvalidOperationException(
                  String.Format("The CSS selector '{0}' returned nothing.",
                                cssSelector));

            Func<Selection, Selection> selectionFunc = this[cssSelector];

            // Invoke the function with the selection as argument
            selection = selectionFunc.Invoke(selection);

            if ((selection == null) || (selection.Count == 0))
               throw new InvalidOperationException(
                  String.Format("After invoking the function corresponding to the CSS selector '{0}', an empty selection was returned.",
                                cssSelector));

            // Get the name of the invoked selection
            string name = selection.Name();

            // Get the value of the invoked selection
            string value = selection.Value();

            result.Add(name, value);
         }

         return result;
      }


      /// <summary>
      /// Validates the specified CSS selector and selection function.
      /// </summary>
      /// <param name="cssSelector">The CSS selector.</param>
      /// <param name="selectionFunc">The selection function.</param>
      private static void Validate(string cssSelector, Func<Selection, Selection> selectionFunc)
      {
         if (String.IsNullOrEmpty(cssSelector))
            throw new InvalidOperationException(
               "The CSS selector provided to Preselection can't be null or empty.");

         if (selectionFunc == null)
            throw new InvalidOperationException(
               "The Selection function (lambda) provided to Preselection can't be null.");
      }


      /// <summary>
      /// Ensures the validity of the dictionary.
      /// </summary>
      private void ValidateDictionary()
      {
         foreach (string cssSelector in Keys)
         {
            Func<Selection, Selection> selectionFunc = this[cssSelector];
            Validate(cssSelector, selectionFunc);
         }
      }
   }
}