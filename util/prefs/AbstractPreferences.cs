using System;
using System.Collections.Generic;
using System.Threading;

/*
 * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.util.prefs
{

	// These imports needed only as a workaround for a JavaDoc bug

	/// <summary>
	/// This class provides a skeletal implementation of the <seealso cref="Preferences"/>
	/// class, greatly easing the task of implementing it.
	/// 
	/// <para><strong>This class is for <tt>Preferences</tt> implementers only.
	/// Normal users of the <tt>Preferences</tt> facility should have no need to
	/// consult this documentation.  The <seealso cref="Preferences"/> documentation
	/// should suffice.</strong>
	/// 
	/// </para>
	/// <para>Implementors must override the nine abstract service-provider interface
	/// (SPI) methods: <seealso cref="#getSpi(String)"/>, <seealso cref="#putSpi(String,String)"/>,
	/// <seealso cref="#removeSpi(String)"/>, <seealso cref="#childSpi(String)"/>, {@link
	/// #removeNodeSpi()}, <seealso cref="#keysSpi()"/>, <seealso cref="#childrenNamesSpi()"/>, {@link
	/// #syncSpi()} and <seealso cref="#flushSpi()"/>.  All of the concrete methods specify
	/// precisely how they are implemented atop these SPI methods.  The implementor
	/// may, at his discretion, override one or more of the concrete methods if the
	/// default implementation is unsatisfactory for any reason, such as
	/// performance.
	/// 
	/// </para>
	/// <para>The SPI methods fall into three groups concerning exception
	/// behavior. The <tt>getSpi</tt> method should never throw exceptions, but it
	/// doesn't really matter, as any exception thrown by this method will be
	/// intercepted by <seealso cref="#get(String,String)"/>, which will return the specified
	/// default value to the caller.  The <tt>removeNodeSpi, keysSpi,
	/// childrenNamesSpi, syncSpi</tt> and <tt>flushSpi</tt> methods are specified
	/// to throw <seealso cref="BackingStoreException"/>, and the implementation is required
	/// to throw this checked exception if it is unable to perform the operation.
	/// The exception propagates outward, causing the corresponding API method
	/// to fail.
	/// 
	/// </para>
	/// <para>The remaining SPI methods <seealso cref="#putSpi(String,String)"/>, {@link
	/// #removeSpi(String)} and <seealso cref="#childSpi(String)"/> have more complicated
	/// exception behavior.  They are not specified to throw
	/// <tt>BackingStoreException</tt>, as they can generally obey their contracts
	/// even if the backing store is unavailable.  This is true because they return
	/// no information and their effects are not required to become permanent until
	/// a subsequent call to <seealso cref="Preferences#flush()"/> or
	/// <seealso cref="Preferences#sync()"/>. Generally speaking, these SPI methods should not
	/// throw exceptions.  In some implementations, there may be circumstances
	/// under which these calls cannot even enqueue the requested operation for
	/// later processing.  Even under these circumstances it is generally better to
	/// simply ignore the invocation and return, rather than throwing an
	/// exception.  Under these circumstances, however, all subsequent invocations
	/// of <tt>flush()</tt> and <tt>sync</tt> should return <tt>false</tt>, as
	/// returning <tt>true</tt> would imply that all previous operations had
	/// successfully been made permanent.
	/// 
	/// </para>
	/// <para>There is one circumstance under which <tt>putSpi, removeSpi and
	/// childSpi</tt> <i>should</i> throw an exception: if the caller lacks
	/// sufficient privileges on the underlying operating system to perform the
	/// requested operation.  This will, for instance, occur on most systems
	/// if a non-privileged user attempts to modify system preferences.
	/// (The required privileges will vary from implementation to
	/// implementation.  On some implementations, they are the right to modify the
	/// contents of some directory in the file system; on others they are the right
	/// to modify contents of some key in a registry.)  Under any of these
	/// circumstances, it would generally be undesirable to let the program
	/// continue executing as if these operations would become permanent at a later
	/// time.  While implementations are not required to throw an exception under
	/// these circumstances, they are encouraged to do so.  A {@link
	/// SecurityException} would be appropriate.
	/// 
	/// </para>
	/// <para>Most of the SPI methods require the implementation to read or write
	/// information at a preferences node.  The implementor should beware of the
	/// fact that another VM may have concurrently deleted this node from the
	/// backing store.  It is the implementation's responsibility to recreate the
	/// node if it has been deleted.
	/// 
	/// </para>
	/// <para>Implementation note: In Sun's default <tt>Preferences</tt>
	/// implementations, the user's identity is inherited from the underlying
	/// operating system and does not change for the lifetime of the virtual
	/// machine.  It is recognized that server-side <tt>Preferences</tt>
	/// implementations may have the user identity change from request to request,
	/// implicitly passed to <tt>Preferences</tt> methods via the use of a
	/// static <seealso cref="ThreadLocal"/> instance.  Authors of such implementations are
	/// <i>strongly</i> encouraged to determine the user at the time preferences
	/// are accessed (for example by the <seealso cref="#get(String,String)"/> or {@link
	/// #put(String,String)} method) rather than permanently associating a user
	/// with each <tt>Preferences</tt> instance.  The latter behavior conflicts
	/// with normal <tt>Preferences</tt> usage and would lead to great confusion.
	/// 
	/// @author  Josh Bloch
	/// </para>
	/// </summary>
	/// <seealso cref=     Preferences
	/// @since   1.4 </seealso>
	public abstract class AbstractPreferences : Preferences
	{
		/// <summary>
		/// Our name relative to parent.
		/// </summary>
		private readonly String Name_Renamed;

		/// <summary>
		/// Our absolute path name.
		/// </summary>
		private readonly String AbsolutePath_Renamed;

		/// <summary>
		/// Our parent node.
		/// </summary>
		internal readonly AbstractPreferences Parent_Renamed;

		/// <summary>
		/// Our root node.
		/// </summary>
		private readonly AbstractPreferences Root; // Relative to this node

		/// <summary>
		/// This field should be <tt>true</tt> if this node did not exist in the
		/// backing store prior to the creation of this object.  The field
		/// is initialized to false, but may be set to true by a subclass
		/// constructor (and should not be modified thereafter).  This field
		/// indicates whether a node change event should be fired when
		/// creation is complete.
		/// </summary>
		protected internal bool NewNode = false;

		/// <summary>
		/// All known unremoved children of this node.  (This "cache" is consulted
		/// prior to calling childSpi() or getChild().
		/// </summary>
		private Map<String, AbstractPreferences> KidCache = new HashMap<String, AbstractPreferences>();

		/// <summary>
		/// This field is used to keep track of whether or not this node has
		/// been removed.  Once it's set to true, it will never be reset to false.
		/// </summary>
		private bool Removed_Renamed = false;

		/// <summary>
		/// Registered preference change listeners.
		/// </summary>
		private PreferenceChangeListener[] PrefListeners_Renamed = new PreferenceChangeListener[0];

		/// <summary>
		/// Registered node change listeners.
		/// </summary>
		private NodeChangeListener[] NodeListeners_Renamed = new NodeChangeListener[0];

		/// <summary>
		/// An object whose monitor is used to lock this node.  This object
		/// is used in preference to the node itself to reduce the likelihood of
		/// intentional or unintentional denial of service due to a locked node.
		/// To avoid deadlock, a node is <i>never</i> locked by a thread that
		/// holds a lock on a descendant of that node.
		/// </summary>
		protected internal readonly Object @lock = new Object();

		/// <summary>
		/// Creates a preference node with the specified parent and the specified
		/// name relative to its parent.
		/// </summary>
		/// <param name="parent"> the parent of this preference node, or null if this
		///               is the root. </param>
		/// <param name="name"> the name of this preference node, relative to its parent,
		///             or <tt>""</tt> if this is the root. </param>
		/// <exception cref="IllegalArgumentException"> if <tt>name</tt> contains a slash
		///          (<tt>'/'</tt>),  or <tt>parent</tt> is <tt>null</tt> and
		///          name isn't <tt>""</tt>. </exception>
		protected internal AbstractPreferences(AbstractPreferences parent, String name)
		{
			if (parent == null)
			{
				if (!name.Equals(""))
				{
					throw new IllegalArgumentException("Root name '" + name + "' must be \"\"");
				}
				this.AbsolutePath_Renamed = "/";
				Root = this;
			}
			else
			{
				if (name.IndexOf('/') != -1)
				{
					throw new IllegalArgumentException("Name '" + name + "' contains '/'");
				}
				if (name.Equals(""))
				{
				  throw new IllegalArgumentException("Illegal name: empty string");
				}

				Root = parent.Root;
				AbsolutePath_Renamed = (parent == Root ? "/" + name : parent.AbsolutePath() + "/" + name);
			}
			this.Name_Renamed = name;
			this.Parent_Renamed = parent;
		}

		/// <summary>
		/// Implements the <tt>put</tt> method as per the specification in
		/// <seealso cref="Preferences#put(String,String)"/>.
		/// 
		/// <para>This implementation checks that the key and value are legal,
		/// obtains this preference node's lock, checks that the node
		/// has not been removed, invokes <seealso cref="#putSpi(String,String)"/>, and if
		/// there are any preference change listeners, enqueues a notification
		/// event for processing by the event dispatch thread.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key with which the specified value is to be associated. </param>
		/// <param name="value"> value to be associated with the specified key. </param>
		/// <exception cref="NullPointerException"> if key or value is <tt>null</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		///       <tt>MAX_KEY_LENGTH</tt> or if <tt>value.length</tt> exceeds
		///       <tt>MAX_VALUE_LENGTH</tt>. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		public override void Put(String key, String value)
		{
			if (key == null || value == null)
			{
				throw new NullPointerException();
			}
			if (key.Length() > MAX_KEY_LENGTH)
			{
				throw new IllegalArgumentException("Key too long: " + key);
			}
			if (value.Length() > MAX_VALUE_LENGTH)
			{
				throw new IllegalArgumentException("Value too long: " + value);
			}

			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}

				PutSpi(key, value);
				EnqueuePreferenceChangeEvent(key, value);
			}
		}

		/// <summary>
		/// Implements the <tt>get</tt> method as per the specification in
		/// <seealso cref="Preferences#get(String,String)"/>.
		/// 
		/// <para>This implementation first checks to see if <tt>key</tt> is
		/// <tt>null</tt> throwing a <tt>NullPointerException</tt> if this is
		/// the case.  Then it obtains this preference node's lock,
		/// checks that the node has not been removed, invokes {@link
		/// #getSpi(String)}, and returns the result, unless the <tt>getSpi</tt>
		/// invocation returns <tt>null</tt> or throws an exception, in which case
		/// this invocation returns <tt>def</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key whose associated value is to be returned. </param>
		/// <param name="def"> the value to be returned in the event that this
		///        preference node has no value associated with <tt>key</tt>. </param>
		/// <returns> the value associated with <tt>key</tt>, or <tt>def</tt>
		///         if no value is associated with <tt>key</tt>. </returns>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <exception cref="NullPointerException"> if key is <tt>null</tt>.  (A
		///         <tt>null</tt> default <i>is</i> permitted.) </exception>
		public override String Get(String key, String def)
		{
			if (key == null)
			{
				throw new NullPointerException("Null key");
			}
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}

				String result = null;
				try
				{
					result = GetSpi(key);
				}
				catch (Exception)
				{
					// Ignoring exception causes default to be returned
				}
				return (result == null ? def : result);
			}
		}

		/// <summary>
		/// Implements the <tt>remove(String)</tt> method as per the specification
		/// in <seealso cref="Preferences#remove(String)"/>.
		/// 
		/// <para>This implementation obtains this preference node's lock,
		/// checks that the node has not been removed, invokes
		/// <seealso cref="#removeSpi(String)"/> and if there are any preference
		/// change listeners, enqueues a notification event for processing by the
		/// event dispatch thread.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key whose mapping is to be removed from the preference node. </param>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <exception cref="NullPointerException"> {@inheritDoc}. </exception>
		public override void Remove(String key)
		{
			Objects.RequireNonNull(key, "Specified key cannot be null");
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}

				RemoveSpi(key);
				EnqueuePreferenceChangeEvent(key, null);
			}
		}

		/// <summary>
		/// Implements the <tt>clear</tt> method as per the specification in
		/// <seealso cref="Preferences#clear()"/>.
		/// 
		/// <para>This implementation obtains this preference node's lock,
		/// invokes <seealso cref="#keys()"/> to obtain an array of keys, and
		/// iterates over the array invoking <seealso cref="#remove(String)"/> on each key.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void clear() throws BackingStoreException
		public override void Clear()
		{
			lock (@lock)
			{
				String[] keys = Keys();
				for (int i = 0; i < keys.Length; i++)
				{
					Remove(keys[i]);
				}
			}
		}

		/// <summary>
		/// Implements the <tt>putInt</tt> method as per the specification in
		/// <seealso cref="Preferences#putInt(String,int)"/>.
		/// 
		/// <para>This implementation translates <tt>value</tt> to a string with
		/// <seealso cref="Integer#toString(int)"/> and invokes <seealso cref="#put(String,String)"/>
		/// on the result.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key with which the string form of value is to be associated. </param>
		/// <param name="value"> value whose string form is to be associated with key. </param>
		/// <exception cref="NullPointerException"> if key is <tt>null</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		///         <tt>MAX_KEY_LENGTH</tt>. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		public override void PutInt(String key, int value)
		{
			Put(key, Convert.ToString(value));
		}

		/// <summary>
		/// Implements the <tt>getInt</tt> method as per the specification in
		/// <seealso cref="Preferences#getInt(String,int)"/>.
		/// 
		/// <para>This implementation invokes {@link #get(String,String) <tt>get(key,
		/// null)</tt>}.  If the return value is non-null, the implementation
		/// attempts to translate it to an <tt>int</tt> with
		/// <seealso cref="Integer#parseInt(String)"/>.  If the attempt succeeds, the return
		/// value is returned by this method.  Otherwise, <tt>def</tt> is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key whose associated value is to be returned as an int. </param>
		/// <param name="def"> the value to be returned in the event that this
		///        preference node has no value associated with <tt>key</tt>
		///        or the associated value cannot be interpreted as an int. </param>
		/// <returns> the int value represented by the string associated with
		///         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		///         associated value does not exist or cannot be interpreted as
		///         an int. </returns>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		public override int GetInt(String key, int def)
		{
			int result = def;
			try
			{
				String value = Get(key, null);
				if (value != null)
				{
					result = Convert.ToInt32(value);
				}
			}
			catch (NumberFormatException)
			{
				// Ignoring exception causes specified default to be returned
			}

			return result;
		}

		/// <summary>
		/// Implements the <tt>putLong</tt> method as per the specification in
		/// <seealso cref="Preferences#putLong(String,long)"/>.
		/// 
		/// <para>This implementation translates <tt>value</tt> to a string with
		/// <seealso cref="Long#toString(long)"/> and invokes <seealso cref="#put(String,String)"/>
		/// on the result.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key with which the string form of value is to be associated. </param>
		/// <param name="value"> value whose string form is to be associated with key. </param>
		/// <exception cref="NullPointerException"> if key is <tt>null</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		///         <tt>MAX_KEY_LENGTH</tt>. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		public override void PutLong(String key, long value)
		{
			Put(key, Convert.ToString(value));
		}

		/// <summary>
		/// Implements the <tt>getLong</tt> method as per the specification in
		/// <seealso cref="Preferences#getLong(String,long)"/>.
		/// 
		/// <para>This implementation invokes {@link #get(String,String) <tt>get(key,
		/// null)</tt>}.  If the return value is non-null, the implementation
		/// attempts to translate it to a <tt>long</tt> with
		/// <seealso cref="Long#parseLong(String)"/>.  If the attempt succeeds, the return
		/// value is returned by this method.  Otherwise, <tt>def</tt> is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key whose associated value is to be returned as a long. </param>
		/// <param name="def"> the value to be returned in the event that this
		///        preference node has no value associated with <tt>key</tt>
		///        or the associated value cannot be interpreted as a long. </param>
		/// <returns> the long value represented by the string associated with
		///         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		///         associated value does not exist or cannot be interpreted as
		///         a long. </returns>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		public override long GetLong(String key, long def)
		{
			long result = def;
			try
			{
				String value = Get(key, null);
				if (value != null)
				{
					result = Convert.ToInt64(value);
				}
			}
			catch (NumberFormatException)
			{
				// Ignoring exception causes specified default to be returned
			}

			return result;
		}

		/// <summary>
		/// Implements the <tt>putBoolean</tt> method as per the specification in
		/// <seealso cref="Preferences#putBoolean(String,boolean)"/>.
		/// 
		/// <para>This implementation translates <tt>value</tt> to a string with
		/// <seealso cref="String#valueOf(boolean)"/> and invokes <seealso cref="#put(String,String)"/>
		/// on the result.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key with which the string form of value is to be associated. </param>
		/// <param name="value"> value whose string form is to be associated with key. </param>
		/// <exception cref="NullPointerException"> if key is <tt>null</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		///         <tt>MAX_KEY_LENGTH</tt>. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		public override void PutBoolean(String key, bool value)
		{
			Put(key, Convert.ToString(value));
		}

		/// <summary>
		/// Implements the <tt>getBoolean</tt> method as per the specification in
		/// <seealso cref="Preferences#getBoolean(String,boolean)"/>.
		/// 
		/// <para>This implementation invokes {@link #get(String,String) <tt>get(key,
		/// null)</tt>}.  If the return value is non-null, it is compared with
		/// <tt>"true"</tt> using <seealso cref="String#equalsIgnoreCase(String)"/>.  If the
		/// comparison returns <tt>true</tt>, this invocation returns
		/// <tt>true</tt>.  Otherwise, the original return value is compared with
		/// <tt>"false"</tt>, again using <seealso cref="String#equalsIgnoreCase(String)"/>.
		/// If the comparison returns <tt>true</tt>, this invocation returns
		/// <tt>false</tt>.  Otherwise, this invocation returns <tt>def</tt>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key whose associated value is to be returned as a boolean. </param>
		/// <param name="def"> the value to be returned in the event that this
		///        preference node has no value associated with <tt>key</tt>
		///        or the associated value cannot be interpreted as a boolean. </param>
		/// <returns> the boolean value represented by the string associated with
		///         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		///         associated value does not exist or cannot be interpreted as
		///         a boolean. </returns>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		public override bool GetBoolean(String key, bool def)
		{
			bool result = def;
			String value = Get(key, null);
			if (value != null)
			{
				if (value.EqualsIgnoreCase("true"))
				{
					result = true;
				}
				else if (value.EqualsIgnoreCase("false"))
				{
					result = false;
				}
			}

			return result;
		}

		/// <summary>
		/// Implements the <tt>putFloat</tt> method as per the specification in
		/// <seealso cref="Preferences#putFloat(String,float)"/>.
		/// 
		/// <para>This implementation translates <tt>value</tt> to a string with
		/// <seealso cref="Float#toString(float)"/> and invokes <seealso cref="#put(String,String)"/>
		/// on the result.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key with which the string form of value is to be associated. </param>
		/// <param name="value"> value whose string form is to be associated with key. </param>
		/// <exception cref="NullPointerException"> if key is <tt>null</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		///         <tt>MAX_KEY_LENGTH</tt>. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		public override void PutFloat(String key, float value)
		{
			Put(key, Convert.ToString(value));
		}

		/// <summary>
		/// Implements the <tt>getFloat</tt> method as per the specification in
		/// <seealso cref="Preferences#getFloat(String,float)"/>.
		/// 
		/// <para>This implementation invokes {@link #get(String,String) <tt>get(key,
		/// null)</tt>}.  If the return value is non-null, the implementation
		/// attempts to translate it to an <tt>float</tt> with
		/// <seealso cref="Float#parseFloat(String)"/>.  If the attempt succeeds, the return
		/// value is returned by this method.  Otherwise, <tt>def</tt> is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key whose associated value is to be returned as a float. </param>
		/// <param name="def"> the value to be returned in the event that this
		///        preference node has no value associated with <tt>key</tt>
		///        or the associated value cannot be interpreted as a float. </param>
		/// <returns> the float value represented by the string associated with
		///         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		///         associated value does not exist or cannot be interpreted as
		///         a float. </returns>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		public override float GetFloat(String key, float def)
		{
			float result = def;
			try
			{
				String value = Get(key, null);
				if (value != null)
				{
					result = Convert.ToSingle(value);
				}
			}
			catch (NumberFormatException)
			{
				// Ignoring exception causes specified default to be returned
			}

			return result;
		}

		/// <summary>
		/// Implements the <tt>putDouble</tt> method as per the specification in
		/// <seealso cref="Preferences#putDouble(String,double)"/>.
		/// 
		/// <para>This implementation translates <tt>value</tt> to a string with
		/// <seealso cref="Double#toString(double)"/> and invokes <seealso cref="#put(String,String)"/>
		/// on the result.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key with which the string form of value is to be associated. </param>
		/// <param name="value"> value whose string form is to be associated with key. </param>
		/// <exception cref="NullPointerException"> if key is <tt>null</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if <tt>key.length()</tt> exceeds
		///         <tt>MAX_KEY_LENGTH</tt>. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		public override void PutDouble(String key, double value)
		{
			Put(key, Convert.ToString(value));
		}

		/// <summary>
		/// Implements the <tt>getDouble</tt> method as per the specification in
		/// <seealso cref="Preferences#getDouble(String,double)"/>.
		/// 
		/// <para>This implementation invokes {@link #get(String,String) <tt>get(key,
		/// null)</tt>}.  If the return value is non-null, the implementation
		/// attempts to translate it to an <tt>double</tt> with
		/// <seealso cref="Double#parseDouble(String)"/>.  If the attempt succeeds, the return
		/// value is returned by this method.  Otherwise, <tt>def</tt> is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> key whose associated value is to be returned as a double. </param>
		/// <param name="def"> the value to be returned in the event that this
		///        preference node has no value associated with <tt>key</tt>
		///        or the associated value cannot be interpreted as a double. </param>
		/// <returns> the double value represented by the string associated with
		///         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		///         associated value does not exist or cannot be interpreted as
		///         a double. </returns>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>. </exception>
		public override double GetDouble(String key, double def)
		{
			double result = def;
			try
			{
				String value = Get(key, null);
				if (value != null)
				{
					result = Convert.ToDouble(value);
				}
			}
			catch (NumberFormatException)
			{
				// Ignoring exception causes specified default to be returned
			}

			return result;
		}

		/// <summary>
		/// Implements the <tt>putByteArray</tt> method as per the specification in
		/// <seealso cref="Preferences#putByteArray(String,byte[])"/>.
		/// </summary>
		/// <param name="key"> key with which the string form of value is to be associated. </param>
		/// <param name="value"> value whose string form is to be associated with key. </param>
		/// <exception cref="NullPointerException"> if key or value is <tt>null</tt>. </exception>
		/// <exception cref="IllegalArgumentException"> if key.length() exceeds MAX_KEY_LENGTH
		///         or if value.length exceeds MAX_VALUE_LENGTH*3/4. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		public override void PutByteArray(String key, sbyte[] value)
		{
			Put(key, Base64.ByteArrayToBase64(value));
		}

		/// <summary>
		/// Implements the <tt>getByteArray</tt> method as per the specification in
		/// <seealso cref="Preferences#getByteArray(String,byte[])"/>.
		/// </summary>
		/// <param name="key"> key whose associated value is to be returned as a byte array. </param>
		/// <param name="def"> the value to be returned in the event that this
		///        preference node has no value associated with <tt>key</tt>
		///        or the associated value cannot be interpreted as a byte array. </param>
		/// <returns> the byte array value represented by the string associated with
		///         <tt>key</tt> in this preference node, or <tt>def</tt> if the
		///         associated value does not exist or cannot be interpreted as
		///         a byte array. </returns>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <exception cref="NullPointerException"> if <tt>key</tt> is <tt>null</tt>.  (A
		///         <tt>null</tt> value for <tt>def</tt> <i>is</i> permitted.) </exception>
		public override sbyte[] GetByteArray(String key, sbyte[] def)
		{
			sbyte[] result = def;
			String value = Get(key, null);
			try
			{
				if (value != null)
				{
					result = Base64.Base64ToByteArray(value);
				}
			}
			catch (RuntimeException)
			{
				// Ignoring exception causes specified default to be returned
			}

			return result;
		}

		/// <summary>
		/// Implements the <tt>keys</tt> method as per the specification in
		/// <seealso cref="Preferences#keys()"/>.
		/// 
		/// <para>This implementation obtains this preference node's lock, checks that
		/// the node has not been removed and invokes <seealso cref="#keysSpi()"/>.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of the keys that have an associated value in this
		///         preference node. </returns>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String[] keys() throws BackingStoreException
		public override String[] Keys()
		{
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}

				return KeysSpi();
			}
		}

		/// <summary>
		/// Implements the <tt>children</tt> method as per the specification in
		/// <seealso cref="Preferences#childrenNames()"/>.
		/// 
		/// <para>This implementation obtains this preference node's lock, checks that
		/// the node has not been removed, constructs a <tt>TreeSet</tt> initialized
		/// to the names of children already cached (the children in this node's
		/// "child-cache"), invokes <seealso cref="#childrenNamesSpi()"/>, and adds all of the
		/// returned child-names into the set.  The elements of the tree set are
		/// dumped into a <tt>String</tt> array using the <tt>toArray</tt> method,
		/// and this array is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the names of the children of this preference node. </returns>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <seealso cref= #cachedChildren() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String[] childrenNames() throws BackingStoreException
		public override String[] ChildrenNames()
		{
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}

				IDictionary<String, AbstractPreferences>.KeyCollection s = new TreeSet<String>(KidCache.KeySet());
				foreach (String kid in ChildrenNamesSpi())
				{
					s.add(kid);
				}
				return s.toArray(EMPTY_STRING_ARRAY);
			}
		}

		private static readonly String[] EMPTY_STRING_ARRAY = new String[0];

		/// <summary>
		/// Returns all known unremoved children of this node.
		/// </summary>
		/// <returns> all known unremoved children of this node. </returns>
		protected internal AbstractPreferences[] CachedChildren()
		{
			return KidCache.Values().ToArray(EMPTY_ABSTRACT_PREFS_ARRAY);
		}

		private static readonly AbstractPreferences[] EMPTY_ABSTRACT_PREFS_ARRAY = new AbstractPreferences[0];

		/// <summary>
		/// Implements the <tt>parent</tt> method as per the specification in
		/// <seealso cref="Preferences#parent()"/>.
		/// 
		/// <para>This implementation obtains this preference node's lock, checks that
		/// the node has not been removed and returns the parent value that was
		/// passed to this node's constructor.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the parent of this preference node. </returns>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		public override Preferences Parent()
		{
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}

				return Parent_Renamed;
			}
		}

		/// <summary>
		/// Implements the <tt>node</tt> method as per the specification in
		/// <seealso cref="Preferences#node(String)"/>.
		/// 
		/// <para>This implementation obtains this preference node's lock and checks
		/// that the node has not been removed.  If <tt>path</tt> is <tt>""</tt>,
		/// this node is returned; if <tt>path</tt> is <tt>"/"</tt>, this node's
		/// root is returned.  If the first character in <tt>path</tt> is
		/// not <tt>'/'</tt>, the implementation breaks <tt>path</tt> into
		/// tokens and recursively traverses the path from this node to the
		/// named node, "consuming" a name and a slash from <tt>path</tt> at
		/// each step of the traversal.  At each step, the current node is locked
		/// and the node's child-cache is checked for the named node.  If it is
		/// not found, the name is checked to make sure its length does not
		/// exceed <tt>MAX_NAME_LENGTH</tt>.  Then the <seealso cref="#childSpi(String)"/>
		/// method is invoked, and the result stored in this node's child-cache.
		/// If the newly created <tt>Preferences</tt> object's <seealso cref="#newNode"/>
		/// field is <tt>true</tt> and there are any node change listeners,
		/// a notification event is enqueued for processing by the event dispatch
		/// thread.
		/// 
		/// </para>
		/// <para>When there are no more tokens, the last value found in the
		/// child-cache or returned by <tt>childSpi</tt> is returned by this
		/// method.  If during the traversal, two <tt>"/"</tt> tokens occur
		/// consecutively, or the final token is <tt>"/"</tt> (rather than a name),
		/// an appropriate <tt>IllegalArgumentException</tt> is thrown.
		/// 
		/// </para>
		/// <para> If the first character of <tt>path</tt> is <tt>'/'</tt>
		/// (indicating an absolute path name) this preference node's
		/// lock is dropped prior to breaking <tt>path</tt> into tokens, and
		/// this method recursively traverses the path starting from the root
		/// (rather than starting from this node).  The traversal is otherwise
		/// identical to the one described for relative path names.  Dropping
		/// the lock on this node prior to commencing the traversal at the root
		/// node is essential to avoid the possibility of deadlock, as per the
		/// <seealso cref="#lock locking invariant"/>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path"> the path name of the preference node to return. </param>
		/// <returns> the specified preference node. </returns>
		/// <exception cref="IllegalArgumentException"> if the path name is invalid (i.e.,
		///         it contains multiple consecutive slash characters, or ends
		///         with a slash character and is more than one character long). </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		public override Preferences Node(String path)
		{
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}
				if (path.Equals(""))
				{
					return this;
				}
				if (path.Equals("/"))
				{
					return Root;
				}
				if (path.CharAt(0) != '/')
				{
					return Node(new StringTokenizer(path, "/", true));
				}
			}

			// Absolute path.  Note that we've dropped our lock to avoid deadlock
			return Root.Node(new StringTokenizer(path.Substring(1), "/", true));
		}

		/// <summary>
		/// tokenizer contains <name> {'/' <name>}*
		/// </summary>
		private Preferences Node(StringTokenizer path)
		{
			String token = path.NextToken();
			if (token.Equals("/")) // Check for consecutive slashes
			{
				throw new IllegalArgumentException("Consecutive slashes in path");
			}
			lock (@lock)
			{
				AbstractPreferences child = KidCache.Get(token);
				if (child == null)
				{
					if (token.Length() > MAX_NAME_LENGTH)
					{
						throw new IllegalArgumentException("Node name " + token + " too long");
					}
					child = ChildSpi(token);
					if (child.NewNode)
					{
						EnqueueNodeAddedEvent(child);
					}
					KidCache.Put(token, child);
				}
				if (!path.HasMoreTokens())
				{
					return child;
				}
				path.NextToken(); // Consume slash
				if (!path.HasMoreTokens())
				{
					throw new IllegalArgumentException("Path ends with slash");
				}
				return child.Node(path);
			}
		}

		/// <summary>
		/// Implements the <tt>nodeExists</tt> method as per the specification in
		/// <seealso cref="Preferences#nodeExists(String)"/>.
		/// 
		/// <para>This implementation is very similar to <seealso cref="#node(String)"/>,
		/// except that <seealso cref="#getChild(String)"/> is used instead of {@link
		/// #childSpi(String)}.
		/// 
		/// </para>
		/// </summary>
		/// <param name="path"> the path name of the node whose existence is to be checked. </param>
		/// <returns> true if the specified node exists. </returns>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
		/// <exception cref="IllegalArgumentException"> if the path name is invalid (i.e.,
		///         it contains multiple consecutive slash characters, or ends
		///         with a slash character and is more than one character long). </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method and
		///         <tt>pathname</tt> is not the empty string (<tt>""</tt>). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean nodeExists(String path) throws BackingStoreException
		public override bool NodeExists(String path)
		{
			lock (@lock)
			{
				if (path.Equals(""))
				{
					return !Removed_Renamed;
				}
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}
				if (path.Equals("/"))
				{
					return true;
				}
				if (path.CharAt(0) != '/')
				{
					return NodeExists(new StringTokenizer(path, "/", true));
				}
			}

			// Absolute path.  Note that we've dropped our lock to avoid deadlock
			return Root.NodeExists(new StringTokenizer(path.Substring(1), "/", true));
		}

		/// <summary>
		/// tokenizer contains <name> {'/' <name>}*
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean nodeExists(StringTokenizer path) throws BackingStoreException
		private bool NodeExists(StringTokenizer path)
		{
			String token = path.NextToken();
			if (token.Equals("/")) // Check for consecutive slashes
			{
				throw new IllegalArgumentException("Consecutive slashes in path");
			}
			lock (@lock)
			{
				AbstractPreferences child = KidCache.Get(token);
				if (child == null)
				{
					child = GetChild(token);
				}
				if (child == null)
				{
					return false;
				}
				if (!path.HasMoreTokens())
				{
					return true;
				}
				path.NextToken(); // Consume slash
				if (!path.HasMoreTokens())
				{
					throw new IllegalArgumentException("Path ends with slash");
				}
				return child.NodeExists(path);
			}
		}

		/// 
		/// <summary>
		/// Implements the <tt>removeNode()</tt> method as per the specification in
		/// <seealso cref="Preferences#removeNode()"/>.
		/// 
		/// <para>This implementation checks to see that this node is the root; if so,
		/// it throws an appropriate exception.  Then, it locks this node's parent,
		/// and calls a recursive helper method that traverses the subtree rooted at
		/// this node.  The recursive method locks the node on which it was called,
		/// checks that it has not already been removed, and then ensures that all
		/// of its children are cached: The <seealso cref="#childrenNamesSpi()"/> method is
		/// invoked and each returned child name is checked for containment in the
		/// child-cache.  If a child is not already cached, the {@link
		/// #childSpi(String)} method is invoked to create a <tt>Preferences</tt>
		/// instance for it, and this instance is put into the child-cache.  Then
		/// the helper method calls itself recursively on each node contained in its
		/// child-cache.  Next, it invokes <seealso cref="#removeNodeSpi()"/>, marks itself
		/// as removed, and removes itself from its parent's child-cache.  Finally,
		/// if there are any node change listeners, it enqueues a notification
		/// event for processing by the event dispatch thread.
		/// 
		/// </para>
		/// <para>Note that the helper method is always invoked with all ancestors up
		/// to the "closest non-removed ancestor" locked.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has already
		///         been removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <exception cref="UnsupportedOperationException"> if this method is invoked on
		///         the root node. </exception>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeNode() throws BackingStoreException
		public override void RemoveNode()
		{
			if (this == Root)
			{
				throw new UnsupportedOperationException("Can't remove the root!");
			}
			lock (Parent_Renamed.@lock)
			{
				RemoveNode2();
				Parent_Renamed.KidCache.Remove(Name_Renamed);
			}
		}

		/*
		 * Called with locks on all nodes on path from parent of "removal root"
		 * to this (including the former but excluding the latter).
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void removeNode2() throws BackingStoreException
		private void RemoveNode2()
		{
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node already removed.");
				}

				// Ensure that all children are cached
				String[] kidNames = ChildrenNamesSpi();
				for (int i = 0; i < kidNames.Length; i++)
				{
					if (!KidCache.ContainsKey(kidNames[i]))
					{
						KidCache.Put(kidNames[i], ChildSpi(kidNames[i]));
					}
				}

				// Recursively remove all cached children
				for (Iterator<AbstractPreferences> i = KidCache.Values().Iterator(); i.HasNext();)
				{
					try
					{
						i.Next().RemoveNode2();
						i.remove();
					}
					catch (BackingStoreException)
					{
					}
				}

				// Now we have no descendants - it's time to die!
				RemoveNodeSpi();
				Removed_Renamed = true;
				Parent_Renamed.EnqueueNodeRemovedEvent(this);
			}
		}

		/// <summary>
		/// Implements the <tt>name</tt> method as per the specification in
		/// <seealso cref="Preferences#name()"/>.
		/// 
		/// <para>This implementation merely returns the name that was
		/// passed to this node's constructor.
		/// 
		/// </para>
		/// </summary>
		/// <returns> this preference node's name, relative to its parent. </returns>
		public override String Name()
		{
			return Name_Renamed;
		}

		/// <summary>
		/// Implements the <tt>absolutePath</tt> method as per the specification in
		/// <seealso cref="Preferences#absolutePath()"/>.
		/// 
		/// <para>This implementation merely returns the absolute path name that
		/// was computed at the time that this node was constructed (based on
		/// the name that was passed to this node's constructor, and the names
		/// that were passed to this node's ancestors' constructors).
		/// 
		/// </para>
		/// </summary>
		/// <returns> this preference node's absolute path name. </returns>
		public override String AbsolutePath()
		{
			return AbsolutePath_Renamed;
		}

		/// <summary>
		/// Implements the <tt>isUserNode</tt> method as per the specification in
		/// <seealso cref="Preferences#isUserNode()"/>.
		/// 
		/// <para>This implementation compares this node's root node (which is stored
		/// in a private field) with the value returned by
		/// <seealso cref="Preferences#userRoot()"/>.  If the two object references are
		/// identical, this method returns true.
		/// 
		/// </para>
		/// </summary>
		/// <returns> <tt>true</tt> if this preference node is in the user
		///         preference tree, <tt>false</tt> if it's in the system
		///         preference tree. </returns>
		public override bool UserNode
		{
			get
			{
				return (bool)AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this));
			}
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Boolean>
		{
			private readonly AbstractPreferences OuterInstance;

			public PrivilegedActionAnonymousInnerClassHelper(AbstractPreferences outerInstance)
			{
				this.OuterInstance = outerInstance;
			}

			public virtual Boolean Run()
			{
				return OuterInstance.Root == Preferences.UserRoot();
			}
		}

		public override void AddPreferenceChangeListener(PreferenceChangeListener pcl)
		{
			if (pcl == null)
			{
				throw new NullPointerException("Change listener is null.");
			}
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}

				// Copy-on-write
				PreferenceChangeListener[] old = PrefListeners_Renamed;
				PrefListeners_Renamed = new PreferenceChangeListener[old.Length + 1];
				System.Array.Copy(old, 0, PrefListeners_Renamed, 0, old.Length);
				PrefListeners_Renamed[old.Length] = pcl;
			}
			StartEventDispatchThreadIfNecessary();
		}

		public override void RemovePreferenceChangeListener(PreferenceChangeListener pcl)
		{
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}
				if ((PrefListeners_Renamed == null) || (PrefListeners_Renamed.Length == 0))
				{
					throw new IllegalArgumentException("Listener not registered.");
				}

				// Copy-on-write
				PreferenceChangeListener[] newPl = new PreferenceChangeListener[PrefListeners_Renamed.Length - 1];
				int i = 0;
				while (i < newPl.Length && PrefListeners_Renamed[i] != pcl)
				{
					newPl[i] = PrefListeners_Renamed[i++];
				}

				if (i == newPl.Length && PrefListeners_Renamed[i] != pcl)
				{
					throw new IllegalArgumentException("Listener not registered.");
				}
				while (i < newPl.Length)
				{
					newPl[i] = PrefListeners_Renamed[++i];
				}
				PrefListeners_Renamed = newPl;
			}
		}

		public override void AddNodeChangeListener(NodeChangeListener ncl)
		{
			if (ncl == null)
			{
				throw new NullPointerException("Change listener is null.");
			}
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}

				// Copy-on-write
				if (NodeListeners_Renamed == null)
				{
					NodeListeners_Renamed = new NodeChangeListener[1];
					NodeListeners_Renamed[0] = ncl;
				}
				else
				{
					NodeChangeListener[] old = NodeListeners_Renamed;
					NodeListeners_Renamed = new NodeChangeListener[old.Length + 1];
					System.Array.Copy(old, 0, NodeListeners_Renamed, 0, old.Length);
					NodeListeners_Renamed[old.Length] = ncl;
				}
			}
			StartEventDispatchThreadIfNecessary();
		}

		public override void RemoveNodeChangeListener(NodeChangeListener ncl)
		{
			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed.");
				}
				if ((NodeListeners_Renamed == null) || (NodeListeners_Renamed.Length == 0))
				{
					throw new IllegalArgumentException("Listener not registered.");
				}

				// Copy-on-write
				int i = 0;
				while (i < NodeListeners_Renamed.Length && NodeListeners_Renamed[i] != ncl)
				{
					i++;
				}
				if (i == NodeListeners_Renamed.Length)
				{
					throw new IllegalArgumentException("Listener not registered.");
				}
				NodeChangeListener[] newNl = new NodeChangeListener[NodeListeners_Renamed.Length - 1];
				if (i != 0)
				{
					System.Array.Copy(NodeListeners_Renamed, 0, newNl, 0, i);
				}
				if (i != newNl.Length)
				{
					System.Array.Copy(NodeListeners_Renamed, i + 1, newNl, i, newNl.Length - i);
				}
				NodeListeners_Renamed = newNl;
			}
		}

		// "SPI" METHODS

		/// <summary>
		/// Put the given key-value association into this preference node.  It is
		/// guaranteed that <tt>key</tt> and <tt>value</tt> are non-null and of
		/// legal length.  Also, it is guaranteed that this node has not been
		/// removed.  (The implementor needn't check for any of these things.)
		/// 
		/// <para>This method is invoked with the lock on this node held.
		/// </para>
		/// </summary>
		/// <param name="key"> the key </param>
		/// <param name="value"> the value </param>
		protected internal abstract void PutSpi(String key, String value);

		/// <summary>
		/// Return the value associated with the specified key at this preference
		/// node, or <tt>null</tt> if there is no association for this key, or the
		/// association cannot be determined at this time.  It is guaranteed that
		/// <tt>key</tt> is non-null.  Also, it is guaranteed that this node has
		/// not been removed.  (The implementor needn't check for either of these
		/// things.)
		/// 
		/// <para> Generally speaking, this method should not throw an exception
		/// under any circumstances.  If, however, if it does throw an exception,
		/// the exception will be intercepted and treated as a <tt>null</tt>
		/// return value.
		/// 
		/// </para>
		/// <para>This method is invoked with the lock on this node held.
		/// 
		/// </para>
		/// </summary>
		/// <param name="key"> the key </param>
		/// <returns> the value associated with the specified key at this preference
		///          node, or <tt>null</tt> if there is no association for this
		///          key, or the association cannot be determined at this time. </returns>
		protected internal abstract String GetSpi(String key);

		/// <summary>
		/// Remove the association (if any) for the specified key at this
		/// preference node.  It is guaranteed that <tt>key</tt> is non-null.
		/// Also, it is guaranteed that this node has not been removed.
		/// (The implementor needn't check for either of these things.)
		/// 
		/// <para>This method is invoked with the lock on this node held.
		/// </para>
		/// </summary>
		/// <param name="key"> the key </param>
		protected internal abstract void RemoveSpi(String key);

		/// <summary>
		/// Removes this preference node, invalidating it and any preferences that
		/// it contains.  The named child will have no descendants at the time this
		/// invocation is made (i.e., the <seealso cref="Preferences#removeNode()"/> method
		/// invokes this method repeatedly in a bottom-up fashion, removing each of
		/// a node's descendants before removing the node itself).
		/// 
		/// <para>This method is invoked with the lock held on this node and its
		/// parent (and all ancestors that are being removed as a
		/// result of a single invocation to <seealso cref="Preferences#removeNode()"/>).
		/// 
		/// </para>
		/// <para>The removal of a node needn't become persistent until the
		/// <tt>flush</tt> method is invoked on this node (or an ancestor).
		/// 
		/// </para>
		/// <para>If this node throws a <tt>BackingStoreException</tt>, the exception
		/// will propagate out beyond the enclosing <seealso cref="#removeNode()"/>
		/// invocation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void removeNodeSpi() throws BackingStoreException;
		protected internal abstract void RemoveNodeSpi();

		/// <summary>
		/// Returns all of the keys that have an associated value in this
		/// preference node.  (The returned array will be of size zero if
		/// this node has no preferences.)  It is guaranteed that this node has not
		/// been removed.
		/// 
		/// <para>This method is invoked with the lock on this node held.
		/// 
		/// </para>
		/// <para>If this node throws a <tt>BackingStoreException</tt>, the exception
		/// will propagate out beyond the enclosing <seealso cref="#keys()"/> invocation.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array of the keys that have an associated value in this
		///         preference node. </returns>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract String[] keysSpi() throws BackingStoreException;
		protected internal abstract String[] KeysSpi();

		/// <summary>
		/// Returns the names of the children of this preference node.  (The
		/// returned array will be of size zero if this node has no children.)
		/// This method need not return the names of any nodes already cached,
		/// but may do so without harm.
		/// 
		/// <para>This method is invoked with the lock on this node held.
		/// 
		/// </para>
		/// <para>If this node throws a <tt>BackingStoreException</tt>, the exception
		/// will propagate out beyond the enclosing <seealso cref="#childrenNames()"/>
		/// invocation.
		/// 
		/// </para>
		/// </summary>
		/// <returns> an array containing the names of the children of this
		///         preference node. </returns>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract String[] childrenNamesSpi() throws BackingStoreException;
		protected internal abstract String[] ChildrenNamesSpi();

		/// <summary>
		/// Returns the named child if it exists, or <tt>null</tt> if it does not.
		/// It is guaranteed that <tt>nodeName</tt> is non-null, non-empty,
		/// does not contain the slash character ('/'), and is no longer than
		/// <seealso cref="#MAX_NAME_LENGTH"/> characters.  Also, it is guaranteed
		/// that this node has not been removed.  (The implementor needn't check
		/// for any of these things if he chooses to override this method.)
		/// 
		/// <para>Finally, it is guaranteed that the named node has not been returned
		/// by a previous invocation of this method or <seealso cref="#childSpi"/> after the
		/// last time that it was removed.  In other words, a cached value will
		/// always be used in preference to invoking this method.  (The implementor
		/// needn't maintain his own cache of previously returned children if he
		/// chooses to override this method.)
		/// 
		/// </para>
		/// <para>This implementation obtains this preference node's lock, invokes
		/// <seealso cref="#childrenNames()"/> to get an array of the names of this node's
		/// children, and iterates over the array comparing the name of each child
		/// with the specified node name.  If a child node has the correct name,
		/// the <seealso cref="#childSpi(String)"/> method is invoked and the resulting
		/// node is returned.  If the iteration completes without finding the
		/// specified name, <tt>null</tt> is returned.
		/// 
		/// </para>
		/// </summary>
		/// <param name="nodeName"> name of the child to be searched for. </param>
		/// <returns> the named child if it exists, or null if it does not. </returns>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected AbstractPreferences getChild(String nodeName) throws BackingStoreException
		protected internal virtual AbstractPreferences GetChild(String nodeName)
		{
			lock (@lock)
			{
				// assert kidCache.get(nodeName)==null;
				String[] kidNames = ChildrenNames();
				for (int i = 0; i < kidNames.Length; i++)
				{
					if (kidNames[i].Equals(nodeName))
					{
						return ChildSpi(kidNames[i]);
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the named child of this preference node, creating it if it does
		/// not already exist.  It is guaranteed that <tt>name</tt> is non-null,
		/// non-empty, does not contain the slash character ('/'), and is no longer
		/// than <seealso cref="#MAX_NAME_LENGTH"/> characters.  Also, it is guaranteed that
		/// this node has not been removed.  (The implementor needn't check for any
		/// of these things.)
		/// 
		/// <para>Finally, it is guaranteed that the named node has not been returned
		/// by a previous invocation of this method or <seealso cref="#getChild(String)"/>
		/// after the last time that it was removed.  In other words, a cached
		/// value will always be used in preference to invoking this method.
		/// Subclasses need not maintain their own cache of previously returned
		/// children.
		/// 
		/// </para>
		/// <para>The implementer must ensure that the returned node has not been
		/// removed.  If a like-named child of this node was previously removed, the
		/// implementer must return a newly constructed <tt>AbstractPreferences</tt>
		/// node; once removed, an <tt>AbstractPreferences</tt> node
		/// cannot be "resuscitated."
		/// 
		/// </para>
		/// <para>If this method causes a node to be created, this node is not
		/// guaranteed to be persistent until the <tt>flush</tt> method is
		/// invoked on this node or one of its ancestors (or descendants).
		/// 
		/// </para>
		/// <para>This method is invoked with the lock on this node held.
		/// 
		/// </para>
		/// </summary>
		/// <param name="name"> The name of the child node to return, relative to
		///        this preference node. </param>
		/// <returns> The named child node. </returns>
		protected internal abstract AbstractPreferences ChildSpi(String name);

		/// <summary>
		/// Returns the absolute path name of this preferences node.
		/// </summary>
		public override String ToString()
		{
			return (this.UserNode ? "User" : "System") + " Preference Node: " + this.AbsolutePath();
		}

		/// <summary>
		/// Implements the <tt>sync</tt> method as per the specification in
		/// <seealso cref="Preferences#sync()"/>.
		/// 
		/// <para>This implementation calls a recursive helper method that locks this
		/// node, invokes syncSpi() on it, unlocks this node, and recursively
		/// invokes this method on each "cached child."  A cached child is a child
		/// of this node that has been created in this VM and not subsequently
		/// removed.  In effect, this method does a depth first traversal of the
		/// "cached subtree" rooted at this node, calling syncSpi() on each node in
		/// the subTree while only that node is locked. Note that syncSpi() is
		/// invoked top-down.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
		/// <exception cref="IllegalStateException"> if this node (or an ancestor) has been
		///         removed with the <seealso cref="#removeNode()"/> method. </exception>
		/// <seealso cref= #flush() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void sync() throws BackingStoreException
		public override void Sync()
		{
			Sync2();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void sync2() throws BackingStoreException
		private void Sync2()
		{
			AbstractPreferences[] cachedKids;

			lock (@lock)
			{
				if (Removed_Renamed)
				{
					throw new IllegalStateException("Node has been removed");
				}
				SyncSpi();
				cachedKids = CachedChildren();
			}

			for (int i = 0; i < cachedKids.Length; i++)
			{
				cachedKids[i].Sync2();
			}
		}

		/// <summary>
		/// This method is invoked with this node locked.  The contract of this
		/// method is to synchronize any cached preferences stored at this node
		/// with any stored in the backing store.  (It is perfectly possible that
		/// this node does not exist on the backing store, either because it has
		/// been deleted by another VM, or because it has not yet been created.)
		/// Note that this method should <i>not</i> synchronize the preferences in
		/// any subnodes of this node.  If the backing store naturally syncs an
		/// entire subtree at once, the implementer is encouraged to override
		/// sync(), rather than merely overriding this method.
		/// 
		/// <para>If this node throws a <tt>BackingStoreException</tt>, the exception
		/// will propagate out beyond the enclosing <seealso cref="#sync()"/> invocation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void syncSpi() throws BackingStoreException;
		protected internal abstract void SyncSpi();

		/// <summary>
		/// Implements the <tt>flush</tt> method as per the specification in
		/// <seealso cref="Preferences#flush()"/>.
		/// 
		/// <para>This implementation calls a recursive helper method that locks this
		/// node, invokes flushSpi() on it, unlocks this node, and recursively
		/// invokes this method on each "cached child."  A cached child is a child
		/// of this node that has been created in this VM and not subsequently
		/// removed.  In effect, this method does a depth first traversal of the
		/// "cached subtree" rooted at this node, calling flushSpi() on each node in
		/// the subTree while only that node is locked. Note that flushSpi() is
		/// invoked top-down.
		/// 
		/// </para>
		/// <para> If this method is invoked on a node that has been removed with
		/// the <seealso cref="#removeNode()"/> method, flushSpi() is invoked on this node,
		/// but not on others.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
		/// <seealso cref= #flush() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws BackingStoreException
		public override void Flush()
		{
			Flush2();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void flush2() throws BackingStoreException
		private void Flush2()
		{
			AbstractPreferences[] cachedKids;

			lock (@lock)
			{
				FlushSpi();
				if (Removed_Renamed)
				{
					return;
				}
				cachedKids = CachedChildren();
			}

			for (int i = 0; i < cachedKids.Length; i++)
			{
				cachedKids[i].Flush2();
			}
		}

		/// <summary>
		/// This method is invoked with this node locked.  The contract of this
		/// method is to force any cached changes in the contents of this
		/// preference node to the backing store, guaranteeing their persistence.
		/// (It is perfectly possible that this node does not exist on the backing
		/// store, either because it has been deleted by another VM, or because it
		/// has not yet been created.)  Note that this method should <i>not</i>
		/// flush the preferences in any subnodes of this node.  If the backing
		/// store naturally flushes an entire subtree at once, the implementer is
		/// encouraged to override flush(), rather than merely overriding this
		/// method.
		/// 
		/// <para>If this node throws a <tt>BackingStoreException</tt>, the exception
		/// will propagate out beyond the enclosing <seealso cref="#flush()"/> invocation.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="BackingStoreException"> if this operation cannot be completed
		///         due to a failure in the backing store, or inability to
		///         communicate with it. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void flushSpi() throws BackingStoreException;
		protected internal abstract void FlushSpi();

		/// <summary>
		/// Returns <tt>true</tt> iff this node (or an ancestor) has been
		/// removed with the <seealso cref="#removeNode()"/> method.  This method
		/// locks this node prior to returning the contents of the private
		/// field used to track this state.
		/// </summary>
		/// <returns> <tt>true</tt> iff this node (or an ancestor) has been
		///       removed with the <seealso cref="#removeNode()"/> method. </returns>
		protected internal virtual bool Removed
		{
			get
			{
				lock (@lock)
				{
					return Removed_Renamed;
				}
			}
		}

		/// <summary>
		/// Queue of pending notification events.  When a preference or node
		/// change event for which there are one or more listeners occurs,
		/// it is placed on this queue and the queue is notified.  A background
		/// thread waits on this queue and delivers the events.  This decouples
		/// event delivery from preference activity, greatly simplifying
		/// locking and reducing opportunity for deadlock.
		/// </summary>
		private static readonly List<EventObject> EventQueue = new LinkedList<EventObject>();

		/// <summary>
		/// These two classes are used to distinguish NodeChangeEvents on
		/// eventQueue so the event dispatch thread knows whether to call
		/// childAdded or childRemoved.
		/// </summary>
		private class NodeAddedEvent : NodeChangeEvent
		{
			private readonly AbstractPreferences OuterInstance;

			internal const long SerialVersionUID = -6743557530157328528L;
			internal NodeAddedEvent(AbstractPreferences outerInstance, Preferences parent, Preferences child) : base(parent, child)
			{
				this.OuterInstance = outerInstance;
			}
		}
		private class NodeRemovedEvent : NodeChangeEvent
		{
			private readonly AbstractPreferences OuterInstance;

			internal const long SerialVersionUID = 8735497392918824837L;
			internal NodeRemovedEvent(AbstractPreferences outerInstance, Preferences parent, Preferences child) : base(parent, child)
			{
				this.OuterInstance = outerInstance;
			}
		}

		/// <summary>
		/// A single background thread ("the event notification thread") monitors
		/// the event queue and delivers events that are placed on the queue.
		/// </summary>
		private class EventDispatchThread : Thread
		{
			public override void Run()
			{
				while (true)
				{
					// Wait on eventQueue till an event is present
					EventObject @event = null;
					lock (EventQueue)
					{
						try
						{
							while (EventQueue.Count == 0)
							{
								Monitor.Wait(EventQueue);
							}
							@event = EventQueue.Remove(0);
						}
						catch (InterruptedException)
						{
							// XXX Log "Event dispatch thread interrupted. Exiting"
							return;
						}
					}

					// Now we have event & hold no locks; deliver evt to listeners
					AbstractPreferences src = (AbstractPreferences)@event.Source;
					if (@event is PreferenceChangeEvent)
					{
						PreferenceChangeEvent pce = (PreferenceChangeEvent)@event;
						PreferenceChangeListener[] listeners = src.PrefListeners();
						for (int i = 0; i < listeners.Length; i++)
						{
							listeners[i].PreferenceChange(pce);
						}
					}
					else
					{
						NodeChangeEvent nce = (NodeChangeEvent)@event;
						NodeChangeListener[] listeners = src.NodeListeners();
						if (nce is NodeAddedEvent)
						{
							for (int i = 0; i < listeners.Length; i++)
							{
								listeners[i].ChildAdded(nce);
							}
						}
						else
						{
							// assert nce instanceof NodeRemovedEvent;
							for (int i = 0; i < listeners.Length; i++)
							{
								listeners[i].ChildRemoved(nce);
							}
						}
					}
				}
			}
		}

		private static Thread EventDispatchThread = null;

		/// <summary>
		/// This method starts the event dispatch thread the first time it
		/// is called.  The event dispatch thread will be started only
		/// if someone registers a listener.
		/// </summary>
		private static void StartEventDispatchThreadIfNecessary()
		{
			lock (typeof(AbstractPreferences))
			{
				if (EventDispatchThread == null)
				{
					// XXX Log "Starting event dispatch thread"
					EventDispatchThread = new EventDispatchThread();
					EventDispatchThread.Daemon = true;
					EventDispatchThread.Start();
				}
			}
		}

		/// <summary>
		/// Return this node's preference/node change listeners.  Even though
		/// we're using a copy-on-write lists, we use synchronized accessors to
		/// ensure information transmission from the writing thread to the
		/// reading thread.
		/// </summary>
		internal virtual PreferenceChangeListener[] PrefListeners()
		{
			lock (@lock)
			{
				return PrefListeners_Renamed;
			}
		}
		internal virtual NodeChangeListener[] NodeListeners()
		{
			lock (@lock)
			{
				return NodeListeners_Renamed;
			}
		}

		/// <summary>
		/// Enqueue a preference change event for delivery to registered
		/// preference change listeners unless there are no registered
		/// listeners.  Invoked with this.lock held.
		/// </summary>
		private void EnqueuePreferenceChangeEvent(String key, String newValue)
		{
			if (PrefListeners_Renamed.Length != 0)
			{
				lock (EventQueue)
				{
					EventQueue.Add(new PreferenceChangeEvent(this, key, newValue));
					Monitor.Pulse(EventQueue);
				}
			}
		}

		/// <summary>
		/// Enqueue a "node added" event for delivery to registered node change
		/// listeners unless there are no registered listeners.  Invoked with
		/// this.lock held.
		/// </summary>
		private void EnqueueNodeAddedEvent(Preferences child)
		{
			if (NodeListeners_Renamed.Length != 0)
			{
				lock (EventQueue)
				{
					EventQueue.Add(new NodeAddedEvent(this, this, child));
					Monitor.Pulse(EventQueue);
				}
			}
		}

		/// <summary>
		/// Enqueue a "node removed" event for delivery to registered node change
		/// listeners unless there are no registered listeners.  Invoked with
		/// this.lock held.
		/// </summary>
		private void EnqueueNodeRemovedEvent(Preferences child)
		{
			if (NodeListeners_Renamed.Length != 0)
			{
				lock (EventQueue)
				{
					EventQueue.Add(new NodeRemovedEvent(this, this, child));
					Monitor.Pulse(EventQueue);
				}
			}
		}

		/// <summary>
		/// Implements the <tt>exportNode</tt> method as per the specification in
		/// <seealso cref="Preferences#exportNode(OutputStream)"/>.
		/// </summary>
		/// <param name="os"> the output stream on which to emit the XML document. </param>
		/// <exception cref="IOException"> if writing to the specified output stream
		///         results in an <tt>IOException</tt>. </exception>
		/// <exception cref="BackingStoreException"> if preference data cannot be read from
		///         backing store. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void exportNode(OutputStream os) throws IOException, BackingStoreException
		public override void ExportNode(OutputStream os)
		{
			XmlSupport.Export(os, this, false);
		}

		/// <summary>
		/// Implements the <tt>exportSubtree</tt> method as per the specification in
		/// <seealso cref="Preferences#exportSubtree(OutputStream)"/>.
		/// </summary>
		/// <param name="os"> the output stream on which to emit the XML document. </param>
		/// <exception cref="IOException"> if writing to the specified output stream
		///         results in an <tt>IOException</tt>. </exception>
		/// <exception cref="BackingStoreException"> if preference data cannot be read from
		///         backing store. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void exportSubtree(OutputStream os) throws IOException, BackingStoreException
		public override void ExportSubtree(OutputStream os)
		{
			XmlSupport.Export(os, this, true);
		}
	}

}