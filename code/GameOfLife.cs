using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System;
using System.Runtime;

namespace GameOfLife
{

	[Library( "game_of_life", Title = "Conways Game of Life" )]
	public partial class GameOfLife : Sandbox.Game
	{

		public static HUD GameHUD;

		public GameOfLife()
		{

			if ( IsServer )
			{

				GameHUD = new HUD();

			}

		}

		public override void ClientJoined( Client client )
		{

			ushort[] package = CellGrid.GeneratePackage();

			if(package.Length > 0)
			{

				CellGrid.BroadcastGrid( To.Single( client ), CellGrid.GeneratePackage() );

			}

		}

	}

}
