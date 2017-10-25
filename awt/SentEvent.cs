using System.Threading;

/*
 * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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
	using SunToolkit = sun.awt.SunToolkit;

	/// <summary>
	/// A wrapping tag for a nested AWTEvent which indicates that the event was
	/// sent from another AppContext. The destination AppContext should handle the
	/// event even if it is currently blocked waiting for a SequencedEvent or
	/// another SentEvent to be handled.
	/// 
	/// @author David Mendenhall
	/// </summary>
	internal class SentEvent : AWTEvent, ActiveEvent
	{
		/*
		 * serialVersionUID
		 */
		private const long SerialVersionUID = -383615247028828931L;

		internal static readonly int ID = java.awt.@event.FocusEvent.FOCUS_LAST + 2;

		internal bool Dispatched;
		private AWTEvent Nested;
		private AppContext ToNotify;

		internal SentEvent() : this(null)
		{
		}
		internal SentEvent(AWTEvent nested) : this(nested, null)
		{
		}
		internal SentEvent(AWTEvent nested, AppContext toNotify) : base((nested != null) ? nested.Source : Toolkit.DefaultToolkit, ID)
		{
			this.Nested = nested;
			this.ToNotify = toNotify;
		}

		public virtual void Dispatch()
		{
			try
			{
				if (Nested != null)
				{
					Toolkit.EventQueue.DispatchEvent(Nested);
				}
			}
			finally
			{
				Dispatched = true;
				if (ToNotify != null)
				{
					SunToolkit.postEvent(ToNotify, new SentEvent());
				}
				lock (this)
				{
					Monitor.PulseAll(this);
				}
			}
		}
		internal void Dispose()
		{
			Dispatched = true;
			if (ToNotify != null)
			{
				SunToolkit.postEvent(ToNotify, new SentEvent());
			}
			lock (this)
			{
				Monitor.PulseAll(this);
			}
		}
	}

}