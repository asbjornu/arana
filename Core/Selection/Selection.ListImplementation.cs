using System.Collections;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace Arana.Core
{
   public partial class Selection : IList<HtmlNode>
   {
      private readonly List<HtmlNode> nodes;

      #region IList<HtmlNode> Members

      public int IndexOf(HtmlNode item)
      {
         return this.nodes.IndexOf(item);
      }

      public void Insert(int index, HtmlNode item)
      {
         this.nodes.Insert(index, item);
      }

      public void RemoveAt(int index)
      {
         this.nodes.RemoveAt(index);
      }

      public HtmlNode this[int index]
      {
         get { return this.nodes[index]; }
         set { this.nodes[index] = value; }
      }

      public void Add(HtmlNode item)
      {
         this.nodes.Add(item);
      }

      public void Clear()
      {
         this.nodes.Clear();
      }

      public bool Contains(HtmlNode item)
      {
         return this.nodes.Contains(item);
      }

      public void CopyTo(HtmlNode[] array, int arrayIndex)
      {
         this.nodes.CopyTo(array, arrayIndex);
      }

      public bool IsReadOnly
      {
         get { return false; }
      }

      public bool Remove(HtmlNode item)
      {
         return this.nodes.Remove(item);
      }

      public IEnumerator<HtmlNode> GetEnumerator()
      {
         return this.nodes.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return this.nodes.GetEnumerator();
      }

      public int Count
      {
         get { return this.nodes.Count; }
      }

      #endregion
   }
}