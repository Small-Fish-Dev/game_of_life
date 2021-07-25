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

			if ( package.Length > 0 )
			{

				CellGrid.BroadcastGrid( To.Single( client ), CellGrid.GeneratePackage() );

			}

		}

		private float lastFrame = 0f;

		[Event.Tick.Server]
		public void OnTick()
		{

			if ( CellGrid.Playing )
			{

				if ( Time.Now >= lastFrame )
				{

					lastFrame = Time.Now + 0.01f;

					CellGrid.NextFrame();

				}

			}

		}

	}

}
