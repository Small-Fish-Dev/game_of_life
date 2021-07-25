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

			NextFrame();

		}

		[ServerCmd]
		public static void NetworkPlay( bool isPlaying)
		{

			Playing = isPlaying;
			BroadcastPlay( isPlaying );

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
