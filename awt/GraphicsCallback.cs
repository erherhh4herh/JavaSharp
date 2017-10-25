/*
 * Copyright (c) 1999, 2000, Oracle and/or its affiliates. All rights reserved.
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

	using SunGraphicsCallback = sun.awt.SunGraphicsCallback;


	internal abstract class GraphicsCallback : SunGraphicsCallback
	{

		internal sealed class PaintCallback : GraphicsCallback
		{
			internal static PaintCallback Instance_Renamed = new PaintCallback();

			internal PaintCallback()
			{
			}
			public void Run(Component comp, Graphics cg)
			{
				comp.Paint(cg);
			}
			internal static PaintCallback Instance
			{
				get
				{
					return Instance_Renamed;
				}
			}
		}
		internal sealed class PrintCallback : GraphicsCallback
		{
			internal static PrintCallback Instance_Renamed = new PrintCallback();

			internal PrintCallback()
			{
			}
			public void Run(Component comp, Graphics cg)
			{
				comp.Print(cg);
			}
			internal static PrintCallback Instance
			{
				get
				{
					return Instance_Renamed;
				}
			}
		}
		internal sealed class PaintAllCallback : GraphicsCallback
		{
			internal static PaintAllCallback Instance_Renamed = new PaintAllCallback();

			internal PaintAllCallback()
			{
			}
			public void Run(Component comp, Graphics cg)
			{
				comp.PaintAll(cg);
			}
			internal static PaintAllCallback Instance
			{
				get
				{
					return Instance_Renamed;
				}
			}
		}
		internal sealed class PrintAllCallback : GraphicsCallback
		{
			internal static PrintAllCallback Instance_Renamed = new PrintAllCallback();

			internal PrintAllCallback()
			{
			}
			public void Run(Component comp, Graphics cg)
			{
				comp.PrintAll(cg);
			}
			internal static PrintAllCallback Instance
			{
				get
				{
					return Instance_Renamed;
				}
			}
		}
		internal sealed class PeerPaintCallback : GraphicsCallback
		{
			internal static PeerPaintCallback Instance_Renamed = new PeerPaintCallback();

			internal PeerPaintCallback()
			{
			}
			public void Run(Component comp, Graphics cg)
			{
				comp.Validate();
				if (comp.Peer_Renamed is LightweightPeer)
				{
					comp.LightweightPaint(cg);
				}
				else
				{
					comp.Peer_Renamed.paint(cg);
				}
			}
			internal static PeerPaintCallback Instance
			{
				get
				{
					return Instance_Renamed;
				}
			}
		}
		internal sealed class PeerPrintCallback : GraphicsCallback
		{
			internal static PeerPrintCallback Instance_Renamed = new PeerPrintCallback();

			internal PeerPrintCallback()
			{
			}
			public void Run(Component comp, Graphics cg)
			{
				comp.Validate();
				if (comp.Peer_Renamed is LightweightPeer)
				{
					comp.LightweightPrint(cg);
				}
				else
				{
					comp.Peer_Renamed.print(cg);
				}
			}
			internal static PeerPrintCallback Instance
			{
				get
				{
					return Instance_Renamed;
				}
			}
		}
		internal sealed class PaintHeavyweightComponentsCallback : GraphicsCallback
		{
			internal static PaintHeavyweightComponentsCallback Instance_Renamed = new PaintHeavyweightComponentsCallback();

			internal PaintHeavyweightComponentsCallback()
			{
			}
			public void Run(Component comp, Graphics cg)
			{
				if (comp.Peer_Renamed is LightweightPeer)
				{
					comp.PaintHeavyweightComponents(cg);
				}
				else
				{
					comp.PaintAll(cg);
				}
			}
			internal static PaintHeavyweightComponentsCallback Instance
			{
				get
				{
					return Instance_Renamed;
				}
			}
		}
		internal sealed class PrintHeavyweightComponentsCallback : GraphicsCallback
		{
			internal static PrintHeavyweightComponentsCallback Instance_Renamed = new PrintHeavyweightComponentsCallback();

			internal PrintHeavyweightComponentsCallback()
			{
			}
			public void Run(Component comp, Graphics cg)
			{
				if (comp.Peer_Renamed is LightweightPeer)
				{
					comp.PrintHeavyweightComponents(cg);
				}
				else
				{
					comp.PrintAll(cg);
				}
			}
			internal static PrintHeavyweightComponentsCallback Instance
			{
				get
				{
					return Instance_Renamed;
				}
			}
		}
	}

}