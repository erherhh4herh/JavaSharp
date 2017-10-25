using System;
using System.Threading;
using System.Runtime.InteropServices;

/*
 * Copyright (c) 2000, 2002, Oracle and/or its affiliates. All rights reserved.
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

	using PlatformLogger = sun.util.logging.PlatformLogger;

	/// <summary>
	/// Windows registry based implementation of  <tt>Preferences</tt>.
	/// <tt>Preferences</tt>' <tt>systemRoot</tt> and <tt>userRoot</tt> are stored in
	/// <tt>HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\Prefs</tt> and
	/// <tt>HKEY_CURRENT_USER\Software\JavaSoft\Prefs</tt> correspondingly.
	/// 
	/// @author  Konstantin Kladko </summary>
	/// <seealso cref= Preferences </seealso>
	/// <seealso cref= PreferencesFactory
	/// @since 1.4 </seealso>

	internal class WindowsPreferences : AbstractPreferences
	{

		/// <summary>
		/// Logger for error messages
		/// </summary>
		private static PlatformLogger Logger_Renamed;

		/// <summary>
		/// Windows registry path to <tt>Preferences</tt>'s root nodes.
		/// </summary>
		private static readonly sbyte[] WINDOWS_ROOT_PATH = StringToByteArray("Software\\JavaSoft\\Prefs");

		/// <summary>
		/// Windows handles to <tt>HKEY_CURRENT_USER</tt> and
		/// <tt>HKEY_LOCAL_MACHINE</tt> hives.
		/// </summary>
		private const int HKEY_CURRENT_USER = unchecked((int)0x80000001);
		private const int HKEY_LOCAL_MACHINE = unchecked((int)0x80000002);

		/// <summary>
		/// Mount point for <tt>Preferences</tt>'  user root.
		/// </summary>
		private const int USER_ROOT_NATIVE_HANDLE = HKEY_CURRENT_USER;

		/// <summary>
		/// Mount point for <tt>Preferences</tt>'  system root.
		/// </summary>
		private const int SYSTEM_ROOT_NATIVE_HANDLE = HKEY_LOCAL_MACHINE;

		/// <summary>
		/// Maximum byte-encoded path length for Windows native functions,
		/// ending <tt>null</tt> character not included.
		/// </summary>
		private const int MAX_WINDOWS_PATH_LENGTH = 256;

		/// <summary>
		/// User root node.
		/// </summary>
		internal static readonly Preferences UserRoot = new WindowsPreferences(USER_ROOT_NATIVE_HANDLE, WINDOWS_ROOT_PATH);

		/// <summary>
		/// System root node.
		/// </summary>
		internal static readonly Preferences SystemRoot = new WindowsPreferences(SYSTEM_ROOT_NATIVE_HANDLE, WINDOWS_ROOT_PATH);

		/*  Windows error codes. */
		private const int ERROR_SUCCESS = 0;
		private const int ERROR_FILE_NOT_FOUND = 2;
		private const int ERROR_ACCESS_DENIED = 5;

		/* Constants used to interpret returns of native functions    */
		private const int NATIVE_HANDLE = 0;
		private const int ERROR_CODE = 1;
		private const int SUBKEYS_NUMBER = 0;
		private const int VALUES_NUMBER = 2;
		private new const int MAX_KEY_LENGTH = 3;
		private const int MAX_VALUE_NAME_LENGTH = 4;
		private const int DISPOSITION = 2;
		private const int REG_CREATED_NEW_KEY = 1;
		private const int REG_OPENED_EXISTING_KEY = 2;
		private const int NULL_NATIVE_HANDLE = 0;

		/* Windows security masks */
		private const int DELETE = 0x10000;
		private const int KEY_QUERY_VALUE = 1;
		private const int KEY_SET_VALUE = 2;
		private const int KEY_CREATE_SUB_KEY = 4;
		private const int KEY_ENUMERATE_SUB_KEYS = 8;
		private const int KEY_READ = 0x20019;
		private const int KEY_WRITE = 0x20006;
		private const int KEY_ALL_ACCESS = 0xf003f;

		/// <summary>
		/// Initial time between registry access attempts, in ms. The time is doubled
		/// after each failing attempt (except the first).
		/// </summary>
		private static int INIT_SLEEP_TIME = 50;

		/// <summary>
		/// Maximum number of registry access attempts.
		/// </summary>
		private static int MAX_ATTEMPTS = 5;

		/// <summary>
		/// BackingStore availability flag.
		/// </summary>
		private bool IsBackingStoreAvailable = true;

		/// <summary>
		/// Java wrapper for Windows registry API RegOpenKey()
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int[] WindowsRegOpenKey(int hKey, sbyte[] subKey, int securityMask);
		/// <summary>
		/// Retries RegOpenKey() MAX_ATTEMPTS times before giving up.
		/// </summary>
		private static int[] WindowsRegOpenKey1(int hKey, sbyte[] subKey, int securityMask)
		{
			int[] result = WindowsRegOpenKey(hKey, subKey, securityMask);
			if (result[ERROR_CODE] == ERROR_SUCCESS)
			{
				return result;
			}
			else if (result[ERROR_CODE] == ERROR_FILE_NOT_FOUND)
			{
				Logger().warning("Trying to recreate Windows registry node " + ByteArrayToString(subKey) + " at root 0x" + hKey.ToString("x") + ".");
				// Try recreation
				int handle = WindowsRegCreateKeyEx(hKey, subKey)[NATIVE_HANDLE];
				WindowsRegCloseKey(handle);
				return WindowsRegOpenKey(hKey, subKey, securityMask);
			}
			else if (result[ERROR_CODE] != ERROR_ACCESS_DENIED)
			{
				long sleepTime = INIT_SLEEP_TIME;
				for (int i = 0; i < MAX_ATTEMPTS; i++)
				{
					try
					{
						Thread.Sleep(sleepTime);
					}
					catch (InterruptedException)
					{
						return result;
					}
					sleepTime *= 2;
					result = WindowsRegOpenKey(hKey, subKey, securityMask);
					if (result[ERROR_CODE] == ERROR_SUCCESS)
					{
						return result;
					}
				}
			}
			return result;
		}

		 /// <summary>
		 /// Java wrapper for Windows registry API RegCloseKey()
		 /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int WindowsRegCloseKey(int hKey);

		/// <summary>
		/// Java wrapper for Windows registry API RegCreateKeyEx()
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int[] WindowsRegCreateKeyEx(int hKey, sbyte[] subKey);

		/// <summary>
		/// Retries RegCreateKeyEx() MAX_ATTEMPTS times before giving up.
		/// </summary>
		private static int[] WindowsRegCreateKeyEx1(int hKey, sbyte[] subKey)
		{
			int[] result = WindowsRegCreateKeyEx(hKey, subKey);
			if (result[ERROR_CODE] == ERROR_SUCCESS)
			{
				return result;
			}
			else
			{
				long sleepTime = INIT_SLEEP_TIME;
				for (int i = 0; i < MAX_ATTEMPTS; i++)
				{
					try
					{
						Thread.Sleep(sleepTime);
					}
					catch (InterruptedException)
					{
						return result;
					}
					sleepTime *= 2;
					result = WindowsRegCreateKeyEx(hKey, subKey);
					if (result[ERROR_CODE] == ERROR_SUCCESS)
					{
						return result;
					}
				}
			}
			return result;
		}
		/// <summary>
		/// Java wrapper for Windows registry API RegDeleteKey()
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int WindowsRegDeleteKey(int hKey, sbyte[] subKey);

		/// <summary>
		/// Java wrapper for Windows registry API RegFlushKey()
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int WindowsRegFlushKey(int hKey);

		/// <summary>
		/// Retries RegFlushKey() MAX_ATTEMPTS times before giving up.
		/// </summary>
		private static int WindowsRegFlushKey1(int hKey)
		{
			int result = WindowsRegFlushKey(hKey);
			if (result == ERROR_SUCCESS)
			{
				return result;
			}
			else
			{
				long sleepTime = INIT_SLEEP_TIME;
				for (int i = 0; i < MAX_ATTEMPTS; i++)
				{
					try
					{
						Thread.Sleep(sleepTime);
					}
					catch (InterruptedException)
					{
						return result;
					}
					sleepTime *= 2;
					result = WindowsRegFlushKey(hKey);
					if (result == ERROR_SUCCESS)
					{
						return result;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Java wrapper for Windows registry API RegQueryValueEx()
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern byte[] WindowsRegQueryValueEx(int hKey, sbyte[] valueName);
		/// <summary>
		/// Java wrapper for Windows registry API RegSetValueEx()
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int WindowsRegSetValueEx(int hKey, sbyte[] valueName, sbyte[] value);
		/// <summary>
		/// Retries RegSetValueEx() MAX_ATTEMPTS times before giving up.
		/// </summary>
		private static int WindowsRegSetValueEx1(int hKey, sbyte[] valueName, sbyte[] value)
		{
			int result = WindowsRegSetValueEx(hKey, valueName, value);
			if (result == ERROR_SUCCESS)
			{
				return result;
			}
			else
			{
				long sleepTime = INIT_SLEEP_TIME;
				for (int i = 0; i < MAX_ATTEMPTS; i++)
				{
					try
					{
						Thread.Sleep(sleepTime);
					}
					catch (InterruptedException)
					{
						return result;
					}
					sleepTime *= 2;
					result = WindowsRegSetValueEx(hKey, valueName, value);
					if (result == ERROR_SUCCESS)
					{
						return result;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Java wrapper for Windows registry API RegDeleteValue()
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int WindowsRegDeleteValue(int hKey, sbyte[] valueName);

		/// <summary>
		/// Java wrapper for Windows registry API RegQueryInfoKey()
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern int[] WindowsRegQueryInfoKey(int hKey);

		/// <summary>
		/// Retries RegQueryInfoKey() MAX_ATTEMPTS times before giving up.
		/// </summary>
		private static int[] WindowsRegQueryInfoKey1(int hKey)
		{
			int[] result = WindowsRegQueryInfoKey(hKey);
			if (result[ERROR_CODE] == ERROR_SUCCESS)
			{
				return result;
			}
			else
			{
				long sleepTime = INIT_SLEEP_TIME;
				for (int i = 0; i < MAX_ATTEMPTS; i++)
				{
					try
					{
						Thread.Sleep(sleepTime);
					}
					catch (InterruptedException)
					{
						return result;
					}
					sleepTime *= 2;
					result = WindowsRegQueryInfoKey(hKey);
					if (result[ERROR_CODE] == ERROR_SUCCESS)
					{
						return result;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Java wrapper for Windows registry API RegEnumKeyEx()
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern byte[] WindowsRegEnumKeyEx(int hKey, int subKeyIndex, int maxKeyLength);

		/// <summary>
		/// Retries RegEnumKeyEx() MAX_ATTEMPTS times before giving up.
		/// </summary>
		private static sbyte[] WindowsRegEnumKeyEx1(int hKey, int subKeyIndex, int maxKeyLength)
		{
			sbyte[] result = WindowsRegEnumKeyEx(hKey, subKeyIndex, maxKeyLength);
			if (result != null)
			{
				return result;
			}
			else
			{
				long sleepTime = INIT_SLEEP_TIME;
				for (int i = 0; i < MAX_ATTEMPTS; i++)
				{
					try
					{
						Thread.Sleep(sleepTime);
					}
					catch (InterruptedException)
					{
						return result;
					}
					sleepTime *= 2;
					result = WindowsRegEnumKeyEx(hKey, subKeyIndex, maxKeyLength);
					if (result != null)
					{
						return result;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Java wrapper for Windows registry API RegEnumValue()
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern byte[] WindowsRegEnumValue(int hKey, int valueIndex, int maxValueNameLength);
		/// <summary>
		/// Retries RegEnumValueEx() MAX_ATTEMPTS times before giving up.
		/// </summary>
		private static sbyte[] WindowsRegEnumValue1(int hKey, int valueIndex, int maxValueNameLength)
		{
			sbyte[] result = WindowsRegEnumValue(hKey, valueIndex, maxValueNameLength);
			if (result != null)
			{
				return result;
			}
			else
			{
				long sleepTime = INIT_SLEEP_TIME;
				for (int i = 0; i < MAX_ATTEMPTS; i++)
				{
					try
					{
						Thread.Sleep(sleepTime);
					}
					catch (InterruptedException)
					{
						return result;
					}
					sleepTime *= 2;
					result = WindowsRegEnumValue(hKey, valueIndex, maxValueNameLength);
					if (result != null)
					{
						return result;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Constructs a <tt>WindowsPreferences</tt> node, creating underlying
		/// Windows registry node and all its Windows parents, if they are not yet
		/// created.
		/// Logs a warning message, if Windows Registry is unavailable.
		/// </summary>
		private WindowsPreferences(WindowsPreferences parent, String name) : base(parent, name)
		{
			int parentNativeHandle = parent.OpenKey(KEY_CREATE_SUB_KEY, KEY_READ);
			if (parentNativeHandle == NULL_NATIVE_HANDLE)
			{
				// if here, openKey failed and logged
				IsBackingStoreAvailable = false;
				return;
			}
			int[] result = WindowsRegCreateKeyEx1(parentNativeHandle, ToWindowsName(name));
			if (result[ERROR_CODE] != ERROR_SUCCESS)
			{
				Logger().warning("Could not create windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ". Windows RegCreateKeyEx(...) returned error code " + result[ERROR_CODE] + ".");
				IsBackingStoreAvailable = false;
				return;
			}
			NewNode = (result[DISPOSITION] == REG_CREATED_NEW_KEY);
			CloseKey(parentNativeHandle);
			CloseKey(result[NATIVE_HANDLE]);
		}

		/// <summary>
		/// Constructs a root node creating the underlying
		/// Windows registry node and all of its parents, if they have not yet been
		/// created.
		/// Logs a warning message, if Windows Registry is unavailable. </summary>
		/// <param name="rootNativeHandle"> Native handle to one of Windows top level keys. </param>
		/// <param name="rootDirectory"> Path to root directory, as a byte-encoded string. </param>
		private WindowsPreferences(int rootNativeHandle, sbyte[] rootDirectory) : base(null, "")
		{
			int[] result = WindowsRegCreateKeyEx1(rootNativeHandle, rootDirectory);
			if (result[ERROR_CODE] != ERROR_SUCCESS)
			{
				Logger().warning("Could not open/create prefs root node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ". Windows RegCreateKeyEx(...) returned error code " + result[ERROR_CODE] + ".");
				IsBackingStoreAvailable = false;
				return;
			}
			// Check if a new node
			NewNode = (result[DISPOSITION] == REG_CREATED_NEW_KEY);
			CloseKey(result[NATIVE_HANDLE]);
		}

		/// <summary>
		/// Returns Windows absolute path of the current node as a byte array.
		/// Java "/" separator is transformed into Windows "\". </summary>
		/// <seealso cref= Preferences#absolutePath() </seealso>
		private sbyte[] WindowsAbsolutePath()
		{
			ByteArrayOutputStream bstream = new ByteArrayOutputStream();
			bstream.Write(WINDOWS_ROOT_PATH, 0, WINDOWS_ROOT_PATH.Length - 1);
			StringTokenizer tokenizer = new StringTokenizer(AbsolutePath(), "/");
			while (tokenizer.HasMoreTokens())
			{
				bstream.Write((sbyte)'\\');
				String nextName = tokenizer.NextToken();
				sbyte[] windowsNextName = ToWindowsName(nextName);
				bstream.Write(windowsNextName, 0, windowsNextName.Length - 1);
			}
			bstream.Write(0);
			return bstream.ToByteArray();
		}

		/// <summary>
		/// Opens current node's underlying Windows registry key using a
		/// given security mask. </summary>
		/// <param name="securityMask"> Windows security mask. </param>
		/// <returns> Windows registry key's handle. </returns>
		/// <seealso cref= #openKey(byte[], int) </seealso>
		/// <seealso cref= #openKey(int, byte[], int) </seealso>
		/// <seealso cref= #closeKey(int) </seealso>
		private int OpenKey(int securityMask)
		{
			return OpenKey(securityMask, securityMask);
		}

		/// <summary>
		/// Opens current node's underlying Windows registry key using a
		/// given security mask. </summary>
		/// <param name="mask1"> Preferred Windows security mask. </param>
		/// <param name="mask2"> Alternate Windows security mask. </param>
		/// <returns> Windows registry key's handle. </returns>
		/// <seealso cref= #openKey(byte[], int) </seealso>
		/// <seealso cref= #openKey(int, byte[], int) </seealso>
		/// <seealso cref= #closeKey(int) </seealso>
		private int OpenKey(int mask1, int mask2)
		{
			return OpenKey(WindowsAbsolutePath(), mask1, mask2);
		}

		 /// <summary>
		 /// Opens Windows registry key at a given absolute path using a given
		 /// security mask. </summary>
		 /// <param name="windowsAbsolutePath"> Windows absolute path of the
		 ///        key as a byte-encoded string. </param>
		 /// <param name="mask1"> Preferred Windows security mask. </param>
		 /// <param name="mask2"> Alternate Windows security mask. </param>
		 /// <returns> Windows registry key's handle. </returns>
		 /// <seealso cref= #openKey(int) </seealso>
		 /// <seealso cref= #openKey(int, byte[],int) </seealso>
		 /// <seealso cref= #closeKey(int) </seealso>
		private int OpenKey(sbyte[] windowsAbsolutePath, int mask1, int mask2)
		{
			/*  Check if key's path is short enough be opened at once
			    otherwise use a path-splitting procedure */
			if (windowsAbsolutePath.Length <= MAX_WINDOWS_PATH_LENGTH + 1)
			{
				int[] result = WindowsRegOpenKey1(RootNativeHandle(), windowsAbsolutePath, mask1);
				if (result[ERROR_CODE] == ERROR_ACCESS_DENIED && mask2 != mask1)
				{
					result = WindowsRegOpenKey1(RootNativeHandle(), windowsAbsolutePath, mask2);
				}

				if (result[ERROR_CODE] != ERROR_SUCCESS)
				{
					Logger().warning("Could not open windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ". Windows RegOpenKey(...) returned error code " + result[ERROR_CODE] + ".");
					result[NATIVE_HANDLE] = NULL_NATIVE_HANDLE;
					if (result[ERROR_CODE] == ERROR_ACCESS_DENIED)
					{
						throw new SecurityException("Could not open windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ": Access denied");
					}
				}
				return result[NATIVE_HANDLE];
			}
			else
			{
				return OpenKey(RootNativeHandle(), windowsAbsolutePath, mask1, mask2);
			}
		}

		 /// <summary>
		 /// Opens Windows registry key at a given relative path
		 /// with respect to a given Windows registry key. </summary>
		 /// <param name="windowsAbsolutePath"> Windows relative path of the
		 ///        key as a byte-encoded string. </param>
		 /// <param name="nativeHandle"> handle to the base Windows key. </param>
		 /// <param name="mask1"> Preferred Windows security mask. </param>
		 /// <param name="mask2"> Alternate Windows security mask. </param>
		 /// <returns> Windows registry key's handle. </returns>
		 /// <seealso cref= #openKey(int) </seealso>
		 /// <seealso cref= #openKey(byte[],int) </seealso>
		 /// <seealso cref= #closeKey(int) </seealso>
		private int OpenKey(int nativeHandle, sbyte[] windowsRelativePath, int mask1, int mask2)
		{
		/* If the path is short enough open at once. Otherwise split the path */
			if (windowsRelativePath.Length <= MAX_WINDOWS_PATH_LENGTH + 1)
			{
				int[] result = WindowsRegOpenKey1(nativeHandle, windowsRelativePath, mask1);
				if (result[ERROR_CODE] == ERROR_ACCESS_DENIED && mask2 != mask1)
				{
					result = WindowsRegOpenKey1(nativeHandle, windowsRelativePath, mask2);
				}

				if (result[ERROR_CODE] != ERROR_SUCCESS)
				{
					Logger().warning("Could not open windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + nativeHandle.ToString("x") + ". Windows RegOpenKey(...) returned error code " + result[ERROR_CODE] + ".");
					result[NATIVE_HANDLE] = NULL_NATIVE_HANDLE;
				}
				return result[NATIVE_HANDLE];
			}
			else
			{
				int separatorPosition = -1;
				// Be greedy - open the longest possible path
				for (int i = MAX_WINDOWS_PATH_LENGTH; i > 0; i--)
				{
					if (windowsRelativePath[i] == ((sbyte)'\\'))
					{
						separatorPosition = i;
						break;
					}
				}
				// Split the path and do the recursion
				sbyte[] nextRelativeRoot = new sbyte[separatorPosition + 1];
				System.Array.Copy(windowsRelativePath, 0, nextRelativeRoot,0, separatorPosition);
				nextRelativeRoot[separatorPosition] = 0;
				sbyte[] nextRelativePath = new sbyte[windowsRelativePath.Length - separatorPosition - 1];
				System.Array.Copy(windowsRelativePath, separatorPosition + 1, nextRelativePath, 0, nextRelativePath.Length);
				int nextNativeHandle = OpenKey(nativeHandle, nextRelativeRoot, mask1, mask2);
				if (nextNativeHandle == NULL_NATIVE_HANDLE)
				{
					return NULL_NATIVE_HANDLE;
				}
				int result = OpenKey(nextNativeHandle, nextRelativePath, mask1,mask2);
				CloseKey(nextNativeHandle);
				return result;
			}
		}

		 /// <summary>
		 /// Closes Windows registry key.
		 /// Logs a warning if Windows registry is unavailable. </summary>
		 /// <param name="key">'s Windows registry handle. </param>
		 /// <seealso cref= #openKey(int) </seealso>
		 /// <seealso cref= #openKey(byte[],int) </seealso>
		 /// <seealso cref= #openKey(int, byte[],int) </seealso>
		private void CloseKey(int nativeHandle)
		{
			int result = WindowsRegCloseKey(nativeHandle);
			if (result != ERROR_SUCCESS)
			{
				Logger().warning("Could not close windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ". Windows RegCloseKey(...) returned error code " + result + ".");
			}
		}

		 /// <summary>
		 /// Implements <tt>AbstractPreferences</tt> <tt>putSpi()</tt> method.
		 /// Puts name-value pair into the underlying Windows registry node.
		 /// Logs a warning, if Windows registry is unavailable. </summary>
		 /// <seealso cref= #getSpi(String) </seealso>
		protected internal override void PutSpi(String javaName, String value)
		{
			int nativeHandle = OpenKey(KEY_SET_VALUE);
			if (nativeHandle == NULL_NATIVE_HANDLE)
			{
				IsBackingStoreAvailable = false;
				return;
			}
			int result = WindowsRegSetValueEx1(nativeHandle, ToWindowsName(javaName), ToWindowsValueString(value));
			if (result != ERROR_SUCCESS)
			{
				Logger().warning("Could not assign value to key " + ByteArrayToString(ToWindowsName(javaName)) + " at Windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ". Windows RegSetValueEx(...) returned error code " + result + ".");
				IsBackingStoreAvailable = false;
			}
			CloseKey(nativeHandle);
		}

		/// <summary>
		/// Implements <tt>AbstractPreferences</tt> <tt>getSpi()</tt> method.
		/// Gets a string value from the underlying Windows registry node.
		/// Logs a warning, if Windows registry is unavailable. </summary>
		/// <seealso cref= #putSpi(String, String) </seealso>
		protected internal override String GetSpi(String javaName)
		{
		int nativeHandle = OpenKey(KEY_QUERY_VALUE);
		if (nativeHandle == NULL_NATIVE_HANDLE)
		{
			return null;
		}
		Object resultObject = WindowsRegQueryValueEx(nativeHandle, ToWindowsName(javaName));
		if (resultObject == null)
		{
			CloseKey(nativeHandle);
			return null;
		}
		CloseKey(nativeHandle);
		return ToJavaValueString((sbyte[]) resultObject);
		}

		/// <summary>
		/// Implements <tt>AbstractPreferences</tt> <tt>removeSpi()</tt> method.
		/// Deletes a string name-value pair from the underlying Windows registry
		/// node, if this value still exists.
		/// Logs a warning, if Windows registry is unavailable or key has already
		/// been deleted.
		/// </summary>
		protected internal override void RemoveSpi(String key)
		{
			int nativeHandle = OpenKey(KEY_SET_VALUE);
			if (nativeHandle == NULL_NATIVE_HANDLE)
			{
			return;
			}
			int result = WindowsRegDeleteValue(nativeHandle, ToWindowsName(key));
			if (result != ERROR_SUCCESS && result != ERROR_FILE_NOT_FOUND)
			{
				Logger().warning("Could not delete windows registry value " + ByteArrayToString(WindowsAbsolutePath()) + "\\" + ToWindowsName(key) + " at root 0x" + RootNativeHandle().ToString("x") + ". Windows RegDeleteValue(...) returned error code " + result + ".");
				IsBackingStoreAvailable = false;
			}
			CloseKey(nativeHandle);
		}

		/// <summary>
		/// Implements <tt>AbstractPreferences</tt> <tt>keysSpi()</tt> method.
		/// Gets value names from the underlying Windows registry node.
		/// Throws a BackingStoreException and logs a warning, if
		/// Windows registry is unavailable.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String[] keysSpi() throws BackingStoreException
		protected internal override String[] KeysSpi()
		{
			// Find out the number of values
			int nativeHandle = OpenKey(KEY_QUERY_VALUE);
			if (nativeHandle == NULL_NATIVE_HANDLE)
			{
				throw new BackingStoreException("Could not open windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ".");
			}
			int[] result = WindowsRegQueryInfoKey1(nativeHandle);
			if (result[ERROR_CODE] != ERROR_SUCCESS)
			{
				String info = "Could not query windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ". Windows RegQueryInfoKeyEx(...) returned error code " + result[ERROR_CODE] + ".";
				Logger().warning(info);
				throw new BackingStoreException(info);
			}
			int maxValueNameLength = result[MAX_VALUE_NAME_LENGTH];
			int valuesNumber = result[VALUES_NUMBER];
			if (valuesNumber == 0)
			{
				CloseKey(nativeHandle);
				return new String[0];
			}
			// Get the values
			String[] valueNames = new String[valuesNumber];
			for (int i = 0; i < valuesNumber; i++)
			{
				sbyte[] windowsName = WindowsRegEnumValue1(nativeHandle, i, maxValueNameLength + 1);
				if (windowsName == null)
				{
					String info = "Could not enumerate value #" + i + "  of windows node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ".";
					Logger().warning(info);
					throw new BackingStoreException(info);
				}
				valueNames[i] = ToJavaName(windowsName);
			}
			CloseKey(nativeHandle);
			return valueNames;
		}

		/// <summary>
		/// Implements <tt>AbstractPreferences</tt> <tt>childrenNamesSpi()</tt> method.
		/// Calls Windows registry to retrive children of this node.
		/// Throws a BackingStoreException and logs a warning message,
		/// if Windows registry is not available.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String[] childrenNamesSpi() throws BackingStoreException
		protected internal override String[] ChildrenNamesSpi()
		{
			// Open key
			int nativeHandle = OpenKey(KEY_ENUMERATE_SUB_KEYS | KEY_QUERY_VALUE);
			if (nativeHandle == NULL_NATIVE_HANDLE)
			{
				throw new BackingStoreException("Could not open windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ".");
			}
			// Get number of children
			int[] result = WindowsRegQueryInfoKey1(nativeHandle);
			if (result[ERROR_CODE] != ERROR_SUCCESS)
			{
				String info = "Could not query windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ". Windows RegQueryInfoKeyEx(...) returned error code " + result[ERROR_CODE] + ".";
				Logger().warning(info);
				throw new BackingStoreException(info);
			}
			int maxKeyLength = result[MAX_KEY_LENGTH];
			int subKeysNumber = result[SUBKEYS_NUMBER];
			if (subKeysNumber == 0)
			{
				CloseKey(nativeHandle);
				return new String[0];
			}
			String[] subkeys = new String[subKeysNumber];
			String[] children = new String[subKeysNumber];
			// Get children
			for (int i = 0; i < subKeysNumber; i++)
			{
				sbyte[] windowsName = WindowsRegEnumKeyEx1(nativeHandle, i, maxKeyLength + 1);
				if (windowsName == null)
				{
					String info = "Could not enumerate key #" + i + "  of windows node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ". ";
					Logger().warning(info);
					throw new BackingStoreException(info);
				}
				String javaName = ToJavaName(windowsName);
				children[i] = javaName;
			}
			CloseKey(nativeHandle);
			return children;
		}

		/// <summary>
		/// Implements <tt>Preferences</tt> <tt>flush()</tt> method.
		/// Flushes Windows registry changes to disk.
		/// Throws a BackingStoreException and logs a warning message if Windows
		/// registry is not available.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws BackingStoreException
		public override void Flush()
		{

			if (Removed)
			{
				Parent_Renamed.Flush();
				return;
			}
			if (!IsBackingStoreAvailable)
			{
				throw new BackingStoreException("flush(): Backing store not available.");
			}
			int nativeHandle = OpenKey(KEY_READ);
			if (nativeHandle == NULL_NATIVE_HANDLE)
			{
				throw new BackingStoreException("Could not open windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ".");
			}
			int result = WindowsRegFlushKey1(nativeHandle);
			if (result != ERROR_SUCCESS)
			{
				String info = "Could not flush windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ". Windows RegFlushKey(...) returned error code " + result + ".";
				Logger().warning(info);
				throw new BackingStoreException(info);
			}
			CloseKey(nativeHandle);
		}


		/// <summary>
		/// Implements <tt>Preferences</tt> <tt>sync()</tt> method.
		/// Flushes Windows registry changes to disk. Equivalent to flush(). </summary>
		/// <seealso cref= flush() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void sync() throws BackingStoreException
		public override void Sync()
		{
			if (Removed)
			{
				throw new IllegalStateException("Node has been removed");
			}
			Flush();
		}

		/// <summary>
		/// Implements <tt>AbstractPreferences</tt> <tt>childSpi()</tt> method.
		/// Constructs a child node with a
		/// given name and creates its underlying Windows registry node,
		/// if it does not exist.
		/// Logs a warning message, if Windows Registry is unavailable.
		/// </summary>
		protected internal override AbstractPreferences ChildSpi(String name)
		{
			return new WindowsPreferences(this, name);
		}

		/// <summary>
		/// Implements <tt>AbstractPreferences</tt> <tt>removeNodeSpi()</tt> method.
		/// Deletes underlying Windows registry node.
		/// Throws a BackingStoreException and logs a warning, if Windows registry
		/// is not available.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeNodeSpi() throws BackingStoreException
		public override void RemoveNodeSpi()
		{
			int parentNativeHandle = ((WindowsPreferences)Parent()).OpenKey(DELETE);
			if (parentNativeHandle == NULL_NATIVE_HANDLE)
			{
				throw new BackingStoreException("Could not open parent windows registry node of " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ".");
			}
			int result = WindowsRegDeleteKey(parentNativeHandle, ToWindowsName(Name()));
			if (result != ERROR_SUCCESS)
			{
				String info = "Could not delete windows registry node " + ByteArrayToString(WindowsAbsolutePath()) + " at root 0x" + RootNativeHandle().ToString("x") + ". Windows RegDeleteKeyEx(...) returned error code " + result + ".";
				Logger().warning(info);
				throw new BackingStoreException(info);
			}
			CloseKey(parentNativeHandle);
		}

		/// <summary>
		/// Converts value's or node's name from its byte array representation to
		/// java string. Two encodings, simple and altBase64 are used. See
		/// <seealso cref="#toWindowsName(String) toWindowsName()"/> for a detailed
		/// description of encoding conventions. </summary>
		/// <param name="windowsNameArray"> Null-terminated byte array. </param>
		private static String ToJavaName(sbyte[] windowsNameArray)
		{
			String windowsName = ByteArrayToString(windowsNameArray);
			// check if Alt64
			if ((windowsName.Length() > 1) && (windowsName.Substring(0, 2).Equals("/!")))
			{
				return ToJavaAlt64Name(windowsName);
			}
			StringBuilder javaName = new StringBuilder();
			char ch;
			// Decode from simple encoding
			for (int i = 0; i < windowsName.Length(); i++)
			{
				if ((ch = windowsName.CharAt(i)) == '/')
				{
					char next = ' ';
					if ((windowsName.Length() > i + 1) && ((next = windowsName.CharAt(i + 1)) >= 'A') && (next <= 'Z'))
					{
						ch = next;
						i++;
					}
					else if ((windowsName.Length() > i + 1) && (next == '/'))
					{
						ch = '\\';
						i++;
					}
				}
				else if (ch == '\\')
				{
					ch = '/';
				}
				javaName.Append(ch);
			}
			return javaName.ToString();
		}

		/// <summary>
		/// Converts value's or node's name from its Windows representation to java
		/// string, using altBase64 encoding. See
		/// <seealso cref="#toWindowsName(String) toWindowsName()"/> for a detailed
		/// description of encoding conventions.
		/// </summary>

		private static String ToJavaAlt64Name(String windowsName)
		{
			sbyte[] byteBuffer = Base64.AltBase64ToByteArray(windowsName.Substring(2));
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < byteBuffer.Length; i++)
			{
				int firstbyte = (byteBuffer[i++] & 0xff);
				int secondbyte = (byteBuffer[i] & 0xff);
				result.Append((char)((firstbyte << 8) + secondbyte));
			}
			return result.ToString();
		}

		/// <summary>
		/// Converts value's or node's name to its Windows representation
		/// as a byte-encoded string.
		/// Two encodings, simple and altBase64 are used.
		/// <para>
		/// <i>Simple</i> encoding is used, if java string does not contain
		/// any characters less, than 0x0020, or greater, than 0x007f.
		/// Simple encoding adds "/" character to capital letters, i.e.
		/// "A" is encoded as "/A". Character '\' is encoded as '//',
		/// '/' is encoded as '\'.
		/// The constructed string is converted to byte array by truncating the
		/// highest byte and adding the terminating <tt>null</tt> character.
		/// </para>
		/// <para>
		/// <i>altBase64</i>  encoding is used, if java string does contain at least
		/// one character less, than 0x0020, or greater, than 0x007f.
		/// This encoding is marked by setting first two bytes of the
		/// Windows string to '/!'. The java name is then encoded using
		/// byteArrayToAltBase64() method from
		/// Base64 class.
		/// </para>
		/// </summary>
		private static sbyte[] ToWindowsName(String javaName)
		{
			StringBuilder windowsName = new StringBuilder();
			for (int i = 0; i < javaName.Length(); i++)
			{
				char ch = javaName.CharAt(i);
				if ((ch < 0x0020) || (ch > 0x007f))
				{
					// If a non-trivial character encountered, use altBase64
					return ToWindowsAlt64Name(javaName);
				}
				if (ch == '\\')
				{
					windowsName.Append("//");
				}
				else if (ch == '/')
				{
					windowsName.Append('\\');
				}
				else if ((ch >= 'A') && (ch <= 'Z'))
				{
					windowsName.Append('/').Append(ch);
				}
				else
				{
					windowsName.Append(ch);
				}
			}
			return StringToByteArray(windowsName.ToString());
		}

		/// <summary>
		/// Converts value's or node's name to its Windows representation
		/// as a byte-encoded string, using altBase64 encoding. See
		/// <seealso cref="#toWindowsName(String) toWindowsName()"/> for a detailed
		/// description of encoding conventions.
		/// </summary>
		private static sbyte[] ToWindowsAlt64Name(String javaName)
		{
			sbyte[] javaNameArray = new sbyte[2 * javaName.Length()];
			// Convert to byte pairs
			int counter = 0;
			for (int i = 0; i < javaName.Length();i++)
			{
				int ch = javaName.CharAt(i);
				javaNameArray[counter++] = (sbyte)((int)((uint)ch >> 8));
				javaNameArray[counter++] = (sbyte)ch;
			}

			return StringToByteArray("/!" + Base64.ByteArrayToAltBase64(javaNameArray));
		}

		/// <summary>
		/// Converts value string from its Windows representation
		/// to java string.  See
		/// <seealso cref="#toWindowsValueString(String) toWindowsValueString()"/> for the
		/// description of the encoding algorithm.
		/// </summary>
		 private static String ToJavaValueString(sbyte[] windowsNameArray)
		 {
			// Use modified native2ascii algorithm
			String windowsName = ByteArrayToString(windowsNameArray);
			StringBuilder javaName = new StringBuilder();
			char ch;
			for (int i = 0; i < windowsName.Length(); i++)
			{
				if ((ch = windowsName.CharAt(i)) == '/')
				{
					char next = ' ';

					if (windowsName.Length() > i + 1 && (next = windowsName.CharAt(i + 1)) == 'u')
					{
						if (windowsName.Length() < i + 6)
						{
							break;
						}
						else
						{
							ch = (char)((char)Convert.ToInt32(StringHelperClass.SubstringSpecial(windowsName, i + 2, i + 6), 16));
							i += 5;
						}
					}
					else
					{
					if ((windowsName.Length() > i + 1) && ((windowsName.CharAt(i + 1)) >= 'A') && (next <= 'Z'))
					{
						ch = next;
						i++;
					}
					else if ((windowsName.Length() > i + 1) && (next == '/'))
					{
						ch = '\\';
						i++;
					}
					}
				}
				else if (ch == '\\')
				{
					ch = '/';
				}
				javaName.Append(ch);
			}
			return javaName.ToString();
		 }

		/// <summary>
		/// Converts value string to it Windows representation.
		/// as a byte-encoded string.
		/// Encoding algorithm adds "/" character to capital letters, i.e.
		/// "A" is encoded as "/A". Character '\' is encoded as '//',
		/// '/' is encoded as  '\'.
		/// Then encoding scheme similar to jdk's native2ascii converter is used
		/// to convert java string to a byte array of ASCII characters.
		/// </summary>
		private static sbyte[] ToWindowsValueString(String javaName)
		{
			StringBuilder windowsName = new StringBuilder();
			for (int i = 0; i < javaName.Length(); i++)
			{
				char ch = javaName.CharAt(i);
				if ((ch < 0x0020) || (ch > 0x007f))
				{
					// write \udddd
					windowsName.Append("/u");
					String hex = javaName.CharAt(i).ToString("x");
					StringBuilder hex4 = new StringBuilder(hex);
					hex4.Reverse();
					int len = 4 - hex4.Length();
					for (int j = 0; j < len; j++)
					{
						hex4.Append('0');
					}
					for (int j = 0; j < 4; j++)
					{
						windowsName.Append(hex4.CharAt(3 - j));
					}
				}
				else if (ch == '\\')
				{
					windowsName.Append("//");
				}
				else if (ch == '/')
				{
					windowsName.Append('\\');
				}
				else if ((ch >= 'A') && (ch <= 'Z'))
				{
					windowsName.Append('/').Append(ch);
				}
				else
				{
					windowsName.Append(ch);
				}
			}
			return StringToByteArray(windowsName.ToString());
		}

		/// <summary>
		/// Returns native handle for the top Windows node for this node.
		/// </summary>
		private int RootNativeHandle()
		{
			return (UserNode ? USER_ROOT_NATIVE_HANDLE : SYSTEM_ROOT_NATIVE_HANDLE);
		}

		/// <summary>
		/// Returns this java string as a null-terminated byte array
		/// </summary>
		private static sbyte[] StringToByteArray(String str)
		{
			sbyte[] result = new sbyte[str.Length() + 1];
			for (int i = 0; i < str.Length(); i++)
			{
				result[i] = (sbyte) str.CharAt(i);
			}
			result[str.Length()] = 0;
			return result;
		}

		/// <summary>
		/// Converts a null-terminated byte array to java string
		/// </summary>
		private static String ByteArrayToString(sbyte[] array)
		{
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < array.Length - 1; i++)
			{
				result.Append((char)array[i]);
			}
			return result.ToString();
		}

	   /// <summary>
	   /// Empty, never used implementation  of AbstractPreferences.flushSpi().
	   /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void flushSpi() throws BackingStoreException
		protected internal override void FlushSpi()
		{
			// assert false;
		}

	   /// <summary>
	   /// Empty, never used implementation  of AbstractPreferences.flushSpi().
	   /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void syncSpi() throws BackingStoreException
		protected internal override void SyncSpi()
		{
			// assert false;
		}

		private static PlatformLogger Logger()
		{
			lock (typeof(WindowsPreferences))
			{
				if (Logger_Renamed == null)
				{
					Logger_Renamed = PlatformLogger.getLogger("java.util.prefs");
				}
				return Logger_Renamed;
			}
		}
	}

}