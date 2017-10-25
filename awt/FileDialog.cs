using System.Runtime.InteropServices;

/*
 * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

	using AWTAccessor = sun.awt.AWTAccessor;

	/// <summary>
	/// The <code>FileDialog</code> class displays a dialog window
	/// from which the user can select a file.
	/// <para>
	/// Since it is a modal dialog, when the application calls
	/// its <code>show</code> method to display the dialog,
	/// it blocks the rest of the application until the user has
	/// chosen a file.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= Window#show
	/// 
	/// @author      Sami Shaio
	/// @author      Arthur van Hoff
	/// @since       JDK1.0 </seealso>
	public class FileDialog : Dialog
	{

		/// <summary>
		/// This constant value indicates that the purpose of the file
		/// dialog window is to locate a file from which to read.
		/// </summary>
		public const int LOAD = 0;

		/// <summary>
		/// This constant value indicates that the purpose of the file
		/// dialog window is to locate a file to which to write.
		/// </summary>
		public const int SAVE = 1;

		/*
		 * There are two <code>FileDialog</code> modes: <code>LOAD</code> and
		 * <code>SAVE</code>.
		 * This integer will represent one or the other.
		 * If the mode is not specified it will default to <code>LOAD</code>.
		 *
		 * @serial
		 * @see getMode()
		 * @see setMode()
		 * @see java.awt.FileDialog#LOAD
		 * @see java.awt.FileDialog#SAVE
		 */
		internal int Mode_Renamed;

		/*
		 * The string specifying the directory to display
		 * in the file dialog.  This variable may be <code>null</code>.
		 *
		 * @serial
		 * @see getDirectory()
		 * @see setDirectory()
		 */
		internal String Dir;

		/*
		 * The string specifying the initial value of the
		 * filename text field in the file dialog.
		 * This variable may be <code>null</code>.
		 *
		 * @serial
		 * @see getFile()
		 * @see setFile()
		 */
		internal String File_Renamed;

		/// <summary>
		/// Contains the File instances for all the files that the user selects.
		/// 
		/// @serial </summary>
		/// <seealso cref= #getFiles
		/// @since 1.7 </seealso>
		private File[] Files_Renamed;

		/// <summary>
		/// Represents whether the file dialog allows the multiple file selection.
		/// 
		/// @serial </summary>
		/// <seealso cref= #setMultipleMode </seealso>
		/// <seealso cref= #isMultipleMode
		/// @since 1.7 </seealso>
		private bool MultipleMode_Renamed = false;

		/*
		 * The filter used as the file dialog's filename filter.
		 * The file dialog will only be displaying files whose
		 * names are accepted by this filter.
		 * This variable may be <code>null</code>.
		 *
		 * @serial
		 * @see #getFilenameFilter()
		 * @see #setFilenameFilter()
		 * @see FileNameFilter
		 */
		internal FilenameFilter Filter;

		private const String @base = "filedlg";
		private static int NameCounter = 0;

		/*
		 * JDK 1.1 serialVersionUID
		 */
		 private const long SerialVersionUID = 5035145889651310422L;


		static FileDialog()
		{
			/* ensure that the necessary native libraries are loaded */
			Toolkit.LoadLibraries();
			if (!GraphicsEnvironment.Headless)
			{
				initIDs();
			}
			AWTAccessor.FileDialogAccessor = new FileDialogAccessorAnonymousInnerClassHelper();
		}

		private class FileDialogAccessorAnonymousInnerClassHelper : AWTAccessor.FileDialogAccessor
		{
			public FileDialogAccessorAnonymousInnerClassHelper()
			{
			}

			public virtual void SetFiles(FileDialog fileDialog, File[] files)
			{
				fileDialog.Files = files;
			}
			public virtual void SetFile(FileDialog fileDialog, String file)
			{
				fileDialog.File_Renamed = ("".Equals(file)) ? null : file;
			}
			public virtual void SetDirectory(FileDialog fileDialog, String directory)
			{
				fileDialog.Dir = ("".Equals(directory)) ? null : directory;
			}
			public virtual bool IsMultipleMode(FileDialog fileDialog)
			{
				lock (fileDialog.ObjectLock)
				{
					return fileDialog.MultipleMode_Renamed;
				}
			}
		}


		/// <summary>
		/// Initialize JNI field and method IDs for fields that may be
		///   accessed from C.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		[DllImport("unknown")]
		private static extern void initIDs();

		/// <summary>
		/// Creates a file dialog for loading a file.  The title of the
		/// file dialog is initially empty.  This is a convenience method for
		/// <code>FileDialog(parent, "", LOAD)</code>.
		/// </summary>
		/// <param name="parent"> the owner of the dialog
		/// @since JDK1.1 </param>
		public FileDialog(Frame parent) : this(parent, "", LOAD)
		{
		}

		/// <summary>
		/// Creates a file dialog window with the specified title for loading
		/// a file. The files shown are those in the current directory.
		/// This is a convenience method for
		/// <code>FileDialog(parent, title, LOAD)</code>.
		/// </summary>
		/// <param name="parent">   the owner of the dialog </param>
		/// <param name="title">    the title of the dialog </param>
		public FileDialog(Frame parent, String title) : this(parent, title, LOAD)
		{
		}

		/// <summary>
		/// Creates a file dialog window with the specified title for loading
		/// or saving a file.
		/// <para>
		/// If the value of <code>mode</code> is <code>LOAD</code>, then the
		/// file dialog is finding a file to read, and the files shown are those
		/// in the current directory.   If the value of
		/// <code>mode</code> is <code>SAVE</code>, the file dialog is finding
		/// a place to write a file.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent">   the owner of the dialog </param>
		/// <param name="title">   the title of the dialog </param>
		/// <param name="mode">   the mode of the dialog; either
		///          <code>FileDialog.LOAD</code> or <code>FileDialog.SAVE</code> </param>
		/// <exception cref="IllegalArgumentException"> if an illegal file
		///                 dialog mode is supplied </exception>
		/// <seealso cref=       java.awt.FileDialog#LOAD </seealso>
		/// <seealso cref=       java.awt.FileDialog#SAVE </seealso>
		public FileDialog(Frame parent, String title, int mode) : base(parent, title, true)
		{
			this.Mode = mode;
			Layout = null;
		}

		/// <summary>
		/// Creates a file dialog for loading a file.  The title of the
		/// file dialog is initially empty.  This is a convenience method for
		/// <code>FileDialog(parent, "", LOAD)</code>.
		/// </summary>
		/// <param name="parent">   the owner of the dialog </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>parent</code>'s
		///            <code>GraphicsConfiguration</code>
		///            is not from a screen device; </exception>
		/// <exception cref="java.lang.IllegalArgumentException"> if <code>parent</code>
		///            is <code>null</code>; this exception is always thrown when
		///            <code>GraphicsEnvironment.isHeadless</code>
		///            returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since 1.5 </seealso>
		public FileDialog(Dialog parent) : this(parent, "", LOAD)
		{
		}

		/// <summary>
		/// Creates a file dialog window with the specified title for loading
		/// a file. The files shown are those in the current directory.
		/// This is a convenience method for
		/// <code>FileDialog(parent, title, LOAD)</code>.
		/// </summary>
		/// <param name="parent">   the owner of the dialog </param>
		/// <param name="title">    the title of the dialog; a <code>null</code> value
		///                     will be accepted without causing a
		///                     <code>NullPointerException</code> to be thrown </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>parent</code>'s
		///            <code>GraphicsConfiguration</code>
		///            is not from a screen device; </exception>
		/// <exception cref="java.lang.IllegalArgumentException"> if <code>parent</code>
		///            is <code>null</code>; this exception is always thrown when
		///            <code>GraphicsEnvironment.isHeadless</code>
		///            returns <code>true</code> </exception>
		/// <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		/// @since     1.5 </seealso>
		public FileDialog(Dialog parent, String title) : this(parent, title, LOAD)
		{
		}

		/// <summary>
		/// Creates a file dialog window with the specified title for loading
		/// or saving a file.
		/// <para>
		/// If the value of <code>mode</code> is <code>LOAD</code>, then the
		/// file dialog is finding a file to read, and the files shown are those
		/// in the current directory.   If the value of
		/// <code>mode</code> is <code>SAVE</code>, the file dialog is finding
		/// a place to write a file.
		/// 
		/// </para>
		/// </summary>
		/// <param name="parent">   the owner of the dialog </param>
		/// <param name="title">    the title of the dialog; a <code>null</code> value
		///                     will be accepted without causing a
		///                     <code>NullPointerException</code> to be thrown </param>
		/// <param name="mode">     the mode of the dialog; either
		///                     <code>FileDialog.LOAD</code> or <code>FileDialog.SAVE</code> </param>
		/// <exception cref="java.lang.IllegalArgumentException"> if an illegal
		///            file dialog mode is supplied; </exception>
		/// <exception cref="java.lang.IllegalArgumentException"> if the <code>parent</code>'s
		///            <code>GraphicsConfiguration</code>
		///            is not from a screen device; </exception>
		/// <exception cref="java.lang.IllegalArgumentException"> if <code>parent</code>
		///            is <code>null</code>; this exception is always thrown when
		///            <code>GraphicsEnvironment.isHeadless</code>
		///            returns <code>true</code> </exception>
		/// <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		/// <seealso cref=       java.awt.FileDialog#LOAD </seealso>
		/// <seealso cref=       java.awt.FileDialog#SAVE
		/// @since     1.5 </seealso>
		public FileDialog(Dialog parent, String title, int mode) : base(parent, title, true)
		{
			this.Mode = mode;
			Layout = null;
		}

		/// <summary>
		/// Constructs a name for this component. Called by <code>getName()</code>
		/// when the name is <code>null</code>.
		/// </summary>
		internal override String ConstructComponentName()
		{
			lock (typeof(FileDialog))
			{
				return @base + NameCounter++;
			}
		}

		/// <summary>
		/// Creates the file dialog's peer.  The peer allows us to change the look
		/// of the file dialog without changing its functionality.
		/// </summary>
		public override void AddNotify()
		{
			lock (TreeLock)
			{
				if (Parent_Renamed != null && Parent_Renamed.Peer == null)
				{
					Parent_Renamed.AddNotify();
				}
				if (Peer_Renamed == null)
				{
					Peer_Renamed = Toolkit.CreateFileDialog(this);
				}
				base.AddNotify();
			}
		}

		/// <summary>
		/// Indicates whether this file dialog box is for loading from a file
		/// or for saving to a file.
		/// </summary>
		/// <returns>   the mode of this file dialog window, either
		///               <code>FileDialog.LOAD</code> or
		///               <code>FileDialog.SAVE</code> </returns>
		/// <seealso cref=      java.awt.FileDialog#LOAD </seealso>
		/// <seealso cref=      java.awt.FileDialog#SAVE </seealso>
		/// <seealso cref=      java.awt.FileDialog#setMode </seealso>
		public virtual int Mode
		{
			get
			{
				return Mode_Renamed;
			}
			set
			{
				switch (value)
				{
				  case LOAD:
				  case SAVE:
					this.Mode_Renamed = value;
					break;
				  default:
					throw new IllegalArgumentException("illegal file dialog mode");
				}
			}
		}


		/// <summary>
		/// Gets the directory of this file dialog.
		/// </summary>
		/// <returns>  the (potentially <code>null</code> or invalid)
		///          directory of this <code>FileDialog</code> </returns>
		/// <seealso cref=       java.awt.FileDialog#setDirectory </seealso>
		public virtual String Directory
		{
			get
			{
				return Dir;
			}
			set
			{
				this.Dir = (value != null && value.Equals("")) ? null : value;
				FileDialogPeer peer = (FileDialogPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.Directory = this.Dir;
				}
			}
		}


		/// <summary>
		/// Gets the selected file of this file dialog.  If the user
		/// selected <code>CANCEL</code>, the returned file is <code>null</code>.
		/// </summary>
		/// <returns>    the currently selected file of this file dialog window,
		///                or <code>null</code> if none is selected </returns>
		/// <seealso cref=       java.awt.FileDialog#setFile </seealso>
		public virtual String File
		{
			get
			{
				return File_Renamed;
			}
			set
			{
				this.File_Renamed = (value != null && value.Equals("")) ? null : value;
				FileDialogPeer peer = (FileDialogPeer)this.Peer_Renamed;
				if (peer != null)
				{
					peer.File = this.File_Renamed;
				}
			}
		}

		/// <summary>
		/// Returns files that the user selects.
		/// <para>
		/// If the user cancels the file dialog,
		/// then the method returns an empty array.
		/// 
		/// </para>
		/// </summary>
		/// <returns>    files that the user selects or an empty array
		///            if the user cancels the file dialog. </returns>
		/// <seealso cref=       #setFile(String) </seealso>
		/// <seealso cref=       #getFile
		/// @since 1.7 </seealso>
		public virtual File[] Files
		{
			get
			{
				lock (ObjectLock)
				{
					if (Files_Renamed != null)
					{
						return Files_Renamed.clone();
					}
					else
					{
						return new File[0];
					}
				}
			}
			set
			{
				lock (ObjectLock)
				{
					this.Files_Renamed = value;
				}
			}
		}



		/// <summary>
		/// Enables or disables multiple file selection for the file dialog.
		/// </summary>
		/// <param name="enable">    if {@code true}, multiple file selection is enabled;
		///                  {@code false} - disabled. </param>
		/// <seealso cref= #isMultipleMode
		/// @since 1.7 </seealso>
		public virtual bool MultipleMode
		{
			set
			{
				lock (ObjectLock)
				{
					this.MultipleMode_Renamed = value;
				}
			}
			get
			{
				lock (ObjectLock)
				{
					return MultipleMode_Renamed;
				}
			}
		}


		/// <summary>
		/// Determines this file dialog's filename filter. A filename filter
		/// allows the user to specify which files appear in the file dialog
		/// window.  Filename filters do not function in Sun's reference
		/// implementation for Microsoft Windows.
		/// </summary>
		/// <returns>    this file dialog's filename filter </returns>
		/// <seealso cref=       java.io.FilenameFilter </seealso>
		/// <seealso cref=       java.awt.FileDialog#setFilenameFilter </seealso>
		public virtual FilenameFilter FilenameFilter
		{
			get
			{
				return Filter;
			}
			set
			{
				lock (this)
				{
					this.Filter = value;
					FileDialogPeer peer = (FileDialogPeer)this.Peer_Renamed;
					if (peer != null)
					{
						peer.FilenameFilter = value;
					}
				}
			}
		}


		/// <summary>
		/// Reads the <code>ObjectInputStream</code> and performs
		/// a backwards compatibility check by converting
		/// either a <code>dir</code> or a <code>file</code>
		/// equal to an empty string to <code>null</code>.
		/// </summary>
		/// <param name="s"> the <code>ObjectInputStream</code> to read </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
		private void ReadObject(ObjectInputStream s)
		{
			s.DefaultReadObject();

			// 1.1 Compatibility: "" is not converted to null in 1.1
			if (Dir != null && Dir.Equals(""))
			{
				Dir = null;
			}
			if (File_Renamed != null && File_Renamed.Equals(""))
			{
				File_Renamed = null;
			}
		}

		/// <summary>
		/// Returns a string representing the state of this <code>FileDialog</code>
		/// window. This method is intended to be used only for debugging purposes,
		/// and the content and format of the returned string may vary between
		/// implementations. The returned string may be empty but may not be
		/// <code>null</code>.
		/// </summary>
		/// <returns>  the parameter string of this file dialog window </returns>
		protected internal override String ParamString()
		{
			String str = base.ParamString();
			str += ",dir= " + Dir;
			str += ",file= " + File_Renamed;
			return str + ((Mode_Renamed == LOAD) ? ",load" : ",save");
		}

		internal override bool PostsOldMouseEvents()
		{
			return false;
		}
	}

}