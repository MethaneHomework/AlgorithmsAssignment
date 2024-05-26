using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

internal class SufficientDungeon : Dungeon
{
	protected Random rnd;
	protected bool[,] walkableGrid;
	protected BSPNode RootArea;
	protected Dictionary<int, Room> roomMap;

	protected int minRoomSize;

	public static readonly BooleanSwitch informativeOutput = new BooleanSwitch("Informative output", "Provides extra info on dungeon generation when enabled");

	public SufficientDungeon(Size pSize) : base(pSize) { }

	protected override void generate(int pMinimumRoomSize)
	{
		rnd = new Random(AlgorithmsAssignment.seed);
		minRoomSize = pMinimumRoomSize;

		walkableGrid = new bool[size.Width, size.Height];

		// Create area and split it to specified depth

		RootArea = new BSPNode(new Rectangle(0, 0, size.Width, size.Height));
		RootArea.Split(pMinimumRoomSize + 2, AlgorithmsAssignment.BSP_DEPTH, rnd);

		Debug.WriteIf(informativeOutput.Enabled, RootArea.TreeToString());

		PlaceRooms(RootArea);
		PlaceDoors(RootArea);
	}

	protected virtual void PlaceRooms(BSPNode rootArea)
	{
		roomMap = new Dictionary<int, Room>();
		foreach (BSPNode leaf in rootArea.Leaves)
		{
			Size roomSize = new Size(leaf.Width, leaf.Height);
			Point roomPosition = new Point(leaf.X, leaf.Y);
			Room room = new Room(new Rectangle(roomPosition, roomSize));
			AddRoom(room, leaf.ID);
		}
	}
	protected virtual void PlaceDoors(BSPNode rootArea)
	{
		Debug.WriteLineIf(informativeOutput.Enabled, "Generating doors...");

		foreach ((BSPNode a, BSPNode b) in rootArea.Pairs)
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

			BSPNode sideA;
			BSPNode sideB;

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

			Debug.WriteLineIf(informativeOutput.Enabled, door);
		}

		Debug.WriteLineIf(informativeOutput.Enabled, "Doors generated");
	}

	protected virtual void AddDoor(Door door)
	{
		doors.Add(door);
		walkableGrid[door.location.X, door.location.Y] = true;
	}
	protected virtual void AddRoom(Room room, int areaID)
	{
		rooms.Add(room);
		roomMap.Add(areaID, room);
		Debug.WriteLineIf(informativeOutput.Enabled, $"Added room #{room.ID} to dictionary under ({areaID})");

		for (int y = 0; y < room.area.Height - 2; y++)
		{
			for (int x = 0; x < room.area.Width - 2; x++)
			{
				walkableGrid[x + room.area.X + 1, y + room.area.Y + 1] = true;
			}
		}
	}
}