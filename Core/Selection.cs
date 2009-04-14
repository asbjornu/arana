using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Arana.Core.Extensions;

using HtmlAgilityPack;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Arana.Core
{
   /// <summary>
   /// A list of HTML elements selected with <see cref="AranaEngine.Select(System.String)" />.
   /// </summary>
   public partial class Selection : List<HtmlNode>
   {
      /// <summary>
      /// Contains the tag names of all HTML form elements that should be posted to
      /// the server when the form is submitted.
      /// </summary>
      private static readonly string[] FormElements = new[]
      {
         "input", "textarea", "button", "select"
      };

      /// <summary>
      /// Contains a comma separated list of the tag names of all HTML form elements
      /// that should be posted to the server when the form is submitted.
      /// </summary>
      private static readonly string FormElementsSelector = String.Join(",", FormElements);

      /// <summary>
      /// The <see cref="AranaEngine" /> to use for requests.
      /// </summary>
      private readonly AranaEngine engine;


      /// <summary>
      /// Initializes a new instance of the <see cref="Selection"/> class.
      /// </summary>
      /// <param name="engine">The engine.</param>
      /// <param name="cssSelector">The CSS selector.</param>
      internal Selection(AranaEngine engine, string cssSelector)
         : this(engine.SelectorEngine.Parse(cssSelector), engine)
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
         : base(nodes)
      {
      }


      /// <summary>
      /// Gets the <see cref="Selection"/> with the specified <param name="tagName"/>.
      /// </summary>
      /// <value>
      /// The <see cref="Selection"/> with the specified <param name="tagName"/>.
      /// </value>
      /// <exception cref="ArgumentNullException">
      /// If <paramref name="tagName"/> is null or empty.
      /// </exception>
      public Selection this[string tagName]
      {
         get
         {
            if (String.IsNullOrEmpty(tagName))
               throw new ArgumentNullException("tagName");

            var v = from htmlNode in this
                    where htmlNode.NameIsEqualTo(tagName)
                    select htmlNode;

            return new Selection(v, this.engine);
         }
      }


      /// <summary>
      /// Gets the inner HTML of the currently selected list of elements.
      /// </summary>
      /// <value>The inner HTML of the currently selected list of elements.</value>
      public string InnerHtml
      {
         get
         {
            StringBuilder sb = new StringBuilder();

            foreach (HtmlNode htmlNode in this)
               sb.AppendLine(htmlNode.InnerHtml);

            return sb.ToString().Trim();
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
               sb.AppendLine(htmlNode.InnerText);

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
      /// Follows the 'href' attribute on the selected HTML elements.
      /// </summary>
      /// <returns>An updated <see cref="AranaEngine" />.</returns>
      /// <exception cref="InvalidOperationException">
      /// 1. If the currently selected elements doesn't contain an 'a' element.
      /// 2. If a currently selected 'a' element doesn't have any attributes.
      /// 3. If a currently selected 'a' element has an empty or non-existent 'href' attribute.
      /// </exception>
      public AranaEngine Follow()
      {
         return Follow(true);
      }


      /// <summary>
      /// Follows the 'href' attribute on the selected HTML elements.
      /// </summary>
      /// <param name="followRedirect"><c>true</c> if the request should automatically follow redirection responses from the
      /// Internet resource; otherwise, <c>false</c>. The default value is true.</param>
      /// <returns>An updated <see cref="AranaEngine"/>.</returns>
      /// <exception cref="InvalidOperationException">
      /// 	<list type="number">
      /// 		<item>If the currently selected elements doesn't contain an 'a' element.</item>
      /// 		<item>If a currently selected 'a' element doesn't have any attributes.</item>
      /// 		<item>If a currently selected 'a' element has an empty or non-existent 'href' attribute.</item>
      /// 	</list>
      /// </exception>
      public AranaEngine Follow(bool followRedirect)
      {
         if (Count == 0)
            throw new InvalidOperationException(
               String.Format("No elements selected with '{0}'.", CssSelector));

         Selection anchors = this["a"];

         if (anchors.Count == 0)
            throw new InvalidOperationException("The selected element(s) does not contain an HTML 'a' element.");

         if ((anchors.Attributes() == null) || (anchors.Attributes().Count() == 0))
            throw new InvalidOperationException("The HTML anchor has no attributes.");

         string href = anchors.Attribute("href");

         if (String.IsNullOrEmpty(href))
            throw new InvalidOperationException("The HTML anchor has an empty 'href' attribute.");

         this.engine.NavigateTo(href, followRedirect);

         return this.engine;
      }


      /// <summary>
      /// Gets the CSS selector for the first element in the list of currently selected elements.
      /// </summary>
      /// <returns>
      /// The CSS selector for the first element in the list of currently selected elements.
      /// </returns>
      public string GetCssSelector()
      {
         return (Count == 0) ? null : this[0].GetCssSelector();
      }


      /// <summary>
      /// Returns the very next sibling for each <see cref="T:HtmlAgilityPack.HtmlNode"/>
      /// in the list of currently selected elements.
      /// </summary>
      /// <returns>
      /// The very next sibling for each <see cref="T:HtmlAgilityPack.HtmlNode"/>
      /// in the list of currently selected elements.
      /// </returns>
      public Selection Next()
      {
         var v = from htmlNode in this
                 let nextSibling = htmlNode.NextSibling
                 where (nextSibling != null)
                 select nextSibling;

         return new Selection(new List<HtmlNode>(v), this.engine);
      }


      /// <summary>
      /// Returns the very previous sibling for each <see cref="T:HtmlAgilityPack.HtmlNode"/>
      /// in the list of currently selected elements.
      /// </summary>
      /// <returns>
      /// The very previous sibling for each <see cref="T:HtmlAgilityPack.HtmlNode"/>
      /// in the list of currently selected elements.
      /// </returns>
      public Selection Previous()
      {
         var v = from htmlNode in this
                 let nextSibling = htmlNode.PreviousSibling
                 where (nextSibling != null)
                 select nextSibling;

         return new Selection(new List<HtmlNode>(v), this.engine);
      }


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
      /// selection.Submit(new Preselection {
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
         if (Count == 0)
            throw new InvalidOperationException(
               String.Format("No elements selected with '{0}'.", CssSelector));

         Selection form = this["form"];

         if (form.Count == 0)
            throw new InvalidOperationException(
               String.Format("The elements selected with '{0}' does not contain an HTML 'form' element.",
                             CssSelector));

         string method = form.Attribute("method");
         string action = form.Attribute("action");
         RequestDictionary requestDictionary = null;

         // Set the HTTP method. Default to "GET".
         method = String.IsNullOrEmpty(method)
                     ? HttpMethod.Get
                     : method.ToUpperInvariant();

         // Set the URI to post the form to. Default to the current URI.
         action = String.IsNullOrEmpty(action)
                     ? this.engine.Uri.ToString()
                     : action;

         if (String.IsNullOrEmpty(action))
            throw new InvalidOperationException("No valid 'action' to perform.");

         // TODO: Use GetFormElementsCssSelector() when Fizzler supports it.
         Selection formElements = this.engine.Select(FormElementsSelector);

         if (formElements.Count > 0)
            requestDictionary = formElements.GetRequestCollection(requestValues);

         this.engine.NavigateTo(action, followRedirect, method, requestDictionary);

         return this.engine;
      }


      /// <summary>
      /// Gets the CSS selector required to select all form elements children of the currently
      /// selected 'form' element.
      /// </summary>
      /// <returns>
      /// The CSS selector required to select all form elements children of the currently
      /// selected 'form' element.
      /// </returns>
      private string GetFormElementsCssSelector()
      {
         string selector = GetCssSelector();

         StringBuilder sb = new StringBuilder();

         for (int i = 0; i < FormElements.Length; i++)
         {
            sb.Append(selector);
            sb.Append(' ');
            sb.Append(FormElements[i]);

            if (i < (FormElements.Length - 1))
               sb.Append(", ");
         }

         return sb.ToString();
      }


      /// <summary>
      /// Gets a <see cref="NameValueCollection"/> containing the names of all
      /// the form elements within the collection, with either the current values
      /// of the elements or with the value provided in the specified
      /// <paramref name="requestValues"/>, matched by the key.
      /// </summary>
      /// <param name="requestValues">The request values.
      /// A <see cref="NameValueCollection"/> containing the names of all the
      /// elements that should have the corresponding value set.</param>
      /// <returns>
      /// The merged <see cref="NameValueCollection"/> with the values from
      /// <paramref name="requestValues"/> applied.
      /// </returns>
      private RequestDictionary GetRequestCollection(NameValueCollection requestValues)
      {
         RequestDictionary requestDictionary = new RequestDictionary(Count);

         (from node in this
          let valueAttr = node.Attributes.Get("value")
          let name = node.Attributes.Get("name")
          let value = String.IsNullOrEmpty(name)
                         ? valueAttr
                         : (requestValues[name] ?? valueAttr)
          where !String.IsNullOrEmpty(value)
          select new { Name = name, Value = value }
         ).Each(result => requestDictionary.Set(result.Name, result.Value));

         return requestDictionary;
      }
   }
}