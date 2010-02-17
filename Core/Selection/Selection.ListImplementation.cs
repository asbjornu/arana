using System.Collections;
using System.Collections.Generic;

using HtmlAgilityPack;

namespace Arana
{
   public partial class Selection : IList<HtmlNode>
   {
      private readonly List<HtmlNode> nodes;


      /// <summary>
      /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
      /// </summary>
      /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
      /// <returns>
      /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
      /// </returns>
      public int IndexOf(HtmlNode item)
      {
         return this.nodes.IndexOf(item);
      }


      /// <summary>
      /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
      /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
      /// <exception cref="T:System.ArgumentOutOfRangeException">
      /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
      /// </exception>
      /// <exception cref="T:System.NotSupportedException">
      /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
      /// </exception>
      public void Insert(int index, HtmlNode item)
      {
         this.nodes.Insert(index, item);
      }


      /// <summary>
      /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the item to remove.</param>
      /// <exception cref="T:System.ArgumentOutOfRangeException">
      /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
      /// </exception>
      /// <exception cref="T:System.NotSupportedException">
      /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
      /// </exception>
      public void RemoveAt(int index)
      {
         this.nodes.RemoveAt(index);
      }


      /// <summary>
      /// Gets or sets the <see cref="HtmlAgilityPack.HtmlNode"/> at the specified index.
      /// </summary>
      /// <value></value>
      public HtmlNode this[int index]
      {
         get { return this.nodes[index]; }
         set { this.nodes[index] = value; }
      }


      /// <summary>
      /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      /// <exception cref="T:System.NotSupportedException">
      /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
      /// </exception>
      public void Add(HtmlNode item)
      {
         this.nodes.Add(item);
      }


      /// <summary>
      /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <exception cref="T:System.NotSupportedException">
      /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
      /// </exception>
      public void Clear()
      {
         this.nodes.Clear();
      }


      /// <summary>
      /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
      /// </summary>
      /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      /// <returns>
      /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
      /// </returns>
      public bool Contains(HtmlNode item)
      {
         return this.nodes.Contains(item);
      }


      /// <summary>
      /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
      /// </summary>
      /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
      /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
      /// <exception cref="T:System.ArgumentNullException">
      /// 	<paramref name="array"/> is null.
      /// </exception>
      /// <exception cref="T:System.ArgumentOutOfRangeException">
      /// 	<paramref name="arrayIndex"/> is less than 0.
      /// </exception>
      /// <exception cref="T:System.ArgumentException">
      /// 	<paramref name="array"/> is multidimensional.
      /// -or-
      /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
      /// -or-
      /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
      /// -or-
      /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
      /// </exception>
      public void CopyTo(HtmlNode[] array, int arrayIndex)
      {
         this.nodes.CopyTo(array, arrayIndex);
      }


      /// <summary>
      /// Gets a value indicating whether the Collection is read-only.
      /// </summary>
      /// <value></value>
      /// <returns>true if the Collection is read-only; otherwise, false.
      /// </returns>
      public bool IsReadOnly
      {
         get { return false; }
      }


      /// <summary>
      /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      /// <returns>
      /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </returns>
      /// <exception cref="T:System.NotSupportedException">
      /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
      /// </exception>
      public bool Remove(HtmlNode item)
      {
         return this.nodes.Remove(item);
      }


      /// <summary>
      /// Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
      /// </returns>
      public IEnumerator<HtmlNode> GetEnumerator()
      {
         return this.nodes.GetEnumerator();
      }


      /// <summary>
      /// Returns an enumerator that iterates through a collection.
      /// </summary>
      /// <returns>
      /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
      /// </returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
         return this.nodes.GetEnumerator();
      }


      /// <summary>
      /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <value></value>
      /// <returns>
      /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </returns>
      public int Count
      {
         get { return this.nodes.Count; }
      }
   }
}