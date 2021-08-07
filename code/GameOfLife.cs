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
		public static List<string[]> ChatMessages = new();

		public GameOfLife()
		{

			if ( IsServer )
			{

				GameHUD = new HUD();

				ChatBox.SayInfo( "Server has been started." );

			}

		}

		public override void ClientJoined( Client client )
		{

			ushort[] package = CellGrid.GeneratePackage();

			if ( package.Length > 0 )
			{

				CellGrid.BroadcastGrid( To.Single( client ), CellGrid.GeneratePackage() );

			}

			foreach( string[] message in ChatMessages ) 
			{ 
			
				ChatBox.AddChatEntry( To.Single( client ), message[0], message[1] );

			}

			ChatBox.SayInfo( $"{client.Name} has joined." );

		}

		public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
		{

			ChatBox.SayInfo( $"{client.Name} has left." );

		}

		private float lastFrame = 0f;

		[Event.Tick.Server]
		public void OnTick()
		{

			if ( CellGrid.Playing )
			{

				if ( Time.Now >= lastFrame )
				{

					lastFrame = Time.Now + 0.1f;

					CellGrid.NextFrame();

				}

			}

		}

	}

}
