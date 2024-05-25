using System;
using System.Collections.Generic;
using System.Drawing;

internal class SufficientDungeon : Dungeon
{
	protected Random rnd;
	protected bool[,] walkableGrid;
	protected Area RootArea;

	public SufficientDungeon(Size pSize) : base(pSize) { }

	protected override void generate(int pMinimumRoomSize)
	{
		rnd = new Random();

		walkableGrid = new bool[size.Width, size.Height];

		RootArea = new Area(new Rectangle(0, 0, size.Width, size.Height));
		RootArea.Split(pMinimumRoomSize + 2, 2, new Random());

		PlaceRooms(RootArea);
		PlaceDoors(RootArea);
	}

	protected virtual void PlaceRooms(Area rootArea)
	{
		foreach (Area leaf in rootArea.Leaves)
		{
			Size roomSize = new Size(leaf.Width, leaf.Height);
			Point roomPosition = new Point(leaf.X, leaf.Y);
			AddRoom(new Room(new Rectangle(roomPosition, roomSize)));
		}
	}
	protected virtual void PlaceDoors(Area rootArea)
	{
		foreach ((Area a, Area b) in rootArea.Pairs)
		{
			bool doorValid = false;
			int doorX = 0;
			int doorY = 0;

			while (!doorValid)
			{
				if (a.X == b.X)
				{
					// Areas are aligned like this
					// A
					// |
					// B
					ref Rectangle rectA = ref a.Rectangle; // No need to copy values
					doorX = rnd.Next(rectA.Left + 1, rectA.Right - 1);
					doorY = a.Rectangle.Bottom - 1;

					doorValid = walkableGrid[doorX, doorY + 1] && walkableGrid[doorX, doorY - 1];
				}
				else
				{
					// Areas are aligned like this
					//
					// A - B
					//
					ref Rectangle rectA = ref a.Rectangle; // No need to copy values
					doorX = a.Rectangle.Right - 1;
					doorY = rnd.Next(rectA.Top + 1, rectA.Bottom - 1);

					doorValid = walkableGrid[doorX + 1, doorY] && walkableGrid[doorX - 1, doorY];
				}
			}

			AddDoor(new Door(new Point(doorX, doorY)));
		}
	}

	private void AddDoor(Door door)
	{
		doors.Add(door);
		walkableGrid[door.location.X, door.location.Y] = true;
	}
	private void AddRoom(Room room)
	{
		rooms.Add(room);

		for (int y = 0; y < room.area.Height - 2; y++)
		{
			for (int x = 0; x < room.area.Width - 2; x++)
			{
				walkableGrid[x + room.area.X + 1, y + room.area.Y + 1] = true;
			}
		}
	}
}