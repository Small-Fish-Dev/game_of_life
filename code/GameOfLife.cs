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
		public static Dictionary<string, Pattern> Patterns = new();

		public GameOfLife()
		{

			if ( IsServer )
			{

				LoadHUD();

				ChatPanel.SendChatLog( "Server has been started." );
				ChatPanel.SendChatLog( "Hello, this is my first serious attempt at making a gamemode, it is still very much in development but I thought that now was a good time to get people to test it. For suggestions message ubre on discord" );

			}

		}

		[ServerCmd( "gol_load_hud", Help = "Load the HUD" )]
		public static void LoadHUD()
		{

			if( Host.IsServer )
			{

				if ( GameHUD != null )
				{

					GameHUD.Delete();

				}

				GameHUD = new HUD();

			}

		}

		public override void ClientJoined( Client client )
		{

			CellGrid.BroadcastGrid( To.Single( client ), Pattern.ToString( CellGrid.Cells ) );


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
