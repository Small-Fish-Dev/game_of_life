using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class GoLHUD : Panel
	{

		/*static float maxSize = Math.Min( Screen.Width, Screen.Height) * 0.7f; //TODO: Bigger screens make it overlap over buttons?
		static int border = 1;
		static Panel container;

		float cellSize =  maxSize / Math.Max( CellGrid.GridSize.x, CellGrid.GridSize.y );*/

		public Panel Sidebar { get; set; }
		public Panel Title { get; set; }
		public Panel Patterns { get; set; }
		public Panel Chat { get; set; }
		public Panel Tools { get; set; }
		public Panel Buttons { get; set; }
		public Panel Grid { get; set; }

		public GoLHUD()
		{

			StyleSheet.Load( "HUD.scss" );

			Sidebar = Add.Panel( "sidebar" );
			Tools = Add.Panel( "tools" );
			Grid = Add.Panel( "grid" );

			Title = Sidebar.Add.Panel( "title" );
			Title.Add.Label( "Game of Life" );
			Patterns = Sidebar.Add.Panel( "patterns" );
			Chat = Sidebar.Add.Panel( "chat" );

			var play = Tools.Add.Panel( "buttons" );
			play.Add.Button( "▸", "play", () => { CellGrid.NetworkPlay( true ); CellGrid.Playing = true; PlaySound( "click2" ); } );

			var stop = Tools.Add.Panel( "buttons" );
			stop.Add.Button( "᱿", "stop", () => { CellGrid.NetworkPlay( false ); CellGrid.Playing = false; PlaySound( "click2" ); } );

			var next = Tools.Add.Panel( "buttons" );
			next.Add.Button( "⇥", "next", () => { CellGrid.NetworkNext(); PlaySound( "click2" ); } );

			var clear = Tools.Add.Panel( "buttons" );
			clear.Add.Button( "⨯", "clear", () => { CellGrid.ClearGrid( true ); PlaySound( "click2" ); } );

			//Grid.Style.Width = CellGrid.GridSize.x >= CellGrid.GridSize.y ? maxSize + border : cellSize * CellGrid.GridSize.x + border;
			//Grid.Style.Height = CellGrid.GridSize.y >= CellGrid.GridSize.x ? maxSize + border: cellSize * CellGrid.GridSize.y + border;
			/*var listtest = new ShadowList();
			listtest.Add( new Shadow { OffsetX = 50f, OffsetY = 50f, Color = Color.White } ); //TODO: Record before and after you make the change, do not delete code, comment it out.
			listtest.Add( new Shadow { OffsetX = 100f, OffsetY = 100f, Color = Color.Black } );
			listtest.Add( new Shadow { OffsetX = 150f, OffsetY = 150f, Color = Color.White } );
			listtest.Add( new Shadow { OffsetX = 200f, OffsetY = 200f, Color = Color.Black } );
			listtest.Add( new Shadow { OffsetX = 250f, OffsetY = 250f, Color = Color.White } );
			container.Style.BoxShadow = listtest;*/
			Grid.Style.PixelSnap = 0;

			var panel = Grid.Add.Panel( "cell" );

			panel.Style.PixelSnap = 0;

			CellGrid.CellPanel = panel;

			var shadowList = new ShadowList();

			for ( int x = 0; x < CellGrid.GridSize.x; x++ )
			{

				for ( int y = 0; y < CellGrid.GridSize.y; y++ )
				{

					var shadow = new Shadow { OffsetX = x * 18 , OffsetY = y * 18 , Color = Color.Black };

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

				var targetBox = Grid.Box;
				var relativeMouseX = Mouse.Position.x - targetBox.Left;
				var relativeMouseY = Mouse.Position.y - targetBox.Top;
				var mousePosRelative = new Vector2( relativeMouseX, relativeMouseY );
				var mousePosRelativeInUIPixels = mousePosRelative * ScaleFromScreen;

				int x = (int)MathX.Floor( mousePosRelativeInUIPixels.x / 10 );
				int y = (int)MathX.Floor( mousePosRelativeInUIPixels.y / 10 );

				if( x >= 0 && x <= CellGrid.GridSize.x && y >= 0 && y <= CellGrid.GridSize.y )
				{

					var state = CellGrid.Cell( x, y ).Alive;

					CellGrid.UpdateCell( x, y, !state, true );

				}

				PlaySound( "click1" );

			}

		}

	}

	public partial class HUD : Sandbox.HudEntity<RootPanel>
	{

		public HUD()
		{
			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "HUD.scss" );

			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<GoLHUD>();


		}

	}



}
