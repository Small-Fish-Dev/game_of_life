using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class ToolsPanel : Panel
	{

		public ToolsPanel()
		{

			// [PLAY] button
			var play = Add.Button( "", "buttons" );
			CellGrid.PlayLabel = play.Add.Label( CellGrid.Playing ? "᱿" : "▸", "play" );
			play.AddEventListener( "onclick", () =>
			{

				CellGrid.Play( !CellGrid.Playing, true );
				PlaySound( "click_button" );

			} );

			// [NEXT] button
			var next = Add.Button( "", "buttons" );
			next.Add.Label( "⇥", "next" );
			next.AddEventListener( "onclick", () =>
			{

				CellGrid.Next( true );
				PlaySound( "click_button" );

			} );

			// [CLEAR] button
			var clear = Add.Button( "", "buttons" );
			clear.Add.Label( "⨯", "clear" );
			clear.AddEventListener( "onclick", () =>
			{

				CellGrid.ClearGrid( true );
				PlaySound( "click_button" );

			} );

			// [LOOP] button
			var loop = Add.Button( "", "buttons" );
			loop.Add.Label( "⟳", "loop" );
			CellGrid.LoopCross = loop.Add.Label( "✕", "cross" );
			CellGrid.LoopCross.Style.Opacity = CellGrid.Looping ? 0 : 1;
			loop.AddEventListener( "onclick", () =>
			{

				CellGrid.Loop( !CellGrid.Looping, true );
				PlaySound( "click_button" );

			} );

			// [SPEED] button
			var speed = Add.Button( "", "buttons" );
			CellGrid.SpeedLabel = speed.Add.Label( $"⨯{(float)CellGrid.ValidSpeeds[CellGrid.Speed] / 10}", "speed" );
			speed.AddEventListener( "onclick", () =>
			{

				CellGrid.SetSpeed( (CellGrid.Speed + 1) % CellGrid.ValidSpeeds.Count, true );
				PlaySound( "click_button" );

			} );

			// [SIZE]+ button
			var plus = Add.Button( "", "buttons" );
			plus.Add.Label( "+", "plus" );
			plus.AddEventListener( "onclick", () =>
			{

				CellGrid.SetSize( CellGrid.GridSize + 1, true );
				PlaySound( "click_button" );

			} );
			// [SIZE]- button
			var minus = Add.Button( "", "buttons" );
			minus.Add.Label( "-", "minus" );
			minus.AddEventListener( "onclick", () =>
			{

				CellGrid.SetSize( CellGrid.GridSize - 1, true );
				PlaySound( "click_button" );

			} );

			// [GRID] button
			var grid = Add.Button( "", "buttons" );
			grid.Add.Label( "⩩", "loop" );
			CellGrid.GridCross = grid.Add.Label( "✕", "cross" );
			CellGrid.GridCross.Style.Opacity = CellGrid.ShowGrid ? 0 : 1;
			grid.AddEventListener( "onclick", () =>
			{

				CellGrid.ToggleGrid( !CellGrid.ShowGrid, true );
				PlaySound( "click_button" );

			} );

		}

	}

}
