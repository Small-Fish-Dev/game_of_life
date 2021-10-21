using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class SidebarPanel : Panel
	{

		public SidebarPanel()
		{

			Add.Label( "Game of Life", "title" );
			AddChild<PatternsPanel>( "patterns" );
			Add.Panel( "chat" ).AddChild<ChatPanel>();

		}

	}

}
