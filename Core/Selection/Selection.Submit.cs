using System;
using System.Collections.Specialized;
using Arana.Core.Extensions;

namespace Arana.Core
{
   public partial class Selection
   {
      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="formElementsSelection">The form elements selection.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain a 'form' element.
      /// 2. If a currently selected 'form' element doesn't have any attributes.
      /// 3. If a currently selected 'form' element has an empty or non-existent 'action' attribute.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      /// 	<paramref name="formElementsSelection"/> is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// 	<paramref name="formElementsSelection"/> is empty.
      /// </exception>
      /// <example>
      /// 	<code>
      /// selection.Submit(new Preselection
      /// {
      ///   { "input[name='product']", input =&gt; input.Value("NewProduct") },
      ///   { "input[name='type']", input =&gt; input.Value("NewType") },
      /// });
      /// </code>
      /// </example>
      public AranaEngine Submit(Preselection formElementsSelection)
      {
         if (formElementsSelection == null)
            throw new ArgumentNullException("formElementsSelection");

         if (formElementsSelection.Count == 0)
            throw new ArgumentException("Preselection can't be empty.",
                                        "formElementsSelection");

         return Submit(true, formElementsSelection.Invoke(this.engine));
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain a 'form' element.
      /// 2. If a currently selected 'form' element doesn't have any attributes.
      /// 3. If a currently selected 'form' element has an empty or non-existent 'action' attribute.
      /// </exception>
      public AranaEngine Submit()
      {
         return Submit(true, null);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="requestValues">The request values.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain a 'form' element.
      /// 2. If a currently selected 'form' element doesn't have any attributes.
      /// 3. If a currently selected 'form' element has an empty or non-existent 'action' attribute.
      /// </exception>
      public AranaEngine Submit(NameValueCollection requestValues)
      {
         return Submit(true, requestValues);
      }

      /// <summary>
      /// Checks a selected radio button or checkbox to be submitted with
      /// <see cref="Submit()" />.
      /// </summary>
      /// <returns>The value of the radio button or checkbox' 'value' attribute.</returns>
      public Selection Check()
      {
         Selection input = Select("input");
         string value = input.Value();

         if (String.IsNullOrEmpty(value))
            throw new InvalidOperationException(
               String.Format("The input field '{0}' does not have a valid 'value'.",
                             input));

         return input.Value(value);
      }

      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <param name="requestValues">The request values.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 	<list type="number">
      /// 		<item>If the currently selected elements doesn't contain a 'form' element.</item>
      /// 		<item>If a currently selected 'form' element has an empty or non-existent 'action' attribute and an action URI can't be deduced from a previous <see cref="AranaRequest"/>.</item>
      /// 	</list>
      /// </exception>
      public AranaEngine Submit(bool followRedirect, NameValueCollection requestValues)
      {
         Selection form = Select("form");

         string method = form.Attribute("method");
         // Set the URI to post the form to. Default to the current URI.
         string action = form.Attribute("action").NullWhenEmpty() ??
                         this.engine.Uri.ToString();

         if (String.IsNullOrEmpty(action))
            throw new InvalidOperationException("No valid 'action' to perform.");

         RequestDictionary requestDictionary = null;

         // Set the HTTP method. Default to "GET".
         method = String.IsNullOrEmpty(method)
                     ? HttpMethod.Get
                     : method.ToUpperInvariant();


         // TODO: Use GetFormElementsCssSelector() when Fizzler supports it.
         Selection formElements = this.engine.Select(FormElementsSelector);

         if (formElements.Count > 0)
            requestDictionary = formElements.GetRequestCollection(requestValues);

         this.engine.NavigateTo(action, followRedirect, method, requestDictionary);

         return this.engine;
      }
   }
}