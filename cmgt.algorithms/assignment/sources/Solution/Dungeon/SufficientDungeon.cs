using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GXPEngine.Mathf;

internal class SufficientDungeon : Dungeon
{
	protected Random rng;

	public BSPNode RootNode;

	public SufficientDungeon(Size pSize) : base(pSize) 
	{
		rng = new Random();
	}

	protected override void generate(int pMinimumRoomSize)
	{
		RootNode = new BSPNode(new Rectangle(1, 1, size.Width - 2, size.Height - 2));

		GenerateRooms(pMinimumRoomSize);
		GenerateDoors();
	}

	protected virtual void GenerateRooms(int pMinimumRoomSize)
	{
		RootNode.SplitRecursive(3, pMinimumRoomSize, rng);

		foreach (Rectangle rect in RootNode.LeafBounds)
		{
			rooms.Add(new Room(new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Height + 2)));
		}
	}
	protected virtual void GenerateDoors()
	{
		foreach (var (a, b) in RootNode.SiblingPairs)
		{
			int doorX;
			int doorY;
			bool valid = false;
			Point doorLoc = new Point();

			while (!valid)
			{
				if (a.Bounds.X == b.Bounds.X)
				{
					// Aligned on top of each other
					doorX = rng.Next(a.Bounds.Left, a.Bounds.Right);
					doorY = a.Bounds.Bottom;
					doorLoc = new Point(doorX, doorY);
					
					valid = RootNode.FindPoint(new Point(doorX, doorY - 1)) != null && RootNode.FindPoint(new Point(doorX, doorY + 1)) != null;
				}
				else
				{
					// Aligned next to each other
					doorX = a.Bounds.Right;
					doorY = rng.Next(a.Bounds.Top, a.Bounds.Bottom);
					doorLoc = new Point(doorX, doorY);

					valid = RootNode.FindPoint(new Point(doorX - 1, doorY)) != null && RootNode.FindPoint(new Point(doorX + 1, doorY)) != null;
				}
			}
			
			Console.WriteLine("Adding door at {0}", doorLoc);
			doors.Add(new Door(doorLoc));
		}
	}

	protected override void drawDoor(Door pDoor, Pen pColor)
	{
		base.drawDoor(pDoor, Pens.LightGray);
	}
}
