using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace GameOfLife
{

	public class LogEntry
	{

		public string User { get; set; }
		public string Message { get; set; }
		public string Button { get; set; }
		public int Multiplier { get; set; } = 1;
		public ChatEntry Entry { get; set; }

		public LogEntry( string _User, string _Message )
		{

			User = _User;
			Message = _Message;

		}

		public LogEntry( string _User , string _Message, string _Button = null )
		{

			User = _User;
			Message = _Message;
			Button = _Button;
				
		}

		public LogEntry( string _User, string _Message, ChatEntry _Entry = null, string _Button = null )
		{

			User = _User;
			Message = _Message;
			Entry = _Entry;
			Button = _Button;

		}

	}

	public partial class ChatEntry : Panel
	{

		public Label User { get; set; }
		public Label Message { get; set; }
		public Label Button { get; set; }
		public Label Multiplier { get; set; }


		public ChatEntry()
		{

			User = Add.Label( "", "name" );
			Message = Add.Label( "", "message" );
			Button = Add.Label( "", "button" );
			Multiplier = Add.Label( "", "multiplier" );

		}

	}

	public partial class ChatPanel : Panel
	{

		public static Panel Canvas { get; protected set; }
		public TextEntry Input { get; protected set; }

		public ChatPanel()
		{

			Canvas = Add.Panel( "chat_canvas" );
			Canvas.PreferScrollToBottom = true;

			Input = Add.TextEntry( "Press [ENTER] to type" );
			Input.AddEventListener( "onsubmit", () => Submit() );
			Input.AddEventListener( "onblur", () => Close() );
			Input.AcceptsFocus = true;
			Input.AllowEmojiReplace = false; //Emojies would stick out too much with the font

			Sandbox.Hooks.Chat.OnOpenChat += Open;

		}

		void Open()
		{

			AddClass( "open" );
			Input.Focus();

			Input.Text = "";

		}

		void Close()
		{

			RemoveClass( "open" );
			Input.Blur();

			Input.Text = "Press [ENTER] to type";

		}

		void Submit()
		{

			var msg = Input.Text.Trim();

			Close();

			if ( string.IsNullOrWhiteSpace( msg ) ) return;

			Say( msg );

		}

		public static void SendChatLog( string message, string name = null, string button = null )
		{

			AddChatLog( name, message, button );

			Log.Info( $"{name} {message} {button}" );
			GameOfLife.ChatMessages.Add( new LogEntry( name, message, button ) );

		}

		[ClientRpc]
		public static void AddChatLog( string name, string message, string button )
		{

			if ( !Global.IsListenServer )
			{

				Log.Info( $"{name} {message} {button}" );

			}

			if ( GameOfLife.ChatMessages.Count > 0 )
			{

				var lastMessage = GameOfLife.ChatMessages[GameOfLife.ChatMessages.Count - 1];

				if ( name == lastMessage.User )
				{

					if ( message == lastMessage.Message )
					{

						if ( button == lastMessage.Button )
						{

							lastMessage.Multiplier++;
							lastMessage.Entry.Multiplier.Text = $" x{lastMessage.Multiplier}";

							return;

						}

						if ( ToggleButtonText( lastMessage, button, "[PLAY]", "[STOP]" ) ) { return; }

						if ( ToggleButtonText( lastMessage, button, "[LOOP]", "[WALL]" ) ) { return; }

						if ( ToggleButtonText( lastMessage, button, "[SHOW]", "[HIDE]" ) ) { return; }

					}

				}

			}

			var entry = Canvas.AddChild<ChatEntry>();
			entry.Message.Text = $"{name} {message} ";
			entry.Button.Text = button;
			entry.AddClass( "noname" );

			GameOfLife.ChatMessages.Add( new LogEntry( name, message, entry, button ) );

		}

		public static void SendChatMsg( string name, string message )
		{

			AddChatMsg( name, message );

			Log.Info( $"{name}: {message}" );
			GameOfLife.ChatMessages.Add( new LogEntry( name, message ) );

		}

		[ClientRpc]
		public static void AddChatMsg( string name, string message )
		{

			if ( !Global.IsListenServer )
			{

				Log.Info( $"{name}: {message}" );

			}

			var entry = Canvas.AddChild<ChatEntry>();
			entry.User.Text = name + ":";
			entry.Message.Text = message;

			GameOfLife.ChatMessages.Add( new LogEntry( name, message, entry) );

		}

		[ConCmd.Server( "say" )]
		public static void Say( string message )
		{

			Assert.NotNull( ConsoleSystem.Caller );
			string output = RemoveDiacritics( message );
			Log.Info( output );
			SendChatMsg( ConsoleSystem.Caller.Name, output );

		}
		static string RemoveDiacritics( string text )
		{
			var normalizedString = text.Normalize( NormalizationForm.FormD );
			var stringBuilder = new StringBuilder( capacity: normalizedString.Length );

			for ( int i = 0; i < normalizedString.Length; i++ )
			{
				char c = normalizedString[i];
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory( c );
				if ( unicodeCategory != UnicodeCategory.NonSpacingMark )
				{
					stringBuilder.Append( c );
				}
			}

			return stringBuilder
				.ToString()
				.Normalize( NormalizationForm.FormC );
		}

		public static bool ToggleButtonText( LogEntry entry, string button, string text1, string text2 )
		{

			if( entry.Button == text1 && button == text2 || entry.Button == text2 && button == text1 )
			{

				entry.Entry.Button.Text = button;
				entry.Button = button;

				entry.Multiplier++;
				entry.Entry.Multiplier.Text = $" x{entry.Multiplier}";

				return true;

			}

			return false;

		}

	}

}
