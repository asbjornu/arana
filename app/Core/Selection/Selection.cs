using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Arana.Extensions;

using Fizzler.Systems.HtmlAgilityPack;

using HtmlAgilityPack;

namespace Arana
{
   /// <summary>
   /// A list of HTML elements selected with <see cref="AranaEngine.Select(System.String)" />.
   /// </summary>
   public partial class Selection
   {
      /// <summary>
      /// The <see cref="AranaEngine" /> to use for requests.
      /// </summary>
      private readonly AranaEngine engine;

      /// <summary>
      /// Contains the tag names of all HTML form elements that should be posted to
      /// the server when the form is submitted.
      /// </summary>
      private static readonly string[] FormElements =
         new[] { "input", "textarea", "button", "select" };

      /// <summary>
      /// Contains a comma separated list of the tag names of all HTML form elements
      /// that should be posted to the server when the form is submitted.
      /// </summary>
      private static readonly string FormElementsSelector = String.Join(",", FormElements);


      /// <summary>
      /// Initializes a new instance of the <see cref="Selection"/> class.
      /// </summary>
      /// <param name="engine">The engine.</param>
      /// <param name="cssSelector">The CSS selector.</param>
      internal Selection(AranaEngine engine, string cssSelector)
         : this(engine.QuerySelectorAll(cssSelector), engine)
      {
         CssSelector = cssSelector;
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="Selection"/> class.
      /// </summary>
      /// <param name="nodes">The nodes.</param>
      /// <param name="engine">The engine.</param>
      private Selection(IEnumerable<HtmlNode> nodes, AranaEngine engine)
         : this(nodes)
      {
         this.engine = engine;
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="Selection"/> class.
      /// </summary>
      /// <param name="nodes">The nodes.</param>
      private Selection(IEnumerable<HtmlNode> nodes)
      {
         this.nodes = new List<HtmlNode>(nodes);
      }


      /// <summary>
      /// Gets the <see cref="Selection"/> with the specified <param name="tagNames"/>.
      /// </summary>
      /// <value>
      /// The <see cref="Selection"/> with the specified <param name="tagNames"/>.
      /// </value>
      /// <exception cref="ArgumentNullException">
      /// If <paramref name="tagNames"/> is null or empty.
      /// </exception>
      public Selection this[params string[] tagNames]
      {
         get
         {
            if ((tagNames == null) || (tagNames.Length == 0))
            {
               throw new ArgumentNullException("tagNames");
            }

            IEnumerable<HtmlNode> v = from htmlNode in this
                                      where htmlNode.Name.IsEqualTo(tagNames)
                                      select htmlNode;

            return new Selection(v, this.engine);
         }
      }


      /// <summary>
      /// Gets the inner text of the currently selected list of elements.
      /// </summary>
      /// <value>The inner text of the currently selected list of elements.</value>
      public string InnerText
      {
         get
         {
            StringBuilder sb = new StringBuilder();

            foreach (HtmlNode htmlNode in this)
            {
               sb.AppendLine(htmlNode.InnerText);
            }

            return sb.ToString().Trim();
         }
      }

      /// <summary>
      /// Gets the tag name of the first element in the currently selected list of elements.
      /// </summary>
      /// <returns>The tag name of the first element in the currently selected list of elements.</returns>
      public string TagName
      {
         get { return (Count == 0) ? null : this[0].Name; }
      }

      /// <summary>
      /// Gets the CSS selector that was used to select the elements in the list of currently selected elements.
      /// </summary>
      /// <value>The CSS selector.</value>
      internal string CssSelector { get; private set; }


      /// <summary>
      /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </returns>
      public override string ToString()
      {
         return String.Join(String.Concat(',', Environment.NewLine),
                            this.Select(node => node.GetCssSelector()).ToArray());
      }


      /// <summary>
      /// Gets the inner HTML of the currently selected list of elements.
      /// </summary>
      /// <value>The inner HTML of the currently selected list of elements.</value>
      public string InnerHtml()
      {
         StringBuilder sb = new StringBuilder();

         foreach (HtmlNode htmlNode in this)
         {
            sb.AppendLine(htmlNode.InnerHtml);
         }

         return sb.ToString().Trim();
      }


      /// <summary>
      /// Sets the inner HTML of the currently selected list of elements.
      /// </summary>
      /// <value>The list of currently selected elements.</value>
      public Selection InnerHtml(string html)
      {
         foreach (HtmlNode htmlNode in this)
         {
            htmlNode.InnerHtml = html;
         }

         return this;
      }


      /// <summary>
      /// Selects a list of elements matching the given CSS selector.
      /// </summary>
      /// <param name="cssSelector">The CSS selector.</param>
      /// <returns>
      /// A list of elements matching the given CSS selector.
      /// </returns>
      public Selection Select(string cssSelector)
      {
         List<HtmlNode> selection = new List<HtmlNode>();

         foreach (HtmlNode node in this)
         {
            selection.AddRange(node.QuerySelectorAll(cssSelector));
         }

         return new Selection(selection, this.engine);
      }


      /// <summary>
      /// Selects elements with the specified tag name.
      /// </summary>
      /// <param name="tagNames">Name of the tags.</param>
      /// <returns>
      /// A <see cref="Selection"/> of the elements with the specified <paramref name="tagNames"/>.
      /// </returns>
      private Selection Get(params string[] tagNames)
      {
         if (Count == 0)
         {
            throw new InvalidOperationException(
               String.Format("No elements selected with '{0}'.", CssSelector));
         }

         Selection selection = this[tagNames];

         if (selection.Count == 0)
         {
            string tags = String.Join(", ", tagNames);
            string message = String.Format("The tags '{0}' was not found in the current "
                                           + "selection of elements selected with '{1}'",
                                           tags,
                                           CssSelector);

            throw new InvalidOperationException(message);
         }

         return selection;
      }


      /// <summary>
      /// Gets a <see cref="RequestDictionary"/> containing the names of all
      /// the form elements within the collection, with either the current values
      /// of the elements or with the value provided in the specified
      /// <paramref name="formElementsSelection"/>, matched by the key.
      /// </summary>
      /// <param name="formElementsSelection">The request values.
      /// A <see cref="NameValueCollection"/> containing the names of all the
      /// elements that should have the corresponding value set.</param>
      /// <returns>
      /// The merged <see cref="RequestDictionary"/> with the values from
      /// <paramref name="formElementsSelection"/> applied.
      /// </returns>
      private RequestDictionary MergeRequestDictionary(Preselection formElementsSelection)
      {
         RequestDictionary requestDictionary = new RequestDictionary(Count);
         RequestDictionary formValueDictionary =
            (formElementsSelection != null)
               ? formElementsSelection.Invoke(this.engine)
               : null;

         foreach (HtmlNode node in this)
         {
            string name = node.Attributes.Get("name");

            // If the name should be empty for whatever reason, skip
            if (String.IsNullOrEmpty(name))
            {
               continue;
            }

            // If the node is a checkbox or radio button that isn't checked, skip it
            if (node.IsRadioOrCheckbox() && !node.IsChecked())
            {
               continue;
            }

            // If the node is a button and isn't set for submission, skip it
            if (node.IsButton() && !node.IsSetForSubmission())
            {
               continue;
            }

            string valueFromAttribute = node.Attributes.Get("value");
            string value = (formValueDictionary != null)
                              ? formValueDictionary.Get(name, valueFromAttribute)
                              : valueFromAttribute;

            requestDictionary.Set(name, value);
         }

         return requestDictionary;
      }
   }
}