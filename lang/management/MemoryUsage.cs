/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace java.lang.management
{

	using MemoryUsageCompositeData = sun.management.MemoryUsageCompositeData;

	/// <summary>
	/// A <tt>MemoryUsage</tt> object represents a snapshot of memory usage.
	/// Instances of the <tt>MemoryUsage</tt> class are usually constructed
	/// by methods that are used to obtain memory usage
	/// information about individual memory pool of the Java virtual machine or
	/// the heap or non-heap memory of the Java virtual machine as a whole.
	/// 
	/// <para> A <tt>MemoryUsage</tt> object contains four values:
	/// <table summary="Describes the MemoryUsage object content">
	/// <tr>
	/// <td valign=top> <tt>init</tt> </td>
	/// <td valign=top> represents the initial amount of memory (in bytes) that
	///      the Java virtual machine requests from the operating system
	///      for memory management during startup.  The Java virtual machine
	///      may request additional memory from the operating system and
	///      may also release memory to the system over time.
	///      The value of <tt>init</tt> may be undefined.
	/// </td>
	/// </tr>
	/// <tr>
	/// <td valign=top> <tt>used</tt> </td>
	/// <td valign=top> represents the amount of memory currently used (in bytes).
	/// </td>
	/// </tr>
	/// <tr>
	/// <td valign=top> <tt>committed</tt> </td>
	/// <td valign=top> represents the amount of memory (in bytes) that is
	///      guaranteed to be available for use by the Java virtual machine.
	///      The amount of committed memory may change over time (increase
	///      or decrease).  The Java virtual machine may release memory to
	///      the system and <tt>committed</tt> could be less than <tt>init</tt>.
	///      <tt>committed</tt> will always be greater than
	///      or equal to <tt>used</tt>.
	/// </td>
	/// </tr>
	/// <tr>
	/// <td valign=top> <tt>max</tt> </td>
	/// <td valign=top> represents the maximum amount of memory (in bytes)
	///      that can be used for memory management. Its value may be undefined.
	///      The maximum amount of memory may change over time if defined.
	///      The amount of used and committed memory will always be less than
	///      or equal to <tt>max</tt> if <tt>max</tt> is defined.
	///      A memory allocation may fail if it attempts to increase the
	///      used memory such that <tt>used &gt; committed</tt> even
	///      if <tt>used &lt;= max</tt> would still be true (for example,
	///      when the system is low on virtual memory).
	/// </td>
	/// </tr>
	/// </table>
	/// 
	/// Below is a picture showing an example of a memory pool:
	/// 
	/// <pre>
	///        +----------------------------------------------+
	///        +////////////////           |                  +
	///        +////////////////           |                  +
	///        +----------------------------------------------+
	/// 
	///        |--------|
	///           init
	///        |---------------|
	///               used
	///        |---------------------------|
	///                  committed
	///        |----------------------------------------------|
	///                            max
	/// </pre>
	/// 
	/// <h3>MXBean Mapping</h3>
	/// <tt>MemoryUsage</tt> is mapped to a <seealso cref="CompositeData CompositeData"/>
	/// with attributes as specified in the <seealso cref="#from from"/> method.
	/// 
	/// @author   Mandy Chung
	/// @since   1.5
	/// </para>
	/// </summary>
	public class MemoryUsage
	{
		private readonly long Init_Renamed;
		private readonly long Used_Renamed;
		private readonly long Committed_Renamed;
		private readonly long Max_Renamed;

		/// <summary>
		/// Constructs a <tt>MemoryUsage</tt> object.
		/// </summary>
		/// <param name="init">      the initial amount of memory in bytes that
		///                  the Java virtual machine allocates;
		///                  or <tt>-1</tt> if undefined. </param>
		/// <param name="used">      the amount of used memory in bytes. </param>
		/// <param name="committed"> the amount of committed memory in bytes. </param>
		/// <param name="max">       the maximum amount of memory in bytes that
		///                  can be used; or <tt>-1</tt> if undefined.
		/// </param>
		/// <exception cref="IllegalArgumentException"> if
		/// <ul>
		/// <li> the value of <tt>init</tt> or <tt>max</tt> is negative
		///      but not <tt>-1</tt>; or</li>
		/// <li> the value of <tt>used</tt> or <tt>committed</tt> is negative;
		///      or</li>
		/// <li> <tt>used</tt> is greater than the value of <tt>committed</tt>;
		///      or</li>
		/// <li> <tt>committed</tt> is greater than the value of <tt>max</tt>
		///      <tt>max</tt> if defined.</li>
		/// </ul> </exception>
		public MemoryUsage(long init, long used, long committed, long max)
		{
			if (init < -1)
			{
				throw new IllegalArgumentException("init parameter = " + init + " is negative but not -1.");
			}
			if (max < -1)
			{
				throw new IllegalArgumentException("max parameter = " + max + " is negative but not -1.");
			}
			if (used < 0)
			{
				throw new IllegalArgumentException("used parameter = " + used + " is negative.");
			}
			if (committed < 0)
			{
				throw new IllegalArgumentException("committed parameter = " + committed + " is negative.");
			}
			if (used > committed)
			{
				throw new IllegalArgumentException("used = " + used + " should be <= committed = " + committed);
			}
			if (max >= 0 && committed > max)
			{
				throw new IllegalArgumentException("committed = " + committed + " should be < max = " + max);
			}

			this.Init_Renamed = init;
			this.Used_Renamed = used;
			this.Committed_Renamed = committed;
			this.Max_Renamed = max;
		}

		/// <summary>
		/// Constructs a <tt>MemoryUsage</tt> object from a
		/// <seealso cref="CompositeData CompositeData"/>.
		/// </summary>
		private MemoryUsage(CompositeData cd)
		{
			// validate the input composite data
			MemoryUsageCompositeData.validateCompositeData(cd);

			this.Init_Renamed = MemoryUsageCompositeData.getInit(cd);
			this.Used_Renamed = MemoryUsageCompositeData.getUsed(cd);
			this.Committed_Renamed = MemoryUsageCompositeData.getCommitted(cd);
			this.Max_Renamed = MemoryUsageCompositeData.getMax(cd);
		}

		/// <summary>
		/// Returns the amount of memory in bytes that the Java virtual machine
		/// initially requests from the operating system for memory management.
		/// This method returns <tt>-1</tt> if the initial memory size is undefined.
		/// </summary>
		/// <returns> the initial size of memory in bytes;
		/// <tt>-1</tt> if undefined. </returns>
		public virtual long Init
		{
			get
			{
				return Init_Renamed;
			}
		}

		/// <summary>
		/// Returns the amount of used memory in bytes.
		/// </summary>
		/// <returns> the amount of used memory in bytes.
		///  </returns>
		public virtual long Used
		{
			get
			{
				return Used_Renamed;
			}
		};

		/// <summary>
		/// Returns the amount of memory in bytes that is committed for
		/// the Java virtual machine to use.  This amount of memory is
		/// guaranteed for the Java virtual machine to use.
		/// </summary>
		/// <returns> the amount of committed memory in bytes.
		///  </returns>
		public virtual long Committed
		{
			get
			{
				return Committed_Renamed;
			}
		};

		/// <summary>
		/// Returns the maximum amount of memory in bytes that can be
		/// used for memory management.  This method returns <tt>-1</tt>
		/// if the maximum memory size is undefined.
		/// 
		/// <para> This amount of memory is not guaranteed to be available
		/// for memory management if it is greater than the amount of
		/// committed memory.  The Java virtual machine may fail to allocate
		/// memory even if the amount of used memory does not exceed this
		/// maximum size.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the maximum amount of memory in bytes;
		/// <tt>-1</tt> if undefined. </returns>
		public virtual long Max
		{
			get
			{
				return Max_Renamed;
			}
		};

		/// <summary>
		/// Returns a descriptive representation of this memory usage.
		/// </summary>
		public override String ToString()
		{
			StringBuffer buf = new StringBuffer();
			buf.Append("init = " + Init_Renamed + "(" + (Init_Renamed >> 10) + "K) ");
			buf.Append("used = " + Used_Renamed + "(" + (Used_Renamed >> 10) + "K) ");
			buf.Append("committed = " + Committed_Renamed + "(" + (Committed_Renamed >> 10) + "K) ");
			buf.Append("max = " + Max_Renamed + "(" + (Max_Renamed >> 10) + "K)");
			return buf.ToString();
		}

		/// <summary>
		/// Returns a <tt>MemoryUsage</tt> object represented by the
		/// given <tt>CompositeData</tt>. The given <tt>CompositeData</tt>
		/// must contain the following attributes:
		/// 
		/// <blockquote>
		/// <table border summary="The attributes and the types the given CompositeData contains">
		/// <tr>
		///   <th align=left>Attribute Name</th>
		///   <th align=left>Type</th>
		/// </tr>
		/// <tr>
		///   <td>init</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// <tr>
		///   <td>used</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// <tr>
		///   <td>committed</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// <tr>
		///   <td>max</td>
		///   <td><tt>java.lang.Long</tt></td>
		/// </tr>
		/// </table>
		/// </blockquote>
		/// </summary>
		/// <param name="cd"> <tt>CompositeData</tt> representing a <tt>MemoryUsage</tt>
		/// </param>
		/// <exception cref="IllegalArgumentException"> if <tt>cd</tt> does not
		///   represent a <tt>MemoryUsage</tt> with the attributes described
		///   above.
		/// </exception>
		/// <returns> a <tt>MemoryUsage</tt> object represented by <tt>cd</tt>
		///         if <tt>cd</tt> is not <tt>null</tt>;
		///         <tt>null</tt> otherwise. </returns>
		public static MemoryUsage From(CompositeData cd)
		{
			if (cd == null)
			{
				return null;
			}

			if (cd is MemoryUsageCompositeData)
			{
				return ((MemoryUsageCompositeData) cd).MemoryUsage;
			}
			else
			{
				return new MemoryUsage(cd);
			}

		}
	}

}