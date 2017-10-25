/*
 * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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

	internal abstract class ModalEventFilter : EventFilter
	{

		protected internal Dialog ModalDialog_Renamed;
		protected internal bool Disabled;

		protected internal ModalEventFilter(Dialog modalDialog)
		{
			this.ModalDialog_Renamed = modalDialog;
			Disabled = false;
		}

		internal virtual Dialog ModalDialog
		{
			get
			{
				return ModalDialog_Renamed;
			}
		}

		public virtual EventFilter_FilterAction AcceptEvent(AWTEvent @event)
		{
			if (Disabled || !ModalDialog_Renamed.Visible)
			{
				return EventFilter_FilterAction.ACCEPT;
			}
			int eventID = @event.ID;
			if ((eventID >= MouseEvent.MOUSE_FIRST && eventID <= MouseEvent.MOUSE_LAST) || (eventID >= ActionEvent.ACTION_FIRST && eventID <= ActionEvent.ACTION_LAST) || eventID == WindowEvent.WINDOW_CLOSING)
			{
				Object o = @event.Source;
				if (o is sun.awt.ModalExclude)
				{
					// Exclude this object from modality and
					// continue to pump it's events.
				}
				else if (o is Component)
				{
					Component c = (Component)o;
					while ((c != null) && !(c is Window))
					{
						c = c.Parent_NoClientCode;
					}
					if (c != null)
					{
						return AcceptWindow((Window)c);
					}
				}
			}
			return EventFilter_FilterAction.ACCEPT;
		}

		protected internal abstract EventFilter_FilterAction AcceptWindow(Window w);

		// When a modal dialog is hidden its modal filter may not be deleted from
		// EventDispatchThread event filters immediately, so we need to mark the filter
		// as disabled to prevent it from working. Simple checking for visibility of
		// the modalDialog is not enough, as it can be hidden and then shown again
		// with a new event pump and a new filter
		internal virtual void Disable()
		{
			Disabled = true;
		}

		internal virtual int CompareTo(ModalEventFilter another)
		{
			Dialog anotherDialog = another.ModalDialog;
			// check if modalDialog is from anotherDialog's hierarchy
			//   or vice versa
			Component c = ModalDialog_Renamed;
			while (c != null)
			{
				if (c == anotherDialog)
				{
					return 1;
				}
				c = c.Parent_NoClientCode;
			}
			c = anotherDialog;
			while (c != null)
			{
				if (c == ModalDialog_Renamed)
				{
					return -1;
				}
				c = c.Parent_NoClientCode;
			}
			// check if one dialog blocks (directly or indirectly) another
			Dialog blocker = ModalDialog_Renamed.ModalBlocker;
			while (blocker != null)
			{
				if (blocker == anotherDialog)
				{
					return -1;
				}
				blocker = blocker.ModalBlocker;
			}
			blocker = anotherDialog.ModalBlocker;
			while (blocker != null)
			{
				if (blocker == ModalDialog_Renamed)
				{
					return 1;
				}
				blocker = blocker.ModalBlocker;
			}
			// compare modality types
			return ModalDialog_Renamed.ModalityType.CompareTo(anotherDialog.ModalityType);
		}

		internal static ModalEventFilter CreateFilterForDialog(Dialog modalDialog)
		{
			switch (modalDialog.ModalityType)
			{
				case DOCUMENT_MODAL:
					return new DocumentModalEventFilter(modalDialog);
				case APPLICATION_MODAL:
					return new ApplicationModalEventFilter(modalDialog);
				case TOOLKIT_MODAL:
					return new ToolkitModalEventFilter(modalDialog);
			}
			return null;
		}

		private class ToolkitModalEventFilter : ModalEventFilter
		{

			internal AppContext AppContext;

			internal ToolkitModalEventFilter(Dialog modalDialog) : base(modalDialog)
			{
				AppContext = modalDialog.AppContext;
			}

			protected internal override EventFilter_FilterAction AcceptWindow(Window w)
			{
				if (w.IsModalExcluded(Dialog.ModalExclusionType.TOOLKIT_EXCLUDE))
				{
					return EventFilter_FilterAction.ACCEPT;
				}
				if (w.AppContext != AppContext)
				{
					return EventFilter_FilterAction.REJECT;
				}
				while (w != null)
				{
					if (w == ModalDialog_Renamed)
					{
						return EventFilter_FilterAction.ACCEPT_IMMEDIATELY;
					}
					w = w.Owner;
				}
				return EventFilter_FilterAction.REJECT;
			}
		}

		private class ApplicationModalEventFilter : ModalEventFilter
		{

			internal AppContext AppContext;

			internal ApplicationModalEventFilter(Dialog modalDialog) : base(modalDialog)
			{
				AppContext = modalDialog.AppContext;
			}

			protected internal override EventFilter_FilterAction AcceptWindow(Window w)
			{
				if (w.IsModalExcluded(Dialog.ModalExclusionType.APPLICATION_EXCLUDE))
				{
					return EventFilter_FilterAction.ACCEPT;
				}
				if (w.AppContext == AppContext)
				{
					while (w != null)
					{
						if (w == ModalDialog_Renamed)
						{
							return EventFilter_FilterAction.ACCEPT_IMMEDIATELY;
						}
						w = w.Owner;
					}
					return EventFilter_FilterAction.REJECT;
				}
				return EventFilter_FilterAction.ACCEPT;
			}
		}

		private class DocumentModalEventFilter : ModalEventFilter
		{

			internal Window DocumentRoot;

			internal DocumentModalEventFilter(Dialog modalDialog) : base(modalDialog)
			{
				DocumentRoot = modalDialog.DocumentRoot;
			}

			protected internal override EventFilter_FilterAction AcceptWindow(Window w)
			{
				// application- and toolkit-excluded windows are blocked by
				// document-modal dialogs from their child hierarchy
				if (w.IsModalExcluded(Dialog.ModalExclusionType.APPLICATION_EXCLUDE))
				{
					Window w1 = ModalDialog_Renamed.Owner;
					while (w1 != null)
					{
						if (w1 == w)
						{
							return EventFilter_FilterAction.REJECT;
						}
						w1 = w1.Owner;
					}
					return EventFilter_FilterAction.ACCEPT;
				}
				while (w != null)
				{
					if (w == ModalDialog_Renamed)
					{
						return EventFilter_FilterAction.ACCEPT_IMMEDIATELY;
					}
					if (w == DocumentRoot)
					{
						return EventFilter_FilterAction.REJECT;
					}
					w = w.Owner;
				}
				return EventFilter_FilterAction.ACCEPT;
			}
		}
	}

}