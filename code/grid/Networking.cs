using Sandbox;
using System.Collections.Generic;

namespace GameOfLife
{

	public static partial class CellGrid
	{

		[ServerCmd]
		public static void NetworkUpdate( int x, int y, bool state )
		{

			UpdateCell( x, y, state, true );

		}

		[ServerCmd]
		public static void NetworkClear()
		{

			ClearGrid( true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, "[CLEAR]" );

		}
		
		[ServerCmd]
		public static void NetworkNext()
		{

			Next( true, ConsoleSystem.Caller );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, "[NEXT]" );

		}

		[ServerCmd]
		public static void NetworkPlay( bool isPlaying)
		{

			Play( isPlaying, true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, isPlaying ? "[PLAY]" : "[STOP]" );

		}

		[ServerCmd]
		public static void NetworkLoop( bool isLooping )
		{

			Loop( isLooping, true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, isLooping ? "[LOOP]" : "[WALL]" );

		}

		[ServerCmd]
		public static void NetworkSpeed( int speed )
		{

			SetSpeed( speed, true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, "[SPEED]" );

		}

		[ServerCmd]
		public static void NetworkSize( int size )
		{

			SetSize( size, true );

			ChatPanel.SendChatLog( "pressed", ConsoleSystem.Caller.Name, "[SIZE]" );

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
		public static void BroadcastGrid( ushort[] grid )
		{

			foreach ( ushort cell in grid )
			{

				int posX = cell % CellGrid.GridSize;
				int posY = (int)MathX.Floor( cell / CellGrid.GridSize );

				UpdateCell( posX, posY, true );

			}

		}

	}

}
