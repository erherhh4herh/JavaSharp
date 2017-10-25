using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1994, 2010, Oracle and/or its affiliates. All rights reserved.
 * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 */

namespace java.util
{

	/// <summary>
	/// The <code>Stack</code> class represents a last-in-first-out
	/// (LIFO) stack of objects. It extends class <tt>Vector</tt> with five
	/// operations that allow a vector to be treated as a stack. The usual
	/// <tt>push</tt> and <tt>pop</tt> operations are provided, as well as a
	/// method to <tt>peek</tt> at the top item on the stack, a method to test
	/// for whether the stack is <tt>empty</tt>, and a method to <tt>search</tt>
	/// the stack for an item and discover how far it is from the top.
	/// <para>
	/// When a stack is first created, it contains no items.
	/// 
	/// </para>
	/// <para>A more complete and consistent set of LIFO stack operations is
	/// provided by the <seealso cref="Deque"/> interface and its implementations, which
	/// should be used in preference to this class.  For example:
	/// <pre>   {@code
	///   Deque<Integer> stack = new ArrayDeque<Integer>();}</pre>
	/// 
	/// @author  Jonathan Payne
	/// @since   JDK1.0
	/// </para>
	/// </summary>
	public class Stack<E> : Vector<E>
	{
		/// <summary>
		/// Creates an empty Stack.
		/// </summary>
		public Stack()
		{
		}

		/// <summary>
		/// Pushes an item onto the top of this stack. This has exactly
		/// the same effect as:
		/// <blockquote><pre>
		/// addElement(item)</pre></blockquote>
		/// </summary>
		/// <param name="item">   the item to be pushed onto this stack. </param>
		/// <returns>  the <code>item</code> argument. </returns>
		/// <seealso cref=     java.util.Vector#addElement </seealso>
		public virtual E Push(E item)
		{
			this.AddElement(item);

			return item;
		}

		/// <summary>
		/// Removes the object at the top of this stack and returns that
		/// object as the value of this function.
		/// </summary>
		/// <returns>  The object at the top of this stack (the last item
		///          of the <tt>Vector</tt> object). </returns>
		/// <exception cref="EmptyStackException">  if this stack is empty. </exception>
		public virtual E Pop()
		{
			lock (this)
			{
				E obj;
				int len = this.Size();
        
				obj = Peek();
				this.RemoveElementAt(len - 1);
        
				return obj;
			}
		}

		/// <summary>
		/// Looks at the object at the top of this stack without removing it
		/// from the stack.
		/// </summary>
		/// <returns>  the object at the top of this stack (the last item
		///          of the <tt>Vector</tt> object). </returns>
		/// <exception cref="EmptyStackException">  if this stack is empty. </exception>
		public virtual E Peek()
		{
			lock (this)
			{
				int len = this.Size();
        
				if (len == 0)
				{
					throw new EmptyStackException();
				}
				return this.ElementAt(len - 1);
			}
		}

		/// <summary>
		/// Tests if this stack is empty.
		/// </summary>
		/// <returns>  <code>true</code> if and only if this stack contains
		///          no items; <code>false</code> otherwise. </returns>
		public virtual bool Empty()
		{
			return this.Size() == 0;
		}

		/// <summary>
		/// Returns the 1-based position where an object is on this stack.
		/// If the object <tt>o</tt> occurs as an item in this stack, this
		/// method returns the distance from the top of the stack of the
		/// occurrence nearest the top of the stack; the topmost item on the
		/// stack is considered to be at distance <tt>1</tt>. The <tt>equals</tt>
		/// method is used to compare <tt>o</tt> to the
		/// items in this stack.
		/// </summary>
		/// <param name="o">   the desired object. </param>
		/// <returns>  the 1-based position from the top of the stack where
		///          the object is located; the return value <code>-1</code>
		///          indicates that the object is not on the stack. </returns>
		public virtual int Search(Object o)
		{
			lock (this)
			{
				int List_Fields.i = LastIndexOf(o);
        
				if (List_Fields.i >= 0)
				{
					return this.Size() - List_Fields.i;
				}
				return -1;
			}
		}

		/// <summary>
		/// use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		private const long SerialVersionUID = 1224463164541339165L;
	}

}