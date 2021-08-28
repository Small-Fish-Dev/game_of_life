using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System;
using System.Runtime;

namespace GameOfLife
{

	public partial class GameOfLife : Sandbox.Game
	{

		public static HUD GameHUD;
		public static List<LogEntry> ChatMessages = new();

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

			base.ClientJoined( client );

			ushort[] package = CellGrid.GeneratePackage();

			if ( package.Length > 0 )
			{

				CellGrid.BroadcastGrid( To.Single( client ), CellGrid.GeneratePackage() );

			}

			foreach( LogEntry message in ChatMessages ) 
			{ 
			
				ChatBox.AddChatEntry( To.Single( client ), message.User, message.Message );

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

					lastFrame = Time.Now + 1/(float)CellGrid.ValidSpeeds[CellGrid.Speed];

					CellGrid.Next( true );

				}

			}

		}

	}

}
