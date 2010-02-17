using System;

using Arana.Extensions;

namespace Arana
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
      /// 	<paramref name="formElementsSelection"/> is null or empty.
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
         if ((formElementsSelection == null) || (formElementsSelection.Count == 0))
         {
            throw new ArgumentNullException("formElementsSelection");
         }

         return Submit(true, null, formElementsSelection);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow
      /// redirection responses from the Internet resource; otherwise, <c>false</c>.</param>
      /// <param name="formElementsSelection">The form elements selection.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain a 'form' element.
      /// 2. If a currently selected 'form' element doesn't have any attributes.
      /// 3. If a currently selected 'form' element has an empty or non-existent 'action' attribute.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      /// 	<paramref name="formElementsSelection"/> is null or empty.
      /// </exception>
      /// <example>
      /// 	<code>
      /// selection.Submit(false, new Preselection
      /// {
      ///   { "input[name='product']", input =&gt; input.Value("NewProduct") },
      ///   { "input[name='type']", input =&gt; input.Value("NewType") },
      /// });
      /// </code>
      /// </example>
      public AranaEngine Submit(bool followRedirect, Preselection formElementsSelection)
      {
         if ((formElementsSelection == null) || (formElementsSelection.Count == 0))
         {
            throw new ArgumentNullException("formElementsSelection");
         }

         if (formElementsSelection.Count == 0)
         {
            throw new ArgumentException("Preselection can't be empty.",
                                        "formElementsSelection");
         }

         return Submit(followRedirect, null, formElementsSelection);
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
         return Submit(true, null, null);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="submitButtonCssSelector">The submit button CSS selector.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain a 'form' element.
      /// 2. If a currently selected 'form' element doesn't have any attributes.
      /// 3. If a currently selected 'form' element has an empty or non-existent 'action' attribute.
      /// </exception>
      /// <example>
      /// 	<code>
      /// selection.Submit("input#submit"));
      /// </code>
      /// </example>
      public AranaEngine Submit(string submitButtonCssSelector)
      {
         if (String.IsNullOrEmpty(submitButtonCssSelector))
         {
            throw new ArgumentNullException("submitButtonCssSelector");
         }

         return Submit(true, submitButtonCssSelector, null);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="submitButtonCssSelector">The submit button CSS selector.</param>
      /// <param name="formElementsSelection">The form elements selection.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain a 'form' element.
      /// 2. If a currently selected 'form' element doesn't have any attributes.
      /// 3. If a currently selected 'form' element has an empty or non-existent 'action' attribute.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      /// 	<paramref name="formElementsSelection"/> is null or empty.
      /// </exception>
      /// <exception cref="ArgumentNullException">
      /// 	<paramref name="submitButtonCssSelector"/> is null or empty.
      /// </exception>
      /// <example>
      /// 	<code>
      /// selection.Submit("input#submit", new Preselection
      /// {
      /// { "input[name='product']", input =&gt; input.Value("NewProduct") },
      /// { "input[name='type']", input =&gt; input.Value("NewType") },
      /// });
      /// </code>
      /// </example>
      public AranaEngine Submit(string submitButtonCssSelector,
                                Preselection formElementsSelection)
      {
         if (String.IsNullOrEmpty(submitButtonCssSelector))
         {
            throw new ArgumentNullException("submitButtonCssSelector");
         }

         if ((formElementsSelection == null) || (formElementsSelection.Count == 0))
         {
            throw new ArgumentNullException("formElementsSelection");
         }

         return Submit(true, submitButtonCssSelector, formElementsSelection);
      }


      /// <summary>
      /// Submits the selected 'form' element, given its 'action' attribute.
      /// </summary>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow
      /// redirection responses from the Internet resource; otherwise, <c>false</c>.</param>
      /// <param name="submitButtonCssSelector">The submit button CSS selector.</param>
      /// <param name="formElementsSelection">The form elements selection.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 	<list type="number">
      /// 		<item>If the currently selected elements doesn't contain a 'form' element.</item>
      /// 		<item>If a currently selected 'form' element has an empty or non-existent 'action'
      /// attribute and an action URI can't be deduced from a previous <see cref="Request"/>.</item>
      /// 	</list>
      /// </exception>
      public AranaEngine Submit(bool followRedirect,
                                string submitButtonCssSelector,
                                Preselection formElementsSelection)
      {
         Selection form = Get("form");
         string method = form.Attribute("method");

         // Set the URI to post the form to. Default to the current URI.
         string action = form.Attribute("action").NullWhenEmpty() ??
                         this.engine.Uri.ToString();

         if (String.IsNullOrEmpty(action))
         {
            throw new InvalidOperationException("No valid 'action' to perform.");
         }

         RequestDictionary requestDictionary = null;

         // Set the HTTP method. Default to "GET".
         method = (method.NullWhenEmpty() ?? HttpMethod.Get).ToUpperInvariant();

         Selection formElements = form.Select(FormElementsSelector);

         // If the submit button's CSS selector is set, add it to the form elements selection
         if (!String.IsNullOrEmpty(submitButtonCssSelector))
         {
            if (formElementsSelection == null)
            {
               formElementsSelection = new Preselection(1);
            }

            formElementsSelection.Add(submitButtonCssSelector,
                                      submit => submit.SetForSubmission());
         }

         if ((formElements.Count > 0) || (formElementsSelection.Count > 0))
         {
            requestDictionary = formElements.MergeRequestDictionary(formElementsSelection);
         }

         this.engine.Navigate(action, followRedirect, method, requestDictionary);

         return this.engine;
      }
   }
}