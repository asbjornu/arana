using System;
using Arana.Core.Extensions;

namespace Arana.Core
{
   public partial class Selection
   {
      /// <summary>
      /// Checks a selected radio button or checkbox to be submitted with
      /// <see cref="Submit()" />.
      /// </summary>
      /// <returns>The value of the radio button or checkbox' 'value' attribute.</returns>
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
         Get("select").Each(select => select.SetSelectedIndex(index));
         return this;
      }
   }
}