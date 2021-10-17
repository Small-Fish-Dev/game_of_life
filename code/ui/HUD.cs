using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class GoLHUD : Panel
	{

		public GoLHUD()
		{

			StyleSheet.Load( "ui/HUD.scss" );

			var sidebar = Add.Panel( "sidebar" );
			var tools = Add.Panel( "tools" );
			CellGrid.GridPanel = Add.Panel( "grid" );
			CellGrid.GridPanel.AddChild<GridPanel>();

			var title = sidebar.Add.Panel( "title" );
			title.Add.Label( "Game of Life" );

			var patterns = sidebar.Add.Panel( "patterns" );
			var patternsTitle = patterns.Add.Panel( "patternstitle");
			patternsTitle.Add.Label( "Patterns (Coming Soon?)" );
			var patternsContainer = patterns.Add.Panel( "patterncontainer" );

			for( int i = 0; i < 100; i++)
			{

				patternsContainer.AddChild<PatternEntry>();


			}

			// [PLAY] button
			var play = tools.Add.Button( "", "buttons" );
			CellGrid.PlayLabel = play.Add.Label( CellGrid.Playing ? "᱿" : "▸", "play" );
			play.AddEventListener( "onclick", () =>
			{

				CellGrid.Play( !CellGrid.Playing, true );
				PlaySound( "click_button" );

			} );
			

			// [NEXT] button
			var next = tools.Add.Button( "", "buttons" );
			next.Add.Label( "⇥", "next" );
			next.AddEventListener( "onclick", () =>
			{

				CellGrid.Next( true );
				PlaySound( "click_button" );

			} );

			// [CLEAR] button
			var clear = tools.Add.Button( "", "buttons" );
			clear.Add.Label( "⨯", "clear" );
			clear.AddEventListener( "onclick", () =>
			{

				CellGrid.ClearGrid( true );
				PlaySound( "click_button" );

			} );

			// [LOOP] button
			var loop = tools.Add.Button( "", "buttons" );
			loop.Add.Label( "⟳", "loop" );
			CellGrid.LoopCross = loop.Add.Label( "✕", "cross" );
			CellGrid.LoopCross.Style.Opacity = CellGrid.Looping ? 0 : 1;
			loop.AddEventListener( "onclick", () =>
			{

				CellGrid.Loop( !CellGrid.Looping, true );
				PlaySound( "click_button" );

			} );

			// [SPEED] button
			var speed = tools.Add.Button( "", "buttons" );
			CellGrid.SpeedLabel = speed.Add.Label( $"⨯{(float)CellGrid.ValidSpeeds[CellGrid.Speed] / 10}", "speed" );
			speed.AddEventListener( "onclick", () =>
			{

				CellGrid.SetSpeed( (CellGrid.Speed + 1 ) % CellGrid.ValidSpeeds.Count , true );
				PlaySound( "click_button" );

			} );

			// [++] button
			var plus = tools.Add.Button( "+", "buttons" );
			plus.AddEventListener( "onclick", () =>
			{

				CellGrid.SetSize( CellGrid.GridSize + 1, true );
				PlaySound( "click_button" );

			} );
			// [--] button
			var minus = tools.Add.Button( "-", "buttons" );
			minus.AddEventListener( "onclick", () =>
			{

				CellGrid.SetSize( CellGrid.GridSize - 1, true );
				PlaySound( "click_button" );

			} );

			var chat = sidebar.Add.Panel( "chat" );
			chat.AddChild<ChatBox>();

		}

		bool turbo = false;
		bool? setState = null;

		public override void OnButtonEvent( ButtonEvent e )
		{

			if ( CellGrid.Playing ) { return; }

			if ( e.Button == "mouseleft" && e.Pressed == true )
			{

				int x = (int)MathX.Floor( CellGrid.GridPanel.MousePosition.x / CellGrid.GridPanel.Box.Rect.width * CellGrid.GridSize );
				int y = (int)MathX.Floor( CellGrid.GridPanel.MousePosition.y / CellGrid.GridPanel.Box.Rect.height * CellGrid.GridSize );

				if( x >= 0 && x < CellGrid.GridSize && y >= 0 && y < CellGrid.GridSize )
				{

					CellGrid.UpdateCell( x, y, !CellGrid.Cells[x, y], true );

					PlaySound( "click_cell" );

				}

			}

			if ( e.Button == "mouseright" )
			{

				turbo = e.Pressed;
				setState = null;

			}

		}

		public override void Tick()
		{
			
			if ( turbo )
			{

				int x = (int)MathX.Floor( CellGrid.GridPanel.MousePosition.x / CellGrid.GridPanel.Box.Rect.width * CellGrid.GridSize );
				int y = (int)MathX.Floor( CellGrid.GridPanel.MousePosition.y / CellGrid.GridPanel.Box.Rect.height * CellGrid.GridSize );

				if ( x >= 0 && x < CellGrid.GridSize && y >= 0 && y < CellGrid.GridSize )
				{

					if ( CellGrid.Cells[x, y] == setState ) return;

					CellGrid.UpdateCell( x, y, !CellGrid.Cells[x, y], true );
					setState = CellGrid.Cells[x, y];

					PlaySound( "click_cell" );

				}

			}

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

	public class GridPanel : Panel
	{

		public GridPanel()
		{

		}

		public override void DrawBackground( ref RenderState state )
		{

			var width = CellGrid.GridPanel.Box.Rect.width - 6; // The white border shifts the cells if not accounted for.
			var height = CellGrid.GridPanel.Box.Rect.height - 6; // Style.BorderWidth doesn't work!
			var cellWidth = width / CellGrid.GridSize;
			var cellHeight = width / CellGrid.GridSize;
			var cellGap = Math.Max( 2 * ( 50 / CellGrid.GridSize ), 1 );

			for ( int x = 0; x < CellGrid.GridSize; x++ )
			{

				for ( int y = 0; y < CellGrid.GridSize; y++ )
				{

					Render.UI.Box( new Rect( this.Box.Left + x * cellWidth + cellGap * this.ScaleToScreen / 2, this.Box.Top + y * cellHeight + cellGap * this.ScaleToScreen / 2, cellWidth - cellGap * this.ScaleToScreen, cellHeight - cellGap * this.ScaleToScreen ),  CellGrid.Cells[x, y] ? Color.White : Color.Black );


				}


			}

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
