using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class GridPanel : Panel
	{

		static float maxSize = Math.Min( Screen.Width, Screen.Height) * 0.7f; //TODO: Bigger screens make it overlap over buttons?
		static int border = 1;
		static Panel container;

		float cellSize =  maxSize / Math.Max( CellGrid.GridSize.x, CellGrid.GridSize.y );

		public GridPanel()
		{

			container = Add.Panel("grid");
			container.Style.Width = CellGrid.GridSize.x >= CellGrid.GridSize.y ? maxSize + border : cellSize * CellGrid.GridSize.x + border;
			container.Style.Height = CellGrid.GridSize.y >= CellGrid.GridSize.x ? maxSize + border: cellSize * CellGrid.GridSize.y + border;
			/*var listtest = new ShadowList();
			listtest.Add( new Shadow { OffsetX = 50f, OffsetY = 50f, Color = Color.White } ); //TODO: Record before and after you make the change, do not delete code, comment it out.
			listtest.Add( new Shadow { OffsetX = 100f, OffsetY = 100f, Color = Color.Black } );
			listtest.Add( new Shadow { OffsetX = 150f, OffsetY = 150f, Color = Color.White } );
			listtest.Add( new Shadow { OffsetX = 200f, OffsetY = 200f, Color = Color.Black } );
			listtest.Add( new Shadow { OffsetX = 250f, OffsetY = 250f, Color = Color.White } );
			container.Style.BoxShadow = listtest;*/
			container.Style.PixelSnap = 0;

			var panel = container.Add.Panel();

			panel.Style.Width = cellSize - border;
			panel.Style.Height = cellSize - border;
			panel.Style.BackgroundColor = Color.Transparent;
			panel.Style.Position = PositionMode.Absolute;
			panel.Style.Left = 0;
			panel.Style.Top = 0;
			panel.Style.PixelSnap = 0;

			CellGrid.CellPanel = panel;

			var shadowList = new ShadowList();

			for ( int x = 0; x < CellGrid.GridSize.x; x++ )
			{

				for ( int y = 0; y < CellGrid.GridSize.y; y++ )
				{

					var shadow = new Shadow { OffsetX = x * cellSize + border, OffsetY = y * cellSize + border, Color = Color.Black };

					CellGrid.Cell( x, y ).Shadow = shadow;

					shadowList.Add( shadow );

				}

			}

			panel.Style.BoxShadow = shadowList;

		}

		public override void OnButtonEvent( ButtonEvent e )
		{

			if ( e.Button == "mouseleft" && e.Pressed == true )
			{

				var targetBox = container.Box;
				var relativeMouseX = Mouse.Position.x - targetBox.Left;
				var relativeMouseY = Mouse.Position.y - targetBox.Top;
				var mousePosRelative = new Vector2( relativeMouseX, relativeMouseY );
				var mousePosRelativeInUIPixels = mousePosRelative * ScaleFromScreen;

				int x = (int)MathX.Floor( mousePosRelativeInUIPixels.x / cellSize );
				int y = (int)MathX.Floor( mousePosRelativeInUIPixels.y / cellSize );

				if( x >= 0 && x <= CellGrid.GridSize.x && y >= 0 && y <= CellGrid.GridSize.y )
				{

					var state = CellGrid.Cell( x, y ).Alive;

					CellGrid.UpdateCell( x, y, !state, true );

				}

				PlaySound( "click1" );

			}

		}

	}

	public class Buttons : Panel
	{

		public Buttons()
		{

			var play = Add.Panel( "buttons" );
			play.Add.Button( "▸", "play", () => { CellGrid.ClearGrid( true ); GameOfLife.GameHUD.Delete(); CellGrid.GridSize = new( CellGrid.GridSize.x - 1, CellGrid.GridSize.y - 1 ); CellGrid.BuildGrid( CellGrid.GridSize.x - 1, CellGrid.GridSize.y - 1 ); GameOfLife.GameHUD = new HUD(); } );

			var stop = Add.Panel( "buttons" );
			stop.Add.Button( "᱿", "stop", () => { CellGrid.ClearGrid( true ); GameOfLife.GameHUD.Delete(); CellGrid.GridSize = new( CellGrid.GridSize.x + 1, CellGrid.GridSize.y + 1 ); CellGrid.BuildGrid( CellGrid.GridSize.x + 1, CellGrid.GridSize.y + 1 );  GameOfLife.GameHUD = new HUD();  } );
			//TODO: Make it work on server and fix that shit lmao!
			var next = Add.Panel( "buttons" );
			next.Add.Button( "⇥", "next", () => { CellGrid.NetworkNext(); PlaySound( "click2" ); } );

			var clear = Add.Panel( "buttons" );
			clear.Add.Button( "⨯", "clear", () => { CellGrid.ClearGrid( true ); PlaySound( "click2" ); } );

		}

	}

	public class Title : Panel
	{

		public Title()
		{
			var title = Add.Panel( "title" );
			title.Add.Label( "Game of Life" );
		}

	}

	public partial class HUD : Sandbox.HudEntity<RootPanel>
	{

		public HUD()
		{
			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "HUD.scss" );

			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<Title>();
			RootPanel.AddChild<GridPanel>();
			RootPanel.AddChild<Buttons>();

		}

	}



}
