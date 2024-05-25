using System;
using System.Collections.Generic;
using System.Drawing;

internal class SufficientDungeon : Dungeon
{
	protected Random rnd;
	protected bool[,] walkableGrid;
	protected Area RootArea;
	protected Dictionary<int, Room> roomMap;

	protected int minRoomSize;

	public SufficientDungeon(Size pSize) : base(pSize) { }

	protected override void generate(int pMinimumRoomSize)
	{
		rnd = new Random();
		minRoomSize = pMinimumRoomSize;

		walkableGrid = new bool[size.Width, size.Height];

		RootArea = new Area(new Rectangle(0, 0, size.Width, size.Height));
		RootArea.Split(pMinimumRoomSize + 2, 10, new Random());
		Console.WriteLine(RootArea.TreeToString());

		PlaceRooms(RootArea);
		PlaceDoors(RootArea);

		Console.WriteLine();
	}

	protected virtual void PlaceRooms(Area rootArea)
	{
		roomMap = new Dictionary<int, Room>();
		foreach (Area leaf in rootArea.Leaves)
		{
			Size roomSize = new Size(leaf.Width, leaf.Height);
			Point roomPosition = new Point(leaf.X, leaf.Y);
			Room room = new Room(new Rectangle(roomPosition, roomSize));
			AddRoom(room, leaf.ID);
		}
	}
	protected virtual void PlaceDoors(Area rootArea)
	{
		Console.WriteLine("Generating doors...");

		foreach ((Area a, Area b) in rootArea.Pairs)
		{
			bool doorValid = false;
			int doorX = 0;
			int doorY = 0;
			bool doorHorizontal = false;

			while (!doorValid)
			{
				if (a.X == b.X)
				{
					// Areas are aligned like this
					// A
					// |
					// B

					doorHorizontal = false;

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

					doorHorizontal = true;

					ref Rectangle rectA = ref a.Rectangle; // No need to copy values
					doorX = a.Rectangle.Right - 1;
					doorY = rnd.Next(rectA.Top + 1, rectA.Bottom - 1);

					doorValid = walkableGrid[doorX + 1, doorY] && walkableGrid[doorX - 1, doorY];
				}
			}

			Area sideA;
			Area sideB;

			if (doorHorizontal)
			{
				sideA = rootArea.FindByPoint(new Point(doorX - minRoomSize / 2, doorY));
				sideB = rootArea.FindByPoint(new Point(doorX + minRoomSize / 2, doorY));
			}
			else
			{
				sideA = rootArea.FindByPoint(new Point(doorX, doorY - minRoomSize / 2));
				sideB = rootArea.FindByPoint(new Point(doorX, doorY + minRoomSize / 2));
			}

			Room roomA = roomMap[sideA.ID];
			Room roomB = roomMap[sideB.ID];

			Door door = new Door(new Point(doorX, doorY))
			{
				horizontal = doorHorizontal,
				roomA = roomA,
				roomB = roomB
			};

			// Add door to rooms and doors
			roomA.doors.Add(door);
			roomB.doors.Add(door);
			AddDoor(door);

			Console.WriteLine(door);
		}

		Console.WriteLine("Doors generated");
	}

	private void AddDoor(Door door)
	{
		doors.Add(door);
		walkableGrid[door.location.X, door.location.Y] = true;
	}
	private void AddRoom(Room room, int areaID)
	{
		rooms.Add(room);
		roomMap.Add(areaID, room);
		Console.WriteLine("Added room #{0} to dictionary under ({1})", room.ID, areaID);

		for (int y = 0; y < room.area.Height - 2; y++)
		{
			for (int x = 0; x < room.area.Width - 2; x++)
			{
				walkableGrid[x + room.area.X + 1, y + room.area.Y + 1] = true;
			}
		}
	}
}