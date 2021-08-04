using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class GoLHUD : Panel
	{

		public Panel Sidebar { get; set; }
		public Panel Title { get; set; }
		public Panel Patterns { get; set; }
		public Panel PatternsTitle { get; set; }
		public Panel PatternContainer { get; set; }
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
			PatternsTitle = Patterns.Add.Panel( "patternstitle" );
			PatternsTitle.Add.Label( "Patterns" );
			PatternContainer = Patterns.Add.Panel( "patterncontainer" );

			for( int i = 0; i < 100; i++)
			{

				PatternContainer.AddChild<PatternEntry>();


			}

			var play = Tools.Add.Panel( "buttons" );
			play.Add.Button( "▸", "play", () => { CellGrid.NetworkPlay( true ); CellGrid.Playing = true; PlaySound( "click2" ); } );

			var stop = Tools.Add.Panel( "buttons" );
			stop.Add.Button( "᱿", "stop", () => { CellGrid.NetworkPlay( false ); CellGrid.Playing = false; PlaySound( "click2" ); } );

			var next = Tools.Add.Panel( "buttons" );
			next.Add.Button( "⇥", "next", () => { CellGrid.NetworkNext(); PlaySound( "click2" ); } );

			var clear = Tools.Add.Panel( "buttons" );
			clear.Add.Button( "⨯", "clear", () => { CellGrid.ClearGrid( true ); PlaySound( "click2" ); } );

			Grid.Style.PixelSnap = 0;

			Grid.AddChild<CellPanel>();
			Chat = Sidebar.Add.Panel( "chat" );
			Chat.AddChild<ChatBox>();

		}

		public override void OnButtonEvent( ButtonEvent e )
		{

			if ( e.Button == "mouseleft" && e.Pressed == true )
			{

				int x = (int)MathX.Floor( Grid.MousePosition.x / Grid.Box.Rect.width * 50 );
				int y = (int)MathX.Floor( Grid.MousePosition.y / Grid.Box.Rect.width * 50 );

				if( x >= 0 && x <= CellGrid.GridSize.x && y >= 0 && y <= CellGrid.GridSize.y )
				{

					var state = CellGrid.Cell( x, y ).Alive;

					CellGrid.UpdateCell( x, y, !state, true );

				}

				PlaySound( "click1" );

			}

		}

	}

	public class CellPanel : Panel
	{

		public CellPanel()
		{

			var panel = Add.Panel( "cell" );

			panel.Style.PixelSnap = 0;

			CellGrid.CellPanel = panel;

			var shadowList = new ShadowList();

			for ( int x = 0; x < CellGrid.GridSize.x; x++ )
			{

				for ( int y = 0; y < CellGrid.GridSize.y; y++ )
				{

					var shadow = new Shadow { OffsetX = x * 19.38f, OffsetY = y * 19.38f, Color = Color.Black };

					CellGrid.Cell( x, y ).Shadow = shadow;

					shadowList.Add( shadow );

				}

			}

			panel.Style.BoxShadow = shadowList;

		}

	}

	public class PatternEntry : Panel
	{

		string[] names = new string[] { "Glider", "Glider Cannon", "Rose", "Bomb", "Block", "Wiggler", "Mark" };

		public PatternEntry()
		{

			var panel = Add.Panel( "pattern" );
			var entryName = panel.Add.Panel( "name" );
			entryName.Add.Label( names[new Random().Int( 0, 6 )] );

		}

	}


	public partial class HUD : Sandbox.HudEntity<RootPanel>
	{

		public HUD()
		{
			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "HUD.scss" );

			
			RootPanel.AddChild<GoLHUD>();


		}

	}

}
