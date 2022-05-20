using Sandbox;
using System.Collections.Generic;

namespace GameOfLife
{

	public static partial class CellGrid
	{

		[ConCmd.Server]
		public static void NetworkUpdate( int x, int y, bool state )
		{

			UpdateCell( x, y, state, true );

		}

		[ConCmd.Server]
		public static void NetworkClear()
		{

			ClearGrid( true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, "[CLEAR]" );

		}
		
		[ConCmd.Server]
		public static void NetworkNext()
		{

			Next( true, ConsoleSystem.Caller );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, "[NEXT]" );

		}

		[ConCmd.Server]
		public static void NetworkPlay( bool isPlaying)
		{

			Play( isPlaying, true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, isPlaying ? "[PLAY]" : "[STOP]" );

		}

		[ConCmd.Server]
		public static void NetworkLoop( bool isLooping )
		{

			Loop( isLooping, true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, isLooping ? "[LOOP]" : "[WALL]" );

		}

		[ConCmd.Server]
		public static void NetworkSpeed( int speed )
		{

			SetSpeed( speed, true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, "[SPEED]" );

		}

		[ConCmd.Server]
		public static void NetworkSize( int size )
		{

			SetSize( size, true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, "[SIZE]" );

		}

		[ConCmd.Server]
		public static void NetworkShowGrid( bool isGridToggled )
		{

			ToggleGrid( isGridToggled, true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, isGridToggled ? "[SHOW]" : "[HIDE]" );

		}


		[ClientRpc]
		public static void BroadcastUpdate( int x, int y, bool state )
		{

			UpdateCell( x, y, state );

		}

		[ClientRpc]
		public static void BroadcastClear()
		{

			ClearGrid();

		}

		[ClientRpc]
		public static void BroadcastNext()
		{

			Next();

		}

		[ClientRpc]
		public static void BroadcastPlay( bool isPlaying )
		{

			Play( isPlaying );

		}

		[ClientRpc]
		public static void BroadcastLoop( bool isLooping )
		{

			Loop( isLooping );

		}

		[ClientRpc]
		public static void BroadcastSpeed( int speed )
		{

			SetSpeed( speed );

		}

		[ClientRpc]
		public static void BroadcastSize( int size )
		{

			SetSize( size );

		}


		[ClientRpc]
		public static void BroadcastGrid( string grid )
		{

			if( grid.Length > 0 )
			{

				bool[,] cells = Pattern.FromString( grid, GridSize, GridSize );

				for ( int y = 0; y < cells.GetLength( 1 ); y++ )
				{

					for ( int x = 0; x < cells.GetLength( 0 ); x++ )
					{

						UpdateCell( x, y, cells[x, y], false );

					}

				}

			}

		}

		[ClientRpc]
		public static void BroadcastShowGrid( bool isGridToggled )
		{

			ToggleGrid( isGridToggled );

		}

	}

}
