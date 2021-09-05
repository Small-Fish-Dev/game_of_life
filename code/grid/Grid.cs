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

		public readonly static Vector2Int GridSize = new( 50, 50 );
		private static Dictionary<Vector2Int, Cell> cells = new();
		public static List<Vector2Int> ActiveCells = new();
		public static Panel CellPanel { get; set; }
		public static bool Playing { get; set; } = false;
		public static bool Looping { get; set; } = true;
		public static int Speed { get; set; } = 2; // Relative to the ValidSpeeds list
		public static Panel GridPanel { get; set; }
		public static Panel LoopCross { get; set; }
		public static Label PlayLabel { get; set; }
		public static Label SpeedLabel { get; set; }

		public static List<int> ValidSpeeds { get; set; } = new() { 1, 5, 10, 20, 50 };

		static CellGrid()
		{

			BuildGrid( GridSize.x, GridSize.y );

		}

		public static Cell Cell( int x, int y )
		{

			if( Looping )
			{

				x = LoopAround( x, GridSize.x );
				y = LoopAround( y, GridSize.y );

			}

			return cells[new Vector2Int( x, y )];

		}

		public static int LoopAround( int variable, int max )
		{

			int loopedVariable = variable;

			if ( variable >= max ) { loopedVariable = loopedVariable % max; }
			if ( variable < 0 ) { loopedVariable = max - ( ( -variable ) % max ); }

			return loopedVariable;

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

		public static void UpdateCell( int x, int y, bool state, bool networked = false )
		{

			if( Cell( x, y ).Alive == state )
			{

				return;

			}

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

		public static void ClearGrid( bool networked = false )
		{

			List<Vector2Int> oldGrid = new( ActiveCells );

			oldGrid.ForEach( delegate ( Vector2Int pos )
			{

				UpdateCell( pos.x, pos.y, false );

			} );

			if( networked )
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

		public static void Next( bool networked = false, Client caller = null )
		{

			Dictionary<Vector2Int, bool> newGeneration = new();
			Dictionary<Vector2Int, int> neighbourCount = new();

			ActiveCells.ForEach( delegate ( Vector2Int pos )
			{

				for ( int x = -1; x <= 1; x++ )
				{

					for ( int y = -1; y <= 1; y++ )
					{

						var checkPos = new Vector2Int( pos.x + x, pos.y + y );

						if ( x == 0 && y == 0 ) {

							if ( !neighbourCount.ContainsKey( checkPos ) )
							{

								neighbourCount.Add( checkPos, 0 );

							}
							
							continue;

						}

						if( Looping )
						{

							checkPos.x = LoopAround( checkPos.x, GridSize.x );
							checkPos.y = LoopAround( checkPos.y, GridSize.y );

						}
						else
						{

							if ( checkPos.x >= GridSize.x || checkPos.x < 0 ) { continue; }
							if ( checkPos.y >= GridSize.y || checkPos.y < 0 ) { continue; }

						}

						if ( neighbourCount.ContainsKey( checkPos ) )
						{

							neighbourCount[checkPos]++;

						}
						else
						{

							neighbourCount.Add( checkPos,  1 );

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

				UpdateCell( newCell.Key.x, newCell.Key.y, newCell.Value );

			}

			if ( networked )
			{

				if ( Host.IsServer )
				{

					foreach ( Client client in Client.All )
					{

						if( client != caller )
						{

							BroadcastNext( To.Single( client ) ); // Send to everyone except the caller

						}

					}

				}
				else
				{

					NetworkNext();

				}

			}

		}

		public static void Play( bool isPlaying, bool networked = false)
		{

			Playing = isPlaying;

			if ( Host.IsClient )
			{

				PlayLabel.SetText( Playing ? "᱿" : "▸" );

				if( Playing )
				{

					GridPanel.AddClass( "block" );

				}
				else
				{

					GridPanel.RemoveClass( "block" );

				}

			}

			if ( networked )
			{

				if ( Host.IsServer )
				{

					BroadcastPlay( isPlaying );

				}
				else
				{

					NetworkPlay( isPlaying );

				}

			}

		}

		public static void Loop( bool isLooping, bool networked = false )
		{

			Looping = isLooping;

			if( Host.IsClient )
			{

				LoopCross.Style.Opacity = Looping ? 0 : 1;

			}

			if ( networked )
			{

				if ( Host.IsServer )
				{

					BroadcastLoop( isLooping );

				}
				else
				{

					NetworkLoop( isLooping );

				}

			}

		}

		public static void SetSpeed( int speed, bool networked = false )
		{

			Speed = speed;

			if ( Host.IsClient )
			{

				Log.Info( speed );

				SpeedLabel.SetText( $"⨯{(float)ValidSpeeds[speed]/10}" );

			}

			if ( networked )
			{

				if ( Host.IsServer )
				{

					BroadcastSpeed( speed );

				}
				else
				{

					NetworkSpeed( speed );

				}

			}

		}

		public static ushort[] GeneratePackage()
		{

			List<ushort> package = new();

			foreach ( Vector2Int cell in ActiveCells )
			{

				ushort cellPos = (ushort)( GridSize.y * cell.y + cell.x );

				package.Add( cellPos );

			}

			return package.ToArray(); // TODO: Switch to List<ushort> when it's fixed

		}

	}

}
