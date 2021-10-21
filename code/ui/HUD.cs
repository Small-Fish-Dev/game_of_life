using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class GoLHUD : Panel
	{

		public GoLHUD()
		{

			AddChild<SidebarPanel>( "sidebar" );
			AddChild<ToolsPanel>( "tools" );
			AddChild<GridPanel>("grid");


		}

	}

	public partial class HUD : Sandbox.HudEntity<RootPanel>
	{

		public HUD()
		{

			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "ui/HUD.scss" );
			RootPanel.AddChild<GoLHUD>();

		}

	}

}
