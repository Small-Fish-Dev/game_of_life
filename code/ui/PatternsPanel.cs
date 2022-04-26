using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class PatternEntry : Panel
	{

		string[] names = new string[] { "Glider", "Glider Cannon", "Rose", "Bomb", "Block", "Wiggler", "Mark" };

		public PatternEntry()
		{

			var panel = Add.Panel( "pattern" );
			var entryName = panel.Add.Panel( "name" );
			entryName.Add.Label( names[new Random().Int( 0, 6 )] );

		}

	}

	public class PatternsPanel : Panel
	{

		public PatternsPanel()
		{

			Add.Panel( "patternstitle" ).Add.Label( "Patterns (WIP)" );

			var patternsContainer = Add.Panel( "patterncontainer" );

			for ( int i = 0; i < 100; i++ )
			{

				patternsContainer.AddChild<PatternEntry>();


			}

		}

	}

}
