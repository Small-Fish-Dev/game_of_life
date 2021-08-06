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

			string message = $"{ConsoleSystem.Caller.Name} pressed [CLEAR] !";

			Log.Info( message );
			ChatBox.AddChatEntry( To.Everyone, "", message );

		}

		[ServerCmd]
		public static void NetworkNext()
		{

			NextFrame();

			string message = $"{ConsoleSystem.Caller.Name} pressed [NEXT] !";

			Log.Info( message );
			ChatBox.AddChatEntry( To.Everyone, "", message );

		}

		[ServerCmd]
		public static void NetworkPlay( bool isPlaying)
		{

			Playing = isPlaying;
			BroadcastPlay( isPlaying );


			string message = $"{ConsoleSystem.Caller.Name} pressed " + ( isPlaying ? "[PLAY]" : "[STOP]" ) + " !" ;

			Log.Info( message );
			ChatBox.AddChatEntry( To.Everyone, "", message );

		}

		//TODO: Broadcast next frame to others

		[ClientRpc]
		public static void BroadcastUpdate( int x, int y, bool state )
		{

			UpdateCell( x, y, state, false );

		}

		[ClientRpc]
		public static void BroadcastClear()
		{

			ClearGrid( false );

		}

		[ClientRpc]
		public static void BroadcastGrid( ushort[] grid )
		{

			foreach(ushort cell in grid )
			{

				int posX = cell % CellGrid.GridSize.x;
				int posY = (int)MathX.Floor( cell / CellGrid.GridSize.x );

				UpdateCell( posX, posY, true, false );

			}

		}

		[ClientRpc]
		public static void BroadcastPlay( bool isPlaying )
		{

			Playing = isPlaying;

		}

	}

}
