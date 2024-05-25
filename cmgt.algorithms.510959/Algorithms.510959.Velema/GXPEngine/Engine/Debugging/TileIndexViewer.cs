using GXPEngine.Control;
using System;
using static GXPEngine.Mathf;

namespace GXPEngine.Debugging
{
	internal class TileIndexViewer : Scene
	{
		readonly Sprite sprite;
		readonly EasyDraw selectionBox;
		readonly int tileWidth;
		readonly int tileHeight;
		readonly int cols;

		public TileIndexViewer(string tileSheetPath, int cols, int rows)
		{
			this.cols = cols;

			sprite = new Sprite(tileSheetPath);
			int multipWidth = game.Width / sprite.width;
			int multipHeight = game.Height / sprite.height;
			int scaleFactor = Min(multipHeight, multipWidth);
			sprite.scale = scaleFactor;
			Console.WriteLine("Scaling by {0}", scaleFactor);
			AddChild(sprite);

			tileWidth = sprite.width / cols;
			tileHeight = sprite.height / rows;

			selectionBox = new EasyDraw(tileWidth, tileHeight);
			selectionBox.NoFill();
			selectionBox.Stroke(200);
			selectionBox.StrokeWeight(5);
			selectionBox.ShapeAlign(CenterMode.Min, CenterMode.Min);
			selectionBox.Rect(0, 0, selectionBox.width, selectionBox.height);
			AddChild(selectionBox);
		}

		private void Update()
		{
			Vector2 mouse = Input.mousePos;
			int tileX = (int)(mouse.X / tileWidth);
			int tileY = (int)(mouse.Y / tileHeight);

			selectionBox.Position = new Vector2(tileX * tileWidth, tileY * tileHeight);

			if (Input.GetMouseButtonDown(0))
			{
				Console.WriteLine("Tile index: {0}", tileY * cols + tileX);
			}

			if (Input.GetKeyDown(Key.F2))
			{
				Previous();
			}
		}
	}
}
