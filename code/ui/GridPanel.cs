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

			var border = 6; // Border is annoying!
			var width = this.Box.Rect.width - border;
			var height = this.Box.Rect.height - border;
			var cellWidth = width / CellGrid.GridSize;
			var cellHeight = width / CellGrid.GridSize;
			var cellGap = Math.Max( 2 * ( 50 / CellGrid.GridSize ), 1 );

			for ( int x = 0; x < CellGrid.GridSize; x++ )
			{

				for ( int y = 0; y < CellGrid.GridSize; y++ )
				{

					Render.UI.Box( new Rect( this.Box.Left + x * cellWidth + cellGap * this.ScaleToScreen / 2 + border / 2, this.Box.Top + y * cellHeight + cellGap * this.ScaleToScreen / 2 + border / 2, cellWidth - cellGap * this.ScaleToScreen, cellHeight - cellGap * this.ScaleToScreen ),  CellGrid.Cells[x, y] ? Color.White : Color.Black );

				}

			}

		}

		bool turbo = false;
		bool? setState = null;

		public override void OnButtonEvent( ButtonEvent e )
		{

			if ( CellGrid.Playing ) { return; }

			if ( e.Button == "mouseleft" && e.Pressed == true )
			{

				int x = (int)MathX.Floor( this.MousePosition.x / this.Box.Rect.width * CellGrid.GridSize );
				int y = (int)MathX.Floor( this.MousePosition.y / this.Box.Rect.height * CellGrid.GridSize );

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

				int x = (int)MathX.Floor( this.MousePosition.x / this.Box.Rect.width * CellGrid.GridSize );
				int y = (int)MathX.Floor( this.MousePosition.y / this.Box.Rect.height * CellGrid.GridSize );

				if ( x >= 0 && x < CellGrid.GridSize && y >= 0 && y < CellGrid.GridSize )
				{

					if ( CellGrid.Cells[x, y] == setState ) return;

					CellGrid.UpdateCell( x, y, !CellGrid.Cells[x, y], true );
					setState = CellGrid.Cells[x, y];

					PlaySound( "click_cell" );

				}

			}

			SetClass( "block", CellGrid.Playing);

		}

	}

}
