using System;
using System.Threading;

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
namespace java.beans
{


	using MethodUtil = sun.reflect.misc.MethodUtil;
	using ReflectUtil = sun.reflect.misc.ReflectUtil;

	/// <summary>
	/// The <code>EventHandler</code> class provides
	/// support for dynamically generating event listeners whose methods
	/// execute a simple statement involving an incoming event object
	/// and a target object.
	/// <para>
	/// The <code>EventHandler</code> class is intended to be used by interactive tools, such as
	/// application builders, that allow developers to make connections between
	/// beans. Typically connections are made from a user interface bean
	/// (the event <em>source</em>)
	/// to an application logic bean (the <em>target</em>). The most effective
	/// connections of this kind isolate the application logic from the user
	/// interface.  For example, the <code>EventHandler</code> for a
	/// connection from a <code>JCheckBox</code> to a method
	/// that accepts a boolean value can deal with extracting the state
	/// of the check box and passing it directly to the method so that
	/// the method is isolated from the user interface layer.
	/// </para>
	/// <para>
	/// Inner classes are another, more general way to handle events from
	/// user interfaces.  The <code>EventHandler</code> class
	/// handles only a subset of what is possible using inner
	/// classes. However, <code>EventHandler</code> works better
	/// with the long-term persistence scheme than inner classes.
	/// Also, using <code>EventHandler</code> in large applications in
	/// which the same interface is implemented many times can
	/// reduce the disk and memory footprint of the application.
	/// </para>
	/// <para>
	/// The reason that listeners created with <code>EventHandler</code>
	/// have such a small
	/// footprint is that the <code>Proxy</code> class, on which
	/// the <code>EventHandler</code> relies, shares implementations
	/// of identical
	/// interfaces. For example, if you use
	/// the <code>EventHandler</code> <code>create</code> methods to make
	/// all the <code>ActionListener</code>s in an application,
	/// all the action listeners will be instances of a single class
	/// (one created by the <code>Proxy</code> class).
	/// In general, listeners based on
	/// the <code>Proxy</code> class require one listener class
	/// to be created per <em>listener type</em> (interface),
	/// whereas the inner class
	/// approach requires one class to be created per <em>listener</em>
	/// (object that implements the interface).
	/// 
	/// </para>
	/// <para>
	/// You don't generally deal directly with <code>EventHandler</code>
	/// instances.
	/// Instead, you use one of the <code>EventHandler</code>
	/// <code>create</code> methods to create
	/// an object that implements a given listener interface.
	/// This listener object uses an <code>EventHandler</code> object
	/// behind the scenes to encapsulate information about the
	/// event, the object to be sent a message when the event occurs,
	/// the message (method) to be sent, and any argument
	/// to the method.
	/// The following section gives examples of how to create listener
	/// objects using the <code>create</code> methods.
	/// 
	/// <h2>Examples of Using EventHandler</h2>
	/// 
	/// The simplest use of <code>EventHandler</code> is to install
	/// a listener that calls a method on the target object with no arguments.
	/// In the following example we create an <code>ActionListener</code>
	/// that invokes the <code>toFront</code> method on an instance
	/// of <code>javax.swing.JFrame</code>.
	/// 
	/// <blockquote>
	/// <pre>
	/// myButton.addActionListener(
	///    (ActionListener)EventHandler.create(ActionListener.class, frame, "toFront"));
	/// </pre>
	/// </blockquote>
	/// 
	/// When <code>myButton</code> is pressed, the statement
	/// <code>frame.toFront()</code> will be executed.  One could get
	/// the same effect, with some additional compile-time type safety,
	/// by defining a new implementation of the <code>ActionListener</code>
	/// interface and adding an instance of it to the button:
	/// 
	/// <blockquote>
	/// <pre>
	/// //Equivalent code using an inner class instead of EventHandler.
	/// myButton.addActionListener(new ActionListener() {
	///    public void actionPerformed(ActionEvent e) {
	///        frame.toFront();
	///    }
	/// });
	/// </pre>
	/// </blockquote>
	/// 
	/// The next simplest use of <code>EventHandler</code> is
	/// to extract a property value from the first argument
	/// of the method in the listener interface (typically an event object)
	/// and use it to set the value of a property in the target object.
	/// In the following example we create an <code>ActionListener</code> that
	/// sets the <code>nextFocusableComponent</code> property of the target
	/// (myButton) object to the value of the "source" property of the event.
	/// 
	/// <blockquote>
	/// <pre>
	/// EventHandler.create(ActionListener.class, myButton, "nextFocusableComponent", "source")
	/// </pre>
	/// </blockquote>
	/// 
	/// This would correspond to the following inner class implementation:
	/// 
	/// <blockquote>
	/// <pre>
	/// //Equivalent code using an inner class instead of EventHandler.
	/// new ActionListener() {
	///    public void actionPerformed(ActionEvent e) {
	///        myButton.setNextFocusableComponent((Component)e.getSource());
	///    }
	/// }
	/// </pre>
	/// </blockquote>
	/// 
	/// It's also possible to create an <code>EventHandler</code> that
	/// just passes the incoming event object to the target's action.
	/// If the fourth <code>EventHandler.create</code> argument is
	/// an empty string, then the event is just passed along:
	/// 
	/// <blockquote>
	/// <pre>
	/// EventHandler.create(ActionListener.class, target, "doActionEvent", "")
	/// </pre>
	/// </blockquote>
	/// 
	/// This would correspond to the following inner class implementation:
	/// 
	/// <blockquote>
	/// <pre>
	/// //Equivalent code using an inner class instead of EventHandler.
	/// new ActionListener() {
	///    public void actionPerformed(ActionEvent e) {
	///        target.doActionEvent(e);
	///    }
	/// }
	/// </pre>
	/// </blockquote>
	/// 
	/// Probably the most common use of <code>EventHandler</code>
	/// is to extract a property value from the
	/// <em>source</em> of the event object and set this value as
	/// the value of a property of the target object.
	/// In the following example we create an <code>ActionListener</code> that
	/// sets the "label" property of the target
	/// object to the value of the "text" property of the
	/// source (the value of the "source" property) of the event.
	/// 
	/// <blockquote>
	/// <pre>
	/// EventHandler.create(ActionListener.class, myButton, "label", "source.text")
	/// </pre>
	/// </blockquote>
	/// 
	/// This would correspond to the following inner class implementation:
	/// 
	/// <blockquote>
	/// <pre>
	/// //Equivalent code using an inner class instead of EventHandler.
	/// new ActionListener {
	///    public void actionPerformed(ActionEvent e) {
	///        myButton.setLabel(((JTextField)e.getSource()).getText());
	///    }
	/// }
	/// </pre>
	/// </blockquote>
	/// 
	/// The event property may be "qualified" with an arbitrary number
	/// of property prefixes delimited with the "." character. The "qualifying"
	/// names that appear before the "." characters are taken as the names of
	/// properties that should be applied, left-most first, to
	/// the event object.
	/// </para>
	/// <para>
	/// For example, the following action listener
	/// 
	/// <blockquote>
	/// <pre>
	/// EventHandler.create(ActionListener.class, target, "a", "b.c.d")
	/// </pre>
	/// </blockquote>
	/// 
	/// might be written as the following inner class
	/// (assuming all the properties had canonical getter methods and
	/// returned the appropriate types):
	/// 
	/// <blockquote>
	/// <pre>
	/// //Equivalent code using an inner class instead of EventHandler.
	/// new ActionListener {
	///    public void actionPerformed(ActionEvent e) {
	///        target.setA(e.getB().getC().isD());
	///    }
	/// }
	/// </pre>
	/// </blockquote>
	/// The target property may also be "qualified" with an arbitrary number
	/// of property prefixs delimited with the "." character.  For example, the
	/// following action listener:
	/// <pre>
	///   EventHandler.create(ActionListener.class, target, "a.b", "c.d")
	/// </pre>
	/// might be written as the following inner class
	/// (assuming all the properties had canonical getter methods and
	/// returned the appropriate types):
	/// <pre>
	///   //Equivalent code using an inner class instead of EventHandler.
	///   new ActionListener {
	///     public void actionPerformed(ActionEvent e) {
	///         target.getA().setB(e.getC().isD());
	///    }
	/// }
	/// </pre>
	/// </para>
	/// <para>
	/// As <code>EventHandler</code> ultimately relies on reflection to invoke
	/// a method we recommend against targeting an overloaded method.  For example,
	/// if the target is an instance of the class <code>MyTarget</code> which is
	/// defined as:
	/// <pre>
	///   public class MyTarget {
	///     public void doIt(String);
	///     public void doIt(Object);
	///   }
	/// </pre>
	/// Then the method <code>doIt</code> is overloaded.  EventHandler will invoke
	/// the method that is appropriate based on the source.  If the source is
	/// null, then either method is appropriate and the one that is invoked is
	/// undefined.  For that reason we recommend against targeting overloaded
	/// methods.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.lang.reflect.Proxy </seealso>
	/// <seealso cref= java.util.EventObject
	/// 
	/// @since 1.4
	/// 
	/// @author Mark Davidson
	/// @author Philip Milne
	/// @author Hans Muller
	///  </seealso>
	public class EventHandler : InvocationHandler
	{
		private Object Target_Renamed;
		private String Action_Renamed;
		private readonly String EventPropertyName_Renamed;
		private readonly String ListenerMethodName_Renamed;
		private readonly AccessControlContext Acc = AccessController.Context;

		/// <summary>
		/// Creates a new <code>EventHandler</code> object;
		/// you generally use one of the <code>create</code> methods
		/// instead of invoking this constructor directly.  Refer to
		/// {@link java.beans.EventHandler#create(Class, Object, String, String)
		/// the general version of create} for a complete description of
		/// the <code>eventPropertyName</code> and <code>listenerMethodName</code>
		/// parameter.
		/// </summary>
		/// <param name="target"> the object that will perform the action </param>
		/// <param name="action"> the name of a (possibly qualified) property or method on
		///        the target </param>
		/// <param name="eventPropertyName"> the (possibly qualified) name of a readable property of the incoming event </param>
		/// <param name="listenerMethodName"> the name of the method in the listener interface that should trigger the action
		/// </param>
		/// <exception cref="NullPointerException"> if <code>target</code> is null </exception>
		/// <exception cref="NullPointerException"> if <code>action</code> is null
		/// </exception>
		/// <seealso cref= EventHandler </seealso>
		/// <seealso cref= #create(Class, Object, String, String, String) </seealso>
		/// <seealso cref= #getTarget </seealso>
		/// <seealso cref= #getAction </seealso>
		/// <seealso cref= #getEventPropertyName </seealso>
		/// <seealso cref= #getListenerMethodName </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConstructorProperties({"target", "action", "eventPropertyName", "listenerMethodName"}) public EventHandler(Object target, String action, String eventPropertyName, String listenerMethodName)
		public EventHandler(Object target, String action, String eventPropertyName, String listenerMethodName)
		{
			this.Target_Renamed = target;
			this.Action_Renamed = action;
			if (target == null)
			{
				throw new NullPointerException("target must be non-null");
			}
			if (action == null)
			{
				throw new NullPointerException("action must be non-null");
			}
			this.EventPropertyName_Renamed = eventPropertyName;
			this.ListenerMethodName_Renamed = listenerMethodName;
		}

		/// <summary>
		/// Returns the object to which this event handler will send a message.
		/// </summary>
		/// <returns> the target of this event handler </returns>
		/// <seealso cref= #EventHandler(Object, String, String, String) </seealso>
		public virtual Object Target
		{
			get
			{
				return Target_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the target's writable property
		/// that this event handler will set,
		/// or the name of the method that this event handler
		/// will invoke on the target.
		/// </summary>
		/// <returns> the action of this event handler </returns>
		/// <seealso cref= #EventHandler(Object, String, String, String) </seealso>
		public virtual String Action
		{
			get
			{
				return Action_Renamed;
			}
		}

		/// <summary>
		/// Returns the property of the event that should be
		/// used in the action applied to the target.
		/// </summary>
		/// <returns> the property of the event
		/// </returns>
		/// <seealso cref= #EventHandler(Object, String, String, String) </seealso>
		public virtual String EventPropertyName
		{
			get
			{
				return EventPropertyName_Renamed;
			}
		}

		/// <summary>
		/// Returns the name of the method that will trigger the action.
		/// A return value of <code>null</code> signifies that all methods in the
		/// listener interface trigger the action.
		/// </summary>
		/// <returns> the name of the method that will trigger the action
		/// </returns>
		/// <seealso cref= #EventHandler(Object, String, String, String) </seealso>
		public virtual String ListenerMethodName
		{
			get
			{
				return ListenerMethodName_Renamed;
			}
		}

		private Object ApplyGetters(Object target, String getters)
		{
			if (getters == null || getters.Equals(""))
			{
				return target;
			}
			int firstDot = getters.IndexOf('.');
			if (firstDot == -1)
			{
				firstDot = getters.Length();
			}
			String first = getters.Substring(0, firstDot);
			String rest = getters.Substring(System.Math.Min(firstDot + 1, getters.Length()));

			try
			{
				Method getter = null;
				if (target != null)
				{
					getter = Statement.GetMethod(target.GetType(), "get" + NameGenerator.Capitalize(first), new Class[]{});
					if (getter == null)
					{
						getter = Statement.GetMethod(target.GetType(), "is" + NameGenerator.Capitalize(first), new Class[]{});
					}
					if (getter == null)
					{
						getter = Statement.GetMethod(target.GetType(), first, new Class[]{});
					}
				}
				if (getter == null)
				{
					throw new RuntimeException("No method called: " + first + " defined on " + target);
				}
				Object newTarget = MethodUtil.invoke(getter, target, new Object[]{});
				return ApplyGetters(newTarget, rest);
			}
			catch (Exception e)
			{
				throw new RuntimeException("Failed to call method: " + first + " on " + target, e);
			}
		}

		/// <summary>
		/// Extract the appropriate property value from the event and
		/// pass it to the action associated with
		/// this <code>EventHandler</code>.
		/// </summary>
		/// <param name="proxy"> the proxy object </param>
		/// <param name="method"> the method in the listener interface </param>
		/// <returns> the result of applying the action to the target
		/// </returns>
		/// <seealso cref= EventHandler </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Object invoke(final Object proxy, final Method method, final Object[] arguments)
		public virtual Object Invoke(Object proxy, Method method, Object[] arguments)
		{
			AccessControlContext acc = this.Acc;
			if ((acc == null) && (System.SecurityManager != null))
			{
				throw new SecurityException("AccessControlContext is not set");
			}
			return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper(this, proxy, method, arguments), acc);
		}

		private class PrivilegedActionAnonymousInnerClassHelper : PrivilegedAction<Object>
		{
			private readonly EventHandler OuterInstance;

			private object Proxy;
			private Method Method;
			private object[] Arguments;

			public PrivilegedActionAnonymousInnerClassHelper(EventHandler outerInstance, object proxy, Method method, object[] arguments)
			{
				this.OuterInstance = outerInstance;
				this.Proxy = proxy;
				this.Method = method;
				this.Arguments = arguments;
			}

			public virtual Object Run()
			{
				return outerInstance.InvokeInternal(Proxy, Method, Arguments);
			}
		}

		private Object InvokeInternal(Object proxy, Method method, Object[] arguments)
		{
			String methodName = method.Name;
			if (method.DeclaringClass == typeof(Object))
			{
				// Handle the Object public methods.
				if (methodName.Equals("hashCode"))
				{
					return new Integer(System.identityHashCode(proxy));
				}
				else if (methodName.Equals("equals"))
				{
					return (proxy == arguments[0] ? true : false);
				}
				else if (methodName.Equals("toString"))
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					return proxy.GetType().FullName + '@' + proxy.HashCode().ToString("x");
				}
			}

			if (ListenerMethodName_Renamed == null || ListenerMethodName_Renamed.Equals(methodName))
			{
				Class[] argTypes = null;
				Object[] newArgs = null;

				if (EventPropertyName_Renamed == null) // Nullary method.
				{
					newArgs = new Object[]{};
					argTypes = new Class[]{};
				}
				else
				{
					Object input = ApplyGetters(arguments[0], EventPropertyName);
					newArgs = new Object[]{input};
					argTypes = new Class[]{input == null ? null : input.GetType()};
				}
				try
				{
					int lastDot = Action_Renamed.LastIndexOf('.');
					if (lastDot != -1)
					{
						Target_Renamed = ApplyGetters(Target_Renamed, Action_Renamed.Substring(0, lastDot));
						Action_Renamed = Action_Renamed.Substring(lastDot + 1);
					}
					Method targetMethod = Statement.GetMethod(Target_Renamed.GetType(), Action_Renamed, argTypes);
					if (targetMethod == null)
					{
						targetMethod = Statement.GetMethod(Target_Renamed.GetType(), "set" + NameGenerator.Capitalize(Action_Renamed), argTypes);
					}
					if (targetMethod == null)
					{
						String argTypeString = (argTypes.Length == 0) ? " with no arguments" : " with argument " + argTypes[0];
						throw new RuntimeException("No method called " + Action_Renamed + " on " + Target_Renamed.GetType() + argTypeString);
					}
					return MethodUtil.invoke(targetMethod, Target_Renamed, newArgs);
				}
				catch (IllegalAccessException ex)
				{
					throw new RuntimeException(ex);
				}
				catch (InvocationTargetException ex)
				{
					Throwable th = ex.TargetException;
					throw (th is RuntimeException) ? (RuntimeException) th : new RuntimeException(th);
				}
			}
			return null;
		}

		/// <summary>
		/// Creates an implementation of <code>listenerInterface</code> in which
		/// <em>all</em> of the methods in the listener interface apply
		/// the handler's <code>action</code> to the <code>target</code>. This
		/// method is implemented by calling the other, more general,
		/// implementation of the <code>create</code> method with both
		/// the <code>eventPropertyName</code> and the <code>listenerMethodName</code>
		/// taking the value <code>null</code>. Refer to
		/// {@link java.beans.EventHandler#create(Class, Object, String, String)
		/// the general version of create} for a complete description of
		/// the <code>action</code> parameter.
		/// <para>
		/// To create an <code>ActionListener</code> that shows a
		/// <code>JDialog</code> with <code>dialog.show()</code>,
		/// one can write:
		/// 
		/// <blockquote>
		/// <pre>
		/// EventHandler.create(ActionListener.class, dialog, "show")
		/// </pre>
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type to create </param>
		/// <param name="listenerInterface"> the listener interface to create a proxy for </param>
		/// <param name="target"> the object that will perform the action </param>
		/// <param name="action"> the name of a (possibly qualified) property or method on
		///        the target </param>
		/// <returns> an object that implements <code>listenerInterface</code>
		/// </returns>
		/// <exception cref="NullPointerException"> if <code>listenerInterface</code> is null </exception>
		/// <exception cref="NullPointerException"> if <code>target</code> is null </exception>
		/// <exception cref="NullPointerException"> if <code>action</code> is null
		/// </exception>
		/// <seealso cref= #create(Class, Object, String, String) </seealso>
		public static T create<T>(Class listenerInterface, Object target, String action)
		{
			return Create(listenerInterface, target, action, null, null);
		}

		/// <summary>
		/// /**
		/// Creates an implementation of <code>listenerInterface</code> in which
		/// <em>all</em> of the methods pass the value of the event
		/// expression, <code>eventPropertyName</code>, to the final method in the
		/// statement, <code>action</code>, which is applied to the <code>target</code>.
		/// This method is implemented by calling the
		/// more general, implementation of the <code>create</code> method with
		/// the <code>listenerMethodName</code> taking the value <code>null</code>.
		/// Refer to
		/// {@link java.beans.EventHandler#create(Class, Object, String, String)
		/// the general version of create} for a complete description of
		/// the <code>action</code> and <code>eventPropertyName</code> parameters.
		/// <para>
		/// To create an <code>ActionListener</code> that sets the
		/// the text of a <code>JLabel</code> to the text value of
		/// the <code>JTextField</code> source of the incoming event,
		/// you can use the following code:
		/// 
		/// <blockquote>
		/// <pre>
		/// EventHandler.create(ActionListener.class, label, "text", "source.text");
		/// </pre>
		/// </blockquote>
		/// 
		/// This is equivalent to the following code:
		/// <blockquote>
		/// <pre>
		/// //Equivalent code using an inner class instead of EventHandler.
		/// new ActionListener() {
		///    public void actionPerformed(ActionEvent event) {
		///        label.setText(((JTextField)(event.getSource())).getText());
		///     }
		/// };
		/// </pre>
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type to create </param>
		/// <param name="listenerInterface"> the listener interface to create a proxy for </param>
		/// <param name="target"> the object that will perform the action </param>
		/// <param name="action"> the name of a (possibly qualified) property or method on
		///        the target </param>
		/// <param name="eventPropertyName"> the (possibly qualified) name of a readable property of the incoming event
		/// </param>
		/// <returns> an object that implements <code>listenerInterface</code>
		/// </returns>
		/// <exception cref="NullPointerException"> if <code>listenerInterface</code> is null </exception>
		/// <exception cref="NullPointerException"> if <code>target</code> is null </exception>
		/// <exception cref="NullPointerException"> if <code>action</code> is null
		/// </exception>
		/// <seealso cref= #create(Class, Object, String, String, String) </seealso>
		public static T create<T>(Class listenerInterface, Object target, String action, String eventPropertyName)
		{
			return Create(listenerInterface, target, action, eventPropertyName, null);
		}

		/// <summary>
		/// Creates an implementation of <code>listenerInterface</code> in which
		/// the method named <code>listenerMethodName</code>
		/// passes the value of the event expression, <code>eventPropertyName</code>,
		/// to the final method in the statement, <code>action</code>, which
		/// is applied to the <code>target</code>. All of the other listener
		/// methods do nothing.
		/// <para>
		/// The <code>eventPropertyName</code> string is used to extract a value
		/// from the incoming event object that is passed to the target
		/// method.  The common case is the target method takes no arguments, in
		/// which case a value of null should be used for the
		/// <code>eventPropertyName</code>.  Alternatively if you want
		/// the incoming event object passed directly to the target method use
		/// the empty string.
		/// The format of the <code>eventPropertyName</code> string is a sequence of
		/// methods or properties where each method or
		/// property is applied to the value returned by the preceding method
		/// starting from the incoming event object.
		/// The syntax is: <code>propertyName{.propertyName}*</code>
		/// where <code>propertyName</code> matches a method or
		/// property.  For example, to extract the <code>point</code>
		/// property from a <code>MouseEvent</code>, you could use either
		/// <code>"point"</code> or <code>"getPoint"</code> as the
		/// <code>eventPropertyName</code>.  To extract the "text" property from
		/// a <code>MouseEvent</code> with a <code>JLabel</code> source use any
		/// of the following as <code>eventPropertyName</code>:
		/// <code>"source.text"</code>,
		/// <code>"getSource.text"</code> <code>"getSource.getText"</code> or
		/// <code>"source.getText"</code>.  If a method can not be found, or an
		/// exception is generated as part of invoking a method a
		/// <code>RuntimeException</code> will be thrown at dispatch time.  For
		/// example, if the incoming event object is null, and
		/// <code>eventPropertyName</code> is non-null and not empty, a
		/// <code>RuntimeException</code> will be thrown.
		/// </para>
		/// <para>
		/// The <code>action</code> argument is of the same format as the
		/// <code>eventPropertyName</code> argument where the last property name
		/// identifies either a method name or writable property.
		/// </para>
		/// <para>
		/// If the <code>listenerMethodName</code> is <code>null</code>
		/// <em>all</em> methods in the interface trigger the <code>action</code> to be
		/// executed on the <code>target</code>.
		/// </para>
		/// <para>
		/// For example, to create a <code>MouseListener</code> that sets the target
		/// object's <code>origin</code> property to the incoming <code>MouseEvent</code>'s
		/// location (that's the value of <code>mouseEvent.getPoint()</code>) each
		/// time a mouse button is pressed, one would write:
		/// <blockquote>
		/// <pre>
		/// EventHandler.create(MouseListener.class, target, "origin", "point", "mousePressed");
		/// </pre>
		/// </blockquote>
		/// 
		/// This is comparable to writing a <code>MouseListener</code> in which all
		/// of the methods except <code>mousePressed</code> are no-ops:
		/// 
		/// <blockquote>
		/// <pre>
		/// //Equivalent code using an inner class instead of EventHandler.
		/// new MouseAdapter() {
		///    public void mousePressed(MouseEvent e) {
		///        target.setOrigin(e.getPoint());
		///    }
		/// };
		/// </pre>
		/// </blockquote>
		/// 
		/// </para>
		/// </summary>
		/// @param <T> the type to create </param>
		/// <param name="listenerInterface"> the listener interface to create a proxy for </param>
		/// <param name="target"> the object that will perform the action </param>
		/// <param name="action"> the name of a (possibly qualified) property or method on
		///        the target </param>
		/// <param name="eventPropertyName"> the (possibly qualified) name of a readable property of the incoming event </param>
		/// <param name="listenerMethodName"> the name of the method in the listener interface that should trigger the action
		/// </param>
		/// <returns> an object that implements <code>listenerInterface</code>
		/// </returns>
		/// <exception cref="NullPointerException"> if <code>listenerInterface</code> is null </exception>
		/// <exception cref="NullPointerException"> if <code>target</code> is null </exception>
		/// <exception cref="NullPointerException"> if <code>action</code> is null
		/// </exception>
		/// <seealso cref= EventHandler </seealso>
		public static T create<T>(Class listenerInterface, Object target, String action, String eventPropertyName, String listenerMethodName)
		{
			// Create this first to verify target/action are non-null
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final EventHandler handler = new EventHandler(target, action, eventPropertyName, listenerMethodName);
			EventHandler handler = new EventHandler(target, action, eventPropertyName, listenerMethodName);
			if (listenerInterface == null)
			{
				throw new NullPointerException("listenerInterface must be non-null");
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClassLoader loader = getClassLoader(listenerInterface);
			ClassLoader loader = GetClassLoader(listenerInterface);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class[] interfaces = {listenerInterface};
			Class[] interfaces = new Class[] {listenerInterface};
			return AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClassHelper2(handler, loader, interfaces));
		}

		private class PrivilegedActionAnonymousInnerClassHelper2 : PrivilegedAction<T>
		{
			private java.beans.EventHandler Handler;
			private java.lang.ClassLoader Loader;
			private Type[] Interfaces;

			public PrivilegedActionAnonymousInnerClassHelper2(java.beans.EventHandler handler, java.lang.ClassLoader loader, Type[] interfaces)
			{
				this.Handler = handler;
				this.Loader = loader;
				this.Interfaces = interfaces;
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T run()
			public virtual T Run()
			{
				return (T) Proxy.newProxyInstance(Loader, Interfaces, Handler);
			}
		}

		private static ClassLoader GetClassLoader(Class type)
		{
			ReflectUtil.checkPackageAccess(type);
			ClassLoader loader = type.ClassLoader;
			if (loader == null)
			{
				loader = Thread.CurrentThread.ContextClassLoader; // avoid use of BCP
				if (loader == null)
				{
					loader = ClassLoader.SystemClassLoader;
				}
			}
			return loader;
		}
	}

}