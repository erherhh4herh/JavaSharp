using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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



	using PlatformLogger = sun.util.logging.PlatformLogger;
	using AWTAccessor = sun.awt.AWTAccessor;

	/// <summary>
	/// A class to encapsulate the bitmap representation of the mouse cursor.
	/// </summary>
	/// <seealso cref= Component#setCursor
	/// @author      Amy Fowler </seealso>
	[Serializable]
	public class Cursor
	{

		/// <summary>
		/// The default cursor type (gets set if no cursor is defined).
		/// </summary>
		public const int DEFAULT_CURSOR = 0;

		/// <summary>
		/// The crosshair cursor type.
		/// </summary>
		public const int CROSSHAIR_CURSOR = 1;

		/// <summary>
		/// The text cursor type.
		/// </summary>
		public const int TEXT_CURSOR = 2;

		/// <summary>
		/// The wait cursor type.
		/// </summary>
		public const int WAIT_CURSOR = 3;

		/// <summary>
		/// The south-west-resize cursor type.
		/// </summary>
		public const int SW_RESIZE_CURSOR = 4;

		/// <summary>
		/// The south-east-resize cursor type.
		/// </summary>
		public const int SE_RESIZE_CURSOR = 5;

		/// <summary>
		/// The north-west-resize cursor type.
		/// </summary>
		public const int NW_RESIZE_CURSOR = 6;

		/// <summary>
		/// The north-east-resize cursor type.
		/// </summary>
		public const int NE_RESIZE_CURSOR = 7;

		/// <summary>
		/// The north-resize cursor type.
		/// </summary>
		public const int N_RESIZE_CURSOR = 8;

		/// <summary>
		/// The south-resize cursor type.
		/// </summary>
		public const int S_RESIZE_CURSOR = 9;

		/// <summary>
		/// The west-resize cursor type.
		/// </summary>
		public const int W_RESIZE_CURSOR = 10;

		/// <summary>
		/// The east-resize cursor type.
		/// </summary>
		public const int E_RESIZE_CURSOR = 11;

		/// <summary>
		/// The hand cursor type.
		/// </summary>
		public const int HAND_CURSOR = 12;

		/// <summary>
		/// The move cursor type.
		/// </summary>
		public const int MOVE_CURSOR = 13;

		/// @deprecated As of JDK version 1.7, the <seealso cref="#getPredefinedCursor(int)"/>
		/// method should be used instead. 
		[Obsolete("As of JDK version 1.7, the <seealso cref="#getPredefinedCursor(int)"/>")]
		protected internal static Cursor[] Predefined = new Cursor[14];

		/// <summary>
		/// This field is a private replacement for 'predefined' array.
		/// </summary>
		private static readonly Cursor[] PredefinedPrivate = new Cursor[14];

		/* Localization names and default values */
		internal static readonly String[][] CursorProperties = new String[][] {new String[] {"AWT.DefaultCursor", "Default Cursor"}, new String[] {"AWT.CrosshairCursor", "Crosshair Cursor"}, new String[] {"AWT.TextCursor", "Text Cursor"}, new String[] {"AWT.WaitCursor", "Wait Cursor"}, new String[] {"AWT.SWResizeCursor", "Southwest Resize Cursor"}, new String[] {"AWT.SEResizeCursor", "Southeast Resize Cursor"}, new String[] {"AWT.NWResizeCursor", "Northwest Resize Cursor"}, new String[] {"AWT.NEResizeCursor", "Northeast Resize Cursor"}, new String[] {"AWT.NResizeCursor", "North Resize Cursor"}, new String[] {"AWT.SResizeCursor", "South Resize Cursor"}, new String[] {"AWT.WResizeCursor", "West Resize Cursor"}, new String[] {"AWT.EResizeCursor", "East Resize Cursor"}, new String[] {"AWT.HandCursor", "Hand Cursor"}, new String[] {"AWT.MoveCursor", "Move Cursor"}};

		/// <summary>
		/// The chosen cursor type initially set to
		/// the <code>DEFAULT_CURSOR</code>.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getType() </seealso>
		internal int Type_Renamed = DEFAULT_CURSOR;

		/// <summary>
		/// The type associated with all custom cursors.
		/// </summary>
		public const int CUSTOM_CURSOR = -1;

		/*
		 * hashtable, filesystem dir prefix, filename, and properties for custom cursors support
		 */

		private static readonly Dictionary<String, Cursor> SystemCustomCursors = new Dictionary<String, Cursor>(1);
		private static readonly String SystemCustomCursorDirPrefix = InitCursorDir();

		private static String InitCursorDir()
		{
			String jhome = AccessController.doPrivileged(new sun.security.action.GetPropertyAction("java.home"));
			return jhome + File.separator + "lib" + File.separator + "images" + File.separator + "cursors" + File.separator;
		}

		private static readonly String SystemCustomCursorPropertiesFile = SystemCustomCursorDirPrefix + "cursors.properties";

		private static Properties SystemCustomCursorProperties = null;

		private const String CursorDotPrefix = "Cursor.";
		private const String DotFileSuffix = ".File";
		private const String DotHotspotSuffix = ".HotSpot";
		private const String DotNameSuffix = ".Name";

		/*
		 * JDK 1.1 serialVersionUID
		 */
		private const long SerialVersionUID = 8028237497568985504L;

		private static readonly PlatformLogger Log = PlatformLogger.getLogger("java.awt.Cursor");

		static Cursor()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}

			AWTAccessor.CursorAccessor = new CursorAccessorAnonymousInnerClassHelper();
		}

		private class CursorAccessorAnonymousInnerClassHelper : AWTAccessor.CursorAccessor
		{
			public CursorAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual long GetPData(Cursor cursor)
			{
				return cursor.PData_Renamed;
			}

			public virtual void SetPData(Cursor cursor, long pData)
			{
				cursor.PData_Renamed = pData;
			}

			public virtual int GetType(Cursor cursor)
			{
				return cursor.Type_Renamed;
			}
		}

		/// <summary>
		/// Initialize JNI field and method IDs for fields that may be
		/// accessed from C.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		/// <summary>
		/// Hook into native data.
		/// </summary>
		[NonSerialized]
		private long PData_Renamed;

		[NonSerialized]
		private Object Anchor = new Object();

		internal class CursorDisposer : sun.java2d.DisposerRecord
		{
			internal volatile long PData;
			public CursorDisposer(long pData)
			{
				this.PData = pData;
			}
			public virtual void Dispose()
			{
				if (PData != 0)
				{
					finalizeImpl(PData);
				}
			}
		}
		[NonSerialized]
		internal CursorDisposer Disposer;
		private long PData
		{
			set
			{
				this.PData_Renamed = value;
				if (GraphicsEnvironment.Headless)
				{
					return;
				}
				if (Disposer == null)
				{
					Disposer = new CursorDisposer(value);
					// anchor is null after deserialization
					if (Anchor == null)
					{
						Anchor = new Object();
					}
					sun.java2d.Disposer.addRecord(Anchor, Disposer);
				}
				else
				{
					Disposer.PData = value;
				}
			}
		}

		/// <summary>
		/// The user-visible name of the cursor.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getName() </seealso>
		protected internal String Name_Renamed;

		/// <summary>
		/// Returns a cursor object with the specified predefined type.
		/// </summary>
		/// <param name="type"> the type of predefined cursor </param>
		/// <returns> the specified predefined cursor </returns>
		/// <exception cref="IllegalArgumentException"> if the specified cursor type is
		///         invalid </exception>
		public static Cursor GetPredefinedCursor(int type)
		{
			if (type < Cursor.DEFAULT_CURSOR || type > Cursor.MOVE_CURSOR)
			{
				throw new IllegalArgumentException("illegal cursor type");
			}
			Cursor c = PredefinedPrivate[type];
			if (c == null)
			{
				PredefinedPrivate[type] = c = new Cursor(type);
			}
			// fill 'predefined' array for backwards compatibility.
			if (Predefined[type] == null)
			{
				Predefined[type] = c;
			}
			return c;
		}

		/// <summary>
		/// Returns a system-specific custom cursor object matching the
		/// specified name.  Cursor names are, for example: "Invalid.16x16"
		/// </summary>
		/// <param name="name"> a string describing the desired system-specific custom cursor </param>
		/// <returns> the system specific custom cursor named </returns>
		/// <exception cref="HeadlessException"> if
		/// <code>GraphicsEnvironment.isHeadless</code> returns true </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Cursor getSystemCustomCursor(final String name) throws AWTException, HeadlessException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static Cursor GetSystemCustomCursor(String name)
		{
			GraphicsEnvironment.CheckHeadless();
			Cursor cursor = SystemCustomCursors[name];

			if (cursor == null)
			{
				lock (SystemCustomCursors)
				{
					if (SystemCustomCursorProperties == null)
					{
						LoadSystemCustomCursorProperties();
					}
				}

				String prefix = CursorDotPrefix + name;
				String key = prefix + DotFileSuffix;

				if (!SystemCustomCursorProperties.ContainsKey(key))
				{
					if (Log.isLoggable(PlatformLogger.Level.FINER))
					{
						Log.finer("Cursor.getSystemCustomCursor(" + name + ") returned null");
					}
					return null;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String fileName = systemCustomCursorProperties.getProperty(key);
				String fileName = SystemCustomCursorProperties.GetProperty(key);

				String localized = SystemCustomCursorProperties.GetProperty(prefix + DotNameSuffix);

				if (localized == null)
				{
					localized = name;
				}

				String hotspot = SystemCustomCursorProperties.GetProperty(prefix + DotHotspotSuffix);

				if (hotspot == null)
				{
					throw new AWTException("no hotspot property defined for cursor: " + name);
				}

				StringTokenizer st = new StringTokenizer(hotspot, ",");

				if (st.CountTokens() != 2)
				{
					throw new AWTException("failed to parse hotspot property for cursor: " + name);
				}

				int x = 0;
				int y = 0;

				try
				{
					x = Convert.ToInt32(st.NextToken());
					y = Convert.ToInt32(st.NextToken());
				}
				catch (NumberFormatException)
				{
					throw new AWTException("failed to parse hotspot property for cursor: " + name);
				}

				try
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fx = x;
					int fx = x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fy = y;
					int fy = y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String flocalized = localized;
					String flocalized = localized;

					cursor = AccessController.doPrivileged<Cursor>(new PrivilegedExceptionActionAnonymousInnerClassHelper(fileName, fx, fy, flocalized));
				}
				catch (Exception e)
				{
					throw new AWTException("Exception: " + e.GetType() + " " + e.Message + " occurred while creating cursor " + name);
				}

				if (cursor == null)
				{
					if (Log.isLoggable(PlatformLogger.Level.FINER))
					{
						Log.finer("Cursor.getSystemCustomCursor(" + name + ") returned null");
					}
				}
				else
				{
					SystemCustomCursors[name] = cursor;
				}
			}

			return cursor;
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper : java.security.PrivilegedExceptionAction<Cursor>
		{
			private string FileName;
			private int Fx;
			private int Fy;
			private string Flocalized;

			public PrivilegedExceptionActionAnonymousInnerClassHelper(string fileName, int fx, int fy, string flocalized)
			{
				this.FileName = fileName;
				this.Fx = fx;
				this.Fy = fy;
				this.Flocalized = flocalized;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Cursor run() throws Exception
			public virtual Cursor Run()
			{
				Toolkit toolkit = Toolkit.DefaultToolkit;
				Image image = toolkit.GetImage(SystemCustomCursorDirPrefix + FileName);
				return toolkit.CreateCustomCursor(image, new Point(Fx,Fy), Flocalized);
			}
		}

		/// <summary>
		/// Return the system default cursor.
		/// </summary>
		public static Cursor DefaultCursor
		{
			get
			{
				return GetPredefinedCursor(Cursor.DEFAULT_CURSOR);
			}
		}

		/// <summary>
		/// Creates a new cursor object with the specified type. </summary>
		/// <param name="type"> the type of cursor </param>
		/// <exception cref="IllegalArgumentException"> if the specified cursor type
		/// is invalid </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConstructorProperties({"type"}) public Cursor(int type)
		public Cursor(int type)
		{
			if (type < Cursor.DEFAULT_CURSOR || type > Cursor.MOVE_CURSOR)
			{
				throw new IllegalArgumentException("illegal cursor type");
			}
			this.Type_Renamed = type;

			// Lookup localized name.
			Name_Renamed = Toolkit.GetProperty(CursorProperties[type][0], CursorProperties[type][1]);
		}

		/// <summary>
		/// Creates a new custom cursor object with the specified name.<para>
		/// Note:  this constructor should only be used by AWT implementations
		/// as part of their support for custom cursors.  Applications should
		/// use Toolkit.createCustomCursor().
		/// </para>
		/// </summary>
		/// <param name="name"> the user-visible name of the cursor. </param>
		/// <seealso cref= java.awt.Toolkit#createCustomCursor </seealso>
		protected internal Cursor(String name)
		{
			this.Type_Renamed = Cursor.CUSTOM_CURSOR;
			this.Name_Renamed = name;
		}

		/// <summary>
		/// Returns the type for this cursor.
		/// </summary>
		public virtual int Type
		{
			get
			{
				return Type_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of this cursor. </summary>
		/// <returns>    a localized description of this cursor.
		/// @since     1.2 </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Returns a string representation of this cursor. </summary>
		/// <returns>    a string representation of this cursor.
		/// @since     1.2 </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[" + Name + "]";
		}

		/*
		 * load the cursor.properties file
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void loadSystemCustomCursorProperties() throws AWTException
		private static void LoadSystemCustomCursorProperties()
		{
			lock (SystemCustomCursors)
			{
				SystemCustomCursorProperties = new Properties();

				try
				{
					AccessController.doPrivileged<Object>(new PrivilegedExceptionActionAnonymousInnerClassHelper2());
				}
				catch (Exception e)
				{
					SystemCustomCursorProperties = null;
					 throw new AWTException("Exception: " + e.GetType() + " " + e.Message + " occurred while loading: " + SystemCustomCursorPropertiesFile);
				}
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClassHelper2 : java.security.PrivilegedExceptionAction<Object>
		{
			public PrivilegedExceptionActionAnonymousInnerClassHelper2()
			{
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object run() throws Exception
			public virtual Object Run()
			{
				FileInputStream fis = null;
				try
				{
					fis = new FileInputStream(SystemCustomCursorPropertiesFile);
					SystemCustomCursorProperties.Load(fis);
				}
				finally
				{
					if (fis != null)
					{
						fis.Close();
					}
				}
				return null;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private extern static void finalizeImpl(long pData);
	}

}