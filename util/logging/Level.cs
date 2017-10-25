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

namespace java.util.logging
{

	/// <summary>
	/// The Level class defines a set of standard logging levels that
	/// can be used to control logging output.  The logging Level objects
	/// are ordered and are specified by ordered integers.  Enabling logging
	/// at a given level also enables logging at all higher levels.
	/// <para>
	/// Clients should normally use the predefined Level constants such
	/// as Level.SEVERE.
	/// </para>
	/// <para>
	/// The levels in descending order are:
	/// <ul>
	/// <li>SEVERE (highest value)
	/// <li>WARNING
	/// <li>INFO
	/// <li>CONFIG
	/// <li>FINE
	/// <li>FINER
	/// <li>FINEST  (lowest value)
	/// </ul>
	/// In addition there is a level OFF that can be used to turn
	/// off logging, and a level ALL that can be used to enable
	/// logging of all messages.
	/// </para>
	/// <para>
	/// It is possible for third parties to define additional logging
	/// levels by subclassing Level.  In such cases subclasses should
	/// take care to chose unique integer level values and to ensure that
	/// they maintain the Object uniqueness property across serialization
	/// by defining a suitable readResolve method.
	/// 
	/// @since 1.4
	/// </para>
	/// </summary>

	[Serializable]
	public class Level
	{
		private const String DefaultBundle = "sun.util.logging.resources.logging";

		/// <summary>
		/// @serial  The non-localized name of the level.
		/// </summary>
		private readonly String Name_Renamed;

		/// <summary>
		/// @serial  The integer value of the level.
		/// </summary>
		private readonly int Value;

		/// <summary>
		/// @serial The resource bundle name to be used in localizing the level name.
		/// </summary>
		private readonly String ResourceBundleName_Renamed;

		// localized level name
		[NonSerialized]
		private String LocalizedLevelName_Renamed;
		[NonSerialized]
		private Locale CachedLocale;

		/// <summary>
		/// OFF is a special level that can be used to turn off logging.
		/// This level is initialized to <CODE>Integer.MAX_VALUE</CODE>.
		/// </summary>
		public static readonly Level OFF = new Level("OFF",Integer.MaxValue, DefaultBundle);

		/// <summary>
		/// SEVERE is a message level indicating a serious failure.
		/// <para>
		/// In general SEVERE messages should describe events that are
		/// of considerable importance and which will prevent normal
		/// program execution.   They should be reasonably intelligible
		/// to end users and to system administrators.
		/// This level is initialized to <CODE>1000</CODE>.
		/// </para>
		/// </summary>
		public static readonly Level SEVERE = new Level("SEVERE",1000, DefaultBundle);

		/// <summary>
		/// WARNING is a message level indicating a potential problem.
		/// <para>
		/// In general WARNING messages should describe events that will
		/// be of interest to end users or system managers, or which
		/// indicate potential problems.
		/// This level is initialized to <CODE>900</CODE>.
		/// </para>
		/// </summary>
		public static readonly Level WARNING = new Level("WARNING", 900, DefaultBundle);

		/// <summary>
		/// INFO is a message level for informational messages.
		/// <para>
		/// Typically INFO messages will be written to the console
		/// or its equivalent.  So the INFO level should only be
		/// used for reasonably significant messages that will
		/// make sense to end users and system administrators.
		/// This level is initialized to <CODE>800</CODE>.
		/// </para>
		/// </summary>
		public static readonly Level INFO = new Level("INFO", 800, DefaultBundle);

		/// <summary>
		/// CONFIG is a message level for static configuration messages.
		/// <para>
		/// CONFIG messages are intended to provide a variety of static
		/// configuration information, to assist in debugging problems
		/// that may be associated with particular configurations.
		/// For example, CONFIG message might include the CPU type,
		/// the graphics depth, the GUI look-and-feel, etc.
		/// This level is initialized to <CODE>700</CODE>.
		/// </para>
		/// </summary>
		public static readonly Level CONFIG = new Level("CONFIG", 700, DefaultBundle);

		/// <summary>
		/// FINE is a message level providing tracing information.
		/// <para>
		/// All of FINE, FINER, and FINEST are intended for relatively
		/// detailed tracing.  The exact meaning of the three levels will
		/// vary between subsystems, but in general, FINEST should be used
		/// for the most voluminous detailed output, FINER for somewhat
		/// less detailed output, and FINE for the  lowest volume (and
		/// most important) messages.
		/// </para>
		/// <para>
		/// In general the FINE level should be used for information
		/// that will be broadly interesting to developers who do not have
		/// a specialized interest in the specific subsystem.
		/// </para>
		/// <para>
		/// FINE messages might include things like minor (recoverable)
		/// failures.  Issues indicating potential performance problems
		/// are also worth logging as FINE.
		/// This level is initialized to <CODE>500</CODE>.
		/// </para>
		/// </summary>
		public static readonly Level FINE = new Level("FINE", 500, DefaultBundle);

		/// <summary>
		/// FINER indicates a fairly detailed tracing message.
		/// By default logging calls for entering, returning, or throwing
		/// an exception are traced at this level.
		/// This level is initialized to <CODE>400</CODE>.
		/// </summary>
		public static readonly Level FINER = new Level("FINER", 400, DefaultBundle);

		/// <summary>
		/// FINEST indicates a highly detailed tracing message.
		/// This level is initialized to <CODE>300</CODE>.
		/// </summary>
		public static readonly Level FINEST = new Level("FINEST", 300, DefaultBundle);

		/// <summary>
		/// ALL indicates that all messages should be logged.
		/// This level is initialized to <CODE>Integer.MIN_VALUE</CODE>.
		/// </summary>
		public static readonly Level ALL = new Level("ALL", Integer.MinValue, DefaultBundle);

		/// <summary>
		/// Create a named Level with a given integer value.
		/// <para>
		/// Note that this constructor is "protected" to allow subclassing.
		/// In general clients of logging should use one of the constant Level
		/// objects such as SEVERE or FINEST.  However, if clients need to
		/// add new logging levels, they may subclass Level and define new
		/// constants.
		/// </para>
		/// </summary>
		/// <param name="name">  the name of the Level, for example "SEVERE". </param>
		/// <param name="value"> an integer value for the level. </param>
		/// <exception cref="NullPointerException"> if the name is null </exception>
		protected internal Level(String name, int value) : this(name, value, null)
		{
		}

		/// <summary>
		/// Create a named Level with a given integer value and a
		/// given localization resource name.
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="name">  the name of the Level, for example "SEVERE". </param>
		/// <param name="value"> an integer value for the level. </param>
		/// <param name="resourceBundleName"> name of a resource bundle to use in
		///    localizing the given name. If the resourceBundleName is null
		///    or an empty string, it is ignored. </param>
		/// <exception cref="NullPointerException"> if the name is null </exception>
		protected internal Level(String name, int value, String resourceBundleName) : this(name, value, resourceBundleName, true)
		{
		}

		// private constructor to specify whether this instance should be added
		// to the KnownLevel list from which Level.parse method does its look up
		private Level(String name, int value, String resourceBundleName, bool visible)
		{
			if (name == null)
			{
				throw new NullPointerException();
			}
			this.Name_Renamed = name;
			this.Value = value;
			this.ResourceBundleName_Renamed = resourceBundleName;
			this.LocalizedLevelName_Renamed = resourceBundleName == null ? name : null;
			this.CachedLocale = null;
			if (visible)
			{
				KnownLevel.Add(this);
			}
		}

		/// <summary>
		/// Return the level's localization resource bundle name, or
		/// null if no localization bundle is defined.
		/// </summary>
		/// <returns> localization resource bundle name </returns>
		public virtual String ResourceBundleName
		{
			get
			{
				return ResourceBundleName_Renamed;
			}
		}

		/// <summary>
		/// Return the non-localized string name of the Level.
		/// </summary>
		/// <returns> non-localized name </returns>
		public virtual String Name
		{
			get
			{
				return Name_Renamed;
			}
		}

		/// <summary>
		/// Return the localized string name of the Level, for
		/// the current default locale.
		/// <para>
		/// If no localization information is available, the
		/// non-localized name is returned.
		/// 
		/// </para>
		/// </summary>
		/// <returns> localized name </returns>
		public virtual String LocalizedName
		{
			get
			{
				return LocalizedLevelName;
			}
		}

		// package-private getLevelName() is used by the implementation
		// instead of getName() to avoid calling the subclass's version
		internal String LevelName
		{
			get
			{
				return this.Name_Renamed;
			}
		}

		private String ComputeLocalizedLevelName(Locale newLocale)
		{
			ResourceBundle rb = ResourceBundle.GetBundle(ResourceBundleName_Renamed, newLocale);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localizedName = rb.getString(name);
			String localizedName = rb.GetString(Name_Renamed);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isDefaultBundle = defaultBundle.equals(resourceBundleName);
			bool isDefaultBundle = DefaultBundle.Equals(ResourceBundleName_Renamed);
			if (!isDefaultBundle)
			{
				return localizedName;
			}

			// This is a trick to determine whether the name has been translated
			// or not. If it has not been translated, we need to use Locale.ROOT
			// when calling toUpperCase().
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Locale rbLocale = rb.getLocale();
			Locale rbLocale = rb.Locale;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Locale locale = java.util.Locale.ROOT.equals(rbLocale) || name.equals(localizedName.toUpperCase(java.util.Locale.ROOT)) ? java.util.Locale.ROOT : rbLocale;
			Locale locale = Locale.ROOT.Equals(rbLocale) || Name_Renamed.Equals(localizedName.ToUpperCase(Locale.ROOT)) ? Locale.ROOT : rbLocale;

			// ALL CAPS in a resource bundle's message indicates no translation
			// needed per Oracle translation guideline.  To workaround this
			// in Oracle JDK implementation, convert the localized level name
			// to uppercase for compatibility reason.
			return Locale.ROOT.Equals(locale) ? Name_Renamed : localizedName.ToUpperCase(locale);
		}

		// Avoid looking up the localizedLevelName twice if we already
		// have it.
		internal String CachedLocalizedLevelName
		{
			get
			{
    
				if (LocalizedLevelName_Renamed != null)
				{
					if (CachedLocale != null)
					{
						if (CachedLocale.Equals(Locale.Default))
						{
							// OK: our cached value was looked up with the same
							//     locale. We can use it.
							return LocalizedLevelName_Renamed;
						}
					}
				}
    
				if (ResourceBundleName_Renamed == null)
				{
					// No resource bundle: just use the name.
					return Name_Renamed;
				}
    
				// We need to compute the localized name.
				// Either because it's the first time, or because our cached
				// value is for a different locale. Just return null.
				return null;
			}
		}

		internal String LocalizedLevelName
		{
			get
			{
				lock (this)
				{
            
					// See if we have a cached localized name
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String cachedLocalizedName = getCachedLocalizedLevelName();
					String cachedLocalizedName = CachedLocalizedLevelName;
					if (cachedLocalizedName != null)
					{
						return cachedLocalizedName;
					}
            
					// No cached localized name or cache invalid.
					// Need to compute the localized name.
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.Locale newLocale = java.util.Locale.getDefault();
					Locale newLocale = Locale.Default;
					try
					{
						LocalizedLevelName_Renamed = ComputeLocalizedLevelName(newLocale);
					}
					catch (Exception)
					{
						LocalizedLevelName_Renamed = Name_Renamed;
					}
					CachedLocale = newLocale;
					return LocalizedLevelName_Renamed;
				}
			}
		}

		// Returns a mirrored Level object that matches the given name as
		// specified in the Level.parse method.  Returns null if not found.
		//
		// It returns the same Level object as the one returned by Level.parse
		// method if the given name is a non-localized name or integer.
		//
		// If the name is a localized name, findLevel and parse method may
		// return a different level value if there is a custom Level subclass
		// that overrides Level.getLocalizedName() to return a different string
		// than what's returned by the default implementation.
		//
		internal static Level FindLevel(String name)
		{
			if (name == null)
			{
				throw new NullPointerException();
			}

			KnownLevel level;

			// Look for a known Level with the given non-localized name.
			level = KnownLevel.FindByName(name);
			if (level != null)
			{
				return level.MirroredLevel;
			}

			// Now, check if the given name is an integer.  If so,
			// first look for a Level with the given value and then
			// if necessary create one.
			try
			{
				int x = Convert.ToInt32(name);
				level = KnownLevel.FindByValue(x);
				if (level == null)
				{
					// add new Level
					Level levelObject = new Level(name, x);
					level = KnownLevel.FindByValue(x);
				}
				return level.MirroredLevel;
			}
			catch (NumberFormatException)
			{
				// Not an integer.
				// Drop through.
			}

			level = KnownLevel.FindByLocalizedLevelName(name);
			if (level != null)
			{
				return level.MirroredLevel;
			}

			return null;
		}

		/// <summary>
		/// Returns a string representation of this Level.
		/// </summary>
		/// <returns> the non-localized name of the Level, for example "INFO". </returns>
		public override sealed String ToString()
		{
			return Name_Renamed;
		}

		/// <summary>
		/// Get the integer value for this level.  This integer value
		/// can be used for efficient ordering comparisons between
		/// Level objects. </summary>
		/// <returns> the integer value for this level. </returns>
		public int IntValue()
		{
			return Value;
		}

		private const long SerialVersionUID = -8176160795706313070L;

		// Serialization magic to prevent "doppelgangers".
		// This is a performance optimization.
		private Object ReadResolve()
		{
			KnownLevel o = KnownLevel.Matches(this);
			if (o != null)
			{
				return o.LevelObject;
			}

			// Woops.  Whoever sent us this object knows
			// about a new log level.  Add it to our list.
			Level level = new Level(this.Name_Renamed, this.Value, this.ResourceBundleName_Renamed);
			return level;
		}

		/// <summary>
		/// Parse a level name string into a Level.
		/// <para>
		/// The argument string may consist of either a level name
		/// or an integer value.
		/// </para>
		/// <para>
		/// For example:
		/// <ul>
		/// <li>     "SEVERE"
		/// <li>     "1000"
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="name">   string to be parsed </param>
		/// <exception cref="NullPointerException"> if the name is null </exception>
		/// <exception cref="IllegalArgumentException"> if the value is not valid.
		/// Valid values are integers between <CODE>Integer.MIN_VALUE</CODE>
		/// and <CODE>Integer.MAX_VALUE</CODE>, and all known level names.
		/// Known names are the levels defined by this class (e.g., <CODE>FINE</CODE>,
		/// <CODE>FINER</CODE>, <CODE>FINEST</CODE>), or created by this class with
		/// appropriate package access, or new levels defined or created
		/// by subclasses.
		/// </exception>
		/// <returns> The parsed value. Passing an integer that corresponds to a known name
		/// (e.g., 700) will return the associated name (e.g., <CODE>CONFIG</CODE>).
		/// Passing an integer that does not (e.g., 1) will return a new level name
		/// initialized to that value. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static synchronized Level parse(String name) throws IllegalArgumentException
		public static Level Parse(String name)
		{
			lock (typeof(Level))
			{
				// Check that name is not null.
				name.Length();
        
				KnownLevel level;
        
				// Look for a known Level with the given non-localized name.
				level = KnownLevel.FindByName(name);
				if (level != null)
				{
					return level.LevelObject;
				}
        
				// Now, check if the given name is an integer.  If so,
				// first look for a Level with the given value and then
				// if necessary create one.
				try
				{
					int x = Convert.ToInt32(name);
					level = KnownLevel.FindByValue(x);
					if (level == null)
					{
						// add new Level
						Level levelObject = new Level(name, x);
						level = KnownLevel.FindByValue(x);
					}
					return level.LevelObject;
				}
				catch (NumberFormatException)
				{
					// Not an integer.
					// Drop through.
				}
        
				// Finally, look for a known level with the given localized name,
				// in the current default locale.
				// This is relatively expensive, but not excessively so.
				level = KnownLevel.FindByLocalizedLevelName(name);
				if (level != null)
				{
					return level.LevelObject;
				}
        
				// OK, we've tried everything and failed
				throw new IllegalArgumentException("Bad level \"" + name + "\"");
			}
		}

		/// <summary>
		/// Compare two objects for value equality. </summary>
		/// <returns> true if and only if the two objects have the same level value. </returns>
		public override bool Equals(Object ox)
		{
			try
			{
				Level lx = (Level)ox;
				return (lx.Value == this.Value);
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Generate a hashcode. </summary>
		/// <returns> a hashcode based on the level value </returns>
		public override int HashCode()
		{
			return this.Value;
		}

		// KnownLevel class maintains the global list of all known levels.
		// The API allows multiple custom Level instances of the same name/value
		// be created. This class provides convenient methods to find a level
		// by a given name, by a given value, or by a given localized name.
		//
		// KnownLevel wraps the following Level objects:
		// 1. levelObject:   standard Level object or custom Level object
		// 2. mirroredLevel: Level object representing the level specified in the
		//                   logging configuration.
		//
		// Level.getName, Level.getLocalizedName, Level.getResourceBundleName methods
		// are non-final but the name and resource bundle name are parameters to
		// the Level constructor.  Use the mirroredLevel object instead of the
		// levelObject to prevent the logging framework to execute foreign code
		// implemented by untrusted Level subclass.
		//
		// Implementation Notes:
		// If Level.getName, Level.getLocalizedName, Level.getResourceBundleName methods
		// were final, the following KnownLevel implementation can be removed.
		// Future API change should take this into consideration.
		internal sealed class KnownLevel
		{
			internal static IDictionary<String, IList<KnownLevel>> NameToLevels = new Dictionary<String, IList<KnownLevel>>();
			internal static IDictionary<Integer, IList<KnownLevel>> IntToLevels = new Dictionary<Integer, IList<KnownLevel>>();
			internal readonly Level LevelObject; // instance of Level class or Level subclass
			internal readonly Level MirroredLevel; // mirror of the custom Level
			internal KnownLevel(Level l)
			{
				this.LevelObject = l;
				if (l.GetType() == typeof(Level))
				{
					this.MirroredLevel = l;
				}
				else
				{
					// this mirrored level object is hidden
					this.MirroredLevel = new Level(l.Name_Renamed, l.Value, l.ResourceBundleName_Renamed, false);
				}
			}

			internal static void Add(Level l)
			{
				lock (typeof(KnownLevel))
				{
					// the mirroredLevel object is always added to the list
					// before the custom Level instance
					KnownLevel o = new KnownLevel(l);
					IList<KnownLevel> list = NameToLevels[l.Name_Renamed];
					if (list == null)
					{
						list = new List<>();
						NameToLevels[l.Name_Renamed] = list;
					}
					list.Add(o);
        
					list = IntToLevels[l.Value];
					if (list == null)
					{
						list = new List<>();
						IntToLevels[l.Value] = list;
					}
					list.Add(o);
				}
			}

			// Returns a KnownLevel with the given non-localized name.
			internal static KnownLevel FindByName(String name)
			{
				lock (typeof(KnownLevel))
				{
					IList<KnownLevel> list = NameToLevels[name];
					if (list != null)
					{
						return list[0];
					}
					return null;
				}
			}

			// Returns a KnownLevel with the given value.
			internal static KnownLevel FindByValue(int value)
			{
				lock (typeof(KnownLevel))
				{
					IList<KnownLevel> list = IntToLevels[value];
					if (list != null)
					{
						return list[0];
					}
					return null;
				}
			}

			// Returns a KnownLevel with the given localized name matching
			// by calling the Level.getLocalizedLevelName() method (i.e. found
			// from the resourceBundle associated with the Level object).
			// This method does not call Level.getLocalizedName() that may
			// be overridden in a subclass implementation
			internal static KnownLevel FindByLocalizedLevelName(String name)
			{
				lock (typeof(KnownLevel))
				{
					foreach (IList<KnownLevel> levels in NameToLevels.Values)
					{
						foreach (KnownLevel l in levels)
						{
							String lname = l.LevelObject.LocalizedLevelName;
							if (name.Equals(lname))
							{
								return l;
							}
						}
					}
					return null;
				}
			}

			internal static KnownLevel Matches(Level l)
			{
				lock (typeof(KnownLevel))
				{
					IList<KnownLevel> list = NameToLevels[l.Name_Renamed];
					if (list != null)
					{
						foreach (KnownLevel level in list)
						{
							Level other = level.MirroredLevel;
							if (l.Value == other.Value && (l.ResourceBundleName_Renamed == other.ResourceBundleName_Renamed || (l.ResourceBundleName_Renamed != null && l.ResourceBundleName_Renamed.Equals(other.ResourceBundleName_Renamed))))
							{
								return level;
							}
						}
					}
					return null;
				}
			}
		}

	}

}