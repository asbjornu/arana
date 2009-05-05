﻿using System;
using System.Linq;
using Fizzler;
using HtmlAgilityPack;

namespace Arana.Core.Fizzler.Systems.HtmlAgilityPack
{
   /// <summary>
   /// An <see cref="IElementOps{TElement}"/> implementation for <see cref="HtmlNode"/>
   /// from <a href="http://www.codeplex.com/htmlagilitypack">HtmlAgilityPack</a>.
   /// </summary>
   internal class HtmlNodeOps : IElementOps<HtmlNode>
   {
      #region IElementOps<HtmlNode> Members

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#type-selectors">type selector</a>,
      /// which represents an instance of the element type in the document tree. 
      /// </summary>
      public Selector<HtmlNode> Type(string type)
      {
         return nodes => nodes.Elements().Where(n => n.Name == type);
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#universal-selector">universal selector</a>,
      /// any single element in the document tree in any namespace 
      /// (including those without a namespace) if no default namespace 
      /// has been specified for selectors. 
      /// </summary>
      public Selector<HtmlNode> Universal()
      {
         return nodes => nodes.Elements();
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#Id-selectors">ID selector</a>,
      /// which represents an element instance that has an identifier that 
      /// matches the identifier in the ID selector.
      /// </summary>
      public Selector<HtmlNode> Id(string id)
      {
         return nodes => nodes.Elements().Where(n => n.Id == id);
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#class-html">class selector</a>,
      /// which is an alternative <see cref="IElementOps{TElement}.AttributeIncludes"/> when 
      /// representing the <c>class</c> attribute. 
      /// </summary>
      public Selector<HtmlNode> Class(string clazz)
      {
         return
            nodes =>
            nodes.Elements().Where(n => n.GetAttributeValue("class", string.Empty)
                                           .Split(' ')
                                           .Contains(clazz));
      }

      /// <summary>
      /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
      /// that represents an element with the given attribute <paramref name="name"/>
      /// whatever the values of the attribute.
      /// </summary>
      public Selector<HtmlNode> AttributeExists(string name)
      {
         return nodes => nodes.Elements().Where(n => n.Attributes[name] != null);
      }

      /// <summary>
      /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
      /// that represents an element with the given attribute <paramref name="name"/>
      /// and whose value is exactly <paramref name="value"/>.
      /// </summary>
      public Selector<HtmlNode> AttributeExact(string name, string value)
      {
         return nodes => from n in nodes.Elements()
                         let a = n.Attributes[name]
                         where a != null && a.Value == value
                         select n;
      }

      /// <summary>
      /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
      /// that represents an element with the given attribute <paramref name="name"/>
      /// and whose value is a whitespace-separated list of words, one of 
      /// which is exactly <paramref name="value"/>.
      /// </summary>
      public Selector<HtmlNode> AttributeIncludes(string name, string value)
      {
         return nodes => from n in nodes.Elements()
                         let a = n.Attributes[name]
                         where a != null && a.Value.Split(' ').Contains(value)
                         select n;
      }

      /// <summary>
      /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
      /// that represents an element with the given attribute <paramref name="name"/>,
      /// its value either being exactly <paramref name="value"/> or beginning 
      /// with <paramref name="value"/> immediately followed by "-" (U+002D).
      /// </summary>
      public Selector<HtmlNode> AttributeDashMatch(string name, string value)
      {
         return string.IsNullOrEmpty(value)
                   ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>())
                   : (nodes => from n in nodes.Elements()
                               let a = n.Attributes[name]
                               where a != null && a.Value.Split('-').Contains(value)
                               select n);
      }

      /// <summary>
      /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
      /// that represents an element with the attribute <paramref name="name"/> 
      /// whose value begins with the prefix <paramref name="value"/>.
      /// </summary>
      public Selector<HtmlNode> AttributePrefixMatch(string name, string value)
      {
         return string.IsNullOrEmpty(value)
                   ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>())
                   : (nodes => from n in nodes.Elements()
                               let a = n.Attributes[name]
                               where a != null && a.Value.StartsWith(value)
                               select n);
      }

      /// <summary>
      /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
      /// that represents an element with the attribute <paramref name="name"/> 
      /// whose value ends with the suffix <paramref name="value"/>.
      /// </summary>
      public Selector<HtmlNode> AttributeSuffixMatch(string name, string value)
      {
         return string.IsNullOrEmpty(value)
                   ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>())
                   : (nodes => from n in nodes.Elements()
                               let a = n.Attributes[name]
                               where a != null && a.Value.EndsWith(value)
                               select n);
      }

      /// <summary>
      /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
      /// that represents an element with the attribute <paramref name="name"/> 
      /// whose value contains at least one instance of the substring <paramref name="value"/>.
      /// </summary>
      public Selector<HtmlNode> AttributeSubstring(string name, string value)
      {
         return string.IsNullOrEmpty(value)
                   ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>())
                   : (nodes => from n in nodes.Elements()
                               let a = n.Attributes[name]
                               where a != null && a.Value.Contains(value)
                               select n);
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
      /// which represents an element that is the first child of some other element.
      /// </summary>
      public Selector<HtmlNode> FirstChild()
      {
         return nodes => nodes.Where(n => !n.ElementsBeforeSelf().Any());
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
      /// which represents an element that is the last child of some other element.
      /// </summary>
      public Selector<HtmlNode> LastChild()
      {
         return nodes => nodes.Where(n => n.ParentNode.NodeType != HtmlNodeType.Document
                                          && !n.ElementsAfterSelf().Any());
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
      /// which represents an element that is the N-th child of some other element.
      /// </summary>
      public Selector<HtmlNode> NthChild(int a, int b)
      {
         if (a != 1)
            throw new NotSupportedException(
               "The nth-child(an+b) selector where a in is not 1 are not supported.");

         return nodes => from n in nodes
                         let elements = n.ParentNode.Elements().Take(b).ToArray()
                         where elements.Length == b && elements.Last().Equals(n)
                         select n;
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
      /// which represents an element that has a parent element and whose parent 
      /// element has no other element children.
      /// </summary>
      public Selector<HtmlNode> OnlyChild()
      {
         return nodes => nodes.Where(n => n.ParentNode.NodeType != HtmlNodeType.Document
                                          &&
                                          !n.ElementsAfterSelf().Concat(
                                              n.ElementsBeforeSelf()).Any());
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
      /// which represents an element that has no children at all.
      /// </summary>
      public Selector<HtmlNode> Empty()
      {
         return nodes => nodes.Elements().Where(n => n.ChildNodes.Count == 0);
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#combinators">combinator</a>,
      /// which represents a childhood relationship between two elements.
      /// </summary>
      public Selector<HtmlNode> Child()
      {
         return nodes => nodes.SelectMany(n => n.Elements());
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#combinators">combinator</a>,
      /// which represents a relationship between two elements where one element is an 
      /// arbitrary descendant of some ancestor element.
      /// </summary>
      public Selector<HtmlNode> Descendant()
      {
         return nodes => nodes.SelectMany(n => n.Descendants().Elements());
      }

      /// <summary>
      /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#combinators">combinator</a>,
      /// which represents elements that share the same parent in the document tree and 
      /// where the first element immediately precedes the second element.
      /// </summary>
      public Selector<HtmlNode> Adjacent()
      {
         return nodes => nodes.SelectMany(n => n.ElementsAfterSelf().Take(1));
      }

      #endregion
   }
}