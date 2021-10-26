using Sandbox;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace GameOfLife
{

	public class Pattern
	{

		public string PatternName { get; set; } = "null";
		public int PatternWidth { get; set; } = 3;
		public int PatternHeight { get; set; } = 3;
		public string GridData { get; set; } = "";

		public Pattern( string patternName, int patternWidth, int patternHeight, string gridData ) //JSON Deserializer only works with same name constructor variables
		{

			PatternName = patternName;
			PatternWidth = patternWidth;
			PatternHeight = patternHeight;
			GridData = gridData;

		}

		public static bool[,] Cut( bool[,] grid, Vector2 topLeft, Vector2 bottomRight )
		{
			int sizeX = (int)bottomRight.x - (int)topLeft.x;
			int sizeY = (int)bottomRight.y - (int)topLeft.y;

			if ( sizeX <= 0 || sizeY <= 0 ) return new bool[0, 0];
			if ( topLeft.x < 0 || topLeft.y < 0 ) return new bool[0, 0];
			if ( bottomRight.x > grid.GetLength( 1 ) || bottomRight.y > grid.GetLength( 0 ) ) return new bool[0, 0];

			bool[,] resultGrid = new bool[sizeX, sizeY];

			for ( int y = (int)topLeft.y; y < (int)bottomRight.y; y++)
			{

				for ( int x = (int)topLeft.x; x < (int)bottomRight.x; x++ )
				{

					resultGrid[x - (int)topLeft.x, y - (int)topLeft.y] = grid[x, y];

				}

			}

			return resultGrid;

		}

		public static string ToString( bool[,] cells )
		{

			string dataString = "";
			int currentCount = 0;
			bool currentCheck = false;

			for ( int y = 0; y < cells.GetLength(1); y++ ) // Row by Row
			{

				for ( int x = 0; x < cells.GetLength(0); x++ )
				{

					bool currentCell = cells[x,y];

					if( currentCell == currentCheck )
					{

						currentCount++;

					}
					else
					{

						dataString = $"{dataString}{(dataString == "" ? "" : ".")}{currentCount}";
						currentCount = 0;
						currentCheck = !currentCheck;

					}

				}

			}

			return dataString;

		}

		public static bool[,] FromString( string dataString, int width, int height )
		{

			bool[,] cells = new bool[width, height];
			string[] explodedString = dataString.Split( new char[] { '.' } );
			int currentNum = 0;
			int currentCount = int.Parse( explodedString[currentNum] );
			bool currentCheck = false;

			for ( int y = 0; y < height; y++ )
			{

				for ( int x = 0; x < width; x++ )
				{

					if( currentCount == 0 )
					{

						if( currentNum < explodedString.Length - 1 )
						{

							currentNum++;
							currentCount = int.Parse( explodedString[currentNum] );
							currentCheck = !currentCheck;

						}
						else
						{

							currentCheck = !currentCheck;
							currentCount = -1;

						}

					}
					else
					{

						currentCount--;

					}

					cells[x, y] = currentCheck;

				}

			}


			return cells;

		}

		[ServerCmd( "gol_save")]
		public static void SaveJson()
		{

			List<Pattern> grid = new List<Pattern>();
			grid.Add( new Pattern( "Pattern1", CellGrid.GridSize, CellGrid.GridSize, Pattern.ToString( CellGrid.Cells ) ) );
			grid.Add( new Pattern( "Pattern3", 3, 3, Pattern.ToString( Pattern.Cut( CellGrid.Cells, new Vector2(0, 0), new Vector2(3, 3) ) ) ) );

			FileSystem.Data.WriteJson( "gol_patterns.json", grid );

		}

		[ServerCmd( "gol_load" )]
		public static void LoadJson()
		{

			GameOfLife.Patterns.Clear();
			CellGrid.ClearGrid();

			List<Pattern> loaded = FileSystem.Data.ReadJson<List<Pattern>>( "gol_patterns.json" );

			bool[,] cells = Pattern.FromString( loaded[1].GridData, loaded[1].PatternWidth, loaded[1].PatternHeight );

			for ( int y = 0; y < cells.GetLength( 1 ); y++ )
			{

				for ( int x = 0; x < cells.GetLength( 0 ); x++ )
				{

					CellGrid.UpdateCell( x, y, cells[x, y], false );

				}

			}

			CellGrid.BroadcastGrid( Pattern.ToString( CellGrid.Cells ) );

		}

	}

}
