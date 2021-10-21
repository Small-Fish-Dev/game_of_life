using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class PlayerCursor : Panel
	{

		public PlayerCursor()
		{

		}

		public override void Tick()
		{

			Style.Top = Parent.MousePosition.y * ScaleToScreen;
			Style.Left = Parent.MousePosition.x * ScaleToScreen;

		}

	}

	public class PreviewPanel : Panel
	{

		public PreviewPanel()
		{

			Style.Width = 100;
			Style.Height = 100;

		}

		public override void DrawBackground( ref RenderState state )
		{

			bool[,] gridDisplay = new bool[3, 3]
			{
			{ false, true, false },
			{ false, false, true },
			{ true, true, true }
			};

			Rect rect = this.Box.Rect;
			float boxWidth = rect.width / 3;
			float boxHeight = rect.height / 3;

			for ( int x = 0; x < 3; x++ )
			{

				for ( int y = 0; y < 3; y++ )
				{

					float boxLeft = rect.left + x * boxWidth;
					float boxTop = rect.top + y * boxHeight;

					Render.UI.Box( new Rect( boxLeft, boxTop, boxWidth, boxHeight ), gridDisplay[y, x] ? Color.White : Color.Black );

				}

			}

		}

		public override void Tick()
		{

			Style.Top = Parent.MousePosition.y;
			Style.Left = Parent.MousePosition.x + 20;

		}

	}

	public partial class HUD : Sandbox.HudEntity<RootPanel>
	{

		public HUD()
		{

			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "ui/HUD.scss" );

			RootPanel.AddChild<PreviewPanel>( "info" );

			RootPanel.AddChild<SidebarPanel>( "sidebar" );
			RootPanel.AddChild<ToolsPanel>( "tools" );
			RootPanel.AddChild<GridPanel>( "grid" );

		}

	}

}
