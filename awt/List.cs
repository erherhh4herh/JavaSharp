using System;
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
	/// The <code>List</code> component presents the user with a
	/// scrolling list of text items. The list can be set up so that
	/// the user can choose either one item or multiple items.
	/// <para>
	/// For example, the code&nbsp;.&nbsp;.&nbsp;.
	/// 
	/// <hr><blockquote><pre>
	/// List lst = new List(4, false);
	/// lst.add("Mercury");
	/// lst.add("Venus");
	/// lst.add("Earth");
	/// lst.add("JavaSoft");
	/// lst.add("Mars");
	/// lst.add("Jupiter");
	/// lst.add("Saturn");
	/// lst.add("Uranus");
	/// lst.add("Neptune");
	/// lst.add("Pluto");
	/// cnt.add(lst);
	/// </pre></blockquote><hr>
	/// </para>
	/// <para>
	/// where <code>cnt</code> is a container, produces the following
	/// scrolling list:
	/// </para>
	/// <para>
	/// <img src="doc-files/List-1.gif"
	/// alt="Shows a list containing: Venus, Earth, JavaSoft, and Mars. Javasoft is selected." style="float:center; margin: 7px 10px;">
	/// </para>
	/// <para>
	/// If the List allows multiple selections, then clicking on
	/// an item that is already selected deselects it. In the preceding
	/// example, only one item from the scrolling list can be selected
	/// at a time, since the second argument when creating the new scrolling
	/// list is <code>false</code>. If the List does not allow multiple
	/// selections, selecting an item causes any other selected item
	/// to be deselected.
	/// </para>
	/// <para>
	/// Note that the list in the example shown was created with four visible
	/// rows.  Once the list has been created, the number of visible rows
	/// cannot be changed.  A default <code>List</code> is created with
	/// four rows, so that <code>lst = new List()</code> is equivalent to
	/// <code>list = new List(4, false)</code>.
	/// </para>
	/// <para>
	/// Beginning with Java&nbsp;1.1, the Abstract Window Toolkit
	/// sends the <code>List</code> object all mouse, keyboard, and focus events
	/// that occur over it. (The old AWT event model is being maintained
	/// only for backwards compatibility, and its use is discouraged.)
	/// </para>
	/// <para>
	/// When an item is selected or deselected by the user, AWT sends an instance
	/// of <code>ItemEvent</code> to the list.
	/// When the user double-clicks on an item in a scrolling list,
	/// AWT sends an instance of <code>ActionEvent</code> to the
	/// list following the item event. AWT also generates an action event
	/// when the user presses the return key while an item in the
	/// list is selected.
	/// </para>
	/// <para>
	/// If an application wants to perform some action based on an item
	/// in this list being selected or activated by the user, it should implement
	/// <code>ItemListener</code> or <code>ActionListener</code>
	/// as appropriate and register the new listener to receive
	/// events from this list.
	/// </para>
	/// <para>
	/// For multiple-selection scrolling lists, it is considered a better
	/// user interface to use an external gesture (such as clicking on a
	/// button) to trigger the action.
	/// @author      Sami Shaio
	/// </para>
	/// </summary>
	/// <seealso cref=         java.awt.event.ItemEvent </seealso>
	/// <seealso cref=         java.awt.event.ItemListener </seealso>
	/// <seealso cref=         java.awt.event.ActionEvent </seealso>
	/// <seealso cref=         java.awt.event.ActionListener
	/// @since       JDK1.0 </seealso>
	public class List : Component, ItemSelectable, Accessible
	{
		/// <summary>
		/// A vector created to contain items which will become
		/// part of the List Component.
		/// 
		/// @serial </summary>
		/// <seealso cref= #addItem(String) </seealso>
		/// <seealso cref= #getItem(int) </seealso>
		internal List<String> Items_Renamed = new List<String>();

		/// <summary>
		/// This field will represent the number of visible rows in the
		/// <code>List</code> Component.  It is specified only once, and
		/// that is when the list component is actually
		/// created.  It will never change.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getRows() </seealso>
		internal int Rows_Renamed = 0;

		/// <summary>
		/// <code>multipleMode</code> is a variable that will
		/// be set to <code>true</code> if a list component is to be set to
		/// multiple selection mode, that is where the user can
		/// select more than one item in a list at one time.
		/// <code>multipleMode</code> will be set to false if the
		/// list component is set to single selection, that is where
		/// the user can only select one item on the list at any
		/// one time.
		/// 
		/// @serial </summary>
		/// <seealso cref= #isMultipleMode() </seealso>
		/// <seealso cref= #setMultipleMode(boolean) </seealso>
		internal bool MultipleMode_Renamed = false;

		/// <summary>
		/// <code>selected</code> is an array that will contain
		/// the indices of items that have been selected.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getSelectedIndexes() </seealso>
		/// <seealso cref= #getSelectedIndex() </seealso>
		internal int[] Selected = new int[0];

		/// <summary>
		/// This variable contains the value that will be used
		/// when trying to make a particular list item visible.
		/// 
		/// @serial </summary>
		/// <seealso cref= #makeVisible(int) </seealso>
		internal int VisibleIndex_Renamed = -1;

		[NonSerialized]
		internal ActionListener ActionListener;
		[NonSerialized]
		internal ItemListener ItemListener;

		private const String @base = "list";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = -3304312411574666869L;

		/// <summary>
		/// Creates a new scrolling list.
		/// By default, there are four visible lines and multiple selections are
		/// not allowed.  Note that this is a convenience method for
		/// <code>List(0, false)</code>.  Also note that the number of visible
		/// lines in the list cannot be changed after it has been created. </summary>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public List() throws HeadlessException
		public List() : this(0, false)
		{
		}

		/// <summary>
		/// Creates a new scrolling list initialized with the specified
		/// number of visible lines. By default, multiple selections are
		/// not allowed.  Note that this is a convenience method for
		/// <code>List(rows, false)</code>.  Also note that the number
		/// of visible rows in the list cannot be changed after it has
		/// been created. </summary>
		/// <param name="rows"> the number of items to show. </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since       JDK1.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public List(int rows) throws HeadlessException
		public List(int rows) : this(rows, false)
		{
		}

		/// <summary>
		/// The default number of visible rows is 4.  A list with
		/// zero rows is unusable and unsightly.
		/// </summary>
		internal const int DEFAULT_VISIBLE_ROWS = 4;

		/// <summary>
		/// Creates a new scrolling list initialized to display the specified
		/// number of rows. Note that if zero rows are specified, then
		/// the list will be created with a default of four rows.
		/// Also note that the number of visible rows in the list cannot
		/// be changed after it has been created.
		/// If the value of <code>multipleMode</code> is
		/// <code>true</code>, then the user can select multiple items from
		/// the list. If it is <code>false</code>, only one item at a time
		/// can be selected. </summary>
		/// <param name="rows">   the number of items to show. </param>
		/// <param name="multipleMode">   if <code>true</code>,
		///                     then multiple selections are allowed;
		///                     otherwise, only one item can be selected at a time. </param>
		/// <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		/// returns true. </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public List(int rows, boolean multipleMode) throws HeadlessException
		public List(int rows, bool multipleMode)
		{
			GraphicsEnvironment.CheckHeadless();
			this.Rows_Renamed = (rows != 0) ? rows : DEFAULT_VISIBLE_ROWS;
			this.MultipleMode_Renamed = multipleMode;
		}

		/// <summary>
		/// Construct a name for this component.  Called by
		/// <code>getName</code> when the name is <code>null</code>.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(List))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the peer for the list.  The peer allows us to modify the
		/// list's appearance without changing its functionality.
		/// </summary>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateList(this);
				}
				base.AddNotify();
			}
		}

		/// <summary>
		/// Removes the peer for this list.  The peer allows us to modify the
		/// list's appearance without changing its functionality.
		/// </summary>
		public override void RemoveNotify()
		{
			lock (TreeLock)
			{
				ListPeer peer = (ListPeer)this.Peer_Renamed;
				if (peer != null)
				{
					Selected = peer.SelectedIndexes;
				}
				base.RemoveNotify();
			}
		}

		/// <summary>
		/// Gets the number of items in the list. </summary>
		/// <returns>     the number of items in the list </returns>
		/// <seealso cref=        #getItem
		/// @since      JDK1.1 </seealso>
		public virtual int ItemCount
		{
			get
			{
				return CountItems();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getItemCount()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual int CountItems()
		{
			return Items_Renamed.Count;
		}

		/// <summary>
		/// Gets the item associated with the specified index. </summary>
		/// <returns>       an item that is associated with
		///                    the specified index </returns>
		/// <param name="index"> the position of the item </param>
		/// <seealso cref=          #getItemCount </seealso>
		public virtual String GetItem(int index)
		{
			return GetItemImpl(index);
		}

		// NOTE: This method may be called by privileged threads.
		//       We implement this functionality in a package-private method
		//       to insure that it cannot be overridden by client subclasses.
		//       DO NOT INVOKE CLIENT CODE ON THIS THREAD!
		internal String GetItemImpl(int index)
		{
			return Items_Renamed[index];
		}

		/// <summary>
		/// Gets the items in the list. </summary>
		/// <returns>       a string array containing items of the list </returns>
		/// <seealso cref=          #select </seealso>
		/// <seealso cref=          #deselect </seealso>
		/// <seealso cref=          #isIndexSelected
		/// @since        JDK1.1 </seealso>
		public virtual String[] Items
		{
			get
			{
				lock (this)
				{
					String[] itemCopies = new String[Items_Renamed.Count];
					Items_Renamed.CopyTo(itemCopies);
					return itemCopies;
				}
			}
		}

		/// <summary>
		/// Adds the specified item to the end of scrolling list. </summary>
		/// <param name="item"> the item to be added
		/// @since JDK1.1 </param>
		public virtual void Add(String item)
		{
			AddItem(item);
		}

		/// @deprecated      replaced by <code>add(String)</code>. 
		[Obsolete("     replaced by <code>add(String)</code>.")]
		public virtual void AddItem(String item)
		{
			AddItem(item, -1);
		}

		/// <summary>
		/// Adds the specified item to the the scrolling list
		/// at the position indicated by the index.  The index is
		/// zero-based.  If the value of the index is less than zero,
		/// or if the value of the index is greater than or equal to
		/// the number of items in the list, then the item is added
		/// to the end of the list. </summary>
		/// <param name="item">   the item to be added;
		///              if this parameter is <code>null</code> then the item is
		///              treated as an empty string, <code>""</code> </param>
		/// <param name="index">  the position at which to add the item
		/// @since       JDK1.1 </param>
		public virtual void Add(String item, int index)
		{
			AddItem(item, index);
		}

		/// @deprecated      replaced by <code>add(String, int)</code>. 
		[Obsolete("     replaced by <code>add(String, int)</code>.")]
		public virtual void AddItem(String item, int index)
		{
			lock (this)
			{
				if (index < -1 || index >= Items_Renamed.Count)
				{
					index = -1;
				}
        
				if (item == null)
				{
					item = "";
				}
        
				if (index == -1)
				{
					Items_Renamed.Add(item);
				}
				else
				{
					Items_Renamed.Insert(index, item);
				}
        
				ListPeer peer = (ListPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.Add(item, index);
				}
			}
		}

		/// <summary>
		/// Replaces the item at the specified index in the scrolling list
		/// with the new string. </summary>
		/// <param name="newValue">   a new string to replace an existing item </param>
		/// <param name="index">      the position of the item to replace </param>
		/// <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code>
		///          is out of range </exception>
		public virtual void ReplaceItem(String newValue, int index)
		{
			lock (this)
			{
				Remove(index);
				Add(newValue, index);
			}
		}

		/// <summary>
		/// Removes all items from this list. </summary>
		/// <seealso cref= #remove </seealso>
		/// <seealso cref= #delItems
		/// @since JDK1.1 </seealso>
		public virtual void RemoveAll()
		{
			Clear();
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>removeAll()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void Clear()
		{
			lock (this)
			{
				ListPeer peer = (ListPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.RemoveAll();
				}
				Items_Renamed = new List<>();
				Selected = new int[0];
			}
		}

		/// <summary>
		/// Removes the first occurrence of an item from the list.
		/// If the specified item is selected, and is the only selected
		/// item in the list, the list is set to have no selection. </summary>
		/// <param name="item">  the item to remove from the list </param>
		/// <exception cref="IllegalArgumentException">
		///                     if the item doesn't exist in the list
		/// @since        JDK1.1 </exception>
		public virtual void Remove(String item)
		{
			lock (this)
			{
				int index = Items_Renamed.IndexOf(item);
				if (index < 0)
				{
					throw new IllegalArgumentException("item " + item + " not found in list");
				}
				else
				{
					Remove(index);
				}
			}
		}

		/// <summary>
		/// Removes the item at the specified position
		/// from this scrolling list.
		/// If the item with the specified position is selected, and is the
		/// only selected item in the list, the list is set to have no selection. </summary>
		/// <param name="position">   the index of the item to delete </param>
		/// <seealso cref=        #add(String, int)
		/// @since      JDK1.1 </seealso>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///               if the <code>position</code> is less than 0 or
		///               greater than <code>getItemCount()-1</code> </exception>
		public virtual void Remove(int position)
		{
			DelItem(position);
		}

		/// @deprecated     replaced by <code>remove(String)</code>
		///                         and <code>remove(int)</code>. 
		[Obsolete("    replaced by <code>remove(String)</code>")]
		public virtual void DelItem(int position)
		{
			DelItems(position, position);
		}

		/// <summary>
		/// Gets the index of the selected item on the list,
		/// </summary>
		/// <returns>        the index of the selected item;
		///                if no item is selected, or if multiple items are
		///                selected, <code>-1</code> is returned. </returns>
		/// <seealso cref=           #select </seealso>
		/// <seealso cref=           #deselect </seealso>
		/// <seealso cref=           #isIndexSelected </seealso>
		public virtual int SelectedIndex
		{
			get
			{
				lock (this)
				{
					int[] sel = SelectedIndexes;
					return (sel.Length == 1) ? sel[0] : -1;
				}
			}
		}

		/// <summary>
		/// Gets the selected indexes on the list.
		/// </summary>
		/// <returns>        an array of the selected indexes on this scrolling list;
		///                if no item is selected, a zero-length array is returned. </returns>
		/// <seealso cref=           #select </seealso>
		/// <seealso cref=           #deselect </seealso>
		/// <seealso cref=           #isIndexSelected </seealso>
		public virtual int[] SelectedIndexes
		{
			get
			{
				lock (this)
				{
					ListPeer peer = (ListPeer)this.Peer_Renamed;
					if (peer != null)
					{
						Selected = peer.SelectedIndexes;
					}
					return Selected.clone();
				}
			}
		}

		/// <summary>
		/// Gets the selected item on this scrolling list.
		/// </summary>
		/// <returns>        the selected item on the list;
		///                if no item is selected, or if multiple items are
		///                selected, <code>null</code> is returned. </returns>
		/// <seealso cref=           #select </seealso>
		/// <seealso cref=           #deselect </seealso>
		/// <seealso cref=           #isIndexSelected </seealso>
		public virtual String SelectedItem
		{
			get
			{
				lock (this)
				{
					int index = SelectedIndex;
					return (index < 0) ? null : GetItem(index);
				}
			}
		}

		/// <summary>
		/// Gets the selected items on this scrolling list.
		/// </summary>
		/// <returns>        an array of the selected items on this scrolling list;
		///                if no item is selected, a zero-length array is returned. </returns>
		/// <seealso cref=           #select </seealso>
		/// <seealso cref=           #deselect </seealso>
		/// <seealso cref=           #isIndexSelected </seealso>
		public virtual String[] SelectedItems
		{
			get
			{
				lock (this)
				{
					int[] sel = SelectedIndexes;
					String[] str = new String[sel.Length];
					for (int i = 0 ; i < sel.Length ; i++)
					{
						str[i] = GetItem(sel[i]);
					}
					return str;
				}
			}
		}

		/// <summary>
		/// Gets the selected items on this scrolling list in an array of Objects. </summary>
		/// <returns>        an array of <code>Object</code>s representing the
		///                selected items on this scrolling list;
		///                if no item is selected, a zero-length array is returned. </returns>
		/// <seealso cref= #getSelectedItems </seealso>
		/// <seealso cref= ItemSelectable </seealso>
		public virtual Object[] SelectedObjects
		{
			get
			{
				return SelectedItems;
			}
		}

		/// <summary>
		/// Selects the item at the specified index in the scrolling list.
		/// <para>
		/// Note that passing out of range parameters is invalid,
		/// and will result in unspecified behavior.
		/// 
		/// </para>
		/// <para>Note that this method should be primarily used to
		/// initially select an item in this component.
		/// Programmatically calling this method will <i>not</i> trigger
		/// an <code>ItemEvent</code>.  The only way to trigger an
		/// <code>ItemEvent</code> is by user interaction.
		/// 
		/// </para>
		/// </summary>
		/// <param name="index"> the position of the item to select </param>
		/// <seealso cref=          #getSelectedItem </seealso>
		/// <seealso cref=          #deselect </seealso>
		/// <seealso cref=          #isIndexSelected </seealso>
		public virtual void Select(int index)
		{
			// Bug #4059614: select can't be synchronized while calling the peer,
			// because it is called from the Window Thread.  It is sufficient to
			// synchronize the code that manipulates 'selected' except for the
			// case where the peer changes.  To handle this case, we simply
			// repeat the selection process.

			ListPeer peer;
			do
			{
				peer = (ListPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.Select(index);
					return;
				}

				lock (this)
				{
					bool alreadySelected = false;

					for (int i = 0 ; i < Selected.Length ; i++)
					{
						if (Selected[i] == index)
						{
							alreadySelected = true;
							break;
						}
					}

					if (!alreadySelected)
					{
						if (!MultipleMode_Renamed)
						{
							Selected = new int[1];
							Selected[0] = index;
						}
						else
						{
							int[] newsel = new int[Selected.Length + 1];
							System.Array.Copy(Selected, 0, newsel, 0, Selected.Length);
							newsel[Selected.Length] = index;
							Selected = newsel;
						}
					}
				}
			} while (peer != this.Peer_Renamed);
		}

		/// <summary>
		/// Deselects the item at the specified index.
		/// <para>
		/// Note that passing out of range parameters is invalid,
		/// and will result in unspecified behavior.
		/// </para>
		/// <para>
		/// If the item at the specified index is not selected,
		/// then the operation is ignored.
		/// </para>
		/// </summary>
		/// <param name="index"> the position of the item to deselect </param>
		/// <seealso cref=          #select </seealso>
		/// <seealso cref=          #getSelectedItem </seealso>
		/// <seealso cref=          #isIndexSelected </seealso>
		public virtual void Deselect(int index)
		{
			lock (this)
			{
				ListPeer peer = (ListPeer)this.Peer_Renamed;
				if (peer != null)
				{
					if (MultipleMode || (SelectedIndex == index))
					{
						peer.Deselect(index);
					}
				}
        
				for (int i = 0 ; i < Selected.Length ; i++)
				{
					if (Selected[i] == index)
					{
						int[] newsel = new int[Selected.Length - 1];
						System.Array.Copy(Selected, 0, newsel, 0, i);
						System.Array.Copy(Selected, i + 1, newsel, i, Selected.Length - (i + 1));
						Selected = newsel;
						return;
					}
				}
			}
		}

		/// <summary>
		/// Determines if the specified item in this scrolling list is
		/// selected. </summary>
		/// <param name="index">   the item to be checked </param>
		/// <returns>     <code>true</code> if the specified item has been
		///                       selected; <code>false</code> otherwise </returns>
		/// <seealso cref=        #select </seealso>
		/// <seealso cref=        #deselect
		/// @since      JDK1.1 </seealso>
		public virtual bool IsIndexSelected(int index)
		{
			return IsSelected(index);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>isIndexSelected(int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool IsSelected(int index)
		{
			int[] sel = SelectedIndexes;
			for (int i = 0 ; i < sel.Length ; i++)
			{
				if (sel[i] == index)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the number of visible lines in this list.  Note that
		/// once the <code>List</code> has been created, this number
		/// will never change. </summary>
		/// <returns>     the number of visible lines in this scrolling list </returns>
		public virtual int Rows
		{
			get
			{
				return Rows_Renamed;
			}
		}

		/// <summary>
		/// Determines whether this list allows multiple selections. </summary>
		/// <returns>     <code>true</code> if this list allows multiple
		///                 selections; otherwise, <code>false</code> </returns>
		/// <seealso cref=        #setMultipleMode
		/// @since      JDK1.1 </seealso>
		public virtual bool MultipleMode
		{
			get
			{
				return AllowsMultipleSelections();
			}
			set
			{
				MultipleSelections = value;
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>isMultipleMode()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool AllowsMultipleSelections()
		{
			return MultipleMode_Renamed;
		}


		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>setMultipleMode(boolean)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual bool MultipleSelections
		{
			set
			{
				lock (this)
				{
					if (value != MultipleMode_Renamed)
					{
						MultipleMode_Renamed = value;
						ListPeer peer = (ListPeer)this.Peer_Renamed;
						if (peer != null)
						{
							peer.MultipleMode = value;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the index of the item that was last made visible by
		/// the method <code>makeVisible</code>. </summary>
		/// <returns>      the index of the item that was last made visible </returns>
		/// <seealso cref=         #makeVisible </seealso>
		public virtual int VisibleIndex
		{
			get
			{
				return VisibleIndex_Renamed;
			}
		}

		/// <summary>
		/// Makes the item at the specified index visible. </summary>
		/// <param name="index">    the position of the item </param>
		/// <seealso cref=         #getVisibleIndex </seealso>
		public virtual void MakeVisible(int index)
		{
			lock (this)
			{
				VisibleIndex_Renamed = index;
				ListPeer peer = (ListPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.MakeVisible(index);
				}
			}
		}

		/// <summary>
		/// Gets the preferred dimensions for a list with the specified
		/// number of rows. </summary>
		/// <param name="rows">    number of rows in the list </param>
		/// <returns>     the preferred dimensions for displaying this scrolling list
		///             given that the specified number of rows must be visible </returns>
		/// <seealso cref=        java.awt.Component#getPreferredSize
		/// @since      JDK1.1 </seealso>
		public virtual Dimension GetPreferredSize(int rows)
		{
			return PreferredSize(rows);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getPreferredSize(int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Dimension PreferredSize(int rows)
		{
			lock (TreeLock)
			{
				ListPeer peer = (ListPeer)this.Peer_Renamed;
				return (peer != null) ? peer.GetPreferredSize(rows) : base.PreferredSize();
			}
		}

		/// <summary>
		/// Gets the preferred size of this scrolling list. </summary>
		/// <returns>     the preferred dimensions for displaying this scrolling list </returns>
		/// <seealso cref=        java.awt.Component#getPreferredSize
		/// @since      JDK1.1 </seealso>
		public override Dimension PreferredSize
		{
			get
			{
				return PreferredSize();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getPreferredSize()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public override Dimension PreferredSize()
		{
			lock (TreeLock)
			{
				return (Rows_Renamed > 0) ? PreferredSize(Rows_Renamed) : base.PreferredSize();
			}
		}

		/// <summary>
		/// Gets the minimum dimensions for a list with the specified
		/// number of rows. </summary>
		/// <param name="rows">    number of rows in the list </param>
		/// <returns>     the minimum dimensions for displaying this scrolling list
		///             given that the specified number of rows must be visible </returns>
		/// <seealso cref=        java.awt.Component#getMinimumSize
		/// @since      JDK1.1 </seealso>
		public virtual Dimension GetMinimumSize(int rows)
		{
			return MinimumSize(rows);
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getMinimumSize(int)</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual Dimension MinimumSize(int rows)
		{
			lock (TreeLock)
			{
				ListPeer peer = (ListPeer)this.Peer_Renamed;
				return (peer != null) ? peer.GetMinimumSize(rows) : base.MinimumSize();
			}
		}

		/// <summary>
		/// Determines the minimum size of this scrolling list. </summary>
		/// <returns>       the minimum dimensions needed
		///                        to display this scrolling list </returns>
		/// <seealso cref=          java.awt.Component#getMinimumSize()
		/// @since        JDK1.1 </seealso>
		public override Dimension MinimumSize
		{
			get
			{
				return MinimumSize();
			}
		}

		/// @deprecated As of JDK version 1.1,
		/// replaced by <code>getMinimumSize()</code>. 
		[Obsolete("As of JDK version 1.1,")]
		public override Dimension MinimumSize()
		{
			lock (TreeLock)
			{
				return (Rows_Renamed > 0) ? MinimumSize(Rows_Renamed) : base.MinimumSize();
			}
		}

		/// <summary>
		/// Adds the specified item listener to receive item events from
		/// this list.  Item events are sent in response to user input, but not
		/// in response to calls to <code>select</code> or <code>deselect</code>.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the item listener </param>
		/// <seealso cref=           #removeItemListener </seealso>
		/// <seealso cref=           #getItemListeners </seealso>
		/// <seealso cref=           #select </seealso>
		/// <seealso cref=           #deselect </seealso>
		/// <seealso cref=           java.awt.event.ItemEvent </seealso>
		/// <seealso cref=           java.awt.event.ItemListener
		/// @since         JDK1.1 </seealso>
		public virtual void AddItemListener(ItemListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				ItemListener = AWTEventMulticaster.Add(ItemListener, l);
				NewEventsOnly = true;
			}
		}

		/// <summary>
		/// Removes the specified item listener so that it no longer
		/// receives item events from this list.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the item listener </param>
		/// <seealso cref=             #addItemListener </seealso>
		/// <seealso cref=             #getItemListeners </seealso>
		/// <seealso cref=             java.awt.event.ItemEvent </seealso>
		/// <seealso cref=             java.awt.event.ItemListener
		/// @since           JDK1.1 </seealso>
		public virtual void RemoveItemListener(ItemListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				ItemListener = AWTEventMulticaster.Remove(ItemListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the item listeners
		/// registered on this list.
		/// </summary>
		/// <returns> all of this list's <code>ItemListener</code>s
		///         or an empty array if no item
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=             #addItemListener </seealso>
		/// <seealso cref=             #removeItemListener </seealso>
		/// <seealso cref=             java.awt.event.ItemEvent </seealso>
		/// <seealso cref=             java.awt.event.ItemListener
		/// @since 1.4 </seealso>
		public virtual ItemListener[] ItemListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(ItemListener));
				}
			}
		}

		/// <summary>
		/// Adds the specified action listener to receive action events from
		/// this list. Action events occur when a user double-clicks
		/// on a list item or types Enter when the list has the keyboard
		/// focus.
		/// <para>
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// </para>
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l"> the action listener </param>
		/// <seealso cref=           #removeActionListener </seealso>
		/// <seealso cref=           #getActionListeners </seealso>
		/// <seealso cref=           java.awt.event.ActionEvent </seealso>
		/// <seealso cref=           java.awt.event.ActionListener
		/// @since         JDK1.1 </seealso>
		public virtual void AddActionListener(ActionListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				ActionListener = AWTEventMulticaster.Add(ActionListener, l);
				NewEventsOnly = true;
			}
		}

		/// <summary>
		/// Removes the specified action listener so that it no longer
		/// receives action events from this list. Action events
		/// occur when a user double-clicks on a list item.
		/// If listener <code>l</code> is <code>null</code>,
		/// no exception is thrown and no action is performed.
		/// <para>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		/// >AWT Threading Issues</a> for details on AWT's threading model.
		/// 
		/// </para>
		/// </summary>
		/// <param name="l">     the action listener </param>
		/// <seealso cref=             #addActionListener </seealso>
		/// <seealso cref=             #getActionListeners </seealso>
		/// <seealso cref=             java.awt.event.ActionEvent </seealso>
		/// <seealso cref=             java.awt.event.ActionListener
		/// @since           JDK1.1 </seealso>
		public virtual void RemoveActionListener(ActionListener l)
		{
			lock (this)
			{
				if (l == null)
				{
					return;
				}
				ActionListener = AWTEventMulticaster.Remove(ActionListener, l);
			}
		}

		/// <summary>
		/// Returns an array of all the action listeners
		/// registered on this list.
		/// </summary>
		/// <returns> all of this list's <code>ActionListener</code>s
		///         or an empty array if no action
		///         listeners are currently registered
		/// </returns>
		/// <seealso cref=             #addActionListener </seealso>
		/// <seealso cref=             #removeActionListener </seealso>
		/// <seealso cref=             java.awt.event.ActionEvent </seealso>
		/// <seealso cref=             java.awt.event.ActionListener
		/// @since 1.4 </seealso>
		public virtual ActionListener[] ActionListeners
		{
			get
			{
				lock (this)
				{
					return GetListeners(typeof(ActionListener));
				}
			}
		}

		/// <summary>
		/// Returns an array of all the objects currently registered
		/// as <code><em>Foo</em>Listener</code>s
		/// upon this <code>List</code>.
		/// <code><em>Foo</em>Listener</code>s are registered using the
		/// <code>add<em>Foo</em>Listener</code> method.
		/// 
		/// <para>
		/// You can specify the <code>listenerType</code> argument
		/// with a class literal, such as
		/// <code><em>Foo</em>Listener.class</code>.
		/// For example, you can query a
		/// <code>List</code> <code>l</code>
		/// for its item listeners with the following code:
		/// 
		/// <pre>ItemListener[] ils = (ItemListener[])(l.getListeners(ItemListener.class));</pre>
		/// 
		/// If no such listeners exist, this method returns an empty array.
		/// 
		/// </para>
		/// </summary>
		/// <param name="listenerType"> the type of listeners requested; this parameter
		///          should specify an interface that descends from
		///          <code>java.util.EventListener</code> </param>
		/// <returns> an array of all objects registered as
		///          <code><em>Foo</em>Listener</code>s on this list,
		///          or an empty array if no such
		///          listeners have been added </returns>
		/// <exception cref="ClassCastException"> if <code>listenerType</code>
		///          doesn't specify a class or interface that implements
		///          <code>java.util.EventListener</code>
		/// </exception>
		/// <seealso cref= #getItemListeners
		/// @since 1.3 </seealso>
		public override T[] getListeners<T>(Class listenerType) where T : java.util.EventListener
		{
			EventListener l = null;
			if (listenerType == typeof(ActionListener))
			{
				l = ActionListener;
			}
			else if (listenerType == typeof(ItemListener))
			{
				l = ItemListener;
			}
			else
			{
				return base.GetListeners(listenerType);
			}
			return AWTEventMulticaster.GetListeners(l, listenerType);
		}

		// REMIND: remove when filtering is done at lower level
		internal override bool EventEnabled(AWTEvent e)
		{
			switch (e.Id)
			{
			  case ActionEvent.ACTION_PERFORMED:
				if ((EventMask & AWTEvent.ACTION_EVENT_MASK) != 0 || ActionListener != null)
				{
					return true;
				}
				return false;
			  case ItemEvent.ITEM_STATE_CHANGED:
				if ((EventMask & AWTEvent.ITEM_EVENT_MASK) != 0 || ItemListener != null)
				{
					return true;
				}
				return false;
			  default:
				break;
			}
			return base.EventEnabled(e);
		}

		/// <summary>
		/// Processes events on this scrolling list. If an event is
		/// an instance of <code>ItemEvent</code>, it invokes the
		/// <code>processItemEvent</code> method. Else, if the
		/// event is an instance of <code>ActionEvent</code>,
		/// it invokes <code>processActionEvent</code>.
		/// If the event is not an item event or an action event,
		/// it invokes <code>processEvent</code> on the superclass.
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the event </param>
		/// <seealso cref=          java.awt.event.ActionEvent </seealso>
		/// <seealso cref=          java.awt.event.ItemEvent </seealso>
		/// <seealso cref=          #processActionEvent </seealso>
		/// <seealso cref=          #processItemEvent
		/// @since        JDK1.1 </seealso>
		protected internal override void ProcessEvent(AWTEvent e)
		{
			if (e is ItemEvent)
			{
				ProcessItemEvent((ItemEvent)e);
				return;
			}
			else if (e is ActionEvent)
			{
				ProcessActionEvent((ActionEvent)e);
				return;
			}
			base.ProcessEvent(e);
		}

		/// <summary>
		/// Processes item events occurring on this list by
		/// dispatching them to any registered
		/// <code>ItemListener</code> objects.
		/// <para>
		/// This method is not called unless item events are
		/// enabled for this component. Item events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>An <code>ItemListener</code> object is registered
		/// via <code>addItemListener</code>.
		/// <li>Item events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the item event </param>
		/// <seealso cref=         java.awt.event.ItemEvent </seealso>
		/// <seealso cref=         java.awt.event.ItemListener </seealso>
		/// <seealso cref=         #addItemListener </seealso>
		/// <seealso cref=         java.awt.Component#enableEvents
		/// @since       JDK1.1 </seealso>
		protected internal virtual void ProcessItemEvent(ItemEvent e)
		{
			ItemListener listener = ItemListener;
			if (listener != null)
			{
				listener.ItemStateChanged(e);
			}
		}

		/// <summary>
		/// Processes action events occurring on this component
		/// by dispatching them to any registered
		/// <code>ActionListener</code> objects.
		/// <para>
		/// This method is not called unless action events are
		/// enabled for this component. Action events are enabled
		/// when one of the following occurs:
		/// <ul>
		/// <li>An <code>ActionListener</code> object is registered
		/// via <code>addActionListener</code>.
		/// <li>Action events are enabled via <code>enableEvents</code>.
		/// </ul>
		/// </para>
		/// <para>Note that if the event parameter is <code>null</code>
		/// the behavior is unspecified and may result in an
		/// exception.
		/// 
		/// </para>
		/// </summary>
		/// <param name="e"> the action event </param>
		/// <seealso cref=         java.awt.event.ActionEvent </seealso>
		/// <seealso cref=         java.awt.event.ActionListener </seealso>
		/// <seealso cref=         #addActionListener </seealso>
		/// <seealso cref=         java.awt.Component#enableEvents
		/// @since       JDK1.1 </seealso>
		protected internal virtual void ProcessActionEvent(ActionEvent e)
		{
			ActionListener listener = ActionListener;
			if (listener != null)
			{
				listener.ActionPerformed(e);
			}
		}

		/// <summary>
		/// Returns the parameter string representing the state of this
		/// scrolling list. This string is useful for debugging. </summary>
		/// <returns>    the parameter string of this scrolling list </returns>
		protected internal override String ParamString()
		{
			return base.ParamString() + ",selected=" + SelectedItem;
		}

		/// @deprecated As of JDK version 1.1,
		/// Not for public use in the future.
		/// This method is expected to be retained only as a package
		/// private method. 
		[Obsolete("As of JDK version 1.1,")]
		public virtual void DelItems(int start, int end)
		{
			lock (this)
			{
				for (int i = end; i >= start; i--)
				{
					Items_Renamed.RemoveAt(i);
				}
				ListPeer peer = (ListPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.DelItems(start, end);
				}
			}
		}

		/*
		 * Serialization support.  Since the value of the selected
		 * field isn't necessarily up to date, we sync it up with the
		 * peer before serializing.
		 */

		/// <summary>
		/// The <code>List</code> component's
		/// Serialized Data Version.
		/// 
		/// @serial
		/// </summary>
		private int ListSerializedDataVersion = 1;

		/// <summary>
		/// Writes default serializable fields to stream.  Writes
		/// a list of serializable <code>ItemListeners</code>
		/// and <code>ActionListeners</code> as optional data.
		/// The non-serializable listeners are detected and
		/// no attempt is made to serialize them.
		/// 
		/// @serialData <code>null</code> terminated sequence of 0
		///  or more pairs; the pair consists of a <code>String</code>
		///  and an <code>Object</code>; the <code>String</code>
		///  indicates the type of object and is one of the
		///  following:
		///  <code>itemListenerK</code> indicating an
		///    <code>ItemListener</code> object;
		///  <code>actionListenerK</code> indicating an
		///    <code>ActionListener</code> object
		/// </summary>
		/// <param name="s"> the <code>ObjectOutputStream</code> to write </param>
		/// <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		/// <seealso cref= java.awt.Component#itemListenerK </seealso>
		/// <seealso cref= java.awt.Component#actionListenerK </seealso>
		/// <seealso cref= #readObject(ObjectInputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
		private void WriteObject(ObjectOutputStream s)
		{
		  lock (this)
		  {
			ListPeer peer = (ListPeer)this.Peer_Renamed;
			if (peer != null)
			{
			  Selected = peer.SelectedIndexes;
			}
		  }
		  s.DefaultWriteObject();

		  AWTEventMulticaster.Save(s, ItemListenerK, ItemListener);
		  AWTEventMulticaster.Save(s, ActionListenerK, ActionListener);
		  s.WriteObject(null);
		}

		/// <summary>
		/// Reads the <code>ObjectInputStream</code> and if it
		/// isn't <code>null</code> adds a listener to receive
		/// both item events and action events (as specified
		/// by the key stored in the stream) fired by the
		/// <code>List</code>.
		/// Unrecognized keys or values will be ignored.
		/// </summary>
		/// <param name="s"> the <code>ObjectInputStream</code> to write </param>
		/// <exception cref="HeadlessException"> if
		///   <code>GraphicsEnvironment.isHeadless</code> returns
		///   <code>true</code> </exception>
		/// <seealso cref= #removeItemListener(ItemListener) </seealso>
		/// <seealso cref= #addItemListener(ItemListener) </seealso>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref= #writeObject(ObjectOutputStream) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException, HeadlessException
		private void ReadObject(ObjectInputStream s)
		{
		  GraphicsEnvironment.CheckHeadless();
		  s.DefaultReadObject();

		  Object keyOrNull;
		  while (null != (keyOrNull = s.ReadObject()))
		  {
			String key = ((String)keyOrNull).intern();

			if (ItemListenerK == key)
			{
			  AddItemListener((ItemListener)(s.ReadObject()));
			}

			else if (ActionListenerK == key)
			{
			  AddActionListener((ActionListener)(s.ReadObject()));
			}

			else // skip value for unrecognized key
			{
			  s.ReadObject();
			}
		  }
		}


	/////////////////
	// Accessibility support
	////////////////


		/// <summary>
		/// Gets the <code>AccessibleContext</code> associated with this
		/// <code>List</code>. For lists, the <code>AccessibleContext</code>
		/// takes the form of an <code>AccessibleAWTList</code>.
		/// A new <code>AccessibleAWTList</code> instance is created, if necessary.
		/// </summary>
		/// <returns> an <code>AccessibleAWTList</code> that serves as the
		///         <code>AccessibleContext</code> of this <code>List</code>
		/// @since 1.3 </returns>
		public override AccessibleContext AccessibleContext
		{
			get
			{
				if (AccessibleContext_Renamed == null)
				{
					AccessibleContext_Renamed = new AccessibleAWTList(this);
				}
				return AccessibleContext_Renamed;
			}
		}

		/// <summary>
		/// This class implements accessibility support for the
		/// <code>List</code> class.  It provides an implementation of the
		/// Java Accessibility API appropriate to list user-interface elements.
		/// @since 1.3
		/// </summary>
		protected internal class AccessibleAWTList : AccessibleAWTComponent, AccessibleSelection, ItemListener, ActionListener
		{
			private readonly List OuterInstance;

			/*
			 * JDK 1.3 serialVersionUID
			 */
			internal const long SerialVersionUID = 7924617370136012829L;

			public AccessibleAWTList(List outerInstance) : base(outerInstance)
			{
				this.OuterInstance = outerInstance;
				outerInstance.AddActionListener(this);
				outerInstance.AddItemListener(this);
			}

			public virtual void ActionPerformed(ActionEvent @event)
			{
			}

			public virtual void ItemStateChanged(ItemEvent @event)
			{
			}

			/// <summary>
			/// Get the state set of this object.
			/// </summary>
			/// <returns> an instance of AccessibleState containing the current state
			/// of the object </returns>
			/// <seealso cref= AccessibleState </seealso>
			public override AccessibleStateSet AccessibleStateSet
			{
				get
				{
					AccessibleStateSet states = base.AccessibleStateSet;
					if (OuterInstance.MultipleMode)
					{
						states.add(AccessibleState.MULTISELECTABLE);
					}
					return states;
				}
			}

			/// <summary>
			/// Get the role of this object.
			/// </summary>
			/// <returns> an instance of AccessibleRole describing the role of the
			/// object </returns>
			/// <seealso cref= AccessibleRole </seealso>
			public override AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibleRole.LIST;
				}
			}

			/// <summary>
			/// Returns the Accessible child contained at the local coordinate
			/// Point, if one exists.
			/// </summary>
			/// <returns> the Accessible at the specified location, if it exists </returns>
			public override Accessible GetAccessibleAt(Point p)
			{
				return null; // fredxFIXME Not implemented yet
			}

			/// <summary>
			/// Returns the number of accessible children in the object.  If all
			/// of the children of this object implement Accessible, than this
			/// method should return the number of children of this object.
			/// </summary>
			/// <returns> the number of accessible children in the object. </returns>
			public override int AccessibleChildrenCount
			{
				get
				{
					return OuterInstance.ItemCount;
				}
			}

			/// <summary>
			/// Return the nth Accessible child of the object.
			/// </summary>
			/// <param name="i"> zero-based index of child </param>
			/// <returns> the nth Accessible child of the object </returns>
			public override Accessible GetAccessibleChild(int i)
			{
				lock (OuterInstance)
				{
					if (i >= OuterInstance.ItemCount)
					{
						return null;
					}
					else
					{
						return new AccessibleAWTListChild(this, OuterInstance, i);
					}
				}
			}

			/// <summary>
			/// Get the AccessibleSelection associated with this object.  In the
			/// implementation of the Java Accessibility API for this class,
			/// return this object, which is responsible for implementing the
			/// AccessibleSelection interface on behalf of itself.
			/// </summary>
			/// <returns> this object </returns>
			public virtual AccessibleSelection AccessibleSelection
			{
				get
				{
					return this;
				}
			}

		// AccessibleSelection methods

			/// <summary>
			/// Returns the number of items currently selected.
			/// If no items are selected, the return value will be 0.
			/// </summary>
			/// <returns> the number of items currently selected. </returns>
			 public virtual int AccessibleSelectionCount
			 {
				 get
				 {
					 return OuterInstance.SelectedIndexes.Length;
				 }
			 }

			/// <summary>
			/// Returns an Accessible representing the specified selected item
			/// in the object.  If there isn't a selection, or there are
			/// fewer items selected than the integer passed in, the return
			/// value will be null.
			/// </summary>
			/// <param name="i"> the zero-based index of selected items </param>
			/// <returns> an Accessible containing the selected item </returns>
			 public virtual Accessible GetAccessibleSelection(int i)
			 {
				 lock (OuterInstance)
				 {
					 int len = AccessibleSelectionCount;
					 if (i < 0 || i >= len)
					 {
						 return null;
					 }
					 else
					 {
						 return GetAccessibleChild(OuterInstance.SelectedIndexes[i]);
					 }
				 }
			 }

			/// <summary>
			/// Returns true if the current child of this object is selected.
			/// </summary>
			/// <param name="i"> the zero-based index of the child in this Accessible
			/// object. </param>
			/// <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			public virtual bool IsAccessibleChildSelected(int i)
			{
				return OuterInstance.IsIndexSelected(i);
			}

			/// <summary>
			/// Adds the specified selected item in the object to the object's
			/// selection.  If the object supports multiple selections,
			/// the specified item is added to any existing selection, otherwise
			/// it replaces any existing selection in the object.  If the
			/// specified item is already selected, this method has no effect.
			/// </summary>
			/// <param name="i"> the zero-based index of selectable items </param>
			 public virtual void AddAccessibleSelection(int i)
			 {
				 OuterInstance.Select(i);
			 }

			/// <summary>
			/// Removes the specified selected item in the object from the object's
			/// selection.  If the specified item isn't currently selected, this
			/// method has no effect.
			/// </summary>
			/// <param name="i"> the zero-based index of selectable items </param>
			 public virtual void RemoveAccessibleSelection(int i)
			 {
				 OuterInstance.Deselect(i);
			 }

			/// <summary>
			/// Clears the selection in the object, so that nothing in the
			/// object is selected.
			/// </summary>
			 public virtual void ClearAccessibleSelection()
			 {
				 lock (OuterInstance)
				 {
					 int[] selectedIndexes = OuterInstance.SelectedIndexes;
					 if (selectedIndexes == null)
					 {
						 return;
					 }
					 for (int i = selectedIndexes.Length - 1; i >= 0; i--)
					 {
						 OuterInstance.Deselect(selectedIndexes[i]);
					 }
				 }
			 }

			/// <summary>
			/// Causes every selected item in the object to be selected
			/// if the object supports multiple selections.
			/// </summary>
			 public virtual void SelectAllAccessibleSelection()
			 {
				 lock (OuterInstance)
				 {
					 for (int i = OuterInstance.ItemCount - 1; i >= 0; i--)
					 {
						 OuterInstance.Select(i);
					 }
				 }
			 }

		   /// <summary>
		   /// This class implements accessibility support for
		   /// List children.  It provides an implementation of the
		   /// Java Accessibility API appropriate to list children
		   /// user-interface elements.
		   /// @since 1.3
		   /// </summary>
			protected internal class AccessibleAWTListChild : AccessibleAWTComponent, Accessible
			{
				private readonly List.AccessibleAWTList OuterInstance;

				/*
				 * JDK 1.3 serialVersionUID
				 */
				internal const long SerialVersionUID = 4412022926028300317L;

			// [[[FIXME]]] need to finish implementing this!!!

				internal List Parent;
				internal int IndexInParent;

				public AccessibleAWTListChild(List.AccessibleAWTList outerInstance, List parent, int indexInParent) : base(outerInstance.OuterInstance)
				{
					this.OuterInstance = outerInstance;
					this.Parent = parent;
					this.AccessibleParent = parent;
					this.IndexInParent = indexInParent;
				}

				//
				// required Accessible methods
				//
			  /// <summary>
			  /// Gets the AccessibleContext for this object.  In the
			  /// implementation of the Java Accessibility API for this class,
			  /// return this object, which acts as its own AccessibleContext.
			  /// </summary>
			  /// <returns> this object </returns>
				public virtual AccessibleContext AccessibleContext
				{
					get
					{
						return this;
					}
				}

				//
				// required AccessibleContext methods
				//

				/// <summary>
				/// Get the role of this object.
				/// </summary>
				/// <returns> an instance of AccessibleRole describing the role of
				/// the object </returns>
				/// <seealso cref= AccessibleRole </seealso>
				public override AccessibleRole AccessibleRole
				{
					get
					{
						return AccessibleRole.LIST_ITEM;
					}
				}

				/// <summary>
				/// Get the state set of this object.  The AccessibleStateSet of an
				/// object is composed of a set of unique AccessibleState's.  A
				/// change in the AccessibleStateSet of an object will cause a
				/// PropertyChangeEvent to be fired for the
				/// ACCESSIBLE_STATE_PROPERTY property.
				/// </summary>
				/// <returns> an instance of AccessibleStateSet containing the
				/// current state set of the object </returns>
				/// <seealso cref= AccessibleStateSet </seealso>
				/// <seealso cref= AccessibleState </seealso>
				/// <seealso cref= #addPropertyChangeListener </seealso>
				public override AccessibleStateSet AccessibleStateSet
				{
					get
					{
						AccessibleStateSet states = base.AccessibleStateSet;
						if (Parent.IsIndexSelected(IndexInParent))
						{
							states.add(AccessibleState.SELECTED);
						}
						return states;
					}
				}

				/// <summary>
				/// Gets the locale of the component. If the component does not
				/// have a locale, then the locale of its parent is returned.
				/// </summary>
				/// <returns> This component's locale.  If this component does not have
				/// a locale, the locale of its parent is returned.
				/// </returns>
				/// <exception cref="IllegalComponentStateException">
				/// If the Component does not have its own locale and has not yet
				/// been added to a containment hierarchy such that the locale can
				/// be determined from the containing parent. </exception>
				public override Locale Locale
				{
					get
					{
						return Parent.Locale;
					}
				}

				/// <summary>
				/// Get the 0-based index of this object in its accessible parent.
				/// </summary>
				/// <returns> the 0-based index of this object in its parent; -1 if
				/// this object does not have an accessible parent.
				/// </returns>
				/// <seealso cref= #getAccessibleParent </seealso>
				/// <seealso cref= #getAccessibleChildrenCount </seealso>
				/// <seealso cref= #getAccessibleChild </seealso>
				public override int AccessibleIndexInParent
				{
					get
					{
						return IndexInParent;
					}
				}

				/// <summary>
				/// Returns the number of accessible children of the object.
				/// </summary>
				/// <returns> the number of accessible children of the object. </returns>
				public override int AccessibleChildrenCount
				{
					get
					{
						return 0; // list elements can't have children
					}
				}

				/// <summary>
				/// Return the specified Accessible child of the object.  The
				/// Accessible children of an Accessible object are zero-based,
				/// so the first child of an Accessible child is at index 0, the
				/// second child is at index 1, and so on.
				/// </summary>
				/// <param name="i"> zero-based index of child </param>
				/// <returns> the Accessible child of the object </returns>
				/// <seealso cref= #getAccessibleChildrenCount </seealso>
				public override Accessible GetAccessibleChild(int i)
				{
					return null; // list elements can't have children
				}


				//
				// AccessibleComponent delegatation to parent List
				//

				/// <summary>
				/// Get the background color of this object.
				/// </summary>
				/// <returns> the background color, if supported, of the object;
				/// otherwise, null </returns>
				/// <seealso cref= #setBackground </seealso>
				public override Color Background
				{
					get
					{
						return Parent.Background;
					}
					set
					{
						Parent.Background = value;
					}
				}


				/// <summary>
				/// Get the foreground color of this object.
				/// </summary>
				/// <returns> the foreground color, if supported, of the object;
				/// otherwise, null </returns>
				/// <seealso cref= #setForeground </seealso>
				public override Color Foreground
				{
					get
					{
						return Parent.Foreground;
					}
					set
					{
						Parent.Foreground = value;
					}
				}


				/// <summary>
				/// Get the Cursor of this object.
				/// </summary>
				/// <returns> the Cursor, if supported, of the object; otherwise, null </returns>
				/// <seealso cref= #setCursor </seealso>
				public override Cursor Cursor
				{
					get
					{
						return Parent.Cursor;
					}
					set
					{
						Parent.Cursor = value;
					}
				}


				/// <summary>
				/// Get the Font of this object.
				/// </summary>
				/// <returns> the Font,if supported, for the object; otherwise, null </returns>
				/// <seealso cref= #setFont </seealso>
				public override Font Font
				{
					get
					{
						return Parent.Font;
					}
					set
					{
						Parent.Font = value;
					}
				}


				/// <summary>
				/// Get the FontMetrics of this object.
				/// </summary>
				/// <param name="f"> the Font </param>
				/// <returns> the FontMetrics, if supported, the object; otherwise, null </returns>
				/// <seealso cref= #getFont </seealso>
				public override FontMetrics GetFontMetrics(Font f)
				{
					return Parent.GetFontMetrics(f);
				}

				/// <summary>
				/// Determine if the object is enabled.  Objects that are enabled
				/// will also have the AccessibleState.ENABLED state set in their
				/// AccessibleStateSet.
				/// </summary>
				/// <returns> true if object is enabled; otherwise, false </returns>
				/// <seealso cref= #setEnabled </seealso>
				/// <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
				/// <seealso cref= AccessibleState#ENABLED </seealso>
				/// <seealso cref= AccessibleStateSet </seealso>
				public override bool Enabled
				{
					get
					{
						return Parent.Enabled;
					}
					set
					{
						Parent.Enabled = value;
					}
				}


				/// <summary>
				/// Determine if the object is visible.  Note: this means that the
				/// object intends to be visible; however, it may not be
				/// showing on the screen because one of the objects that this object
				/// is contained by is currently not visible.  To determine if an
				/// object is showing on the screen, use isShowing().
				/// <para>Objects that are visible will also have the
				/// AccessibleState.VISIBLE state set in their AccessibleStateSet.
				/// 
				/// </para>
				/// </summary>
				/// <returns> true if object is visible; otherwise, false </returns>
				/// <seealso cref= #setVisible </seealso>
				/// <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
				/// <seealso cref= AccessibleState#VISIBLE </seealso>
				/// <seealso cref= AccessibleStateSet </seealso>
				public override bool Visible
				{
					get
					{
						// [[[FIXME]]] needs to work like isShowing() below
						return false;
						// return parent.isVisible();
					}
					set
					{
						// [[[FIXME]]] should scroll to item to make it show!
						Parent.Visible = value;
					}
				}


				/// <summary>
				/// Determine if the object is showing.  This is determined by
				/// checking the visibility of the object and visibility of the
				/// object ancestors.
				/// Note: this will return true even if the object is obscured
				/// by another (for example, it to object is underneath a menu
				/// that was pulled down).
				/// </summary>
				/// <returns> true if object is showing; otherwise, false </returns>
				public override bool Showing
				{
					get
					{
						// [[[FIXME]]] only if it's showing!!!
						return false;
						// return parent.isShowing();
					}
				}

				/// <summary>
				/// Checks whether the specified point is within this object's
				/// bounds, where the point's x and y coordinates are defined to
				/// be relative to the coordinate system of the object.
				/// </summary>
				/// <param name="p"> the Point relative to the coordinate system of the
				/// object </param>
				/// <returns> true if object contains Point; otherwise false </returns>
				/// <seealso cref= #getBounds </seealso>
				public override bool Contains(Point p)
				{
					// [[[FIXME]]] - only if p is within the list element!!!
					return false;
					// return parent.contains(p);
				}

				/// <summary>
				/// Returns the location of the object on the screen.
				/// </summary>
				/// <returns> location of object on screen; null if this object
				/// is not on the screen </returns>
				/// <seealso cref= #getBounds </seealso>
				/// <seealso cref= #getLocation </seealso>
				public override Point LocationOnScreen
				{
					get
					{
						// [[[FIXME]]] sigh
						return null;
					}
				}

				/// <summary>
				/// Gets the location of the object relative to the parent in the
				/// form of a point specifying the object's top-left corner in the
				/// screen's coordinate space.
				/// </summary>
				/// <returns> An instance of Point representing the top-left corner of
				/// the objects's bounds in the coordinate space of the screen; null
				/// if this object or its parent are not on the screen </returns>
				/// <seealso cref= #getBounds </seealso>
				/// <seealso cref= #getLocationOnScreen </seealso>
				public override Point Location
				{
					get
					{
						// [[[FIXME]]]
						return null;
					}
					set
					{
						// [[[FIXME]]] maybe - can simply return as no-op
					}
				}


				/// <summary>
				/// Gets the bounds of this object in the form of a Rectangle object.
				/// The bounds specify this object's width, height, and location
				/// relative to its parent.
				/// </summary>
				/// <returns> A rectangle indicating this component's bounds; null if
				/// this object is not on the screen. </returns>
				/// <seealso cref= #contains </seealso>
				public override Rectangle Bounds
				{
					get
					{
						// [[[FIXME]]]
						return null;
					}
					set
					{
						// no-op; not supported
					}
				}


				/// <summary>
				/// Returns the size of this object in the form of a Dimension
				/// object.  The height field of the Dimension object contains this
				/// objects's height, and the width field of the Dimension object
				/// contains this object's width.
				/// </summary>
				/// <returns> A Dimension object that indicates the size of this
				/// component; null if this object is not on the screen </returns>
				/// <seealso cref= #setSize </seealso>
				public override Dimension Size
				{
					get
					{
						// [[[FIXME]]]
						return null;
					}
					set
					{
						// not supported; no-op
					}
				}


				/// <summary>
				/// Returns the <code>Accessible</code> child, if one exists,
				/// contained at the local coordinate <code>Point</code>.
				/// </summary>
				/// <param name="p"> the point relative to the coordinate system of this
				///     object </param>
				/// <returns> the <code>Accessible</code>, if it exists,
				///     at the specified location; otherwise <code>null</code> </returns>
				public override Accessible GetAccessibleAt(Point p)
				{
					return null; // object cannot have children!
				}

				/// <summary>
				/// Returns whether this object can accept focus or not.   Objects
				/// that can accept focus will also have the
				/// <code>AccessibleState.FOCUSABLE</code> state set in their
				/// <code>AccessibleStateSet</code>.
				/// </summary>
				/// <returns> true if object can accept focus; otherwise false </returns>
				/// <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
				/// <seealso cref= AccessibleState#FOCUSABLE </seealso>
				/// <seealso cref= AccessibleState#FOCUSED </seealso>
				/// <seealso cref= AccessibleStateSet </seealso>
				public override bool FocusTraversable
				{
					get
					{
						return false; // list element cannot receive focus!
					}
				}

				/// <summary>
				/// Requests focus for this object.  If this object cannot accept
				/// focus, nothing will happen.  Otherwise, the object will attempt
				/// to take focus. </summary>
				/// <seealso cref= #isFocusTraversable </seealso>
				public override void RequestFocus()
				{
					// nothing to do; a no-op
				}

				/// <summary>
				/// Adds the specified focus listener to receive focus events from
				/// this component.
				/// </summary>
				/// <param name="l"> the focus listener </param>
				/// <seealso cref= #removeFocusListener </seealso>
				public override void AddFocusListener(FocusListener l)
				{
					// nothing to do; a no-op
				}

				/// <summary>
				/// Removes the specified focus listener so it no longer receives
				/// focus events from this component.
				/// </summary>
				/// <param name="l"> the focus listener </param>
				/// <seealso cref= #addFocusListener </seealso>
				public override void RemoveFocusListener(FocusListener l)
				{
					// nothing to do; a no-op
				}



			} // inner class AccessibleAWTListChild

		} // inner class AccessibleAWTList

	}

}