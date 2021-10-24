using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public static partial class CellGrid
	{

		public static int GridSize { get; set; } = 50;
		public static bool Playing { get; set; } = false;
		public static bool Looping { get; set; } = true;
		public static int Speed { get; set; } = 2; // Relative to the ValidSpeeds list
		public static Panel LoopCross { get; set; }
		public static Label PlayLabel { get; set; }
		public static Label SpeedLabel { get; set; }
		public static bool[,] Cells { get; set; } = new bool[GridSize, GridSize];
		public static List<Vector2> ActiveCells = new();

		public static List<int> ValidSpeeds { get; set; } = new() { 1, 5, 10, 20, 50 };

		static CellGrid()
		{

		}

		public static int LoopAround( int variable, int max )
		{

			int loopedVariable = variable;

			if ( variable >= max ) { loopedVariable = loopedVariable % max; }
			if ( variable < 0 ) { loopedVariable = max - ( ( -variable ) % max ); }

			return loopedVariable;

		}

		public static void UpdateCell( int x, int y, bool state, bool networked = false )
		{

			if( Looping )
			{

				x = LoopAround( x, GridSize );
				y = LoopAround( y, GridSize );

			}

			if( Cells[x, y] == state )
			{

				return;

			}

			Cells[x, y] = state;

			if ( state )
			{

				ActiveCells.Add( new Vector2( x, y ) );

			}
			else
			{

				ActiveCells.Remove( new Vector2( x, y ) );

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

			List<Vector2> oldGrid = new( ActiveCells );

			oldGrid.ForEach( delegate ( Vector2 pos )
			{

				UpdateCell( (int)pos.x, (int)pos.y, false );

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

			Dictionary<Vector2, bool> newGeneration = new();
			Dictionary<Vector2, int> neighbourCount = new();

			ActiveCells.ForEach( delegate ( Vector2 pos )
			{

				for ( int x = -1; x <= 1; x++ )
				{

					for ( int y = -1; y <= 1; y++ )
					{

						var checkPos = new Vector2( pos.x + x, pos.y + y );

						if ( x == 0 && y == 0 ) {

							if ( !neighbourCount.ContainsKey( checkPos ) )
							{

								neighbourCount.Add( checkPos, 0 );

							}
							
							continue;

						}

						if( Looping )
						{

							checkPos.x = LoopAround( (int)checkPos.x, GridSize );
							checkPos.y = LoopAround( (int)checkPos.y, GridSize );

						}
						else
						{

							if ( checkPos.x >= GridSize || checkPos.x < 0 ) { continue; }
							if ( checkPos.y >= GridSize || checkPos.y < 0 ) { continue; }

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

			foreach ( KeyValuePair<Vector2, int> curCell in neighbourCount )
			{

				if ( CellGrid.Cells[(int)curCell.Key.x, (int)curCell.Key.y] )
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

			foreach ( KeyValuePair<Vector2, bool> newCell in newGeneration )
			{

				UpdateCell( (int)newCell.Key.x, (int)newCell.Key.y, newCell.Value );

			}

			if ( networked )
			{

				if ( Host.IsServer )
				{

					foreach ( Client client in Client.All )
					{

						if( client != caller ) // TODO: Does this even work?
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

		public static void SetSize( int newSize, bool networked = false)
		{

			if ( newSize < 3 || newSize > 256) return;

			List<Vector2> oldCells = new( ActiveCells );

			ClearGrid( false );
			GridSize = newSize;
			Cells = new bool[ newSize, newSize ];

			foreach ( Vector2 cell in oldCells )
			{

				if ( cell.x < GridSize && cell.y < GridSize )
				{

					UpdateCell( (int)cell.x, (int)cell.y, true, false );

				}

			}

			if ( networked )
			{

				if ( Host.IsServer )
				{

					BroadcastSize( newSize );

				}
				else
				{

					NetworkSize( newSize );

				}

			}

		}

	}

}
