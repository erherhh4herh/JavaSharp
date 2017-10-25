using System.Threading;

/*
 * Copyright (c) 2006, 2012, Oracle and/or its affiliates. All rights reserved.
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

namespace java.io
{

	/// <summary>
	/// Context during upcalls from object stream to class-defined
	/// readObject/writeObject methods.
	/// Holds object currently being deserialized and descriptor for current class.
	/// 
	/// This context keeps track of the thread it was constructed on, and allows
	/// only a single call of defaultReadObject, readFields, defaultWriteObject
	/// or writeFields which must be invoked on the same thread before the class's
	/// readObject/writeObject method has returned.
	/// If not set to the current thread, the getObj method throws NotActiveException.
	/// </summary>
	internal sealed class SerialCallbackContext
	{
		private readonly Object Obj_Renamed;
		private readonly ObjectStreamClass Desc_Renamed;
		/// <summary>
		/// Thread this context is in use by.
		/// As this only works in one thread, we do not need to worry about thread-safety.
		/// </summary>
		private Thread Thread;

		public SerialCallbackContext(Object obj, ObjectStreamClass desc)
		{
			this.Obj_Renamed = obj;
			this.Desc_Renamed = desc;
			this.Thread = Thread.CurrentThread;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getObj() throws NotActiveException
		public Object Obj
		{
			get
			{
				CheckAndSetUsed();
				return Obj_Renamed;
			}
		}

		public ObjectStreamClass Desc
		{
			get
			{
				return Desc_Renamed;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void check() throws NotActiveException
		public void Check()
		{
			if (Thread != null && Thread != Thread.CurrentThread)
			{
				throw new NotActiveException("expected thread: " + Thread + ", but got: " + Thread.CurrentThread);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkAndSetUsed() throws NotActiveException
		private void CheckAndSetUsed()
		{
			if (Thread != Thread.CurrentThread)
			{
				 throw new NotActiveException("not in readObject invocation or fields already read");
			}
			Thread = null;
		}

		public void SetUsed()
		{
			Thread = null;
		}
	}

}