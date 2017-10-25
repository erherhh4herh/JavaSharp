/*
 * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.instrument
{

	/*
	 * Copyright 2003 Wily Technology, Inc.
	 */

	/// <summary>
	/// This class serves as a parameter block to the <code>Instrumentation.redefineClasses</code> method.
	/// Serves to bind the <code>Class</code> that needs redefining together with the new class file bytes.
	/// </summary>
	/// <seealso cref=     java.lang.instrument.Instrumentation#redefineClasses
	/// @since   1.5 </seealso>
	public sealed class ClassDefinition
	{
		/// <summary>
		///  The class to redefine
		/// </summary>
		private readonly Class MClass;

		/// <summary>
		///  The replacement class file bytes
		/// </summary>
		private readonly sbyte[] MClassFile;

		/// <summary>
		///  Creates a new <code>ClassDefinition</code> binding using the supplied
		///  class and class file bytes. Does not copy the supplied buffer, just captures a reference to it.
		/// </summary>
		/// <param name="theClass"> the <code>Class</code> that needs redefining </param>
		/// <param name="theClassFile"> the new class file bytes
		/// </param>
		/// <exception cref="java.lang.NullPointerException"> if the supplied class or array is <code>null</code>. </exception>
		public ClassDefinition(Class theClass, sbyte[] theClassFile)
		{
			if (theClass == null || theClassFile == null)
			{
				throw new NullPointerException();
			}
			MClass = theClass;
			MClassFile = theClassFile;
		}

		/// <summary>
		/// Returns the class.
		/// </summary>
		/// <returns>    the <code>Class</code> object referred to. </returns>
		public Class DefinitionClass
		{
			get
			{
				return MClass;
			}
		}

		/// <summary>
		/// Returns the array of bytes that contains the new class file.
		/// </summary>
		/// <returns>    the class file bytes. </returns>
		public sbyte[] DefinitionClassFile
		{
			get
			{
				return MClassFile;
			}
		}
	}

}