using System;
using System.Collections.Generic;

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
namespace java.awt
{

	using AppContext = sun.awt.AppContext;

	/// <summary>
	/// An <code>AWTKeyStroke</code> represents a key action on the
	/// keyboard, or equivalent input device. <code>AWTKeyStroke</code>s
	/// can correspond to only a press or release of a
	/// particular key, just as <code>KEY_PRESSED</code> and
	/// <code>KEY_RELEASED</code> <code>KeyEvent</code>s do;
	/// alternately, they can correspond to typing a specific Java character, just
	/// as <code>KEY_TYPED</code> <code>KeyEvent</code>s do.
	/// In all cases, <code>AWTKeyStroke</code>s can specify modifiers
	/// (alt, shift, control, meta, altGraph, or a combination thereof) which must be present
	/// during the action for an exact match.
	/// <para>
	/// <code>AWTKeyStrokes</code> are immutable, and are intended
	/// to be unique. Client code should never create an
	/// <code>AWTKeyStroke</code> on its own, but should instead use
	/// a variant of <code>getAWTKeyStroke</code>. Client use of these factory
	/// methods allows the <code>AWTKeyStroke</code> implementation
	/// to cache and share instances efficiently.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= #getAWTKeyStroke
	/// 
	/// @author Arnaud Weber
	/// @author David Mendenhall
	/// @since 1.4 </seealso>
	[Serializable]
	public class AWTKeyStroke
	{
		internal const long SerialVersionUID = -6430539691155161871L;

		private static IDictionary<String, Integer> ModifierKeywords;
		/// <summary>
		/// Associates VK_XXX (as a String) with code (as Integer). This is
		/// done to avoid the overhead of the reflective call to find the
		/// constant.
		/// </summary>
		private static VKCollection Vks;

		//A key for the collection of AWTKeyStrokes within AppContext.
		private static Object APP_CONTEXT_CACHE_KEY = new Object();
		//A key withing the cache
		private static AWTKeyStroke APP_CONTEXT_KEYSTROKE_KEY = new AWTKeyStroke();

		/*
		 * Reads keystroke class from AppContext and if null, puts there the
		 * AWTKeyStroke class.
		 * Must be called under locked AWTKeyStro
		 */
		private static Class AWTKeyStrokeClass
		{
			get
			{
				Class clazz = (Class)AppContext.AppContext.get(typeof(AWTKeyStroke));
				if (clazz == null)
				{
					clazz = typeof(AWTKeyStroke);
					AppContext.AppContext.put(typeof(AWTKeyStroke), typeof(AWTKeyStroke));
				}
				return clazz;
			}
		}

		private char KeyChar_Renamed = KeyEvent.CHAR_UNDEFINED;
		private int KeyCode_Renamed = KeyEvent.VK_UNDEFINED;
		private int Modifiers_Renamed;
		private bool OnKeyRelease_Renamed;

		static AWTKeyStroke()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
		}

		/// <summary>
		/// Constructs an <code>AWTKeyStroke</code> with default values.
		/// The default values used are:
		/// <table border="1" summary="AWTKeyStroke default values">
		/// <tr><th>Property</th><th>Default Value</th></tr>
		/// <tr>
		///    <td>Key Char</td>
		///    <td><code>KeyEvent.CHAR_UNDEFINED</code></td>
		/// </tr>
		/// <tr>
		///    <td>Key Code</td>
		///    <td><code>KeyEvent.VK_UNDEFINED</code></td>
		/// </tr>
		/// <tr>
		///    <td>Modifiers</td>
		///    <td>none</td>
		/// </tr>
		/// <tr>
		///    <td>On key release?</td>
		///    <td><code>false</code></td>
		/// </tr>
		/// </table>
		/// 
		/// <code>AWTKeyStroke</code>s should not be constructed
		/// by client code. Use a variant of <code>getAWTKeyStroke</code>
		/// instead.
		/// </summary>
		/// <seealso cref= #getAWTKeyStroke </seealso>
		protected internal AWTKeyStroke()
		{
		}

		/// <summary>
		/// Constructs an <code>AWTKeyStroke</code> with the specified
		/// values. <code>AWTKeyStroke</code>s should not be constructed
		/// by client code. Use a variant of <code>getAWTKeyStroke</code>
		/// instead.
		/// </summary>
		/// <param name="keyChar"> the character value for a keyboard key </param>
		/// <param name="keyCode"> the key code for this <code>AWTKeyStroke</code> </param>
		/// <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		/// <param name="onKeyRelease"> <code>true</code> if this
		///        <code>AWTKeyStroke</code> corresponds
		///        to a key release; <code>false</code> otherwise </param>
		/// <seealso cref= #getAWTKeyStroke </seealso>
		protected internal AWTKeyStroke(char keyChar, int keyCode, int modifiers, bool onKeyRelease)
		{
			this.KeyChar_Renamed = keyChar;
			this.KeyCode_Renamed = keyCode;
			this.Modifiers_Renamed = modifiers;
			this.OnKeyRelease_Renamed = onKeyRelease;
		}

		/// <summary>
		/// Registers a new class which the factory methods in
		/// <code>AWTKeyStroke</code> will use when generating new
		/// instances of <code>AWTKeyStroke</code>s. After invoking this
		/// method, the factory methods will return instances of the specified
		/// Class. The specified Class must be either <code>AWTKeyStroke</code>
		/// or derived from <code>AWTKeyStroke</code>, and it must have a
		/// no-arg constructor. The constructor can be of any accessibility,
		/// including <code>private</code>. This operation
		/// flushes the current <code>AWTKeyStroke</code> cache.
		/// </summary>
		/// <param name="subclass"> the new Class of which the factory methods should create
		///        instances </param>
		/// <exception cref="IllegalArgumentException"> if subclass is <code>null</code>,
		///         or if subclass does not have a no-arg constructor </exception>
		/// <exception cref="ClassCastException"> if subclass is not
		///         <code>AWTKeyStroke</code>, or a class derived from
		///         <code>AWTKeyStroke</code> </exception>
		protected internal static void RegisterSubclass(Class subclass)
		{
			if (subclass == null)
			{
				throw new IllegalArgumentException("subclass cannot be null");
			}
			lock (typeof(AWTKeyStroke))
			{
				Class keyStrokeClass = (Class)AppContext.AppContext.get(typeof(AWTKeyStroke));
				if (keyStrokeClass != null && keyStrokeClass.Equals(subclass))
				{
					// Already registered
					return;
				}
			}
			if (!subclass.IsSubclassOf(typeof(AWTKeyStroke)))
			{
				throw new ClassCastException("subclass is not derived from AWTKeyStroke");
			}

			Constructor ctor = GetCtor(subclass);

			String couldNotInstantiate = "subclass could not be instantiated";

			if (ctor == null)
			{
				throw new IllegalArgumentException(couldNotInstantiate);
			}
			try
			{
				AWTKeyStroke stroke = (AWTKeyStroke)ctor.newInstance((Object[]) null);
				if (stroke == null)
				{
					throw new IllegalArgumentException(couldNotInstantiate);
				}
			}
			catch (NoSuchMethodError)
			{
				throw new IllegalArgumentException(couldNotInstantiate);
			}
			catch (ExceptionInInitializerError)
			{
				throw new IllegalArgumentException(couldNotInstantiate);
			}
			catch (InstantiationException)
			{
				throw new IllegalArgumentException(couldNotInstantiate);
			}
			catch (IllegalAccessException)
			{
				throw new IllegalArgumentException(couldNotInstantiate);
			}
			catch (InvocationTargetException)
			{
				throw new IllegalArgumentException(couldNotInstantiate);
			}

			lock (typeof(AWTKeyStroke))
			{
				AppContext.AppContext.put(typeof(AWTKeyStroke), subclass);
				AppContext.AppContext.remove(APP_CONTEXT_CACHE_KEY);
				AppContext.AppContext.remove(APP_CONTEXT_KEYSTROKE_KEY);
			}
		}

		/* returns noarg Constructor for class with accessible flag. No security
		   threat as accessible flag is set only for this Constructor object,
		   not for Class constructor.
		 */
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static Constructor getCtor(final Class clazz)
		private static Constructor GetCtor(Class clazz)
		{
			Constructor ctor = AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(clazz));
			return (Constructor)ctor;
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Constructor>
		{
			private Type Clazz;

			public PrivilegedActionAnonymousInnerClassHelper(Type clazz)
			{
				this.Clazz = clazz;
			}

			public virtual Constructor Run()
			{
				try
				{
					Constructor ctor = Clazz.GetDeclaredConstructor((Class[]) null);
					if (ctor != null)
					{
						ctor.Accessible = true;
					}
					return ctor;
				}
				catch (SecurityException)
				{
				}
				catch (NoSuchMethodException)
				{
				}
				return null;
			}
		}

		private static AWTKeyStroke GetCachedStroke(char keyChar, int keyCode, int modifiers, bool onKeyRelease)
		{
			lock (typeof(AWTKeyStroke))
			{
				IDictionary<AWTKeyStroke, AWTKeyStroke> cache = (IDictionary)AppContext.AppContext.get(APP_CONTEXT_CACHE_KEY);
				AWTKeyStroke cacheKey = (AWTKeyStroke)AppContext.AppContext.get(APP_CONTEXT_KEYSTROKE_KEY);
        
				if (cache == null)
				{
					cache = new Dictionary<>();
					AppContext.AppContext.put(APP_CONTEXT_CACHE_KEY, cache);
				}
        
				if (cacheKey == null)
				{
					try
					{
						Class clazz = AWTKeyStrokeClass;
						cacheKey = (AWTKeyStroke)GetCtor(clazz).newInstance((Object[]) null);
						AppContext.AppContext.put(APP_CONTEXT_KEYSTROKE_KEY, cacheKey);
					}
					catch (InstantiationException)
					{
						assert(false);
					}
					catch (IllegalAccessException)
					{
						assert(false);
					}
					catch (InvocationTargetException)
					{
						assert(false);
					}
				}
				cacheKey.KeyChar_Renamed = keyChar;
				cacheKey.KeyCode_Renamed = keyCode;
				cacheKey.Modifiers_Renamed = MapNewModifiers(MapOldModifiers(modifiers));
				cacheKey.OnKeyRelease_Renamed = onKeyRelease;
        
				AWTKeyStroke stroke = (AWTKeyStroke)cache[cacheKey];
				if (stroke == null)
				{
					stroke = cacheKey;
					cache[stroke] = stroke;
					AppContext.AppContext.remove(APP_CONTEXT_KEYSTROKE_KEY);
				}
				return stroke;
			}
		}

		/// <summary>
		/// Returns a shared instance of an <code>AWTKeyStroke</code>
		/// that represents a <code>KEY_TYPED</code> event for the
		/// specified character.
		/// </summary>
		/// <param name="keyChar"> the character value for a keyboard key </param>
		/// <returns> an <code>AWTKeyStroke</code> object for that key </returns>
		public static AWTKeyStroke GetAWTKeyStroke(char keyChar)
		{
			return GetCachedStroke(keyChar, KeyEvent.VK_UNDEFINED, 0, false);
		}

		/// <summary>
		/// Returns a shared instance of an {@code AWTKeyStroke}
		/// that represents a {@code KEY_TYPED} event for the
		/// specified Character object and a set of modifiers. Note
		/// that the first parameter is of type Character rather than
		/// char. This is to avoid inadvertent clashes with
		/// calls to <code>getAWTKeyStroke(int keyCode, int modifiers)</code>.
		/// 
		/// The modifiers consist of any combination of following:<ul>
		/// <li>java.awt.event.InputEvent.SHIFT_DOWN_MASK
		/// <li>java.awt.event.InputEvent.CTRL_DOWN_MASK
		/// <li>java.awt.event.InputEvent.META_DOWN_MASK
		/// <li>java.awt.event.InputEvent.ALT_DOWN_MASK
		/// <li>java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
		/// </ul>
		/// The old modifiers listed below also can be used, but they are
		/// mapped to _DOWN_ modifiers. <ul>
		/// <li>java.awt.event.InputEvent.SHIFT_MASK
		/// <li>java.awt.event.InputEvent.CTRL_MASK
		/// <li>java.awt.event.InputEvent.META_MASK
		/// <li>java.awt.event.InputEvent.ALT_MASK
		/// <li>java.awt.event.InputEvent.ALT_GRAPH_MASK
		/// </ul>
		/// also can be used, but they are mapped to _DOWN_ modifiers.
		/// 
		/// Since these numbers are all different powers of two, any combination of
		/// them is an integer in which each bit represents a different modifier
		/// key. Use 0 to specify no modifiers.
		/// </summary>
		/// <param name="keyChar"> the Character object for a keyboard character </param>
		/// <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		/// <returns> an <code>AWTKeyStroke</code> object for that key </returns>
		/// <exception cref="IllegalArgumentException"> if <code>keyChar</code> is
		///       <code>null</code>
		/// </exception>
		/// <seealso cref= java.awt.event.InputEvent </seealso>
		public static AWTKeyStroke GetAWTKeyStroke(Character keyChar, int modifiers)
		{
			if (keyChar == null)
			{
				throw new IllegalArgumentException("keyChar cannot be null");
			}
			return GetCachedStroke(keyChar.CharValue(), KeyEvent.VK_UNDEFINED, modifiers, false);
		}

		/// <summary>
		/// Returns a shared instance of an <code>AWTKeyStroke</code>,
		/// given a numeric key code and a set of modifiers, specifying
		/// whether the key is activated when it is pressed or released.
		/// <para>
		/// The "virtual key" constants defined in
		/// <code>java.awt.event.KeyEvent</code> can be
		/// used to specify the key code. For example:<ul>
		/// <li><code>java.awt.event.KeyEvent.VK_ENTER</code>
		/// <li><code>java.awt.event.KeyEvent.VK_TAB</code>
		/// <li><code>java.awt.event.KeyEvent.VK_SPACE</code>
		/// </ul>
		/// Alternatively, the key code may be obtained by calling
		/// <code>java.awt.event.KeyEvent.getExtendedKeyCodeForChar</code>.
		/// 
		/// The modifiers consist of any combination of:<ul>
		/// <li>java.awt.event.InputEvent.SHIFT_DOWN_MASK
		/// <li>java.awt.event.InputEvent.CTRL_DOWN_MASK
		/// <li>java.awt.event.InputEvent.META_DOWN_MASK
		/// <li>java.awt.event.InputEvent.ALT_DOWN_MASK
		/// <li>java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
		/// </ul>
		/// The old modifiers <ul>
		/// <li>java.awt.event.InputEvent.SHIFT_MASK
		/// <li>java.awt.event.InputEvent.CTRL_MASK
		/// <li>java.awt.event.InputEvent.META_MASK
		/// <li>java.awt.event.InputEvent.ALT_MASK
		/// <li>java.awt.event.InputEvent.ALT_GRAPH_MASK
		/// </ul>
		/// also can be used, but they are mapped to _DOWN_ modifiers.
		/// 
		/// Since these numbers are all different powers of two, any combination of
		/// them is an integer in which each bit represents a different modifier
		/// key. Use 0 to specify no modifiers.
		/// 
		/// </para>
		/// </summary>
		/// <param name="keyCode"> an int specifying the numeric code for a keyboard key </param>
		/// <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		/// <param name="onKeyRelease"> <code>true</code> if the <code>AWTKeyStroke</code>
		///        should represent a key release; <code>false</code> otherwise </param>
		/// <returns> an AWTKeyStroke object for that key
		/// </returns>
		/// <seealso cref= java.awt.event.KeyEvent </seealso>
		/// <seealso cref= java.awt.event.InputEvent </seealso>
		public static AWTKeyStroke GetAWTKeyStroke(int keyCode, int modifiers, bool onKeyRelease)
		{
			return GetCachedStroke(KeyEvent.CHAR_UNDEFINED, keyCode, modifiers, onKeyRelease);
		}

		/// <summary>
		/// Returns a shared instance of an <code>AWTKeyStroke</code>,
		/// given a numeric key code and a set of modifiers. The returned
		/// <code>AWTKeyStroke</code> will correspond to a key press.
		/// <para>
		/// The "virtual key" constants defined in
		/// <code>java.awt.event.KeyEvent</code> can be
		/// used to specify the key code. For example:<ul>
		/// <li><code>java.awt.event.KeyEvent.VK_ENTER</code>
		/// <li><code>java.awt.event.KeyEvent.VK_TAB</code>
		/// <li><code>java.awt.event.KeyEvent.VK_SPACE</code>
		/// </ul>
		/// The modifiers consist of any combination of:<ul>
		/// <li>java.awt.event.InputEvent.SHIFT_DOWN_MASK
		/// <li>java.awt.event.InputEvent.CTRL_DOWN_MASK
		/// <li>java.awt.event.InputEvent.META_DOWN_MASK
		/// <li>java.awt.event.InputEvent.ALT_DOWN_MASK
		/// <li>java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
		/// </ul>
		/// The old modifiers <ul>
		/// <li>java.awt.event.InputEvent.SHIFT_MASK
		/// <li>java.awt.event.InputEvent.CTRL_MASK
		/// <li>java.awt.event.InputEvent.META_MASK
		/// <li>java.awt.event.InputEvent.ALT_MASK
		/// <li>java.awt.event.InputEvent.ALT_GRAPH_MASK
		/// </ul>
		/// also can be used, but they are mapped to _DOWN_ modifiers.
		/// 
		/// Since these numbers are all different powers of two, any combination of
		/// them is an integer in which each bit represents a different modifier
		/// key. Use 0 to specify no modifiers.
		/// 
		/// </para>
		/// </summary>
		/// <param name="keyCode"> an int specifying the numeric code for a keyboard key </param>
		/// <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		/// <returns> an <code>AWTKeyStroke</code> object for that key
		/// </returns>
		/// <seealso cref= java.awt.event.KeyEvent </seealso>
		/// <seealso cref= java.awt.event.InputEvent </seealso>
		public static AWTKeyStroke GetAWTKeyStroke(int keyCode, int modifiers)
		{
			return GetCachedStroke(KeyEvent.CHAR_UNDEFINED, keyCode, modifiers, false);
		}

		/// <summary>
		/// Returns an <code>AWTKeyStroke</code> which represents the
		/// stroke which generated a given <code>KeyEvent</code>.
		/// <para>
		/// This method obtains the keyChar from a <code>KeyTyped</code>
		/// event, and the keyCode from a <code>KeyPressed</code> or
		/// <code>KeyReleased</code> event. The <code>KeyEvent</code> modifiers are
		/// obtained for all three types of <code>KeyEvent</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="anEvent"> the <code>KeyEvent</code> from which to
		///      obtain the <code>AWTKeyStroke</code> </param>
		/// <exception cref="NullPointerException"> if <code>anEvent</code> is null </exception>
		/// <returns> the <code>AWTKeyStroke</code> that precipitated the event </returns>
		public static AWTKeyStroke GetAWTKeyStrokeForEvent(KeyEvent anEvent)
		{
			int id = anEvent.ID;
			switch (id)
			{
			  case KeyEvent.KEY_PRESSED:
			  case KeyEvent.KEY_RELEASED:
				return GetCachedStroke(KeyEvent.CHAR_UNDEFINED, anEvent.KeyCode, anEvent.Modifiers, (id == KeyEvent.KEY_RELEASED));
			  case KeyEvent.KEY_TYPED:
				return GetCachedStroke(anEvent.KeyChar, KeyEvent.VK_UNDEFINED, anEvent.Modifiers, false);
			  default:
				// Invalid ID for this KeyEvent
				return null;
			}
		}

		/// <summary>
		/// Parses a string and returns an <code>AWTKeyStroke</code>.
		/// The string must have the following syntax:
		/// <pre>
		///    &lt;modifiers&gt;* (&lt;typedID&gt; | &lt;pressedReleasedID&gt;)
		/// 
		///    modifiers := shift | control | ctrl | meta | alt | altGraph
		///    typedID := typed &lt;typedKey&gt;
		///    typedKey := string of length 1 giving Unicode character.
		///    pressedReleasedID := (pressed | released) key
		///    key := KeyEvent key code name, i.e. the name following "VK_".
		/// </pre>
		/// If typed, pressed or released is not specified, pressed is assumed. Here
		/// are some examples:
		/// <pre>
		///     "INSERT" =&gt; getAWTKeyStroke(KeyEvent.VK_INSERT, 0);
		///     "control DELETE" =&gt; getAWTKeyStroke(KeyEvent.VK_DELETE, InputEvent.CTRL_MASK);
		///     "alt shift X" =&gt; getAWTKeyStroke(KeyEvent.VK_X, InputEvent.ALT_MASK | InputEvent.SHIFT_MASK);
		///     "alt shift released X" =&gt; getAWTKeyStroke(KeyEvent.VK_X, InputEvent.ALT_MASK | InputEvent.SHIFT_MASK, true);
		///     "typed a" =&gt; getAWTKeyStroke('a');
		/// </pre>
		/// </summary>
		/// <param name="s"> a String formatted as described above </param>
		/// <returns> an <code>AWTKeyStroke</code> object for that String </returns>
		/// <exception cref="IllegalArgumentException"> if <code>s</code> is <code>null</code>,
		///        or is formatted incorrectly </exception>
		public static AWTKeyStroke GetAWTKeyStroke(String s)
		{
			if (s == null)
			{
				throw new IllegalArgumentException("String cannot be null");
			}

			const String errmsg = "String formatted incorrectly";

			StringTokenizer st = new StringTokenizer(s, " ");

			int mask = 0;
			bool released = false;
			bool typed = false;
			bool pressed = false;

			lock (typeof(AWTKeyStroke))
			{
				if (ModifierKeywords == null)
				{
					IDictionary<String, Integer> uninitializedMap = new Dictionary<String, Integer>(8, 1.0f);
					uninitializedMap["shift"] = Convert.ToInt32(InputEvent.SHIFT_DOWN_MASK | InputEvent.SHIFT_MASK);
					uninitializedMap["control"] = Convert.ToInt32(InputEvent.CTRL_DOWN_MASK | InputEvent.CTRL_MASK);
					uninitializedMap["ctrl"] = Convert.ToInt32(InputEvent.CTRL_DOWN_MASK | InputEvent.CTRL_MASK);
					uninitializedMap["meta"] = Convert.ToInt32(InputEvent.META_DOWN_MASK | InputEvent.META_MASK);
					uninitializedMap["alt"] = Convert.ToInt32(InputEvent.ALT_DOWN_MASK | InputEvent.ALT_MASK);
					uninitializedMap["altGraph"] = Convert.ToInt32(InputEvent.ALT_GRAPH_DOWN_MASK | InputEvent.ALT_GRAPH_MASK);
					uninitializedMap["button1"] = Convert.ToInt32(InputEvent.BUTTON1_DOWN_MASK);
					uninitializedMap["button2"] = Convert.ToInt32(InputEvent.BUTTON2_DOWN_MASK);
					uninitializedMap["button3"] = Convert.ToInt32(InputEvent.BUTTON3_DOWN_MASK);
					ModifierKeywords = Collections.SynchronizedMap(uninitializedMap);
				}
			}

			int count = st.CountTokens();

			for (int i = 1; i <= count; i++)
			{
				String token = st.NextToken();

				if (typed)
				{
					if (token.Length() != 1 || i != count)
					{
						throw new IllegalArgumentException(errmsg);
					}
					return GetCachedStroke(token.CharAt(0), KeyEvent.VK_UNDEFINED, mask, false);
				}

				if (pressed || released || i == count)
				{
					if (i != count)
					{
						throw new IllegalArgumentException(errmsg);
					}

					String keyCodeName = "VK_" + token;
					int keyCode = GetVKValue(keyCodeName);

					return GetCachedStroke(KeyEvent.CHAR_UNDEFINED, keyCode, mask, released);
				}

				if (token.Equals("released"))
				{
					released = true;
					continue;
				}
				if (token.Equals("pressed"))
				{
					pressed = true;
					continue;
				}
				if (token.Equals("typed"))
				{
					typed = true;
					continue;
				}

				Integer tokenMask = (Integer)ModifierKeywords[token];
				if (tokenMask != null)
				{
					mask |= tokenMask.IntValue();
				}
				else
				{
					throw new IllegalArgumentException(errmsg);
				}
			}

			throw new IllegalArgumentException(errmsg);
		}

		private static VKCollection VKCollection
		{
			get
			{
				if (Vks == null)
				{
					Vks = new VKCollection();
				}
				return Vks;
			}
		}
		/// <summary>
		/// Returns the integer constant for the KeyEvent.VK field named
		/// <code>key</code>. This will throw an
		/// <code>IllegalArgumentException</code> if <code>key</code> is
		/// not a valid constant.
		/// </summary>
		private static int GetVKValue(String key)
		{
			VKCollection vkCollect = VKCollection;

			Integer value = vkCollect.FindCode(key);

			if (value == null)
			{
				int keyCode = 0;
				const String errmsg = "String formatted incorrectly";

				try
				{
					keyCode = typeof(KeyEvent).GetField(key).getInt(typeof(KeyEvent));
				}
				catch (NoSuchFieldException)
				{
					throw new IllegalArgumentException(errmsg);
				}
				catch (IllegalAccessException)
				{
					throw new IllegalArgumentException(errmsg);
				}
				value = Convert.ToInt32(keyCode);
				vkCollect.Put(key, value);
			}
			return value.IntValue();
		}

		/// <summary>
		/// Returns the character for this <code>AWTKeyStroke</code>.
		/// </summary>
		/// <returns> a char value </returns>
		/// <seealso cref= #getAWTKeyStroke(char) </seealso>
		/// <seealso cref= KeyEvent#getKeyChar </seealso>
		public char KeyChar
		{
			get
			{
				return KeyChar_Renamed;
			}
		}

		/// <summary>
		/// Returns the numeric key code for this <code>AWTKeyStroke</code>.
		/// </summary>
		/// <returns> an int containing the key code value </returns>
		/// <seealso cref= #getAWTKeyStroke(int,int) </seealso>
		/// <seealso cref= KeyEvent#getKeyCode </seealso>
		public int KeyCode
		{
			get
			{
				return KeyCode_Renamed;
			}
		}

		/// <summary>
		/// Returns the modifier keys for this <code>AWTKeyStroke</code>.
		/// </summary>
		/// <returns> an int containing the modifiers </returns>
		/// <seealso cref= #getAWTKeyStroke(int,int) </seealso>
		public int Modifiers
		{
			get
			{
				return Modifiers_Renamed;
			}
		}

		/// <summary>
		/// Returns whether this <code>AWTKeyStroke</code> represents a key release.
		/// </summary>
		/// <returns> <code>true</code> if this <code>AWTKeyStroke</code>
		///          represents a key release; <code>false</code> otherwise </returns>
		/// <seealso cref= #getAWTKeyStroke(int,int,boolean) </seealso>
		public bool OnKeyRelease
		{
			get
			{
				return OnKeyRelease_Renamed;
			}
		}

		/// <summary>
		/// Returns the type of <code>KeyEvent</code> which corresponds to
		/// this <code>AWTKeyStroke</code>.
		/// </summary>
		/// <returns> <code>KeyEvent.KEY_PRESSED</code>,
		///         <code>KeyEvent.KEY_TYPED</code>,
		///         or <code>KeyEvent.KEY_RELEASED</code> </returns>
		/// <seealso cref= java.awt.event.KeyEvent </seealso>
		public int KeyEventType
		{
			get
			{
				if (KeyCode_Renamed == KeyEvent.VK_UNDEFINED)
				{
					return KeyEvent.KEY_TYPED;
				}
				else
				{
					return (OnKeyRelease_Renamed) ? KeyEvent.KEY_RELEASED : KeyEvent.KEY_PRESSED;
				}
			}
		}

		/// <summary>
		/// Returns a numeric value for this object that is likely to be unique,
		/// making it a good choice as the index value in a hash table.
		/// </summary>
		/// <returns> an int that represents this object </returns>
		public override int HashCode()
		{
			return (((int)KeyChar_Renamed) + 1) * (2 * (KeyCode_Renamed + 1)) * (Modifiers_Renamed + 1) + (OnKeyRelease_Renamed ? 1 : 2);
		}

		/// <summary>
		/// Returns true if this object is identical to the specified object.
		/// </summary>
		/// <param name="anObject"> the Object to compare this object to </param>
		/// <returns> true if the objects are identical </returns>
		public sealed override bool Equals(Object anObject)
		{
			if (anObject is AWTKeyStroke)
			{
				AWTKeyStroke ks = (AWTKeyStroke)anObject;
				return (ks.KeyChar_Renamed == KeyChar_Renamed && ks.KeyCode_Renamed == KeyCode_Renamed && ks.OnKeyRelease_Renamed == OnKeyRelease_Renamed && ks.Modifiers_Renamed == Modifiers_Renamed);
			}
			return false;
		}

		/// <summary>
		/// Returns a string that displays and identifies this object's properties.
		/// The <code>String</code> returned by this method can be passed
		/// as a parameter to <code>getAWTKeyStroke(String)</code> to produce
		/// a key stroke equal to this key stroke.
		/// </summary>
		/// <returns> a String representation of this object </returns>
		/// <seealso cref= #getAWTKeyStroke(String) </seealso>
		public override String ToString()
		{
			if (KeyCode_Renamed == KeyEvent.VK_UNDEFINED)
			{
				return GetModifiersText(Modifiers_Renamed) + "typed " + KeyChar_Renamed;
			}
			else
			{
				return GetModifiersText(Modifiers_Renamed) + (OnKeyRelease_Renamed ? "released" : "pressed") + " " + GetVKText(KeyCode_Renamed);
			}
		}

		internal static String GetModifiersText(int modifiers)
		{
			StringBuilder buf = new StringBuilder();

			if ((modifiers & InputEvent.SHIFT_DOWN_MASK) != 0)
			{
				buf.Append("shift ");
			}
			if ((modifiers & InputEvent.CTRL_DOWN_MASK) != 0)
			{
				buf.Append("ctrl ");
			}
			if ((modifiers & InputEvent.META_DOWN_MASK) != 0)
			{
				buf.Append("meta ");
			}
			if ((modifiers & InputEvent.ALT_DOWN_MASK) != 0)
			{
				buf.Append("alt ");
			}
			if ((modifiers & InputEvent.ALT_GRAPH_DOWN_MASK) != 0)
			{
				buf.Append("altGraph ");
			}
			if ((modifiers & InputEvent.BUTTON1_DOWN_MASK) != 0)
			{
				buf.Append("button1 ");
			}
			if ((modifiers & InputEvent.BUTTON2_DOWN_MASK) != 0)
			{
				buf.Append("button2 ");
			}
			if ((modifiers & InputEvent.BUTTON3_DOWN_MASK) != 0)
			{
				buf.Append("button3 ");
			}

			return buf.ToString();
		}

		internal static String GetVKText(int keyCode)
		{
			VKCollection vkCollect = VKCollection;
			Integer key = Convert.ToInt32(keyCode);
			String name = vkCollect.FindName(key);
			if (name != null)
			{
				return name.Substring(3);
			}
			int expected_modifiers = (Modifier.PUBLIC | Modifier.STATIC | Modifier.FINAL);

			Field[] fields = typeof(KeyEvent).DeclaredFields;
			for (int i = 0; i < fields.Length; i++)
			{
				try
				{
					if (fields[i].Modifiers == expected_modifiers && fields[i].Type == Integer.TYPE && fields[i].Name.StartsWith("VK_", StringComparison.Ordinal) && fields[i].getInt(typeof(KeyEvent)) == keyCode)
					{
						name = fields[i].Name;
						vkCollect.Put(name, key);
						return name.Substring(3);
					}
				}
				catch (IllegalAccessException)
				{
					assert(false);
				}
			}
			return "UNKNOWN";
		}

		/// <summary>
		/// Returns a cached instance of <code>AWTKeyStroke</code> (or a subclass of
		/// <code>AWTKeyStroke</code>) which is equal to this instance.
		/// </summary>
		/// <returns> a cached instance which is equal to this instance </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object readResolve() throws java.io.ObjectStreamException
		protected internal virtual Object ReadResolve()
		{
			lock (typeof(AWTKeyStroke))
			{
				if (this.GetType().Equals(AWTKeyStrokeClass))
				{
					return GetCachedStroke(KeyChar_Renamed, KeyCode_Renamed, Modifiers_Renamed, OnKeyRelease_Renamed);
				}
			}
			return this;
		}

		private static int MapOldModifiers(int modifiers)
		{
			if ((modifiers & InputEvent.SHIFT_MASK) != 0)
			{
				modifiers |= InputEvent.SHIFT_DOWN_MASK;
			}
			if ((modifiers & InputEvent.ALT_MASK) != 0)
			{
				modifiers |= InputEvent.ALT_DOWN_MASK;
			}
			if ((modifiers & InputEvent.ALT_GRAPH_MASK) != 0)
			{
				modifiers |= InputEvent.ALT_GRAPH_DOWN_MASK;
			}
			if ((modifiers & InputEvent.CTRL_MASK) != 0)
			{
				modifiers |= InputEvent.CTRL_DOWN_MASK;
			}
			if ((modifiers & InputEvent.META_MASK) != 0)
			{
				modifiers |= InputEvent.META_DOWN_MASK;
			}

			modifiers &= InputEvent.SHIFT_DOWN_MASK | InputEvent.ALT_DOWN_MASK | InputEvent.ALT_GRAPH_DOWN_MASK | InputEvent.CTRL_DOWN_MASK | InputEvent.META_DOWN_MASK | InputEvent.BUTTON1_DOWN_MASK | InputEvent.BUTTON2_DOWN_MASK | InputEvent.BUTTON3_DOWN_MASK;

			return modifiers;
		}

		private static int MapNewModifiers(int modifiers)
		{
			if ((modifiers & InputEvent.SHIFT_DOWN_MASK) != 0)
			{
				modifiers |= InputEvent.SHIFT_MASK;
			}
			if ((modifiers & InputEvent.ALT_DOWN_MASK) != 0)
			{
				modifiers |= InputEvent.ALT_MASK;
			}
			if ((modifiers & InputEvent.ALT_GRAPH_DOWN_MASK) != 0)
			{
				modifiers |= InputEvent.ALT_GRAPH_MASK;
			}
			if ((modifiers & InputEvent.CTRL_DOWN_MASK) != 0)
			{
				modifiers |= InputEvent.CTRL_MASK;
			}
			if ((modifiers & InputEvent.META_DOWN_MASK) != 0)
			{
				modifiers |= InputEvent.META_MASK;
			}

			return modifiers;
		}

	}

	internal class VKCollection
	{
		internal IDictionary<Integer, String> Code2name;
		internal IDictionary<String, Integer> Name2code;

		public VKCollection()
		{
			Code2name = new Dictionary<>();
			Name2code = new Dictionary<>();
		}

		public virtual void Put(String name, Integer code)
		{
			lock (this)
			{
				assert((name != null) && (code != null));
				assert(FindName(code) == null);
				assert(FindCode(name) == null);
				Code2name[code] = name;
				Name2code[name] = code;
			}
		}

		public virtual Integer FindCode(String name)
		{
			lock (this)
			{
				assert(name != null);
				return (Integer)Name2code[name];
			}
		}

		public virtual String FindName(Integer code)
		{
			lock (this)
			{
				assert(code != null);
				return (String)Code2name[code];
			}
		}
	}

}