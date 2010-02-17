using System;

using Arana.Extensions;

using HtmlAgilityPack;

namespace Arana
{
   public partial class Selection
   {
      internal const string SetForSubmissionAttribute = "arana-set-for-submission";


      /// <summary>
      /// Checks a selected radio button or checkbox to be submitted with
      /// <see cref="Submit()"/>.
      /// </summary>
      /// <returns>
      /// The list of currently selected elements.
      /// </returns>
      public Selection Check()
      {
         Selection input = Get("input");
         string value = input.Value();

         if (String.IsNullOrEmpty(value))
         {
            value = "on";
         }

         return
            // Set the "checked" attribute
            input.Attribute("checked", "checked")
               // Set the value
               .Attribute("value", value);
      }


      /// <summary>
      /// Chooses the specified option index.
      /// </summary>
      /// <param name="index">The index of the option to choose.</param>
      /// <returns>The list of currently selected elements.</returns>
      public Selection Choose(int index)
      {
         foreach (HtmlNode select in Get("select"))
         {
            select.SetSelectedIndex(index);
         }

         return this;
      }


      /// <summary>
      /// Unchecks a selected radio button or checkbox to be submitted with
      /// <see cref="Submit()"/>.
      /// </summary>
      /// <returns>
      /// The list of currently selected elements.
      /// </returns>
      public Selection Uncheck()
      {
         Selection input = Get("input");

         return
            // Set the "checked" attribute to null
            input.Attribute("checked", null)
               // Set the value to null
               .Attribute("value", null);
      }


      /// <summary>
      /// Sets the selected submit button to be used to submit the form.
      /// </summary>
      /// <returns>The selected submit button.</returns>
      internal Selection SetForSubmission()
      {
         foreach (HtmlNode button in Get("input", "button"))
         {
            button.SetAttributeValue(SetForSubmissionAttribute,
                                     SetForSubmissionAttribute);
         }

         return this;
      }
   }
}