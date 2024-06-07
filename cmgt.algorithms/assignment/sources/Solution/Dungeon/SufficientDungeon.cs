using System;
using System.Drawing;

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

		foreach (Room room in rooms) Console.WriteLine(room.ToString());
		foreach (Door door in doors) Console.WriteLine(door.ToString());
	}

	protected virtual void GenerateRooms(int pMinimumRoomSize)
	{
		RootNode.SplitRecursive(3, pMinimumRoomSize, rng);

		foreach (BSPNode node in RootNode.LeafNodes)
		{
			Room room = new Room( new Rectangle(node.Bounds.X - 1, node.Bounds.Y - 1, node.Bounds.Width + 2, node.Bounds.Height + 2) );
			rooms.Add(room);
			node.Room = room;
		}
	}
	protected virtual void GenerateDoors()
	{
		foreach (var (a, b) in RootNode.SiblingPairs)
		{
			GenerateDoor(a, b);
		}
	}

	protected virtual void GenerateDoor(BSPNode a, BSPNode b)
	{
		int doorX;
		int doorY;
		bool valid = false;
		bool horizontal = false;
		Point doorLoc = new Point();

		Room connectedA = null;
		Room connectedB = null;

		while (!valid)
		{
			if (a.Bounds.X == b.Bounds.X)
			{
				// Aligned on top of each other
				doorX = rng.Next(a.Bounds.Left, a.Bounds.Right);
				doorY = a.Bounds.Bottom;
				doorLoc = new Point(doorX, doorY);

				BSPNode sideA = RootNode.FindPoint(new Point(doorX, doorY - 1));
				BSPNode sideB = RootNode.FindPoint(new Point(doorX, doorY + 1));

				if (sideA == null || sideB == null) continue;
				else valid = true;

				connectedA = sideA.Room;
				connectedB = sideB.Room;

				horizontal = false;
			}
			else
			{
				// Aligned next to each other
				doorX = a.Bounds.Right;
				doorY = rng.Next(a.Bounds.Top, a.Bounds.Bottom);
				doorLoc = new Point(doorX, doorY);

				BSPNode sideA = RootNode.FindPoint(new Point(doorX - 1, doorY));
				BSPNode sideB = RootNode.FindPoint(new Point(doorX + 1, doorY));

				if (sideA == null || sideB == null) continue;
				else valid = true;

				connectedA = sideA.Room;
				connectedB = sideB.Room;

				horizontal = true;
			}
		}

		Door door = new Door(doorLoc)
		{
			roomA = connectedA,
			roomB = connectedB,
			horizontal = horizontal,
		};

		connectedA.doors.Add(door);
		connectedB.doors.Add(door);

		doors.Add(door);
	}

	protected override void drawDoor(Door pDoor, Pen pColor)
	{
		base.drawDoor(pDoor, Pens.LightGray);
	}
}
