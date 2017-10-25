using System;

/*
 * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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
	/// <para>
	/// The root class from which all event state objects shall be derived.
	/// </para>
	/// <para>
	/// All Events are constructed with a reference to the object, the "source",
	/// that is logically deemed to be the object upon which the Event in question
	/// initially occurred upon.
	/// 
	/// @since JDK1.1
	/// </para>
	/// </summary>

	[Serializable]
	public class EventObject
	{

		private const long SerialVersionUID = 5516075349620653480L;

		/// <summary>
		/// The object on which the Event initially occurred.
		/// </summary>
		[NonSerialized]
		protected internal Object Source_Renamed;

		/// <summary>
		/// Constructs a prototypical Event.
		/// </summary>
		/// <param name="source">    The object on which the Event initially occurred. </param>
		/// <exception cref="IllegalArgumentException">  if source is null. </exception>
		public EventObject(Object source)
		{
			if (source == null)
			{
				throw new IllegalArgumentException("null source");
			}

			this.Source_Renamed = source;
		}

		/// <summary>
		/// The object on which the Event initially occurred.
		/// </summary>
		/// <returns>   The object on which the Event initially occurred. </returns>
		public virtual Object Source
		{
			get
			{
				return Source_Renamed;
			}
		}

		/// <summary>
		/// Returns a String representation of this EventObject.
		/// </summary>
		/// <returns>  A a String representation of this EventObject. </returns>
		public override String ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[source=" + Source_Renamed + "]";
		}
	}

}