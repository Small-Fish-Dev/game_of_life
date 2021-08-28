using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class GoLHUD : Panel
	{

		public Panel Grid { get; internal set; }

		public GoLHUD()
		{

			StyleSheet.Load( "ui/HUD.scss" );

			var sidebar = Add.Panel( "sidebar" );
			var tools = Add.Panel( "tools" );
			Grid = Add.Panel( "grid" );

			var title = sidebar.Add.Panel( "title" );
			title.Add.Label( "Game of Life" );

			var patterns = sidebar.Add.Panel( "patterns" );
			var patternsTitle = patterns.Add.Panel( "patternstitle" );
			patternsTitle.Add.Label( "Patterns" );
			var patternsContainer = patterns.Add.Panel( "patterncontainer" );

			for( int i = 0; i < 100; i++)
			{

				patternsContainer.AddChild<PatternEntry>();


			}

			var play = tools.Add.Button( "▸", "buttons" );
			play.AddEventListener( "onclick", () =>
			{

				CellGrid.Play( !CellGrid.Playing, true );
				play.SetText( CellGrid.Playing ? "᱿" : "▸" );
				PlaySound( "click2" );

			} );

			var next = tools.Add.Button( "⇥", "buttons", () =>
			{

				CellGrid.Next( true );
				PlaySound( "click2" );

			} );

			var clear = tools.Add.Button( "⨯", "buttons", () =>
			{

				CellGrid.ClearGrid( true );
				PlaySound( "click2" );

			} );

			var loop = tools.Add.Button( "⟳", "buttons" );
			CellGrid.LoopCross = loop.Add.Label( "✕", "cross" );
			CellGrid.LoopCross.Style.Opacity = 0;
			loop.AddEventListener( "onclick", () =>
			{

				CellGrid.Loop( !CellGrid.Looping, true );
				PlaySound( "click2" );

			} );

			Grid.Style.PixelSnap = 0;

			Grid.AddChild<CellPanel>();
			var chat = sidebar.Add.Panel( "chat" );
			chat.AddChild<ChatBox>();

		}

		public override void OnButtonEvent( ButtonEvent e )
		{

			if ( e.Button == "mouseleft" && e.Pressed == true )
			{

				int x = (int)MathX.Floor( Grid.MousePosition.x / Grid.Box.Rect.width * 50.3f );
				int y = (int)MathX.Floor( Grid.MousePosition.y / Grid.Box.Rect.height * 50.3f );

				if( x >= 0 && x <= CellGrid.GridSize.x && y >= 0 && y <= CellGrid.GridSize.y )
				{

					CellGrid.UpdateCell( x, y, !CellGrid.Cell( x, y ).Alive, true );

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

					var shadow = new Shadow { OffsetX = x * 19.24f + 3, OffsetY = y * 19.24f + 3, Color = Color.Black };

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

			RootPanel.StyleSheet.Load( "ui/HUD.scss" );

			
			RootPanel.AddChild<GoLHUD>();


		}

	}

}
