using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class GridPanel : Panel
	{

		public GridPanel()
		{

		}

		public override void DrawBackground( ref RenderState state )
		{

			var border = 0; // Border is annoying!
			var width = Box.Rect.Width - border;
			var height = Box.Rect.Height - border;
			var cellWidth = width / CellGrid.GridSize;
			var cellHeight = height / CellGrid.GridSize;
			var cellGap = Math.Max( 2 * ( 50 / CellGrid.GridSize ), 2 );

			Style.BackgroundSizeX = cellWidth * ScaleFromScreen;
			Style.BackgroundSizeY = cellHeight * ScaleFromScreen;


			if ( !CellGrid.ShowGrid )
			{

				new Draw2D().Quad( new Rect( Box.Left, Box.Top, Box.Rect.Width, Box.Rect.Height ), Color.Black );

			}

			for ( int x = 0; x < CellGrid.GridSize; x++ )
			{

				for ( int y = 0; y < CellGrid.GridSize; y++ )
				{

					if ( CellGrid.Cells[x, y] )
					{

						new Draw2D().Quad( new Rect( Box.Left + x * cellWidth + cellGap * ScaleToScreen / 2 + border / 2, Box.Top + y * cellHeight + cellGap * ScaleToScreen / 2 + border / 2, cellWidth - cellGap * ScaleToScreen, cellHeight - cellGap * ScaleToScreen ), Color.White );


					}

				}

			}


		}

		bool turbo = false;
		float nextTick = 0f;
		bool? setState = null;

		public override void OnButtonEvent( ButtonEvent e )
		{

			if ( CellGrid.Playing ) { return; }

			if ( e.Button == "mouseleft" && e.Pressed == true )
			{

				int x = (int)MathX.Floor( MousePosition.x / Box.Rect.Width * CellGrid.GridSize );
				int y = (int)MathX.Floor( MousePosition.y / Box.Rect.Height * CellGrid.GridSize );

				if ( x >= 0 && x < CellGrid.GridSize && y >= 0 && y < CellGrid.GridSize )
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

				if( Time.Now >= nextTick )

				{

					int x = (int)MathX.Floor( MousePosition.x / Box.Rect.Width * CellGrid.GridSize );
					int y = (int)MathX.Floor( MousePosition.y / Box.Rect.Height * CellGrid.GridSize );

					if ( x >= 0 && x < CellGrid.GridSize && y >= 0 && y < CellGrid.GridSize )
					{

						if ( CellGrid.Cells[x, y] == setState ) return;

						CellGrid.UpdateCell( x, y, !CellGrid.Cells[x, y], true );
						setState = CellGrid.Cells[x, y];

						PlaySound( "click_cell" );

						nextTick = Time.Now + 1/30f; // Server won't accept more than 30 packets each second, temporary fix

					}


				}

			}

			SetClass( "block", CellGrid.Playing); // Bit lazy I know, might fix later

		}

	}

}
