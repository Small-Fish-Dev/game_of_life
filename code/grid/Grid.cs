using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class Cell
	{

		public bool Alive { get; set; } = false;
		public Shadow Shadow { get; set; }

	}

	public struct Vector2Int
	{

		public int x;
		public int y;

		public Vector2Int( int x, int y)
		{
			this.x = x;
			this.y = y;
		}

	}

	public static partial class CellGrid
	{

		public static Vector2Int GridSize = new( 32, 32 );
		private static Dictionary<Vector2Int, Cell> cells = new();
		public static Panel CellPanel { get; set; }

		static CellGrid()
		{

			BuildGrid( GridSize.x, GridSize.y );

		}

		public static Cell Cell( int x, int y )
		{

			return cells[new Vector2Int( x, y )];

		}

		public static void BuildGrid( int sizeX, int sizeY )
		{

			cells.Clear();

			for ( int x = 0; x < GridSize.x; x++ )
			{

				for ( int y = 0; y < GridSize.y; y++ )
				{

					cells.Add( new Vector2Int( x, y ), new Cell() );

				}

			}

		}

		public static void UpdateCell( int x, int y, bool state, bool networked )
		{

			Cell( x, y ).Alive = state;

			if ( Host.IsClient )
			{

				CellPanel.Style.BoxShadow[x * GridSize.y + y] = new Shadow 
				{
					OffsetX = Cell( x, y ).Shadow.OffsetX,
					OffsetY = Cell( x, y ).Shadow.OffsetY,
					Color = state ? Color.White : Color.Black
				};

				CellPanel.Style.Dirty();

			}

			if ( networked )
			{

				if ( Host.IsServer )
				{

					BroadcastUpdate( x, y, state );

				}
				else
				{

					NetworkUpdate( x, y, state );

				}

			}

		}

		public static void ClearGrid( bool networked )
		{

			for ( int x = 0; x < CellGrid.GridSize.x; x++ )
			{

				for ( int y = 0; y < CellGrid.GridSize.y; y++ )
				{

					UpdateCell( x, y, false, false );

				}

			}

			if ( networked )
			{

				if ( Host.IsServer )
				{

					BroadcastClear();

				}
				else
				{

					NetworkClear();

				}

			}

		}

		private static int CalcNeighbours( int xPos, int yPos )
		{

			int neighbours = 0;

			for ( int x = -1; x <= 1; x++ )
			{

				for ( int y = -1; y <= 1; y++ )
				{

					int lookX = xPos + x;
					int lookY = yPos + y;

					if ( x == 0 && y == 0 ) continue;
					//if ( lookX >= CellGrid.GridSize.x || lookX < 0 ) continue; // Uncomment these two to disable looping
					//if ( lookY >= CellGrid.GridSize.y || lookY < 0 ) continue;
					if ( lookX < 0 ) { lookX = CellGrid.GridSize.x - 1; }
					if ( lookY < 0 ) { lookY = CellGrid.GridSize.y - 1; }
					if ( lookX >= CellGrid.GridSize.x ) { lookX = 0; }
					if ( lookY >= CellGrid.GridSize.y ) { lookY = 0; }


					if ( Cell( lookX, lookY ).Alive )
					{

						neighbours++;

					}

				}

			}

			return neighbours;

		}

		public static void NextFrame()
		{

			Dictionary<Vector2Int, bool> newGeneration = new();

			for ( int x = 0; x < CellGrid.GridSize.x; x++ )
			{

				for ( int y = 0; y < CellGrid.GridSize.y; y++ )
				{

					int neighbours = CalcNeighbours( x, y );

					if( CellGrid.Cell( x, y ).Alive )
					{

						if( neighbours < 2 || neighbours > 3 )
						{

							newGeneration.Add( new Vector2Int( x, y ), false );

						}

					}
					else
					{

						if( neighbours == 3 )
						{

							newGeneration.Add( new Vector2Int( x, y ), true );

						}

					}

				}

			}

			foreach ( KeyValuePair<Vector2Int, bool> newCell in newGeneration )
			{

				UpdateCell( newCell.Key.x, newCell.Key.y, newCell.Value, true );

			}

		}

		public static ushort[] GeneratePackage()
		{

			List<ushort> package = new();

			for ( int x = 0; x < GridSize.x; x++ )
			{

				for ( int y = 0; y < GridSize.y; y++ )
				{

					if( Cell( x, y ).Alive )
					{

						ushort cellPos = (ushort)( GridSize.y * y + x );

						package.Add( cellPos );

					}

				}

			}

			return package.ToArray(); // TODO: Switch to List<ushort> when it's fixed

		}

	}

}
