using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.awt
{



	/// <summary>
	/// A <code>CardLayout</code> object is a layout manager for a
	/// container. It treats each component in the container as a card.
	/// Only one card is visible at a time, and the container acts as
	/// a stack of cards. The first component added to a
	/// <code>CardLayout</code> object is the visible component when the
	/// container is first displayed.
	/// <para>
	/// The ordering of cards is determined by the container's own internal
	/// ordering of its component objects. <code>CardLayout</code>
	/// defines a set of methods that allow an application to flip
	/// through these cards sequentially, or to show a specified card.
	/// The <seealso cref="CardLayout#addLayoutComponent"/>
	/// method can be used to associate a string identifier with a given card
	/// for fast random access.
	/// 
	/// @author      Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=         java.awt.Container
	/// @since       JDK1.0 </seealso>

	[Serializable]
	public class CardLayout : LayoutManager2
	{

		private const long SerialVersionUID = -4328196481005934313L;

		/*
		 * This creates a Vector to store associated
		 * pairs of components and their names.
		 * @see java.util.Vector
		 */
		internal List<Card> Vector = new List<Card>();

		/*
		 * A pair of Component and String that represents its name.
		 */
		[Serializable]
		internal class Card
		{
			private readonly CardLayout OuterInstance;

			internal const long SerialVersionUID = 6640330810709497518L;
			public String Name;
			public Component Comp;
			public Card(CardLayout outerInstance, String cardName, Component cardComponent)
			{
				this.OuterInstance = outerInstance;
				Name = cardName;
				Comp = cardComponent;
			}
		}

		/*
		 * Index of Component currently displayed by CardLayout.
		 */
		internal int CurrentCard = 0;


		/*
		* A cards horizontal Layout gap (inset). It specifies
		* the space between the left and right edges of a
		* container and the current component.
		* This should be a non negative Integer.
		* @see getHgap()
		* @see setHgap()
		*/
		internal int Hgap_Renamed;

		/*
		* A cards vertical Layout gap (inset). It specifies
		* the space between the top and bottom edges of a
		* container and the current component.
		* This should be a non negative Integer.
		* @see getVgap()
		* @see setVgap()
		*/
		internal int Vgap_Renamed;

		/// <summary>
		/// @serialField tab         Hashtable
		///      deprectated, for forward compatibility only
		/// @serialField hgap        int
		/// @serialField vgap        int
		/// @serialField vector      Vector
		/// @serialField currentCard int
		/// </summary>
		private static readonly ObjectStreamField[] SerialPersistentFields = new ObjectStreamField[] {new ObjectStreamField("tab", typeof(Hashtable)), new ObjectStreamField("hgap", Integer.TYPE), new ObjectStreamField("vgap", Integer.TYPE), new ObjectStreamField("vector", typeof(ArrayList)), new ObjectStreamField("currentCard", Integer.TYPE)};

		/// <summary>
		/// Creates a new card layout with gaps of size zero.
		/// </summary>
		public CardLayout() : this(0, 0)
		{
		}

		/// <summary>
		/// Creates a new card layout with the specified horizontal and
		/// vertical gaps. The horizontal gaps are placed at the left and
		/// right edges. The vertical gaps are placed at the top and bottom
		/// edges. </summary>
		/// <param name="hgap">   the horizontal gap. </param>
		/// <param name="vgap">   the vertical gap. </param>
		public CardLayout(int hgap, int vgap)
		{
			this.Hgap_Renamed = hgap;
			this.Vgap_Renamed = vgap;
		}

		/// <summary>
		/// Gets the horizontal gap between components. </summary>
		/// <returns>    the horizontal gap between components. </returns>
		/// <seealso cref=       java.awt.CardLayout#setHgap(int) </seealso>
		/// <seealso cref=       java.awt.CardLayout#getVgap()
		/// @since     JDK1.1 </seealso>
		public virtual int Hgap
		{
			get
			{
				return Hgap_Renamed;
			}
			set
			{
				this.Hgap_Renamed = value;
			}
		}


		/// <summary>
		/// Gets the vertical gap between components. </summary>
		/// <returns> the vertical gap between components. </returns>
		/// <seealso cref=       java.awt.CardLayout#setVgap(int) </seealso>
		/// <seealso cref=       java.awt.CardLayout#getHgap() </seealso>
		public virtual int Vgap
		{
			get
			{
				return Vgap_Renamed;
			}
			set
			{
				this.Vgap_Renamed = value;
			}
		}


		/// <summary>
		/// Adds the specified component to this card layout's internal
		/// table of names. The object specified by <code>constraints</code>
		/// must be a string. The card layout stores this string as a key-value
		/// pair that can be used for random access to a particular card.
		/// By calling the <code>show</code> method, an application can
		/// display the component with the specified name. </summary>
		/// <param name="comp">          the component to be added. </param>
		/// <param name="constraints">   a tag that identifies a particular
		///                                        card in the layout. </param>
		/// <seealso cref=       java.awt.CardLayout#show(java.awt.Container, java.lang.String) </seealso>
		/// <exception cref="IllegalArgumentException">  if the constraint is not a string. </exception>
		public virtual void AddLayoutComponent(Component comp, Object constraints)
		{
		  lock (comp.TreeLock)
		  {
			  if (constraints == null)
			  {
				  constraints = "";
			  }
			if (constraints is String)
			{
				AddLayoutComponent((String)constraints, comp);
			}
			else
			{
				throw new IllegalArgumentException("cannot add to layout: constraint must be a string");
			}
		  }
		}

		/// @deprecated   replaced by
		///      <code>addLayoutComponent(Component, Object)</code>. 
		[Obsolete("  replaced by")]
		public virtual void AddLayoutComponent(String name, Component comp)
		{
			lock (comp.TreeLock)
			{
				if (Vector.Count > 0)
				{
					comp.Visible = false;
				}
				for (int i = 0; i < Vector.Count; i++)
				{
					if (((Card)Vector[i]).Name.Equals(name))
					{
						((Card)Vector[i]).Comp = comp;
						return;
					}
				}
				Vector.Add(new Card(this, name, comp));
			}
		}

		/// <summary>
		/// Removes the specified component from the layout.
		/// If the card was visible on top, the next card underneath it is shown. </summary>
		/// <param name="comp">   the component to be removed. </param>
		/// <seealso cref=     java.awt.Container#remove(java.awt.Component) </seealso>
		/// <seealso cref=     java.awt.Container#removeAll() </seealso>
		public virtual void RemoveLayoutComponent(Component comp)
		{
			lock (comp.TreeLock)
			{
				for (int i = 0; i < Vector.Count; i++)
				{
					if (((Card)Vector[i]).Comp == comp)
					{
						// if we remove current component we should show next one
						if (comp.Visible && (comp.Parent != null))
						{
							Next(comp.Parent);
						}

						Vector.RemoveAt(i);

						// correct currentCard if this is necessary
						if (CurrentCard > i)
						{
							CurrentCard--;
						}
						break;
					}
				}
			}
		}

		/// <summary>
		/// Determines the preferred size of the container argument using
		/// this card layout. </summary>
		/// <param name="parent"> the parent container in which to do the layout </param>
		/// <returns>  the preferred dimensions to lay out the subcomponents
		///                of the specified container </returns>
		/// <seealso cref=     java.awt.Container#getPreferredSize </seealso>
		/// <seealso cref=     java.awt.CardLayout#minimumLayoutSize </seealso>
		public virtual Dimension PreferredLayoutSize(Container parent)
		{
			lock (parent.TreeLock)
			{
				Insets insets = parent.Insets;
				int ncomponents = parent.ComponentCount;
				int w = 0;
				int h = 0;

				for (int i = 0 ; i < ncomponents ; i++)
				{
					Component comp = parent.GetComponent(i);
					Dimension d = comp.PreferredSize;
					if (d.Width_Renamed > w)
					{
						w = d.Width_Renamed;
					}
					if (d.Height_Renamed > h)
					{
						h = d.Height_Renamed;
					}
				}
				return new Dimension(insets.Left + insets.Right + w + Hgap_Renamed * 2, insets.Top + insets.Bottom + h + Vgap_Renamed * 2);
			}
		}

		/// <summary>
		/// Calculates the minimum size for the specified panel. </summary>
		/// <param name="parent"> the parent container in which to do the layout </param>
		/// <returns>    the minimum dimensions required to lay out the
		///                subcomponents of the specified container </returns>
		/// <seealso cref=       java.awt.Container#doLayout </seealso>
		/// <seealso cref=       java.awt.CardLayout#preferredLayoutSize </seealso>
		public virtual Dimension MinimumLayoutSize(Container parent)
		{
			lock (parent.TreeLock)
			{
				Insets insets = parent.Insets;
				int ncomponents = parent.ComponentCount;
				int w = 0;
				int h = 0;

				for (int i = 0 ; i < ncomponents ; i++)
				{
					Component comp = parent.GetComponent(i);
					Dimension d = comp.MinimumSize;
					if (d.Width_Renamed > w)
					{
						w = d.Width_Renamed;
					}
					if (d.Height_Renamed > h)
					{
						h = d.Height_Renamed;
					}
				}
				return new Dimension(insets.Left + insets.Right + w + Hgap_Renamed * 2, insets.Top + insets.Bottom + h + Vgap_Renamed * 2);
			}
		}

		/// <summary>
		/// Returns the maximum dimensions for this layout given the components
		/// in the specified target container. </summary>
		/// <param name="target"> the component which needs to be laid out </param>
		/// <seealso cref= Container </seealso>
		/// <seealso cref= #minimumLayoutSize </seealso>
		/// <seealso cref= #preferredLayoutSize </seealso>
		public virtual Dimension MaximumLayoutSize(Container target)
		{
			return new Dimension(Integer.MaxValue, Integer.MaxValue);
		}

		/// <summary>
		/// Returns the alignment along the x axis.  This specifies how
		/// the component would like to be aligned relative to other
		/// components.  The value should be a number between 0 and 1
		/// where 0 represents alignment along the origin, 1 is aligned
		/// the furthest away from the origin, 0.5 is centered, etc.
		/// </summary>
		public virtual float GetLayoutAlignmentX(Container parent)
		{
			return 0.5f;
		}

		/// <summary>
		/// Returns the alignment along the y axis.  This specifies how
		/// the component would like to be aligned relative to other
		/// components.  The value should be a number between 0 and 1
		/// where 0 represents alignment along the origin, 1 is aligned
		/// the furthest away from the origin, 0.5 is centered, etc.
		/// </summary>
		public virtual float GetLayoutAlignmentY(Container parent)
		{
			return 0.5f;
		}

		/// <summary>
		/// Invalidates the layout, indicating that if the layout manager
		/// has cached information it should be discarded.
		/// </summary>
		public virtual void InvalidateLayout(Container target)
		{
		}

		/// <summary>
		/// Lays out the specified container using this card layout.
		/// <para>
		/// Each component in the <code>parent</code> container is reshaped
		/// to be the size of the container, minus space for surrounding
		/// insets, horizontal gaps, and vertical gaps.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent"> the parent container in which to do the layout </param>
		/// <seealso cref=       java.awt.Container#doLayout </seealso>
		public virtual void LayoutContainer(Container parent)
		{
			lock (parent.TreeLock)
			{
				Insets insets = parent.Insets;
				int ncomponents = parent.ComponentCount;
				Component comp = null;
				bool currentFound = false;

				for (int i = 0 ; i < ncomponents ; i++)
				{
					comp = parent.GetComponent(i);
					comp.SetBounds(Hgap_Renamed + insets.Left, Vgap_Renamed + insets.Top, parent.Width_Renamed - (Hgap_Renamed * 2 + insets.Left + insets.Right), parent.Height_Renamed - (Vgap_Renamed * 2 + insets.Top + insets.Bottom));
					if (comp.Visible)
					{
						currentFound = true;
					}
				}

				if (!currentFound && ncomponents > 0)
				{
					parent.GetComponent(0).Visible = true;
				}
			}
		}

		/// <summary>
		/// Make sure that the Container really has a CardLayout installed.
		/// Otherwise havoc can ensue!
		/// </summary>
		internal virtual void CheckLayout(Container parent)
		{
			if (parent.Layout != this)
			{
				throw new IllegalArgumentException("wrong parent for CardLayout");
			}
		}

		/// <summary>
		/// Flips to the first card of the container. </summary>
		/// <param name="parent">   the parent container in which to do the layout </param>
		/// <seealso cref=       java.awt.CardLayout#last </seealso>
		public virtual void First(Container parent)
		{
			lock (parent.TreeLock)
			{
				CheckLayout(parent);
				int ncomponents = parent.ComponentCount;
				for (int i = 0 ; i < ncomponents ; i++)
				{
					Component comp = parent.GetComponent(i);
					if (comp.Visible)
					{
						comp.Visible = false;
						break;
					}
				}
				if (ncomponents > 0)
				{
					CurrentCard = 0;
					parent.GetComponent(0).Visible = true;
					parent.Validate();
				}
			}
		}

		/// <summary>
		/// Flips to the next card of the specified container. If the
		/// currently visible card is the last one, this method flips to the
		/// first card in the layout. </summary>
		/// <param name="parent">   the parent container in which to do the layout </param>
		/// <seealso cref=       java.awt.CardLayout#previous </seealso>
		public virtual void Next(Container parent)
		{
			lock (parent.TreeLock)
			{
				CheckLayout(parent);
				int ncomponents = parent.ComponentCount;
				for (int i = 0 ; i < ncomponents ; i++)
				{
					Component comp = parent.GetComponent(i);
					if (comp.Visible)
					{
						comp.Visible = false;
						CurrentCard = (i + 1) % ncomponents;
						comp = parent.GetComponent(CurrentCard);
						comp.Visible = true;
						parent.Validate();
						return;
					}
				}
				ShowDefaultComponent(parent);
			}
		}

		/// <summary>
		/// Flips to the previous card of the specified container. If the
		/// currently visible card is the first one, this method flips to the
		/// last card in the layout. </summary>
		/// <param name="parent">   the parent container in which to do the layout </param>
		/// <seealso cref=       java.awt.CardLayout#next </seealso>
		public virtual void Previous(Container parent)
		{
			lock (parent.TreeLock)
			{
				CheckLayout(parent);
				int ncomponents = parent.ComponentCount;
				for (int i = 0 ; i < ncomponents ; i++)
				{
					Component comp = parent.GetComponent(i);
					if (comp.Visible)
					{
						comp.Visible = false;
						CurrentCard = ((i > 0) ? i - 1 : ncomponents - 1);
						comp = parent.GetComponent(CurrentCard);
						comp.Visible = true;
						parent.Validate();
						return;
					}
				}
				ShowDefaultComponent(parent);
			}
		}

		internal virtual void ShowDefaultComponent(Container parent)
		{
			if (parent.ComponentCount > 0)
			{
				CurrentCard = 0;
				parent.GetComponent(0).Visible = true;
				parent.Validate();
			}
		}

		/// <summary>
		/// Flips to the last card of the container. </summary>
		/// <param name="parent">   the parent container in which to do the layout </param>
		/// <seealso cref=       java.awt.CardLayout#first </seealso>
		public virtual void Last(Container parent)
		{
			lock (parent.TreeLock)
			{
				CheckLayout(parent);
				int ncomponents = parent.ComponentCount;
				for (int i = 0 ; i < ncomponents ; i++)
				{
					Component comp = parent.GetComponent(i);
					if (comp.Visible)
					{
						comp.Visible = false;
						break;
					}
				}
				if (ncomponents > 0)
				{
					CurrentCard = ncomponents - 1;
					parent.GetComponent(CurrentCard).Visible = true;
					parent.Validate();
				}
			}
		}

		/// <summary>
		/// Flips to the component that was added to this layout with the
		/// specified <code>name</code>, using <code>addLayoutComponent</code>.
		/// If no such component exists, then nothing happens. </summary>
		/// <param name="parent">   the parent container in which to do the layout </param>
		/// <param name="name">     the component name </param>
		/// <seealso cref=       java.awt.CardLayout#addLayoutComponent(java.awt.Component, java.lang.Object) </seealso>
		public virtual void Show(Container parent, String name)
		{
			lock (parent.TreeLock)
			{
				CheckLayout(parent);
				Component next = null;
				int ncomponents = Vector.Count;
				for (int i = 0; i < ncomponents; i++)
				{
					Card card = (Card)Vector[i];
					if (card.Name.Equals(name))
					{
						next = card.Comp;
						CurrentCard = i;
						break;
					}
				}
				if ((next != null) && !next.Visible)
				{
					ncomponents = parent.ComponentCount;
					for (int i = 0; i < ncomponents; i++)
					{
						Component comp = parent.GetComponent(i);
						if (comp.Visible)
						{
							comp.Visible = false;
							break;
						}
					}
					next.Visible = true;
					parent.Validate();
				}
			}
		}

		/// <summary>
		/// Returns a string representation of the state of this card layout. </summary>
		/// <returns>    a string representation of this card layout. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[hgap=" + Hgap_Renamed + ",vgap=" + Vgap_Renamed + "]";
		}

		/// <summary>
		/// Reads serializable fields from stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream s)
		{
			ObjectInputStream.GetField f = s.ReadFields();

			Hgap_Renamed = f.Get("hgap", 0);
			Vgap_Renamed = f.Get("vgap", 0);

			if (f.Defaulted("vector"))
			{
				//  pre-1.4 stream
				Dictionary<String, Component> tab = (Hashtable)f.Get("tab", null);
				Vector = new List<>();
				if (tab != null && tab.Count > 0)
				{
					for (IEnumerator<String> e = tab.Keys.GetEnumerator(); e.MoveNext();)
					{
						String key = (String)e.Current;
						Component comp = (Component)tab[key];
						Vector.Add(new Card(this, key, comp));
						if (comp.Visible)
						{
							CurrentCard = Vector.Count - 1;
						}
					}
				}
			}
			else
			{
				Vector = (ArrayList)f.Get("vector", null);
				CurrentCard = f.Get("currentCard", 0);
			}
		}

		/// <summary>
		/// Writes serializable fields to stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
			Dictionary<String, Component> tab = new Dictionary<String, Component>();
			int ncomponents = Vector.Count;
			for (int i = 0; i < ncomponents; i++)
			{
				Card card = (Card)Vector[i];
				tab[card.Name] = card.Comp;
			}

			ObjectOutputStream.PutField f = s.PutFields();
			f.Put("hgap", Hgap_Renamed);
			f.Put("vgap", Vgap_Renamed);
			f.Put("vector", Vector);
			f.Put("currentCard", CurrentCard);
			f.Put("tab", tab);
			s.WriteFields();
		}
	}

}