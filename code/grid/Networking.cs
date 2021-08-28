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

		}
		
		[ServerCmd]
		public static void NetworkNext()
		{

			Next( true, ConsoleSystem.Caller );

			//string message = $"{ConsoleSystem.Caller.Name} pressed [NEXT] !";

			//ChatBox.SayInfo( message );

		}

		[ServerCmd]
		public static void NetworkPlay( bool isPlaying)
		{

			Play( isPlaying, true );


			//string message = $"{ConsoleSystem.Caller.Name} pressed " + ( isPlaying ? "[PLAY]" : "[STOP]" ) + " !" ;

			//ChatBox.SayInfo( message );

		}

		[ServerCmd]
		public static void NetworkLoop( bool isLooping )
		{

			Loop( isLooping, true );


			//string message = $"{ConsoleSystem.Caller.Name} pressed [LOOP] !";

			//ChatBox.SayInfo( message );

		}

		[ServerCmd]
		public static void NetworkSpeed( int speed )
		{

			SetSpeed( speed, true );


			//string message = $"{ConsoleSystem.Caller.Name} pressed [SPEED] !";

			//ChatBox.SayInfo( message );

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
		public static void BroadcastGrid( ushort[] grid )
		{

			foreach ( ushort cell in grid )
			{

				int posX = cell % CellGrid.GridSize.x;
				int posY = (int)MathX.Floor( cell / CellGrid.GridSize.x );

				UpdateCell( posX, posY, true );

			}

		}

	}

}
