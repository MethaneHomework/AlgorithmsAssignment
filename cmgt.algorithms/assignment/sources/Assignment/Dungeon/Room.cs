using System.Collections.Generic;
using System.Drawing;

/**
 * This class represents (the data for) a Room, at this moment only a rectangle in the dungeon.
 */
class Room
{
	public Rectangle area;

	private static int lastID = 0;
	public readonly int ID;

	public readonly List<Door> doors;

	public Room(Rectangle pArea)
	{
		area = pArea;
		ID = lastID++;

		doors = new List<Door>();
	}

	public Rectangle internalArea
	{
		get
		{
			Rectangle internals = area;
			internals.Inflate(-1, -1);
			return internals;
		}
	}

	//Return information about the type of object and it's data
	//eg Room: (x, y, width, height)
	public override string ToString()
	{
		return $"Room #{ID} is located at {area.Location}, and has {doors.Count} connections";
	}
}
