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

				ChatPanel.SendChatLog( "Server has been started." );
				ChatPanel.SendChatLog( "Hello, this is my first serious attempt at making a gamemode, it is still very much in development but I thought that now was a good time to get people to test it. For suggestions message ubre on discord" );

			}

		}

		public override void ClientJoined( Client client )
		{

			ushort[] package = CellGrid.GeneratePackage();

			if ( package.Length > 0 )
			{

				CellGrid.BroadcastGrid( To.Single( client ), CellGrid.GeneratePackage() );

			}

			foreach( LogEntry message in ChatMessages ) 
			{ 
			
				ChatPanel.AddChatLog( To.Single( client ), message.User, message.Message, message.Button );

			}



			ChatPanel.SendChatLog( $"{client.Name} has joined." );

			PlayMusic( To.Single( client ) );

		}

		public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
		{

			ChatPanel.SendChatLog( $"{client.Name} has left." );

		}

		[ClientRpc]
		public void PlayMusic()
		{
			PlaySound( "blippy" );
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
					PlaySound( $"click_next{CellGrid.Speed}" );

				}

			}

		}

	}

}
