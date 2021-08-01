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

		public static Vector2Int GridSize = new( 50, 50 );
		private static Dictionary<Vector2Int, Cell> cells = new();
		public static List<Vector2Int> ActiveCells = new();
		public static Panel CellPanel { get; set; }
		public static bool Playing { get; set; } = false;

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

			if ( state )
			{

				ActiveCells.Add( new Vector2Int( x, y ) );

			}
			else
			{

				ActiveCells.Remove( new Vector2Int( x, y ) );

			}

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

			List<Vector2Int> oldGrid = new( ActiveCells );

			oldGrid.ForEach( delegate ( Vector2Int pos )
			{

				UpdateCell( pos.x, pos.y, false, false );

			} );

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

		public static void NextFrame()
		{

			Dictionary<Vector2Int, bool> newGeneration = new();
			Dictionary<Vector2Int, int> neighbourCount = new();

			ActiveCells.ForEach( delegate ( Vector2Int pos )
			{

				for ( int x = -1; x <= 1; x++ )
				{

					for ( int y = -1; y <= 1; y++ )
					{

						var itself = false;

						if ( x == 0 && y == 0 )
						{

							itself = true;

						}

						var checkPos = new Vector2Int( pos.x + x, pos.y + y );

						if ( checkPos.x < 0 ) { checkPos.x = CellGrid.GridSize.x - 1; }
						if ( checkPos.y < 0 ) { checkPos.y = CellGrid.GridSize.y - 1; }
						if ( checkPos.x >= CellGrid.GridSize.x ) { checkPos.x = 0; }
						if ( checkPos.y >= CellGrid.GridSize.y ) { checkPos.y = 0; }

						if ( neighbourCount.ContainsKey( checkPos ) )
						{

							neighbourCount[checkPos] += itself ? 0 : 1;

						}
						else
						{

							neighbourCount.Add( checkPos, itself ? 0 : 1 );

						}

					}

				}

			} );

			foreach ( KeyValuePair<Vector2Int, int> curCell in neighbourCount )
			{

				if ( CellGrid.Cell( curCell.Key.x, curCell.Key.y ).Alive )
				{

					if ( curCell.Value < 2 || curCell.Value > 3 )
					{

						newGeneration.Add( curCell.Key, false );

					}

				}
				else
				{

					if ( curCell.Value == 3 )
					{

						newGeneration.Add( curCell.Key, true );

					}

				}

			};

			foreach ( KeyValuePair<Vector2Int, bool> newCell in newGeneration )
			{

				UpdateCell( newCell.Key.x, newCell.Key.y, newCell.Value, true );

			}

		}

		public static ushort[] GeneratePackage() // TODO: Use activeCells list
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
